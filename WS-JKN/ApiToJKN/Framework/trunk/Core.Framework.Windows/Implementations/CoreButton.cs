using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Core.Framework.Helper;
using Core.Framework.Helper.Contracts;
using Core.Framework.Model;
using Core.Framework.Windows.Contracts;
using Core.Framework.Windows.Controls;
using Core.Framework.Windows.Helper;

namespace Core.Framework.Windows.Implementations
{
    public class CoreButton : Button, IControlToUseGenericCalling, IValueElement, IControlAuthentication
    {
        #region Static Fields

        public static readonly DependencyProperty AutoBusyWhenClickProperty =
            DependencyProperty.Register(
                "AutoBusyWhenClick",
                typeof(bool),
                typeof(CoreButton),
                new UIPropertyMetadata(false));

        public static readonly DependencyProperty FormGridProperty = DependencyProperty.Register(
            "FormGrid",
            typeof(object),
            typeof(CoreButton),
            new UIPropertyMetadata(0));

        #endregion

        #region Fields

        private bool isBusy;

        private object tempContont;

        #endregion

        #region Constructors and Destructors

        public object[] ArgsObjects { get; set; }
        public event EventHandler PreviewBeforeCommonExecute;

        public string ModuleName
        {
            get { return (string)GetValue(ModuleNameProperty); }
            set { SetValue(ModuleNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ModuleName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModuleNameProperty =
            DependencyProperty.Register("ModuleName", typeof(string), typeof(CoreButton), new UIPropertyMetadata(""));

        public string CommonName
        {
            get { return (string)GetValue(CommonNameProperty); }
            set { SetValue(CommonNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommonName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommonNameProperty =
            DependencyProperty.Register("CommonName", typeof(string), typeof(CoreButton), new UIPropertyMetadata(""));



        public CoreButton()
        {
            var rDictionary = new ResourceDictionary
            {
                Source = new Uri(
                                          string.Format("/Core.Framework.Windows;component/Styles/Controls.Buttons.xaml"),
                                          UriKind.Relative)
            };
            Style = rDictionary["MetroButton"] as Style;
            Loaded += OnLoaded;
            IsApplycolor = true;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();            
        }

        public bool IsApplycolor { get; set; }

        private XElement SetMenuEditor(XElement doc, string tagXml)
        {
            var element = doc.Element(tagXml);
            return element != null ? element : null;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            //Manager.RegisterFormGrid(this);
            //if (!(string.IsNullOrEmpty(CommonName) && string.IsNullOrEmpty(ModuleName)))
            //    Visibility = Manager.CheckModule(CommonName, ModuleName) ? Visibility.Visible : Visibility.Collapsed            

            try
            {
                if (IsApplycolor)
                {
                    var pathFile = Path.GetDirectoryName(Application.ResourceAssembly.Location) + "\\";
                    const string fileName = "Tema.xml";
                    var readFile = XDocument.Load(pathFile + fileName);
                    var doc = readFile.Descendants("tema").FirstOrDefault();

                    var temaDefault = SetMenuEditor(SetMenuEditor(doc, "tombol"), "default").Value.Trim();
                    var temaSimpan = SetMenuEditor(SetMenuEditor(doc, "tombol"), "simpan").Value.Trim();
                    var temaBatal = SetMenuEditor(SetMenuEditor(doc, "tombol"), "batal").Value.Trim();
                    var temaTutup = SetMenuEditor(SetMenuEditor(doc, "tombol"), "tutup").Value.Trim();

                    //BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF7400"));
                    //Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF7400"));
                    //BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#673AB7"));
                    //Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#673AB7"));
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(temaDefault));
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(temaDefault));
                    if (IsClearForm)
                    {
                        //BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#673AB7"));
                        //Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#673AB7"));
                        //BorderBrush = new SolidColorBrush(Colors.Orange);
                        //Background = new SolidColorBrush(Colors.Orange);
                        BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(temaBatal));
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(temaBatal));
                        if (Content is string)
                        {
                            var text = new TextBlock() { Text = Content.ToString().Replace("_", "").ToUpper(), Margin = new Thickness(2, 0, 0, 0), Foreground = new SolidColorBrush(Colors.White) };
                            var panel = new StackPanel() { Orientation = Orientation.Horizontal };
                            var ico = new IconClear() { Width = 15, Height = 15 };
                            panel.Children.Add(ico);
                            panel.Children.Add(text);
                            Content = panel;
                        }
                    }
                    else if (IsClose)
                    {
                        //BorderBrush = new SolidColorBrush(Colors.OrangeRed);
                        //Background = new SolidColorBrush(Colors.OrangeRed);
                        //BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF262626"));
                        //Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF262626"));
                        //BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DB033D"));
                        //Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DB033D"));
                        BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(temaTutup));
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(temaTutup));
                        if (Content is string)
                        {
                            var text = new TextBlock() { Text = Content.ToString().Replace("_", "").ToUpper(), Margin = new Thickness(2, 0, 0, 0), Foreground = new SolidColorBrush(Colors.White) };
                            var panel = new StackPanel() { Orientation = Orientation.Horizontal };
                            var ico = new IconClose() { Width = 15, Height = 15 };
                            panel.Children.Add(ico);
                            panel.Children.Add(text);
                            Content = panel;
                        }
                    }
                    else if (IsRemove)
                    {
                        if (Content is string)
                        {
                            var text = new TextBlock() { Text = Content.ToString().Replace("_", "").ToUpper(), Margin = new Thickness(2, 0, 0, 0), Foreground = new SolidColorBrush(Colors.White) };
                            var panel = new StackPanel() { Orientation = Orientation.Horizontal };
                            var ico = new IconRemove() { Width = 15, Height = 15 };
                            panel.Children.Add(ico);
                            panel.Children.Add(text);
                            Content = panel;
                        }
                    }
                    else if (IsValidate)
                    {
                        //BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF7400"));
                        //Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF7400"));
                        BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(temaSimpan));
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(temaSimpan));
                        if (Content is string)
                        {
                            if (!Content.ToString().ToLower().Contains("login"))
                            {
                                var text = new TextBlock() { Text = Content.ToString().Replace("_", "").ToUpper(), Margin = new Thickness(2, 0, 0, 0), Foreground = new SolidColorBrush(Colors.White) };
                                var panel = new StackPanel() { Orientation = Orientation.Horizontal };
                                var ico = new IconSave() { Width = 15, Height = 15 };
                                panel.Children.Add(ico);
                                panel.Children.Add(text);
                                Content = panel;
                            }
                        }
                    }


                    if (Content is string)
                    {
                        if (Content.ToString().ToLower().Equals("keluar"))
                        {
                            //BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DB033D"));
                            //Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DB033D"));
                            BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(temaTutup));
                            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(temaTutup));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                
            }                 

