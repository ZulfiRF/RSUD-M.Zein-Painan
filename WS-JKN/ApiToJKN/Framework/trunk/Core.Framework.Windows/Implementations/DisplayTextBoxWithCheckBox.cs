using System.Linq;

namespace Core.Framework.Windows.Implementations
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;

    using Core.Framework.Windows.Contracts;
    using Core.Framework.Windows.Helper;

    /// <summary>
    ///     Interaction logic for DisplayTextBoxWithCheckBox.xaml
    /// </summary>
    [TemplatePart(Name = PartCheckbox, Type = typeof(DisplayTextBoxWithCheckBox))]
    public class DisplayTextBoxWithCheckBox : UserControl, IValidateControl, IValueElement, IDisplyaValueElement
    {
        
        // Using a DependencyProperty as the backing store for ControlVerticalPositionAligment.  This enables animation, styling, binding, etc...

        #region Constants

        private const string PART_Texbox = "TbTextboxContent";

        private const string PartCheckbox = "TbContent";

        #endregion

        #region Static Fields

        public static readonly DependencyProperty ControlVerticalPositionAligmentProperty =
            DependencyProperty.Register(
                "ControlVerticalPositionAligment",
                typeof(VerticalAlignment),
                typeof(DisplayTextBoxWithCheckBox),
                new UIPropertyMetadata(VerticalAlignment.Top));

        public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register(
            "DisplayText",
            typeof(string),
            typeof(DisplayTextBoxWithCheckBox),
            new UIPropertyMetadata("Title"));

        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(
            "IsChecked",
            typeof(bool?),
            typeof(DisplayTextBoxWithCheckBox),
            new UIPropertyMetadata(false, PropertyChangedCallback));

        public static readonly DependencyProperty IsErrorProperty = DependencyProperty.Register(
            "IsError",
            typeof(bool),
            typeof(DisplayTextBoxWithCheckBox),
            new UIPropertyMetadata(false));

        // Using a DependencyProperty as the backing store for TextVerticalPositionAligment.  This enables animation, styling, binding, etc...

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(DisplayTextBoxWithCheckBox),
            new UIPropertyMetadata("", PropertyChangedCallbackTextBox));

        public static readonly DependencyProperty TextVerticalPositionAligmentProperty =
            DependencyProperty.Register(
                "TextVerticalPositionAligment",
                typeof(VerticalAlignment),
                typeof(DisplayTextBoxWithCheckBox),
                new UIPropertyMetadata(VerticalAlignment.Top));

        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register(
            "Watermark",
            typeof(string),
            typeof(DisplayTextBoxWithCheckBox),
            new UIPropertyMetadata("Type Here"));

        public static readonly DependencyProperty WitdhDisplayTextProperty =
            DependencyProperty.Register(
                "WitdhDisplayText",
                typeof(GridLength),
                typeof(DisplayTextBoxWithCheckBox),
                new PropertyMetadata(new GridLength(0.5, GridUnitType.Star)));

        #endregion

        #region Fields

        private bool isRequired;

        private Regex regex;

        #endregion

        #region Constructors and Destructors


        public DisplayTextBoxWithCheckBox()
        {
            Manager.RegisterFormGrid(this);
            var rDictionary = new ResourceDictionary();
            rDictionary.Source =
                new Uri(
                    string.Format("/Core.Framework.Windows;component/Styles/Controls.DisplayTextBoxWithCheckBox.xaml"),
                    UriKind.Relative);
            this.Style = rDictionary["DisplayMetroInTextboxWithCheckBox"] as Style;
            this.Tag = typeof(StatusEnabledConverBack);
        }

        #endregion

        #region Public Events

        public event EventHandler<HandleArgs> BeforeValidate;

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

        public Type FilterType { get; set; }

        public bool? IsChecked
        {
            get
            {
                return (bool?)this.GetValue(IsCheckedProperty);
            }
            set
            {
                this.SetValue(IsCheckedProperty, value);
            }
        }

        public bool IsError
        {
            get
            {
                return (bool)this.GetValue(IsErrorProperty);
            }
            set
            {
                this.SetValue(IsErrorProperty, value);
            }
        }

        public bool IsNull
        {
            get
            {
                return false;
            }
        }

        private bool isReadOnly;
        public bool IsReadOnly
        {
            get
            {
                if (this.PartTextBox == null)
                    return isReadOnly;
                return this.PartTextBox.IsReadOnly;
            }
            set
            {
                if (this.PartTextBox == null)
                    isReadOnly = value;
                else
                    PartTextBox.IsReadOnly = value;
            }
        }

        public bool IsRequired
        {
            get
            {
                if (this.PartCheckBox == null)
                {
                    return this.isRequired;
                }
                return this.PartCheckBox.IsRequired;
            }
            set
            {
                this.isRequired = value;
            }
        }

        public bool CanFocus
        {
            get { return true; }
        }

        public string Key
        {
            get
            {
                return this.Name + ",StatusEnabled";
            }
        }

        public int? MaxLength { get; set; }
        public CoreCheckBox PartCheckBox { get; set; }
        public CoreTextBox PartTextBox { get; set; }

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

        public object Value
        {
            get
            {
                return new object[] { this.Text, this.IsChecked };
            }
            set
            {
                if (value == null)
                {
                    this.IsChecked = false;
                }
                if (value is bool)
                {
                    this.IsChecked = (bool)value;
                }
                else
                {
                    this.IsChecked = false;
                }
            }
        }

        // Using a DependencyProperty as the backing store for IsError.  This enables animation, styling, binding, etc...

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

        #region Public Methods and Operators

        public void ClearValueControl()
        {
            this.IsChecked = false;
            this.Text = string.Empty;
        }

        public void FocusControl()
        {
            ThreadPool.QueueUserWorkItem(state => Manager.Timeout(this.Dispatcher, () => this.PartCheckBox.Focus()));
        }

        public override void OnApplyTemplate()
        {
            this.PartCheckBox = this.GetTemplateChild(PartCheckbox) as CoreCheckBox;

            base.OnApplyTemplate();
            var bindingText = new Binding("IsChecked");
            bindingText.Mode = BindingMode.TwoWay;
            bindingText.Source = this;
            if (this.PartCheckBox != null)
            {
                BindingOperations.SetBinding(this.PartCheckBox, ToggleButton.IsCheckedProperty, bindingText);
                this.PartCheckBox.IsRequired = this.isRequired;
            }
            var readOnly = IsReadOnly;
            this.PartTextBox = this.GetTemplateChild(PART_Texbox) as CoreTextBox;
            this.PartTextBox.IsReadOnly = readOnly;
            base.OnApplyTemplate();
            bindingText = new Binding("Text");
            bindingText.Mode = BindingMode.TwoWay;
            bindingText.Source = this;
            if (this.PartTextBox != null)
            {
                BindingOperations.SetBinding(this.PartTextBox, TextBox.TextProperty, bindingText);
                this.PartTextBox.TextChanged += this.PartTextBoxOnTextChanged;
                this.PartTextBox.LostFocus += this.PartTextBoxOnLostFocus;
                this.PartTextBox.IsRequired =false;
                this.PartTextBox.IsEnabled = !this.IsReadOnly;
                PartTextBox.KeyDown += PartTextBoxOnKeyDown;
                this.PartTextBox.PreviewTextInput += this.PartTextBoxOnPreviewTextInput;
                if (this.MaxLength.HasValue)
                {
                    this.PartTextBox.MaxLength = this.MaxLength.Value;
                }
            }
        }
        public void ControlFocus()
        {
            if (PartTextBox != null)
                PartTextBox.Focus();
        }
    
        private void PartTextBoxOnKeyDown(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(e);
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

        private static void PropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var form = dependencyObject as DisplayTextBoxWithCheckBox;
            if (form != null)
            {
                if (form.PartCheckBox != null)
                {
                    form.PartCheckBox.IsChecked = dependencyPropertyChangedEventArgs.NewValue as bool?;
                }
            }
        }

        private static void PropertyChangedCallbackTextBox(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var form = dependencyObject as DisplayTextBoxWithCheckBox;
            if (form != null)
            {
                if (form.PartTextBox != null)
                {
                    form.PartTextBox.Text = dependencyPropertyChangedEventArgs.NewValue as string;
                }
            }
        }

        private void PartTextBoxOnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            if (this.IsRequired)
            {
                if (this.PartTextBox != null)
                {
                    this.IsError = string.IsNullOrEmpty(this.PartTextBox.Text);
                }
            }
        }

        private void PartTextBoxOnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (this.FilterType != null)
            {
                if (this.FilterType.Name.Contains("Int"))
                {
                    this.regex = new Regex(@"^[0-9]$");
                }
                else if (this.FilterType.Name.Contains("double") || this.FilterType.Name.Contains("float")
                         || this.FilterType.Name.Contains("decimal"))
                {
                    this.regex = new Regex(@"^[0-9\.]$");
                }
                else
                {
                    this.regex = null;
                }
            }
            if (this.regex != null && !this.regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
            else
            {
                base.OnPreviewTextInput(e);
            }
        }

        private void PartTextBoxOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            if (this.IsRequired)
            {
                if (this.PartTextBox != null)
                {
                    this.IsError = string.IsNullOrEmpty(this.PartTextBox.Text);
                }
            }
            this.OnTextChanged(textChangedEventArgs);
        }

        #endregion

        // Using a DependencyProperty as the backing store for WitdhDisplayText.  This enables animation, styling, binding, etc...

        public bool IsPrimary { get; set; }

        protected virtual void OnBeforeValidate(HandleArgs e)
        {
            var handler = BeforeValidate;
            if (handler != null) handler(this, e);
        }
    }
}