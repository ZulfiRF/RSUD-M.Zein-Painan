using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Windows.Threading;
using Core.Framework.Helper.Configuration;
using Core.Framework.Helper.Services;
using Core.Framework.Windows.Windows;
using Newtonsoft.Json;
using Path = System.IO.Path;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml.Linq;
using Core.Framework.Helper;
using Core.Framework.Helper.Contracts;
using Core.Framework.Helper.Presenters;
using Core.Framework.Windows.Contracts;
using Core.Framework.Windows.Controls;
using Core.Framework.Windows.Helper;
using Core.Framework.Helper.Logging;

namespace Core.Framework.Windows.Implementations
{
    [TemplatePart(Name = PART_OverlayBox, Type = typeof(Grid))]
    [TemplatePart(Name = PART_FlyoutModal, Type = typeof(Rectangle))]
    [TemplatePart(Name = PART_GridStatusBar, Type = typeof(Grid))]
    public class CoreUserControl : UserControl, IHistoryControl<CoreUserControl>, ICloseControl, IControlAuthentication, IDisposable
    {
        private bool useOtorisasi = false;
        private const string PART_FlyoutModal = "PART_FlyoutModal";
        private const string PART_OverlayBox = "PART_OverlayBox";
        private const string PART_GridStatusBar = "PART_GridStatusBar";

        public List<FrameworkElement> FocusItems { get; set; }
        public static readonly RoutedEvent FlyoutsStatusChangedEvent = EventManager.RegisterRoutedEvent(
            "FlyoutsStatusChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CoreUserControl));
        public List<FrameworkElement> FocusBackItems { get; set; }
        #region Fields

        private readonly Dictionary<string, object> dictionaryBack = new Dictionary<string, object>();

        private readonly Dictionary<string, object> dictionaryNext = new Dictionary<string, object>();

        private bool animateMessage;

        private CoreUserControl backControl;

        private object beforeControl;

        private string hotKeys;

        private bool press;

        #endregion


        #region Constructors and Destructors

        private bool isPostBack;

        public class FlyoutStatusChangedRoutedEventArgs : RoutedEventArgs
        {
            internal FlyoutStatusChangedRoutedEventArgs(RoutedEvent rEvent, object source)
                : base(rEvent, source)
            { }

            public Flyout ChangedFlyout { get; internal set; }
        }
        internal void HandleFlyoutStatusChange(Flyout flyout, IEnumerable<Flyout> visibleFlyouts)
        {
            //checks a recently opened flyout's position.
            if (flyout.Position == Position.Right || flyout.Position == Position.Top)
            {
                //get it's zindex
                //var zIndex = flyout.IsOpen ? Panel.GetZIndex(flyout) + 3 : visibleFlyouts.Count() + 2;

                //if ShowWindowCommandsOnTop is true, set the window commands' zindex to a number that is higher than the flyout's. 
                //WindowCommandsPresenter.SetValue(Panel.ZIndexProperty, this.ShowWindowCommandsOnTop ? zIndex : (zIndex > 0 ? zIndex - 1 : 0));
                //WindowButtonCommands.SetValue(Panel.ZIndexProperty, zIndex);

                this.HandleWindowCommandsForFlyouts(visibleFlyouts);
            }

            flyoutModal.Visibility = visibleFlyouts.Any(x => x.IsModal) ? Visibility.Visible : Visibility.Hidden;

            RaiseEvent(new FlyoutStatusChangedRoutedEventArgs(FlyoutsStatusChangedEvent, this)
            {
                ChangedFlyout = flyout
            });
        }

        public CoreUserControl()
        {
            this.Unloaded += OnUnloaded;
            if (Application.Current != null)
                this.Style = Application.Current.Resources["CoreUserControlStyle"] as Style;
            FocusItems = new List<FrameworkElement>();
            FocusBackItems = new List<FrameworkElement>();
            //DefaultStyleKey = typeof(CoreUserControl);
            this.Initialized += OnInitialized;
            this.Loaded += this.OnLoaded;
            this.MyGuid = Guid.NewGuid().ToString();
            KeyUp += OnKeyUp;

        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11)
            {
                var settingVisibilitas = SourceKeyValues.KeyValues.FirstOrDefault(n => n.Key.ToString().Equals("CommonModuleVisibility"));
                if (settingVisibilitas != null) ExecuteControlForm(settingVisibilitas.Value as Type);
            }
        }