            try
            {
                if (!string.IsNullOrEmpty(Departemen))
                {
                    if (Departemen.Contains(","))
                    {
                        var split = Departemen.Split(',');
                        foreach (var departemen in split)
                        {
                            var ruanganLogin = BaseDependency.Get<IHistoryLoginRepository>();
                            if (ruanganLogin.Departemen == departemen)
                            {
                                Visibility = Visibility.Visible;
                                break;
                            }
                            else
                            {
                                Visibility = Visibility.Collapsed;
                            }
                        }
                    }
                    else
                    {
                        var ruanganLogin = BaseDependency.Get<IHistoryLoginRepository>();
                        if (ruanganLogin.Departemen == Departemen)
                        {
                            Visibility = Visibility.Visible;
                        }
                        else
                        {
                            Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        #endregion

        #region Public Events

        [Category("Behavior")]
        public event RoutedEventHandler AfterExecuteControl;

        #endregion

        #region Public Properties

        public bool AutoBusyWhenClick
        {
            get { return (bool)GetValue(AutoBusyWhenClickProperty); }
            set { SetValue(AutoBusyWhenClickProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoBusyWhenClick.  This enables animation, styling, binding, etc...

        public object FormGrid
        {
            get { return GetValue(FormGridProperty); }
            set { SetValue(FormGridProperty, value); }
        }

        public bool UseAuthentication
        {
            get
            {
                return (bool)GetValue(UseAuthenticationProperty);
            }
            set { SetValue(UseAuthenticationProperty, value); }
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
                    return Name;
                return str + "-" + Name;
            }
        }

        // Using a DependencyProperty as the backing store for UseAuthentication.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseAuthenticationProperty =
            DependencyProperty.Register("UseAuthentication", typeof(bool), typeof(CoreButton), new UIPropertyMetadata(false, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {

        }


        public bool IsBusy
        {
            get
            {

                if (Content is System.Windows.Controls.Viewbox)
                    if ((Content as System.Windows.Controls.Viewbox).Child is ProgressRing)
                        return true;
                return isBusy;
            }
            set
            {
                isBusy = value;
                if (isBusy)
                {
                    if (Content is Viewbox)
                        if ((Content as Viewbox).Child is ProgressRing)
                            return;
                    tempContont = Content;
                    var viewbox = new Viewbox
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        Width = 16,
                        Stretch = Stretch.Fill,
                        Height = 16,
                        Child = new ProgressRing
                        {
                            IsActive = true,
                            Height = 10,
                            Width = 10,
                            Foreground = new SolidColorBrush(Colors.White)
                        },
                    };

                    Content = viewbox;
                    //IsEnabled = false;
                }
                else
                {
                    if (tempContont != null)
                    {
                        Manager.Timeout(Dispatcher, () => Content = tempContont);
                    }
                    //IsEnabled = true;
                }
            }
        }

        public bool IsClearForm { get; set; }

        public bool IsClose { get; set; }

        public bool IsValidate { get; set; }

        public bool IsRemove { get; set; }

        #region IControlToUseGenericCalling Members

        public string ControlNameSpace { get; set; }

        #endregion

        #region IValueElement Members

        public bool CanFocus
        {
            get { return true; }
        }

        public string Key { get; protected set; }

        public object Value { get; set; }

        #endregion

        public string Departemen
        {
            get { return (string)GetValue(DepartemenProperty); }
            set { SetValue(DepartemenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DepartemenProperty =
            DependencyProperty.Register("Departemen", typeof(string), typeof(CoreButton), new PropertyMetadata(""));

        #endregion

        #region Public Methods and Operators

        public void ExecuteControl()
        {
            Manager.ExecuteGenericModule(ControlNameSpace);
            OnAfterExecuteControl();
        }

        public void OnAfterExecuteControl(RoutedEventArgs e = null)
        {
            RoutedEventHandler handler = AfterExecuteControl;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Methods              

        public bool IsSkip { get; set; }
        public event EventHandler<ItemEventArgs<bool>> PreviewClick;
        protected virtual void OnPreviewClick(ItemEventArgs<bool> e)
        {
            PreviewClick?.Invoke(this, e);
        }

        protected override void OnClick()
        {
            OnPreviewClick(new ItemEventArgs<bool>(IsSkip));
            if (IsSkip) return;
            OnPreviewBeforeCommonExecute();
            if (!(string.IsNullOrEmpty(CommonName) && string.IsNullOrEmpty(ModuleName)))
                Manager.CallModule(CommonName, ModuleName, ArgsObjects);
            if (IsBusy)
            {
                return;
            }

            if (!string.IsNullOrEmpty(ControlNameSpace))
            {
                ExecuteControl();
            }
            else
            {
                bool valid = true;
                if (FormGrid != null)
                {
                    if (IsClose)
                    {
                        var userControl = Manager.FindVisualParentContract<IBackHistoryControl>(this);
                        if (userControl == null)
                        {
                            var panelManager = Manager.FindVisualParentContract<ICloseControl>(this);
                            if (panelManager != null)
                            {
                                var disposable = panelManager as IDisposable;
                                if (disposable != null) disposable.Dispose();
                                panelManager.CloseControl();
                                //if(panelManager is FrameworkElement)
                            }
                            base.OnClick();
                            return;
                        }
                        {
                            if (userControl.Back(MustReload) == null || userControl.Back(MustReload).GetType().Name.Equals("ForbiddenView"))
                            {
                                var panelManager = Manager.FindVisualParentContract<ICloseControl>(this);
                                if (panelManager != null)
                                {
                                    var disposable = panelManager as IDisposable;
                                    if (disposable != null) disposable.Dispose();
                                    panelManager.CloseControl();
                                }
                            }

                            base.OnClick();
                            return;
                        }
                    }
                    DependencyObject panel = null;
                    if (FormGrid is DependencyObject)
                    {
                        panel = FormGrid as DependencyObject;
                    }
                    if (FormGrid is string)
                    {
                    }
                    if (panel == null)
                    {
                        panel = Manager.FindVisualParent<FormGrid>(this);
                    }
                    IEnumerable<IValidateControl> children =
                        Manager.FindVisualChildren<FrameworkElement>(panel).OfType<IValidateControl>();
                    if (panel is IValueElement && IsClearForm)
                    {
                        if (panel is FormGrid)
                        {
                            if (!(panel as FormGrid).IsCannotClearFrom)
                                (panel as IValueElement).Value = null;
                        }
                        else (panel as IValueElement).Value = null;
                    }
                    foreach (IValidateControl validateControl in children)
                    {
                        //var requiredControl = Manager.FindVisualParent<RequiredGrid>(validateControl as FrameworkElement);
                        //if (requiredControl != null)
                        //{
                        //    if (!requiredControl.IsRequired)
                        //        continue;
                        //}
                        if (IsValidate)
                        {
                            if (validateControl.IsRequired)
                            {
                                if (validateControl.IsNull && (validateControl as FrameworkElement).IsVisible)
                                {
                                    if (valid)
                                    {
                                        valid = false;
                                        var tabItem =
                                            Manager.FindVisualParent<TabItem>((validateControl as FrameworkElement));
                                        if (tabItem != null)
                                        {
                                            var tabControl = tabItem.Parent as TabControl;
                                            if (tabControl != null)
                                            {
                                                tabControl.SelectedItem = tabItem;
                                            }
                                        }
                                        validateControl.FocusControl();
                                    }
                                    validateControl.IsError = true;
                                }
                                else
                                {
                                    validateControl.IsError = false;
                                }
                            }
                        }
                        else if (IsClearForm)
                        {
                            if (!string.IsNullOrEmpty(ExcpetedGridForm))
                            {
                                foreach (var name in ExcpetedGridForm.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    var frameworkElement = validateControl as FrameworkElement;
                                    {
                                        if (frameworkElement != null && frameworkElement.Name.Equals(name))
                                            continue;
                                        if (frameworkElement.FindChildren<FrameworkElement>().Any(n => n.Name.Equals(name)))
                                            continue;
                                    }
                                    if ((validateControl as FrameworkElement).TryFindParent<FrameworkElement>(name) == null)
                                    {

                                        if (validateControl is IValueElement)
                                        {
                                            if (validateControl is CoreDatePicker)
                                                (validateControl as CoreDatePicker).Value = DateTime.Now;
                                            else
                                                (validateControl as IValueElement).Value = null;
                                        }
                                        validateControl.IsError = false;
                                    }
                                }
                            }
                            else
                            {
                                if (validateControl is IValueElement)
                                {
                                    if (validateControl is CoreDatePicker)
                                        (validateControl as CoreDatePicker).Value = DateTime.Now;
                                    else
                                        (validateControl as IValueElement).Value = null;
                                }
                                validateControl.IsError = false;
                            }

                        }
                    }

                    if (IsClearForm)
                    {
                        //var gridChildren =
                        //    Manager.FindVisualChildren<FrameworkElement>(panel).OfType<IGridViewControl>();
                        //foreach (var gridViewControl in gridChildren)
                        //{
                        //    if (gridViewControl is InsertDataGrid)
                        //        (gridViewControl as InsertDataGrid).DataSource = null;
                        //    else if (gridViewControl is CoreListView)
                        //        (gridViewControl as CoreListView).DataSource = null;
                        //    else if (gridViewControl is CoreDataGrid)
                        //        (gridViewControl as CoreDataGrid).DataSource = null;
                        //}
                    }
                }

                if (valid)
                {
                    if (IsValidate)
                    {
                        if (AutoBusyWhenClick)
                            IsBusy = true;
                        base.OnClick();
                        IsBusy = false;
                        //ThreadPool.QueueUserWorkItem(
                        //    delegate
                        //    {
                        //        Thread.Sleep(100);
                        //        Manager.Timeout(
                        //            Dispatcher,
                        //            delegate
                        //            {

                        //            });
                        //    });
                    }
                    else
                    {
                        if (AutoBusyWhenClick)
                        {
                            if (AutoBusyWhenClick)
                                IsBusy = true;
                            ThreadPool.QueueUserWorkItem(
                                delegate
                                {
                                    Thread.Sleep(100);
                                    Manager.Timeout(
                                        Dispatcher,
                                        delegate
                                        {
                                            base.OnClick();
                                            if (AutoBusyWhenClick)
                                                IsBusy = false;

                                        });
                                });
                        }
                        else
                        {
                            //IsBusy = false;

                            base.OnClick();

                        }
                    }
                }
            }
        }

        #endregion

        public string ExcpetedGridForm { get; set; }
        // Using a DependencyProperty as the backing store for FormGrid.  This enables animation, styling, binding, etc...

        public bool MustReload { get; set; }

        protected virtual void OnPreviewBeforeCommonExecute()
        {
            var handler = PreviewBeforeCommonExecute;
            if (handler != null) handler(this, EventArgs.Empty);
        }


    }
}