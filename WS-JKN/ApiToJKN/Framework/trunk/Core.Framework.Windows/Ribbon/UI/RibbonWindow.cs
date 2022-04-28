#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

#endregion

namespace Core.Framework.Windows.Ribbon.UI
{
    /// <summary>
    /// This class represents a window with an integrated ribbon.
    /// </summary>
    [TemplatePart(Name = RibbonHostName, Type = typeof (Border))]
    [TemplatePart(Name = StatusBarName, Type = typeof (StatusBar))]
    [TemplatePart(Name = ApplicationMenuHostName, Type = typeof (Grid))]
    [TemplatePart(Name = RibbonOptionsPopupName, Type = typeof (Popup))]
    [TemplatePart(Name = RibbonOptionsListName, Type = typeof (ListView))]
    public class RibbonWindow : Window
    {
        #region Private Fields

        private ApplicationMenu appMenu;
        private Storyboard appMenuCloseStoryboard;
        private Grid appMenuHost;
        private Storyboard appMenuOpenStoryboard;
        private ContentPresenter contentPresenter;
        private HwndSource hwndSource;
        private Ribbon ribbon;
        private Border ribbonHost;
        private ListView ribbonOptionsList;
        private Popup ribbonOptionsPopup;
        private StatusBar statusBar;

        #endregion

        #region Xaml Support

        public const string RibbonHostName = "PART_RibbonHost";
        public const string StatusBarName = "PART_StatusBar";
        public const string ApplicationMenuHostName = "PART_ApplicationMenuHost";
        public const string RibbonOptionsListName = "PART_RibbonOptionsList";
        public const string RibbonOptionsPopupName = "PART_RibbonOptionsPopup";
        public const string ContentPresenterName = "PART_ContentPresenter";

        #endregion

        #region Construction