        private void ExecuteControlForm(Type typeDomain)
        {
            try
            {
                var keyValuesContaintCommon = SourceKeyValues.KeyValues.FirstOrDefault(n => n.Key.ToString().Contains(typeDomain.Name) && n.Key.ToString().Contains("Common"));
                if (keyValuesContaintCommon != null)
                {
                    if (typeof(IGenericCalling).IsAssignableFrom(keyValuesContaintCommon.Value as Type))
                    {
                        if (keyValuesContaintCommon.Value is Type)
                        {
                            var type = keyValuesContaintCommon.Value as Type;
                            if (type.IsClass)
                            {
                                var instanace = Activator.CreateInstance(type);
                                if (instanace != null)
                                {
                                    try
                                    {
                                        var tag = instanace as IFrameworkElementTag;
                                        if (tag != null) tag.FrameworkElement = this;

                                        var generic = instanace as IGenericCalling;
                                        if (generic != null) generic.InitView();
                                    }
                                    catch (Exception e)
                                    {
                                        Manager.HandleException(e);
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Manager.HandleException(e, "Error ExecuteControlForm : Domain Type atau DomainNameSpace!");
            }
        }

        public event EventHandler<ItemEventArgs<string>> ReciveMessage;
        private void ClientOnReciveMessage(object sender, ItemEventArgs<string> itemEventArgs)
        {
            Log.Info("Have Message " + itemEventArgs.Item);
            if (itemEventArgs.Item.Equals("Dispose"))
            {
                this.CloseControl();
            }
            OnReciveMessage(itemEventArgs);
        }


        private void OnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            IsPostBack = false;
            MouseMove -= OnMouseMove;
        }

        private void OnInitialized(object sender, EventArgs eventArgs)
        {
        }

        #endregion

        #region Public Events

        public event EventHandler FirstLoad;

        #endregion

        #region Public Properties

        //public ContentControl ParentForNavigation { get; set; }

        private ContentControl parent;

        public ContentControl ParentForNavigation
        {
            get { return parent; }
            set
            {
                parent = value;
            }
        }


        public virtual MessageItem Message
        {
            set
            {
                var message = value;
                Manager.Timeout(
                    this.Dispatcher,
                    () =>
                    {

                        if (PartGridMessage == null) return;
                        var parent = Manager.FindVisualParent<MetroWindow>(this);
                        this.PartGridMessage.Visibility = Visibility.Visible;
                        switch (message.MessageType)
                        {
                            case MessageType.Error:
                                this.PartGridMessage.Background = new SolidColorBrush(Color.FromRgb(220, 61, 61));
                                break;
                            case MessageType.Success:
                                if (parent != null)
                                {
                                    if (this.PartGridMessage != null)
                                    {
                                        this.PartGridMessage.Background = new SolidColorBrush(Color.FromRgb(101, 215, 101));
                                        var grid = parent.TitleBar as Grid;
                                        if (grid != null)
                                        {

                                        }
                                    }
                                }
                                break;
                            case MessageType.Information:
                                if (parent != null)
                                {
                                    if (this.PartGridMessage != null)
                                    {
                                        var grid = parent.TitleBar as Grid;
                                        if (grid != null)
                                        {
                                            this.PartGridMessage.Background = new SolidColorBrush(Colors.DarkMagenta);
                                        }
                                    }
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        this.PartGridMessage.Visibility = Visibility.Visible;
                        if (message.Exceptions != null)
                        {
                            message.Content = message.Exceptions.Message;
                        }
                        this.PartTextBlock.Text = message.Content.ToString();
                        var log = BaseDependency.Get<ILogRepository>();
                        if (log != null)
                        {
                            if (message.Exceptions != null)
                            {
                                log.Error(message.Exceptions);
                            }
                            else
                                log.Info(message.Content.ToString());
                        }
                        this.animateMessage = true;
                    });
            }
        }

        public string MyGuid { get; set; }

        #endregion

        #region Properties

        protected IClientMessaging Client { get; set; }
        protected Button PartClose { get; set; }
        protected Grid PartGridMessage { get; set; }
        protected TextBlock PartTextBlock { get; set; }

        #endregion

        public bool NotBusyLoad
        {
            get { return (bool)GetValue(NotBusyLoadProperty); }
            set { SetValue(NotBusyLoadProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NotBusyLoad.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NotBusyLoadProperty =
            DependencyProperty.Register("NotBusyLoad", typeof(bool), typeof(CoreUserControl), new UIPropertyMetadata(true));




        #region Public Methods and Operators

        public object Back(bool mustReload = false)
        {
            try
            {
                //if (this.backControl != null)
                //{
                //    object result;
                //    if (this.backControl.dictionaryBack.TryGetValue(this.GetType().FullName, out result))
                //    {
                //        this.backControl.Content = this.backControl.dictionaryBack[this.GetType().FullName];
                //        if (clear)
                //        {
                //            this.backControl.dictionaryBack.Remove(this.GetType().FullName);
                //            this.backControl.dictionaryNext.Remove(this.GetType().FullName);
                //        }
                //    }
                //}
                var tabItem = this.ParentForNavigation as ContentControl;
                object result = null;
                if (tabItem != null)
                {
                    // = controlOld;
                    if (this.Tag is DockPane)
                    {
                        var dockPane = this.Tag as DockPane;
                        if (dockPane.Content != null)
                        {
                            if (!tabItem.Equals(this.Tag))
                            {
                                tabItem.Content = this.Tag as FrameworkElement;
                                result = tabItem.Content;
                            }
                        }
                        result = null;
                    }
                    else
                    {
                        //caun
                        tabItem.Content = this.Tag as FrameworkElement;
                        if (tabItem.Content is IBackHistoryControl)
                            (tabItem.Content as IBackHistoryControl).IsPostBack = true;
                        result = tabItem.Content;
                    }
                    var view = tabItem.Content as IRefreshData;
                    if (mustReload)
                        if (view != null)
                            view.InitLoad();
                    return result;
                }
            }
            catch (Exception exception)
            {
                BaseDependency.Get<ILogRepository>().Error(exception.ToString());
            } //Content = this.backControl;
            return null;

        }

        public bool IsPostBack
        {
            get
            {
                //var temp = isPostBack;
                //  isPostBack = false;
                return isPostBack;
            }
            set { isPostBack = value; }
        }

        public void CloseControl()
        {

            if (this.Tag is ICloseControl)
            {
                (this.Tag as ICloseControl).CloseControl();
            }
            else
            {
                Dispatcher.BeginInvoke((ThreadStart)delegate ()
                {
                    var panel = this.Parent as Panel;
                    if (panel != null) panel.Children.Remove(this);
                    var window = this.Parent as Window;
                    if (window != null)
                    {
                        window.Content = null;
                        GC.Collect();
                        window.Close();
                        window = null;
                    }
                }, DispatcherPriority.ApplicationIdle);


                var contentControl = Parent as ContentControl;
                if (contentControl != null) contentControl.Content = null;
                var border = Parent as Decorator;
                if (border != null) border.Child = null;
            }
        }

        public void CloseForm()
        {
        }

        public void Next(CoreUserControl control)
        {
            try
            {
                control.beforeControl = this.Content;

                if (this.backControl != null)
                {
                    control.backControl = this.backControl;

                    object ctrl;
                    if (!control.backControl.dictionaryNext.TryGetValue(control.GetType().FullName, out ctrl))
                    {
                        control.backControl.dictionaryBack.Add(control.GetType().FullName, this.Content);
                        control.backControl.dictionaryNext.Add(control.GetType().FullName, control.Content);
                        this.backControl.Content = control.Content;

                        object content = control.Content;
                        control.Content = null;
                        this.Content = content;
                    }
                    else
                    {
                        this.backControl.Content = ctrl;
                    }
                }
                else
                {
                    object ctrl;
                    control.backControl = this;
                    if (!this.dictionaryNext.TryGetValue(control.GetType().FullName, out ctrl))
                    {
                        this.dictionaryBack.Add(control.GetType().FullName, this.Content);
                        this.dictionaryNext.Add(control.GetType().FullName, control.Content);
                        object content = control.Content;
                        control.Content = null;
                        this.Content = content;
                    }
                    else
                    {
                        if (ctrl is FormGrid)
                        {
                            (ctrl as FormGrid).ClearForm();
                        }
                        this.Content = ctrl;
                    }
                }
            }
            catch (Exception exception)
            {
                BaseDependency.Get<ILogRepository>().Error(exception.ToString());
            } //Content = this.backControl;
        }

        public override void OnApplyTemplate()
        {
            try
            {


                if (Flyouts == null)
                    Flyouts = new FlyoutsControl();
                if (Application.Current != null)
                {
                    if (Flyouts.Style == null)
                        Flyouts.Style = Application.Current.Resources["MetroFlyoutControls"] as Style;
                    //Flyouts.OnApplyTemplate();
                }
                this.Background = new SolidColorBrush(Colors.White);
                //Background = new SolidColorBrush((Color)Application.Current.Resources["AccentColorBlack"]);
                base.OnApplyTemplate();
                this.PartTextBlock = this.GetTemplateChild("tbLabelMessage") as TextBlock;
                this.PartClose = this.GetTemplateChild("PART_Close") as Button;
                this.PartGridStatusBar = this.GetTemplateChild("PART_GridStatusBar") as Grid;
                if (this.PartClose != null)
                {
                    this.PartClose.Click += this.PartCloseOnClick;
                }
                this.PartGridMessage = this.GetTemplateChild("gridMessage") as Grid;
                if (this.PartGridMessage != null)
                {

                    this.PartGridMessage.Visibility = Visibility.Collapsed;
                    this.PartGridMessage.MouseLeftButtonUp += this.PartGridMessageOnMouseLeftButtonUp;
                }

                if (this.PartTextBlock != null)
                {

                    PartTextBlock.SizeChanged += this.PartTextBlockOnSizeChanged;
                    PartTextBlock.Foreground = new SolidColorBrush(Colors.White);
                }

                overlayBox = GetTemplateChild(PART_OverlayBox) as Grid;
                flyoutModal = GetTemplateChild(PART_FlyoutModal) as Rectangle;
                if (flyoutModal != null) flyoutModal.PreviewMouseDown += FlyoutsPreviewMouseDown;
                this.PreviewMouseDown += FlyoutsPreviewMouseDown;

                Resources.MergedDictionaries.Add(new ResourceDictionary
                {
                    Source =
                        new Uri(
                        "pack://application:,,,/Core.Framework.Windows;component/Styles/Controls.xaml",
                        UriKind.RelativeOrAbsolute)
                });
            }
            catch (Exception)
            {
            }
        }

        public Grid PartGridStatusBar { get; set; }

        /// <summary>
        /// Gets/sets the FlyoutsControl that hosts the window's flyouts.
        /// </summary>
        public FlyoutsControl Flyouts
        {
            get { return (FlyoutsControl)GetValue(FlyoutsProperty); }
            set { SetValue(FlyoutsProperty, value); }
        }
        public static readonly DependencyProperty FlyoutsProperty = DependencyProperty.Register("Flyouts", typeof(FlyoutsControl), typeof(CoreUserControl), new PropertyMetadata(null));
        private void FlyoutsPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (e.OriginalSource as FrameworkElement);
            if (element != null && element.TryFindParent<Flyout>() != null)
            {
                return;
            }

            if (Flyouts.OverrideExternalCloseButton == null)
            {
                foreach (Flyout flyout in Flyouts.GetFlyouts())
                {
                    if (flyout.ExternalCloseButton == e.ChangedButton && (flyout.IsPinned == false || Flyouts.OverrideIsPinned == true))
                    {
                        flyout.IsOpen = false;
                    }
                }
            }
            else if (Flyouts.OverrideExternalCloseButton == e.ChangedButton)
            {
                foreach (Flyout flyout in Flyouts.GetFlyouts())
                {
                    if (flyout.IsPinned == false || Flyouts.OverrideIsPinned == true)
                    {
                        flyout.IsOpen = false;
                    }
                }
            }
        }
        #endregion

        #region Methods

        protected internal virtual void OnFirstLoad()
        {
            EventHandler handler = this.FirstLoad;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected override void OnChildDesiredSizeChanged(UIElement child)
        {
            base.OnChildDesiredSizeChanged(child);
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            var frameworkElement = newContent as FrameworkElement;
            if (frameworkElement != null)
            {
                this.Width = frameworkElement.Width;
                this.Height = frameworkElement.Height;
            }
            base.OnContentChanged(oldContent, newContent);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            ThreadPool.QueueUserWorkItem
                (
                this.FirstLoadCallBack
                );
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.System || e.Key == Key.LeftAlt || e.Key == Key.LeftCtrl || e.Key == Key.LeftShift
                || e.Key == Key.RightShift || e.Key == Key.RightAlt || e.Key == Key.RightCtrl)
            {
                this.press = true;
                this.hotKeys = e.Key.ToString();
            }
            else if (this.press)
            {
                if (e.Key.ToString().Equals("Left"))
                {
                    //this.Back();
                }
                this.press = false;
            }
            //else if (e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Up || e.Key == Key.Down)
            //{
            //    TraversalRequest request = null;
            //    switch (e.Key)
            //    {
            //        case Key.Left:
            //            request = new TraversalRequest(FocusNavigationDirection.Left);
            //            break;
            //        case Key.Right:
            //            request = new TraversalRequest(FocusNavigationDirection.Right);
            //            break;
            //        case Key.Up:
            //            request = new TraversalRequest(FocusNavigationDirection.Up);
            //            break;
            //        case Key.Down:
            //            request = new TraversalRequest(FocusNavigationDirection.Down);
            //            break;
            //    }

            //    // Gets the element with keyboard focus.
            //    var elementWithFocus = Keyboard.FocusedElement as UIElement;
            //    //if (elementWithFocus is TextBox)
            //    //{
            //    //    return;
            //    //}
            //    // Change keyboard focus. 
            //    if (elementWithFocus != null)
            //    {
            //        elementWithFocus.MoveFocus(request);
            //    }
            //}
            base.OnPreviewKeyDown(e);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
        }

        private void FirstLoadCallBack(object state)
        {
            Manager.Timeout(this.Dispatcher, () => this.OnFirstLoad());
        }

        private bool hasLoadText;
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {

            if (ThemeManager.CurrentAccent != null)
                ThemeManager.ChangeTheme(this.Resources, ThemeManager.CurrentAccent, ThemeManager.CurrentTheme);
            MouseMove += OnMouseMove;
            if (Application.Current != null)
            {
                this.Style = Application.Current.Resources[typeof(CoreUserControl)] as Style;
            }
            if (!hasLoadText)
            {
                Manager.Timeout(Dispatcher, () =>
            {
                foreach (var frameworkElement in FocusItems)
                {
                    if (frameworkElement.IsEnabled)
                    {
                        frameworkElement.Focus();
                        break;
                    }
                }
            });
            }
            else
            {
                Manager.Timeout(Dispatcher, () =>
                {
                    foreach (var frameworkElement in FocusBackItems)
                    {
                        if (frameworkElement.IsEnabled)
                        {
                            frameworkElement.Focus();
                            break;
                        }
                    }
                });
            }
            if (hasLoadText) return;
            //try
            //{
            //    if (!SocketReciveItem.errorSocket)
            //    {
            //        this.Client = BaseDependency.GetNewInstance<IClientMessaging>();
            //        Client.SubscribeModule(GetType().Name);
            //        Client.ReciveMessage += ClientOnReciveMessage;
            //    }
            //}
            //catch (Exception exception)
            //{
            //    SocketReciveItem.errorSocket = true;
            //    Log.Error(exception);
            //}

            if (StatusBars != null && StatusBars.Count != 0)
            {
                if (PartGridStatusBar != null)
                {
                    PartGridStatusBar.Visibility = Visibility.Visible;
                    for (int i = 0; i < StatusBars.Count; i++)
                    {
                        PartGridStatusBar.ColumnDefinitions.Add(new ColumnDefinition()
                        {
                            Width = new GridLength(1, GridUnitType.Star)
                        });
                        var statusItem = new TextBlock()
                        {
                            Text = StatusBars[i],
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(0)
                        };
                        Grid.SetColumn(statusItem, i);
                        PartGridStatusBar.Children.Add(statusItem);
                    }
                }

            }
            var pathLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Language";
            if (!Directory.Exists(pathLocation))
            {
                Directory.CreateDirectory(pathLocation);
            }
            var pathFile = pathLocation + "\\" + GetType().Name;
            if (File.Exists(pathFile))
            {
                var listTextBlock = (Content as Panel).FindChildren<TextBlock>().ToList();
                ISetting setting = new SettingXml(pathFile, SettingXml.TypeFile.PathFile);
                var findChildren = listTextBlock.ToArray();
                if (findChildren.Any())
                    foreach (var findChild in findChildren)
                    {
                        if (!string.IsNullOrEmpty(findChild.Text))
                        {
                            var result = setting.GetValue("Label-" + findChild.Text);
                            if (!string.IsNullOrEmpty(result))
                                findChild.Text = result;
                        }
                        else
                            foreach (var inline in findChild.Inlines.OfType<Run>().ToList())
                            {
                                var result = setting.GetValue("Label-" + inline.Text);
                                if (!string.IsNullOrEmpty(result))
                                    inline.Text = result;
                            }
                    }
                var listButton = (Content as Panel).FindChildren<CoreButton>().ToList();
                if (listButton.Any())
                    foreach (var findChild in listButton)
                        if (findChild.Content is string)
                        {
                            var toolTip = findChild.ToolTip as ToolTip;
                            if (toolTip != null)
                            {
                                var panel = toolTip.Content as StackPanel;
                                if (panel != null)
                                {
                                    foreach (var child in panel.Children.OfType<TextBlock>())
                                    {
                                        var result = setting.GetValue("Button-Text-" + child.Text);
                                        if (!string.IsNullOrEmpty(result))
                                            child.Text = result;
                                    }
                                }
                            }
                            var resultButton = setting.GetValue("Button-" + findChild.Content);
                            if (!string.IsNullOrEmpty(resultButton))
                                findChild.Content = resultButton;
                        }
                var listGroupBox = (Content as Panel).FindChildren<GroupBox>().ToList();
                if (listGroupBox.Any())
                    foreach (var findChild in listGroupBox)
                        if (findChild.Header is string)
                        {
                            var result = setting.GetValue("Group-" + findChild.Header);
                            if (!string.IsNullOrEmpty(result))
                                findChild.Header = result;
                        }

                var listCheckBox = (Content as Panel).FindChildren<CoreCheckBox>().ToList();
                if (listCheckBox.Any())
                    foreach (var findChild in listCheckBox)
                        if (findChild.Content is string)
                        {
                            var result = setting.GetValue("CheckBox-" + findChild.Content);
                            if (!string.IsNullOrEmpty(result))
                                findChild.Content = result;
                        }

                var listRadioButton = (Content as Panel).FindChildren<CoreRadioButton>().ToList();
                if (listRadioButton.Any())
                    foreach (var findChild in listRadioButton)
                        if (findChild.Content is string)
                        {
                            var result = setting.GetValue("RadioButton-" + findChild.Content);
                            if (!string.IsNullOrEmpty(result))
                                findChild.Content = result;
                        }

                var listListView = (Content as Panel).FindChildren<CoreListView>().ToList();
                if (listListView.Any())
                    foreach (var findChild in listListView)
                    {
                        var view = findChild.View as GridView;
                        if (view != null)
                            foreach (var coreListView in view.Columns.Cast<GridViewColumn>())
                                if (coreListView.Header is string)
                                {
                                    var result = setting.GetValue("DataGrid-" + coreListView.Header);
                                    if (!string.IsNullOrEmpty(result))
                                        coreListView.Header = result;
                                }
                    }

                var listDataGrid = (Content as Panel).FindChildren<CoreDataGrid>().ToList();
                if (listDataGrid.Any())
                    foreach (var findChild in listDataGrid)
                    {
                        foreach (var coreListView in findChild.Columns.Cast<DataGridColumn>())
                            if (coreListView.Header is string)
                            {
                                var result = setting.GetValue("Grid-" + coreListView.Header);
                                if (!string.IsNullOrEmpty(result))
                                    coreListView.Header = result;
                            }
                    }
            }
            hasLoadText = true;

            var control = this as FrameworkElement;
            if (useOtorisasi && !control.GetType().Name.Equals("ForbiddenView") && !control.GetType().Name.Equals("LoginControl"))
            {
                var otorisasi = BaseDependency.Get<IOtorisasiLogin>();
                if (otorisasi != null)
                {
                    var isHavingAccess = otorisasi.isHavingAccess(control.GetType().Name);
                    if (!isHavingAccess)
                    {
                        var forbidden = Manager.GetGenericModule().FirstOrDefault(n => n.Key.StartsWith("CommonModuleForbidden"));
                        var activator = forbidden.Value;

                        var passing = Activator.CreateInstance(activator) as IPassingView;
                        if (passing != null)
                            passing.PassingView = control;

                        var genericCalling = passing as IGenericCalling;
                        if (genericCalling != null)
                            genericCalling.InitView();
                        CloseControl();
                    }
                }
            }

            SetVisibility(false);

            try
            {
                var pathTema = Path.GetDirectoryName(Application.ResourceAssembly.Location) + "\\";
                const string fileName = "Tema.xml";
                var readFile = XDocument.Load(pathTema + fileName);
                var doc = readFile.Descendants("tema").FirstOrDefault();
                if (doc != null)
                {
                    var temaHeaderMain = SetMenuEditor(doc, "header-main").Value.Trim();
                    if (this.GetType().Name != "PemeriksaanPasienRekamMedisView")
                    {
                        var childGroupBox = (Content as Panel).FindChildren<GroupBox>();
                        foreach (var groupBox in childGroupBox)
                        {
                            groupBox.Background =
                                new SolidColorBrush((Color)ColorConverter.ConvertFromString(temaHeaderMain));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                
            }            
        }

        private XElement SetMenuEditor(XElement doc, string tagXml)
        {
            var element = doc.Element(tagXml);
            return element != null ? element : null;
        }

        private void SetVisibility(bool isEnabled)
        {
            if (isEnabled)
            {
                try
                {
                    var repository = BaseDependency.Get<IDefaultVisibilityRepository>();
                    if (repository == null) return;
                    var jSons = repository.GetVisibilityControlsByName(GetType().FullName);
                    if (jSons != null)
                    {
                        var findControls = (Content as Panel).FindChildren<FrameworkElement>().Where(n =>
                        n.GetType() == typeof(TextBlock) ||
                        n.GetType() == typeof(CoreButton) ||
                        n.GetType() == typeof(CoreCheckBox) ||
                        n.GetType() == typeof(CoreRadioButton) ||
                        n.GetType() == typeof(InsertDataGrid) ||
                        n.GetType() == typeof(CoreListView)).Distinct();

                        foreach (var findControl in findControls)
                        {
                            if (findControl.GetType() == typeof(TextBlock))
                            {
                                var textBlock = findControl as TextBlock;
                                var text = textBlock.Text;
                                if (string.IsNullOrEmpty(text))
                                {
                                    var labelInLine = textBlock.Inlines.FirstInline as Run;
                                    text = labelInLine.Text;
                                }

                                if (jSons.Any(n => n.Control.Equals(text) && !n.IsVisible))
                                {
                                    var parentGrid = Manager.FindVisualParent<Grid>(findControl);
                                    if (parentGrid != null) parentGrid.Visibility = Visibility.Collapsed;
                                    else
                                    {
                                        var parentBorder = Manager.FindVisualParent<Border>(findControl);
                                        if (parentBorder != null) parentBorder.Visibility = Visibility.Collapsed;
                                    }
                                }

                                var getValue = jSons.SingleOrDefault(n => textBlock != null && n.Control.Equals(textBlock.Text));
                                if (getValue != null)
                                {
                                    var gridParent = Manager.FindVisualParent<Grid>(textBlock);
                                    GetValueElement(gridParent, getValue);
                                }

                                continue;
                            }

                            if (findControl.GetType() == typeof(CoreButton))
                            {
                                var button = findControl as CoreButton;
                                if (jSons.Any(n => button != null && n.Control.Equals(button.Content) && !n.IsVisible))
                                    findControl.Visibility = Visibility.Collapsed;
                                continue;
                            }

                            if (findControl.GetType() == typeof(CoreCheckBox))
                            {
                                var checkBox = findControl as CoreCheckBox;
                                if (jSons.Any(n => checkBox != null && n.Control.Equals(checkBox.Content) && !n.IsVisible))
                                    findControl.Visibility = Visibility.Collapsed;
                                var getValue = jSons.SingleOrDefault(n => checkBox != null && n.Control.Equals(checkBox.Content));
                                if (checkBox != null)
                                    checkBox.IsChecked = getValue != null && (getValue.DefaultValue != null && Convert.ToBoolean(getValue.DefaultValue));
                                continue;
                            }

                            if (findControl.GetType() == typeof(CoreRadioButton))
                            {
                                var radio = findControl as CoreRadioButton;
                                if (jSons.Any(n => radio != null && n.Control.Equals(radio.Content) && !n.IsVisible))
                                    findControl.Visibility = Visibility.Collapsed;
                                continue;
                            }

                            if (findControl.GetType() == typeof(InsertDataGrid))
                            {
                                var grids = findControl as InsertDataGrid;
                                if (grids != null)
                                    foreach (var dataGridColumn in grids.Columns)
                                    {
                                        if (dataGridColumn.Header is string)
                                        {
                                            if (jSons.Any(n => dataGridColumn != null && n.Control.Equals(dataGridColumn.Header) && !n.IsVisible))
                                                dataGridColumn.Visibility = Visibility.Collapsed;
                                        }
                                        else if (dataGridColumn.Header.GetType() == typeof(CoreCheckBox))
                                        {
                                            var checkBox = dataGridColumn.Header as CoreCheckBox;
                                            if (jSons.Any(n => checkBox != null && n.Control.Equals(checkBox.Content) && !n.IsVisible))
                                                dataGridColumn.Visibility = Visibility.Collapsed;
                                        }
                                    }
                            }

                            if (findControl.GetType() == typeof(CoreListView))
                            {
                                var list = findControl as CoreListView;
                                if (list != null)
                                {
                                    var views = list.View as GridView;
                                    if (views != null)
                                        foreach (var column in views.Columns)
                                        {
                                            if (column.Header is string)
                                            {
                                                if (jSons.Any(n => column != null && n.Control.Equals(column.Header) && !n.IsVisible))
                                                    column.Width = 0;
                                            }
                                        }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        private void GetValueElement(Grid gridParent, VisibilityControl getValue)
        {
            var controlTextBox = Manager.FindVisualChild<CoreTextBox>(gridParent);
            if (controlTextBox != null)
            {
                controlTextBox.Text = getValue.DefaultValue;
                return;
            }

            var controlComboBox = Manager.FindVisualChild<CoreComboBox>(gridParent);
            if (controlComboBox != null)
            {
                controlComboBox.Value = getValue.DefaultValue;
                return;
            }

            var controlDatePicker = Manager.FindVisualChild<CoreDatePicker>(gridParent);
            if (controlDatePicker != null)
            {
                controlDatePicker.SelectedDate = getValue.DefaultValue != null ? Convert.ToDateTime(getValue.DefaultValue) : DateTime.Now;
                return;
            }

            var controlAutoComplete = Manager.FindVisualChild<CoreAutoComplete>(gridParent);
            if (controlAutoComplete != null)
            {
                controlAutoComplete.Value = getValue.DefaultValue;
                return;
            }
        }

        private void PartCloseOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            this.Reset();
        }

        private void PartGridMessageOnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            this.Reset();
        }

        private void PartTextBlockOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            if (this.animateMessage)
            {
                //if (sizeChangedEventArgs.NewSize.Height > 16)
                //this.PartGridMessage.Height = sizeChangedEventArgs.NewSize.Height + 40;
                var transformGroup = new TransformGroup();
                transformGroup.Children.Add(new ScaleTransform());
                transformGroup.Children.Add(new SkewTransform());
                transformGroup.Children.Add(new RotateTransform());
                transformGroup.Children.Add(new TranslateTransform(0, this.ActualHeight + this.PartGridMessage.ActualHeight));
                this.PartGridMessage.Tag = this.ActualHeight + this.PartGridMessage.ActualHeight;
                this.PartGridMessage.RenderTransform = transformGroup;
                this.animateMessage = false;

                var storyboard = this.PartGridMessage.Resources["ShowMessage"] as Storyboard;
                if (storyboard != null)
                {
                    var doubleAnimationUsingKeyFrames = storyboard.Children[0] as DoubleAnimationUsingKeyFrames;
                    if (doubleAnimationUsingKeyFrames != null)
                    {
                        var easingDoubleKeyFrame = doubleAnimationUsingKeyFrames.KeyFrames[0] as EasingDoubleKeyFrame;
                        if (easingDoubleKeyFrame != null)
                        {
                            easingDoubleKeyFrame.Value = this.ActualHeight - sizeChangedEventArgs.NewSize.Height - 60;
                        }
                        ThreadPool.QueueUserWorkItem(delegate (object state)
                        {
                            Thread.Sleep(5000);
                            Manager.Timeout(Dispatcher, () => Reset());
                        });
                        storyboard.Begin();
                    }
                }
            }
        }

        private void Reset()
        {
            var storyboard = this.PartGridMessage.Resources["ShowMessage"] as Storyboard;
            if (storyboard != null)
            {
                var doubleAnimationUsingKeyFrames = storyboard.Children[0] as DoubleAnimationUsingKeyFrames;
                if (doubleAnimationUsingKeyFrames != null)
                {
                    var easingDoubleKeyFrame = doubleAnimationUsingKeyFrames.KeyFrames[0] as EasingDoubleKeyFrame;
                    if (easingDoubleKeyFrame != null)
                    {
                        easingDoubleKeyFrame.Value = Convert.ToDouble(this.PartGridMessage.Tag);
                    }
                }
                storyboard.Completed += this.StoryboardOnCompleted;
                storyboard.Begin();
            }
        }

        private void StoryboardOnCompleted(object sender, EventArgs eventArgs)
        {
            var storyboard = this.PartGridMessage.Resources["ShowMessage"] as Storyboard;
            if (storyboard != null)
            {
                storyboard.Completed -= this.StoryboardOnCompleted;
            }
            this.PartTextBlock.Text = "";

            this.PartGridMessage.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Implementation of IControlAuthentication

        /// <summary>
        /// Flag yang di gunakan untuk mengetahui Control itu harus di authentication atau tidak
        /// </summary>
        public bool UseAuthentication
        {
            get
            {
                return (bool)GetValue(UseAuthenticationProperty);
            }
            set
            {

                SetValue(UseAuthenticationProperty, value);
            }
        }
        // Using a DependencyProperty as the backing store for UseAuthentication.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseAuthenticationProperty =
            DependencyProperty.Register("UseAuthentication", typeof(bool), typeof(CoreUserControl), new UIPropertyMetadata(false, PropertyChangedCallback));
        private Grid overlayBox;
        private Rectangle flyoutModal;

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {

        }

        /// <summary>
        /// Method yang di eksekusi ketika Control memiliki flag true    
        /// </summary>
        /// <returns></returns>
        public bool ExecuteAuthentication()
        {
            return false;
        }

        /// <summary>
        /// Identity dari control
        /// </summary>
        public string IdentityAuthentication
        {
            get
            {
                var parent = this.TryFindParent<IControlAuthentication>();
                var str = new StringBuilder();
                int index = 0;
                while (parent != null)
                {
                    if (index == 0)
                        str.Append(parent.IdentityAuthentication);
                    else
                        str.Append("-" + parent.IdentityAuthentication);
                    index++;
                    parent = (parent as FrameworkElement).TryFindParent<IControlAuthentication>();
                }
                if (string.IsNullOrEmpty(str.ToString()))
                    return GetType().Name;
                return str + "-" + GetType().Name;
            }
        }

        //public object StatusBars { get; set; }


        public IList<string> StatusBars
        {
            get { return (IList<string>)GetValue(StatusBarsProperty); }
            set { SetValue(StatusBarsProperty, value); }
        }

        //public bool IsBusy { get; set; }
        private bool isBusy;

        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                Manager.Timeout(Dispatcher, () =>
                {
                    if (flyoutModal != null)
                    {
                        flyoutModal.Visibility = isBusy ? Visibility.Visible : Visibility.Hidden
                            ;
                    }
                });
            }
        }


        // Using a DependencyProperty as the backing store for StatusBars.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusBarsProperty =
            DependencyProperty.Register("StatusBars", typeof(IList<string>), typeof(CoreUserControl), new PropertyMetadata(null, StatusBarsCallback));

        private static void StatusBarsCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {

        }

        #endregion

        public void Dispose()
        {
            try
            {
                this.RemoveLogicalChild(this.Content);
                var panel = this.Parent as Panel;
                if (panel != null) panel.Children.Remove(this);
                var contentControl = Parent as ContentControl;
                if (contentControl != null) contentControl.Content = null;
                var clientMessaging = this.Client;
                if (clientMessaging != null) clientMessaging.Dispose();
            }
            catch (Exception exception)
            {
                Log.Error(exception);

            }
        }
        ~CoreUserControl()
        {
            if (Client is IDisposable)
                (Client as IDisposable).Dispose();
        }
        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            this.Initialized -= OnInitialized;
            this.Loaded -= this.OnLoaded;

        }

        protected virtual void OnReciveMessage(ItemEventArgs<string> e)
        {
            var handler = ReciveMessage;
            if (handler != null) handler(this, e);
        }
    }
}