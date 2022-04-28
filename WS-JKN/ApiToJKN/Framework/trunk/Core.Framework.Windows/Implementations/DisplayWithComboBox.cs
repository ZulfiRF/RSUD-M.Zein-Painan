using System.Linq;
using System.Windows.Input;
using Core.Framework.Helper;

namespace Core.Framework.Windows.Implementations
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;

    using Core.Framework.Helper.Contracts;
    using Core.Framework.Windows.Contracts;
    using Core.Framework.Windows.Helper;

    /// <summary>
    ///     Interaction logic for DisplayWithComboBox.xaml
    /// </summary>
    [TemplatePart(Name = PART_ComboBox, Type = typeof(CoreComboBox))]
    public class DisplayWithComboBox : UserControl, IValidateControl, IValueElement, IDisplyaValueElement
    {
        // Using a DependencyProperty as the backing store for ControlVerticalPositionAligment.  This enables animation, styling, binding, etc...

        #region Constants

        private const string PART_ComboBox = "TbContent";

        #endregion

        #region Static Fields

        public static readonly DependencyProperty ControlVerticalPositionAligmentProperty =
            DependencyProperty.Register(
                "ControlVerticalPositionAligment",
                typeof(VerticalAlignment),
                typeof(DisplayWithComboBox),
                new UIPropertyMetadata(VerticalAlignment.Top));

        // Using a DependencyProperty as the backing store for DisplayMemberPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register(
                "DisplayMemberPath",
                typeof(string),
                typeof(DisplayWithComboBox),
                new UIPropertyMetadata(null, ChangeDisplayMemberPath));

        public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register(
            "DisplayText",
            typeof(string),
            typeof(DisplayWithComboBox),
            new UIPropertyMetadata("Title"));

        public static readonly DependencyProperty IsErrorProperty = DependencyProperty.Register(
            "IsError",
            typeof(bool),
            typeof(DisplayWithComboBox),
            new UIPropertyMetadata(false));

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(IEnumerable),
            typeof(DisplayWithComboBox),
            new UIPropertyMetadata(null, ChangeItemsSource));

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            "SelectedItem",
            typeof(object),
            typeof(DisplayWithComboBox),
            new UIPropertyMetadata(null, ChangeSelectedItem));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(DisplayWithComboBox),
            new UIPropertyMetadata("", PropertyChangedCallback));

        public static readonly DependencyProperty TextVerticalPositionAligmentProperty =
            DependencyProperty.Register(
                "TextVerticalPositionAligment",
                typeof(VerticalAlignment),
                typeof(DisplayWithComboBox),
                new UIPropertyMetadata(VerticalAlignment.Top));

        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register(
            "Watermark",
            typeof(string),
            typeof(DisplayWithComboBox),
            new UIPropertyMetadata("Type Here"));

        public static readonly DependencyProperty WitdhDisplayTextProperty =
            DependencyProperty.Register(
                "WitdhDisplayText",
                typeof(GridLength),
                typeof(DisplayWithComboBox),
                new PropertyMetadata(new GridLength(0.5, GridUnitType.Star)));

        #endregion

        #region Fields

        private bool isRequired;

        private string models;

        #endregion

        #region Constructors and Destructors


        public DisplayWithComboBox()
        {
            Manager.RegisterFormGrid(this);
            var rDictionary = new ResourceDictionary();
            rDictionary.Source =
                new Uri(
                    string.Format("/Core.Framework.Windows;component/Styles/Controls.DisplayWithComboBox.xaml"),
                    UriKind.Relative);
            this.Style = rDictionary["DisplayMetroInComboBox"] as Style;
        }

        #endregion

        #region Public Events

        public event EventHandler<HandleArgs> BeforeValidate;

        [Category("Behavior")]
        public event SelectionChangedEventHandler SelectionChanged;

        public event TextChangedEventHandler TextChanged;

        #endregion

        #region Public Properties

        public VerticalAlignment ControlVerticalPositionAligment
        {
            get
            {
                return (VerticalAlignment)this.GetValue(ControlVerticalPositionAligmentProperty);
            }
            set
            {
                this.SetValue(ControlVerticalPositionAligmentProperty, value);
            }
        }

        public string DisplayMemberPath
        {
            get
            {
                return (string)this.GetValue(DisplayMemberPathProperty);
            }
            set
            {
                this.SetValue(DisplayMemberPathProperty, value);
            }
        }

        public string DisplayText
        {
            get
            {
                return (string)this.GetValue(DisplayTextProperty);
            }
            set
            {
                this.SetValue(DisplayTextProperty, value);
            }
        }

        public string DomainNameSpaces { get; set; }

        public bool IsError
        {
            get
            {
                return (bool)this.GetValue(IsErrorProperty);
            }
            private set
            {
                this.SetValue(IsErrorProperty, value);
            }
        }

        public bool IsNull
        {
            get
            {
                var args = new HandleArgs();
                this.OnBeforeValidate(args);
                if (args.Handled)
                {
                    return true;
                }
                if (this.PartComboBox != null)
                {
                    return this.PartComboBox.IsNull;
                }
                return false;
            }
        }

        public bool IsRequired
        {
            get
            {
                if (this.PartComboBox != null)
                {
                    return this.PartComboBox.IsRequired;
                }
                return false;
            }
            set
            {
                this.isRequired = value;
            }
        }

        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)this.GetValue(ItemsSourceProperty);
            }
            set
            {
                this.SetValue(ItemsSourceProperty, value);
            }
        }

        public bool CanFocus
        {
            get { return true; }
        }

        public string Key { get; set; }
        public string ValuePath { get; set; }

        public string Models
        {
            get
            {
                return this.models;
            }
            set
            {
                this.models = value;
                if (string.IsNullOrEmpty(this.models))
                {
                }
            }
        }

        public SearchType SearchTypeWith { get; set; }

        public object SelectedItem
        {
            get
            {
                return this.GetValue(SelectedItemProperty);
            }
            set
            {
                this.SetValue(SelectedItemProperty, value);
                if (value == null)
                {
                    if (PartComboBox != null)
                        PartComboBox.Value = null;
                }
                else
                {
                    if (!string.IsNullOrEmpty(PartComboBox.ValuePath))
                    {
                        if (PartComboBox.Value!=null && PartComboBox.Value.Equals(value.GetType().GetProperty(PartComboBox.ValuePath).GetValue(value, null)))
                            return;
                    }
                    PartComboBox.Value = value;
                }

            }
        }

        public bool SkipAutoFocus { get; set; }

        public string Text
        {
            get
            {
                return (string)this.GetValue(TextProperty);
            }
            set
            {
                this.SetValue(TextProperty, value);
            }
        }

        public VerticalAlignment TextVerticalPositionAligment
        {
            get
            {
                return (VerticalAlignment)this.GetValue(TextVerticalPositionAligmentProperty);
            }
            set
            {
                this.SetValue(TextVerticalPositionAligmentProperty, value);
            }
        }

        public bool UsingSearchByFramework { get; set; }

        public object Value
        {
            get
            {
                try
                {
                    if (this.SelectedItem == null && string.IsNullOrEmpty(PartComboBox.SelectedText))
                    {
                        return "";
                    }
                    if (PartComboBox.Value != null)
                        return PartComboBox.Value;
                    if (string.IsNullOrEmpty(this.Models))
                    {
                        return this.SelectedItem.GetType().GetProperty(this.Key).GetValue(this.SelectedItem, null);
                    }
                    return this.SelectedItem.GetType().GetProperty("Key").GetValue(this.SelectedItem, null);
                }
                catch (Exception)
                {
                    return this.SelectedItem;
                }
            }
            set
            {
                if (this.SelectedItem == null)
                {
                    return;
                }
                this.SelectedItem.GetType().GetProperty(this.Key).SetValue(this.SelectedItem, value, null);
            }
        }

        public void ControlFocus()
        {
            if (PartComboBox != null)
                PartComboBox.Focus();
        }

        public string Watermark
        {
            get
            {
                return (string)this.GetValue(WatermarkProperty);
            }
            set
            {
                this.SetValue(WatermarkProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...

        public GridLength WitdhDisplayText
        {
            get
            {
                return (GridLength)this.GetValue(WitdhDisplayTextProperty);
            }
            set
            {
                this.SetValue(WitdhDisplayTextProperty, value);
            }
        }

        #endregion

        #region Explicit Interface Properties

        bool IValidateControl.IsError
        {
            get
            {
                return this.IsError;
            }
            set
            {
                this.IsError = value;
            }
        }

        #endregion

        // Using a DependencyProperty as the backing store for WitdhDisplayText.  This enables animation, styling, binding, etc...

        #region Properties

        internal CoreComboBox PartComboBox { get; set; }

        #endregion

        #region Public Methods and Operators

        public void ClearValueControl()
        {
            this.SelectedItem = null;
            this.Text = "";
        }

        public void FocusControl()
        {
            ThreadPool.QueueUserWorkItem(state => Manager.Timeout(this.Dispatcher, () => this.PartComboBox.Focus()));
        }

        public override void OnApplyTemplate()
        {
            this.PartComboBox = this.GetTemplateChild(PART_ComboBox) as CoreComboBox;
            base.OnApplyTemplate();
            var bindingText = new Binding("SelectedItem");
            bindingText.Mode = BindingMode.TwoWay;
            bindingText.Source = this;

            //var binding = new Binding("SelectedItem");
            //binding.Mode = BindingMode.TwoWay;
            //binding.Source = PART_ComboBox;
            //BindingOperations.SetBinding(this, SelectedItemProperty, binding);
            //var bindingItemsSource = new Binding("ItemsSource");
            //bindingItemsSource.Mode = BindingMode.OneWay;
            //bindingItemsSource.Source = this;
            if (this.PartComboBox != null)
            {
                //   BindingOperations.SetBinding(this.PartComboBox, Selector.SelectedItemProperty, bindingText);
                //BindingOperations.SetBinding(PartComboBox, ItemsControl.ItemsSourceProperty, bindingItemsSource);
                this.PartComboBox.DisplayMemberPath = this.DisplayMemberPath;
                //this.PartComboBox.ValuePath = this.Key;
                this.PartComboBox.ValuePath = ValuePath;
                this.PartComboBox.IsRequired = this.isRequired;
                if (!string.IsNullOrEmpty(this.DomainNameSpaces))
                {
                    this.PartComboBox.DomainNameSpaces = this.DomainNameSpaces;
                }
                if (!string.IsNullOrEmpty(this.Models))
                {
                    this.PartComboBox.Models = this.Models;
                }
                this.PartComboBox.ItemsSource = this.ItemsSource;
                this.PartComboBox.SelectedItem = this.SelectedItem;
                this.PartComboBox.TextChanged += this.PartTextBoxOnTextChanged;
                PartComboBox.KeyDown += PartComboBoxOnKeyDown;
                PartComboBox.PreviewKeyDown += PartComboBoxOnPreviewKeyDown;
                this.PartComboBox.SelectionChanged += this.PartComboBoxOnSelectionChanged;
                this.PartComboBox.UsingSearchByFramework = this.UsingSearchByFramework;
                this.PartComboBox.SearchTypeWith = this.SearchTypeWith;
            }
        }

        private void PartComboBoxOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Return && !this.SkipAutoFocus)
            {
                var parent = Manager.FindVisualParent<UserControl>(this);
                if (parent != null)
                {
                    bool valid = false;
                    foreach (IDisplyaValueElement findVisualChild in Manager.FindVisualChildren<FrameworkElement>(parent).OfType<IDisplyaValueElement>())
                    {
                        if (valid && findVisualChild is IDisplyaValueElement)
                        {
                            findVisualChild.ControlFocus();
                            break;
                        }
                        if (findVisualChild.Equals(this))
                        {
                            valid = true;
                        }
                    }
                }
            }
        }

        private void PartComboBoxOnKeyDown(object sender, KeyEventArgs e)
        {
            //Mereference Ke Form Lain sesuai DomainType / DomainNameSpace
            if (e.Key == System.Windows.Input.Key.F12)
            {
                #region Mereference Ke Form Lain sesuai DomainType

                var main = Application.Current.MainWindow as IMainWindow;
                if (main != null)
                {
                    if (!string.IsNullOrEmpty(DomainNameSpaces))
                    {
                        var domain = HelperManager.GetListSearchFramework(DomainNameSpaces);
                        if (domain != null)
                            ExecuteControlForm(domain);
                        else
                        {
                            if (SourceKeyValues.KeyValues != null)
                            {
                                var typeDomain = HelperManager.GetModule(DomainNameSpaces);
                                ExecuteControlForm(typeDomain);
                            }
                        }
                    }
                }
                #endregion
            }           
        }

        private void ExecuteControlForm(Type typeDomain)
        {
            try
            {
                var keyValue = SourceKeyValues.KeyValues.FirstOrDefault(n => n.Key != null && n.Key.ToString().Replace(" ", "") == typeDomain.Name);
                if (keyValue != null)
                {
                    var coreMenuItem = keyValue.Value as CoreMenuItem;
                    if (coreMenuItem != null) coreMenuItem.ExecuteControl();
                }
                else
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
                                    var instanace = Activator.CreateInstance(type) as IGenericCalling;
                                    if (instanace != null)
                                    {
                                        try
                                        {
                                            instanace.InitView();
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
            }
            catch (Exception e)
            {
                Manager.HandleException(e, "Error ExecuteControlForm : Domain Type atau DomainNameSpace!");
            }
        }

        public void OnBeforeValidate(HandleArgs e = null)
        {
            EventHandler<HandleArgs> handler = this.BeforeValidate;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            SelectionChangedEventHandler handler = this.SelectionChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void OnTextChanged(TextChangedEventArgs e)
        {
            TextChangedEventHandler handler = this.TextChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Methods

        private static void ChangeDisplayMemberPath(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var form = dependencyObject as DisplayWithComboBox;
            if (form != null)
            {
                if (form.PartComboBox != null)
                {
                    form.PartComboBox.DisplayMemberPath = dependencyPropertyChangedEventArgs.NewValue as string;
                }
            }
        }

        private static void ChangeItemsSource(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var form = dependencyObject as DisplayWithComboBox;
            if (form != null)
            {
                if (form.PartComboBox != null)
                {
                    form.PartComboBox.ItemsSource = dependencyPropertyChangedEventArgs.NewValue as IEnumerable;
                }
            }
        }

        private static void ChangeSelectedItem(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            //var form = dependencyObject as DisplayWithComboBox;
            //if (form != null)
            //{
            //    if (form.PartComboBox != null)
            //    {
            //        form.PartComboBox.SelectedItem = dependencyPropertyChangedEventArgs.NewValue;
            //    }
            //}
        }

        private static void PropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var form = dependencyObject as DisplayWithComboBox;
            if (form != null)
            {
                if (form.PartComboBox != null)
                {
                    form.PartComboBox.Text = dependencyPropertyChangedEventArgs.NewValue as string;
                }
            }
        }

        private void DisplayWithComboBox_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.DataContext = e.NewValue;
        }

        private void PartComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            var coreComboBox = sender as CoreComboBox;
            if (coreComboBox != null)
            {
                this.SelectedItem = coreComboBox.SelectedItem;
            }
            this.OnSelectionChanged(selectionChangedEventArgs);
        }

        private void PartTextBoxOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            if (this.IsRequired)
            {
                this.IsError = string.IsNullOrEmpty(this.PartComboBox.Text);
            }
            this.OnTextChanged(textChangedEventArgs);
        }

        #endregion
    }
}