        static RibbonWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (RibbonWindow),
                                                     new FrameworkPropertyMetadata(typeof (RibbonWindow)));
        }

        public RibbonWindow()
        {
            if (Application.Current != null)
            {
                Style = Application.Current.Resources["RibbonWindowStyle"] as Style;
                AccentBrush = Application.Current.Resources["AccentColorBrush"] as Brush;
                HoverBrush = Application.Current.Resources["AccentSelectedColorBrush"] as Brush;
            }
            Loaded += OnWindowLoaded;
            MouseLeftButtonUp += OnMouseLeftButtonUp;
            SourceInitialized += OnSourceInitialized;

            CommandBindings.Add(new CommandBinding(RibbonCommands.OpenAppMenu, OnOpenAppMenu));
            CommandBindings.Add(new CommandBinding(RibbonCommands.CloseAppMenu, OnCloseAppMenu));
            CommandBindings.Add(new CommandBinding(RibbonCommands.BlendInRibbon, OnBlendInRibbon));
            CommandBindings.Add(new CommandBinding(RibbonCommands.OpenRibbonOptions, OnOpenRibbonOptions));
            CommandBindings.Add(new CommandBinding(RibbonCommands.AddQuickAccess, OnAddQuickAccess));
            CommandBindings.Add(new CommandBinding(RibbonCommands.RemoveQuickAccess, OnRemoveQuickAccess));
            CommandBindings.Add(new CommandBinding(WindowCommands.Maximize, OnMaximize));
            CommandBindings.Add(new CommandBinding(WindowCommands.Minimize, OnMinimize));
            CommandBindings.Add(new CommandBinding(WindowCommands.RestoreDown, OnRestoredDown));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, OnClose));

            QuickAccessItems = new QuickAccessCollection();
        }

        #endregion

        #region Public Events

        public event EventHandler RibbonStateChanged;

        protected virtual void OnRibbonStateChanged()
        {
            EventHandler handler = RibbonStateChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion

        #region Dependency Properties

        // Using a DependencyProperty as the backing store for ApplicationMenu.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ApplicationMenuProperty =
            DependencyProperty.Register("ApplicationMenu", typeof (ApplicationMenu), typeof (RibbonWindow),
                                        new PropertyMetadata(null, OnApplicationMenuChanged));

        // Using a DependencyProperty as the backing store for QuickAccessItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty QuickAccessItemsProperty =
            DependencyProperty.Register("QuickAccessItems", typeof (QuickAccessCollection), typeof (RibbonWindow),
                                        new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for DefaultState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefaultStateProperty =
            DependencyProperty.Register("DefaultState", typeof (RibbonState), typeof (RibbonWindow),
                                        new PropertyMetadata(RibbonState.Tabs));


        // Using a DependencyProperty as the backing store for RibbonState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RibbonStateProperty =
            DependencyProperty.Register("RibbonState", typeof (RibbonState), typeof (RibbonWindow),
                                        new PropertyMetadata(RibbonState.Tabs, OnRibbonStateChanged));

        // Using a DependencyProperty as the backing store for FramePadding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FramePaddingProperty =
            DependencyProperty.Register("FramePadding", typeof (Thickness), typeof (RibbonWindow),
                                        new PropertyMetadata(new Thickness(0)));

        // Using a DependencyProperty as the backing store for QuickAccessCommands.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsNormalizedProperty =
            DependencyProperty.Register("IsNormalized", typeof (bool), typeof (RibbonWindow),
                                        new PropertyMetadata(false));

        // Using a DependencyProperty as the backing store for IsMaximized.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMaximizedProperty =
            DependencyProperty.Register("IsMaximized", typeof (bool), typeof (RibbonWindow), new PropertyMetadata(false));

        // Using a DependencyProperty as the backing store for StatusBarItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusBarItemsSourceProperty =
            DependencyProperty.Register("StatusBarItemsSource", typeof (object), typeof (RibbonWindow),
                                        new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for StatusBarItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusBarItemTemplateProperty =
            DependencyProperty.Register("StatusBarItemTemplate", typeof (DataTemplate), typeof (RibbonWindow),
                                        new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for StatusBarItemsPanel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusBarItemsPanelProperty =
            DependencyProperty.Register("StatusBarItemsPanel", typeof (ItemsPanelTemplate), typeof (RibbonWindow),
                                        new PropertyMetadata(null));


        // Using a DependencyProperty as the backing store for StatusBarContainerStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusBarContainerStyleProperty =
            DependencyProperty.Register("StatusBarContainerStyle", typeof (Style), typeof (RibbonWindow),
                                        new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for Ribbon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RibbonProperty =
            DependencyProperty.Register("Ribbon", typeof (Ribbon), typeof (RibbonWindow),
                                        new PropertyMetadata(null, OnRibbonChanged));

        // Using a DependencyProperty as the backing store for IsAppMenuOpened.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAppMenuOpenedProperty =
            DependencyProperty.Register("IsAppMenuOpened", typeof (bool), typeof (RibbonWindow),
                                        new PropertyMetadata(false, OnIsAppMenuOpenedChanged));

        // Using a DependencyProperty as the backing store for AccentBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AccentBrushProperty =
            DependencyProperty.Register("AccentBrush", typeof (Brush), typeof (RibbonWindow), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for HoverBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HoverBrushProperty =
            DependencyProperty.Register("HoverBrush", typeof (Brush), typeof (RibbonWindow), new PropertyMetadata(null));

        public ApplicationMenu ApplicationMenu
        {
            get { return (ApplicationMenu) GetValue(ApplicationMenuProperty); }
            set { SetValue(ApplicationMenuProperty, value); }
        }

        public QuickAccessCollection QuickAccessItems
        {
            get { return (QuickAccessCollection) GetValue(QuickAccessItemsProperty); }
            set { SetValue(QuickAccessItemsProperty, value); }
        }

        public RibbonState DefaultState
        {
            get { return (RibbonState) GetValue(DefaultStateProperty); }
            set { SetValue(DefaultStateProperty, value); }
        }

        public RibbonState RibbonState
        {
            get { return (RibbonState) GetValue(RibbonStateProperty); }
            set { SetValue(RibbonStateProperty, value); }
        }

        public Thickness FramePadding
        {
            get { return (Thickness) GetValue(FramePaddingProperty); }
            set { SetValue(FramePaddingProperty, value); }
        }

        public bool IsNormalized
        {
            get { return (bool) GetValue(IsNormalizedProperty); }
            set { SetValue(IsNormalizedProperty, value); }
        }

        public bool IsMaximized
        {
            get { return (bool) GetValue(IsMaximizedProperty); }
            set { SetValue(IsMaximizedProperty, value); }
        }

        public object StatusBarItemsSource
        {
            get { return GetValue(StatusBarItemsSourceProperty); }
            set { SetValue(StatusBarItemsSourceProperty, value); }
        }

        public DataTemplate StatusBarItemTemplate
        {
            get { return (DataTemplate) GetValue(StatusBarItemTemplateProperty); }
            set { SetValue(StatusBarItemTemplateProperty, value); }
        }

        public ItemsPanelTemplate StatusBarItemsPanel
        {
            get { return (ItemsPanelTemplate) GetValue(StatusBarItemsPanelProperty); }
            set { SetValue(StatusBarItemsPanelProperty, value); }
        }

        public Style StatusBarContainerStyle
        {
            get { return (Style) GetValue(StatusBarContainerStyleProperty); }
            set { SetValue(StatusBarContainerStyleProperty, value); }
        }

        public Ribbon Ribbon
        {
            get { return (Ribbon) GetValue(RibbonProperty); }
            set { SetValue(RibbonProperty, value); }
        }

        public bool IsAppMenuOpened
        {
            get { return (bool) GetValue(IsAppMenuOpenedProperty); }
            set { SetValue(IsAppMenuOpenedProperty, value); }
        }

        public Brush AccentBrush
        {
            get { return (Brush) GetValue(AccentBrushProperty); }
            set { SetValue(AccentBrushProperty, value); }
        }

        public Brush HoverBrush
        {
            get { return (Brush) GetValue(HoverBrushProperty); }
            set { SetValue(HoverBrushProperty, value); }
        }

        #endregion

        #region Event Handlers

        private static void OnApplicationMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = (RibbonWindow) d;

            var old = e.OldValue as ApplicationMenu;
            if (old != null)
            {
                window.DetachAppMenu(old);
            }

            var @new = e.NewValue as ApplicationMenu;
            if (@new != null)
            {
                window.AttachAppMenu(@new);
            }
        }

        private void AttachAppMenu(ApplicationMenu menu)
        {
            appMenu = menu;
            appMenu.SelectionChanged += OnAppMenuSelectionChanged;
        }

        private void DetachAppMenu(Selector menu)
        {
            if (menu != null)
            {
                menu.SelectionChanged -= OnAppMenuSelectionChanged;
            }
        }

        private void OnAppMenuSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RunExchangeAnimation();
        }

        private static void OnIsAppMenuOpenedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = (RibbonWindow) d;
            var value = (bool) e.NewValue;
            if (value)
            {
                window.OpenAppMenu();
            }
            else
            {
                window.CloseAppMenu();
            }
        }

        protected void OnOpenAppMenu(object sender, ExecutedRoutedEventArgs e)
        {
            if (appMenu == null)
            {
                return;
            }

            IsAppMenuOpened = true;
            contentPresenter.Visibility = Visibility.Collapsed;
            ribbonHost.BlendOut();
            appMenu.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }

        protected void OnCloseAppMenu(object sender, ExecutedRoutedEventArgs e)
        {
            contentPresenter.Visibility = Visibility.Visible;
            IsAppMenuOpened = false;
        }

        private static void OnRibbonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = (RibbonWindow) d;
            if (e.OldValue is Ribbon)
            {
                window.DetachRibbon(e.OldValue as Ribbon);
            }
            window.AttachRibbon(e.NewValue as Ribbon);
        }

        private static void OnRibbonStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = (RibbonWindow) d;
            window.OnRibbonStateChanged();
            window.UpdateRibbonBehavior();
            window.SyncRibbonOptionsSelection();
        }

        private void OnRemoveQuickAccess(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(e.Parameter is IQuickAccessConform))
                return;

            QuickAccessItems.Remove(e.Parameter as IQuickAccessConform);
            StoreQuickAccessAsync();
        }

        private void OnAddQuickAccess(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(e.Parameter is IQuickAccessConform))
                return;

            QuickAccessItems.Add(e.Parameter as IQuickAccessConform);
            StoreQuickAccessAsync();
        }

        private void OnRibbonSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && !ribbon.IsCommandStripVisible && RibbonState == RibbonState.Tabs)
            {
                ribbon.SlideInCommandStrip();
            }
        }

        private void OnRibbonOptionsPopupMouseUp(object sender, MouseButtonEventArgs e)
        {
            ribbonOptionsPopup.IsOpen = false;
        }

        private void OnRibbonOptionSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var option = (RibbonOption) ribbonOptionsList.SelectedValue;
            RibbonState = option.Visibility;
        }

        private void OnBlendInRibbon(object sender, ExecutedRoutedEventArgs e)
        {
            if (ribbon == null || ribbonHost == null)
            {
                return;
            }

            ribbon.RestoreSelection();
            ribbonHost.BlendIn();
            statusBar.BlendIn();
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ribbon == null || ribbonHost == null)
            {
                return;
            }

            Point point = e.GetPosition(sender as IInputElement);
            if (RibbonState == RibbonState.TabsAndCommands)
            {
                return;
            }

            bool hit = HitTestFloatingControls(point);
            if (hit) return;

            if (RibbonState == RibbonState.Hidden)
            {
                ribbonHost.BlendOut();
                statusBar.BlendOut();
                return;
            }

            ribbon.IsCommandStripVisible = false;
            ribbon.ClearSelection();
        }

        internal void OpenAppMenu()
        {
            statusBar.Visibility = Visibility.Collapsed;
            appMenuOpenStoryboard.Begin();
        }

        internal void CloseAppMenu()
        {
            appMenuCloseStoryboard.Begin();
        }

        private bool HitTestFloatingControls(Point point)
        {
            var visuals = new List<DependencyObject>();
            VisualTreeHelper.HitTest(this, OnFilterHitTestResult, target =>
                                                                      {
                                                                          visuals.Add(target.VisualHit);
                                                                          return HitTestResultBehavior.Continue;
                                                                      }, new PointHitTestParameters(point));

            return visuals.Contains(ribbon) || visuals.Contains(statusBar);
        }

        private static HitTestFilterBehavior OnFilterHitTestResult(DependencyObject target)
        {
            if (target is Ribbon)
            {
                return HitTestFilterBehavior.ContinueSkipChildren;
            }

            if (target is StatusBar)
            {
                return HitTestFilterBehavior.ContinueSkipChildren;
            }

            return HitTestFilterBehavior.Continue;
        }

        private void OnOpenRibbonOptions(object sender, ExecutedRoutedEventArgs e)
        {
            ribbonOptionsPopup.PlacementTarget = (UIElement) e.OriginalSource;
            ribbonOptionsPopup.IsOpen = true;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            UpdateWindowStates();
            RestoreQuickAccess();
        }

        private void OnMinimize(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
            e.Handled = true;
        }

        private void OnMaximize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
            e.Handled = true;
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
            e.Handled = true;
        }

        private void OnRestoredDown(object sender, RoutedEventArgs e)
        {
            if (RibbonState == RibbonState.Hidden)
            {
                RibbonState = DefaultState;
            }
            WindowState = WindowState.Normal;
            e.Handled = true;
        }

        private void OnSourceInitialized(object sender, EventArgs e)
        {
            var helper = new WindowInteropHelper(this);
            hwndSource = HwndSource.FromHwnd(helper.Handle);
        }

        #endregion

        #region Class Overrides

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
            if (WindowState != WindowState.Maximized && RibbonState == RibbonState.Hidden)
            {
                RibbonState = DefaultState;
            }
            UpdateWindowStates();
            UpdateWindowBounds();
        }

        public override void OnApplyTemplate()
        {
            // Should be called first
            // See http://msdn.microsoft.com/en-us/library/system.windows.frameworkelement.onapplytemplate(v=vs.110).aspx
            base.OnApplyTemplate();

            contentPresenter = (ContentPresenter) Template.FindName(ContentPresenterName, this);

            ribbonHost = (Border) Template.FindName(RibbonHostName, this);
            statusBar = (StatusBar) Template.FindName(StatusBarName, this);

            if (ribbonOptionsPopup != null)
            {
                ribbonOptionsPopup.MouseUp -= OnRibbonOptionsPopupMouseUp;
            }

            ribbonOptionsPopup = (Popup) Template.FindName(RibbonOptionsPopupName, this);
            ribbonOptionsPopup.DataContext = this;
            ribbonOptionsPopup.MouseUp += OnRibbonOptionsPopupMouseUp;

            if (ribbonOptionsList != null)
            {
                ribbonOptionsList.SelectionChanged -= OnRibbonOptionSelectionChanged;
            }

            ribbonOptionsList = (ListView) Template.FindName(RibbonOptionsListName, this);
            ribbonOptionsList.SelectionChanged += OnRibbonOptionSelectionChanged;
            ribbonOptionsList.Items.AddRange(new[]
                                                  {
                                                      new RibbonOption
                                                          {
                                                              Title = Properties.Resources.AutoHideRibbonTitle,
                                                              Description =
                                                                  Properties.Resources.AutoHideRibbonDescription,
                                                              Visibility = RibbonState.Hidden,
                                                              ImageSource =
                                                                  new BitmapImage(
                                                                  new Uri(
                                                                      "/Core.Framework.Windows;component/Assets/autohide.png",
                                                                      UriKind.Relative))
                                                          },
                                                      new RibbonOption
                                                          {
                                                              Title = Properties.Resources.ShowTabsTitle,
                                                              Description = Properties.Resources.ShowTabsDescription,
                                                              Visibility = RibbonState.Tabs,
                                                              ImageSource =
                                                                  new BitmapImage(
                                                                  new Uri(
                                                                      "/Core.Framework.Windows;component/Assets/show.tabs.png",
                                                                      UriKind.Relative))
                                                          },
                                                      new RibbonOption
                                                          {
                                                              Title = Properties.Resources.ShowTabsAndCommandsTitle,
                                                              Description =
                                                                  Properties.Resources.ShowTabsAndCommandsDescription,
                                                              Visibility = RibbonState.TabsAndCommands,
                                                              ImageSource =
                                                                  new BitmapImage(
                                                                  new Uri(
                                                                      "/Core.Framework.Windows;component/Assets/show.tabs.commands.png",
                                                                      UriKind.Relative))
                                                          }
                                                  });

            appMenuHost = (Grid) Template.FindName(ApplicationMenuHostName, this);

            if (appMenuOpenStoryboard != null)
            {
                appMenuOpenStoryboard.Completed -= OnAppMenuOpened;
            }

            appMenuOpenStoryboard = (Storyboard) appMenuHost.FindResource("OpenApplicationMenuStoryboard");
            appMenuOpenStoryboard.Completed += OnAppMenuOpened;

            if (appMenuCloseStoryboard != null)
            {
                appMenuCloseStoryboard.Completed -= OnAppMenuClosed;
            }

            appMenuCloseStoryboard = (Storyboard) appMenuHost.FindResource("CloseApplicationMenuStoryboard");
            appMenuCloseStoryboard.Completed += OnAppMenuClosed;

            SyncRibbonOptionsSelection();
        }

        private void OnAppMenuClosed(object sender, EventArgs e)
        {
            appMenuHost.IsHitTestVisible = false;
            if (RibbonState != RibbonState.Hidden)
            {
                statusBar.BlendIn();
            }
        }

        private void OnAppMenuOpened(object sender, EventArgs e)
        {
            appMenuHost.IsHitTestVisible = true;
        }

        #endregion

        #region Methods

        private void RunExchangeAnimation()
        {
            var story = (Storyboard) appMenuHost.FindResource("ExchangeAppMenuContentStoryboard");
            story.Begin();
        }

        private void RestoreQuickAccess()
        {
            const string name = "ribbon.xml";
            if (!File.Exists(name))
            {
                return;
            }

            try
            {
                FileStream fs = File.OpenRead(name);
                using (var reader = new StreamReader(fs))
                {
                    var serializer = new XmlSerializer(typeof (ArrayList));
                    var list = serializer.Deserialize(reader) as ArrayList;
                    if (list == null)
                        return;
                    IEnumerable<string> names = list.OfType<string>();
                    IEnumerable<IQuickAccessConform> items = names.Select(QuickAccessRegistry.Find).Where(x => x != null);
                    QuickAccessItems.AddRange(items);
                }
            }
            catch (IOException ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void StoreQuickAccessAsync()
        {
            var keys = new ArrayList(QuickAccessItems
                                         .Where(x => x.Key != null)
                                         .Select(x => x.Key)
                                         .ToArray());

            if (keys.Count == 0)
            {
                return;
            }

            const string name = "ribbon.xml";
            try
            {
                FileStream fs = !File.Exists(name)
                                    ? File.Create(name)
                                    : File.Open(name, FileMode.Truncate, FileAccess.Write);

                using (var writer = new StreamWriter(fs))
                {
                    var serializer = new XmlSerializer(typeof (ArrayList));
                    serializer.Serialize(writer, keys);
                    writer.Flush();
                }
            }
            catch (IOException ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void AttachRibbon(Ribbon ribbon)
        {
            this.ribbon = ribbon;
            this.ribbon.SelectionChanged += OnRibbonSelectionChanged;
        }

        private void DetachRibbon(Selector ribbon)
        {
            if (ribbon != null)
            {
                ribbon.SelectionChanged -= OnRibbonSelectionChanged;
            }
        }

        private void UpdateRibbonBehavior()
        {
            if (ribbon == null || statusBar == null || ribbonHost == null)
            {
                return;
            }

            switch (RibbonState)
            {
                case RibbonState.Tabs:
                    ribbon.IsCommandStripVisible = false;
                    ribbon.IsWindowCommandStripVisible = false;
                    ribbon.ClearSelection();
                    statusBar.SnapIn();
                    ribbonHost.SnapIn();
                    ribbonHost.ExtendIntoContent();
                    break;
                case RibbonState.TabsAndCommands:
                    ribbon.IsCommandStripVisible = true;
                    ribbon.IsWindowCommandStripVisible = false;
                    ribbon.RestoreSelection();
                    statusBar.SnapIn();
                    ribbonHost.SnapIn();
                    ribbonHost.RetractFromContent();
                    break;
                case RibbonState.Hidden:
                    WindowState = WindowState.Maximized;
                    ribbon.IsCommandStripVisible = true;
                    ribbon.IsWindowCommandStripVisible = true;
                    statusBar.SnapOut();
                    ribbonHost.SnapOut();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SyncRibbonOptionsSelection()
        {
            if (ribbonOptionsList == null)
            {
                return;
            }

            RibbonOption option = ribbonOptionsList.Items
                .OfType<RibbonOption>()
                .First(x => x.Visibility == RibbonState);

            option.IsSelected = true;
        }

        private void UpdateWindowStates()
        {
            IsNormalized = WindowState == WindowState.Normal;
            IsMaximized = WindowState == WindowState.Maximized;
        }

        #endregion

        #region Native Window Support

        // ReSharper disable InconsistentNaming
        // ReSharper disable UnusedMember.Local
        // ReSharper disable FieldCanBeMadeReadOnly.Local
        // ReSharper disable MemberCanBePrivate.Local

        private const int MONITOR_DEFAULTTONEAREST = 2;
        private const int WINDOWPOSCHANGING = 0x0046;

        private void UpdateWindowBounds()
        {
            if (WindowState == WindowState.Normal)
            {
                BorderThickness = new Thickness(1);
                FramePadding = new Thickness(0);
                return;
            }

            IntPtr monitor = SafeNativeMethods.MonitorFromWindow(hwndSource.Handle, MONITOR_DEFAULTTONEAREST);
            var info = new MONITORINFOEX {cbSize = Marshal.SizeOf(typeof (MONITORINFOEX))};
            SafeNativeMethods.GetMonitorInfo(new HandleRef(this, monitor), ref info);

            if (hwndSource.CompositionTarget == null)
            {
                throw new NullReferenceException("_hwndSource.CompositionTarget == null");
            }

            // All points queried from the Win32 API are not DPI aware.
            // Since WPF is DPI aware, one WPF pixel does not necessarily correspond to a device pixel.
            // In order to convert device pixels (Win32 API) into screen independent pixels (WPF), 
            // the following transformation must be applied to points queried using the Win32 API.
            Matrix matrix = hwndSource.CompositionTarget.TransformFromDevice;

            // Not DPI aware
            RECT workingArea = info.rcWork;
            RECT monitorRect = info.rcMonitor;

            // DPI aware
            //var bounds = matrix.Transform(new Point(workingArea.right - workingArea.left,
            //        workingArea.bottom - workingArea.top));

            // DPI aware
            Vector origin = matrix.Transform(new Point(workingArea.left, workingArea.top))
                            - matrix.Transform(new Point(monitorRect.left, monitorRect.top));

            // Calulates the offset required to adjust the anchor position for the missing client frame border.
            // An additional -1 must be added to the top to perfectly fit the screen, reason is of yet unknown.
            double left = SystemParameters.ResizeFrameVerticalBorderWidth + origin.X;
            double top = SystemParameters.ResizeFrameHorizontalBorderHeight
                         - SystemParameters.CaptionHeight + origin.Y - 1;

            FramePadding = new Thickness(left, top, 0, 0);
            //MaxWidth = bounds.X + SystemParameters.ResizeFrameVerticalBorderWidth + SystemParameters.WindowNonClientFrameThickness.Right;
            //MaxHeight = bounds.Y + SystemParameters.ResizeFrameHorizontalBorderHeight + SystemParameters.WindowNonClientFrameThickness.Bottom;
            BorderThickness = new Thickness(0);
        }

        #region Nested type: MONITORINFOEX

        [StructLayout(LayoutKind.Sequential)]
        public struct MONITORINFOEX
        {
            public int cbSize;
            public RECT rcMonitor; // Total area
            public RECT rcWork; // Working area
            public int dwFlags;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)] public char[] szDevice;
        }

        #endregion

        #region Nested type: RECT

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        #endregion

        #region Nested type: SafeNativeMethods

        [SuppressUnmanagedCodeSecurity]
        private static class SafeNativeMethods
        {
            // To get a handle to the specified monitor
            [DllImport("user32.dll")]
            public static extern IntPtr MonitorFromWindow(IntPtr hwnd, int dwFlags);

            [DllImport("user32.dll")]
            public static extern bool GetMonitorInfo(HandleRef hmonitor, ref MONITORINFOEX monitorInfo);
        }

        #endregion

        // ReSharper restore InconsistentNaming
        // ReSharper restore UnusedMember.Local
        // ReSharper restore FieldCanBeMadeReadOnly.Local
        // ReSharper restore MemberCanBePrivate.Local

        #endregion
    }
}