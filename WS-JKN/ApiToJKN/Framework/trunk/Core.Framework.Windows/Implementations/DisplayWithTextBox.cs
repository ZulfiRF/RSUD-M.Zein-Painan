using System.Linq;
using System.Windows.Input;

namespace Core.Framework.Windows.Implementations
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    using Core.Framework.Windows.Contracts;
    using Core.Framework.Windows.Helper;

    /// <summary>
    ///     Interaction logic for DisplayWithTextBox.xaml
    /// </summary>
    [TemplatePart(Name = PART_Texbox, Type = typeof(CoreTextBox))]
    public class DisplayWithTextBox : UserControl, IValidateControl, IValueElement, IDisplyaValueElement
    {
        // Using a DependencyProperty as the backing store for ControlVerticalPositionAligment.  This enables animation, styling, binding, etc...

        #region Constants

        private const string PART_Texbox = "TbContent";

        private const string PART_TextBlock = "LbTitle";

        #endregion

        #region Static Fields

        public static readonly DependencyProperty ControlVerticalPositionAligmentProperty =
            DependencyProperty.Register(
                "ControlVerticalPositionAligment",
                typeof(VerticalAlignment),
                typeof(DisplayWithTextBox),
                new UIPropertyMetadata(VerticalAlignment.Top));

        // Using a DependencyProperty as the backing store for DisplayText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register(
            "DisplayText",
            typeof(string),
            typeof(DisplayWithTextBox),
            new UIPropertyMetadata("Title"));

        // Using a DependencyProperty as the backing store for IsError.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsErrorProperty = DependencyProperty.Register(
            "IsError",
            typeof(bool),
            typeof(DisplayWithTextBox),
            new UIPropertyMetadata(false));

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(DisplayWithTextBox),
            new UIPropertyMetadata("", PropertyChangedCallback));

        // Using a DependencyProperty as the backing store for TextVerticalPositionAligment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextVerticalPositionAligmentProperty =
            DependencyProperty.Register(
                "TextVerticalPositionAligment",
                typeof(VerticalAlignment),
                typeof(DisplayWithTextBox),
                new UIPropertyMetadata(VerticalAlignment.Top));

        // Using a DependencyProperty as the backing store for Watermark.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register(
            "Watermark",
            typeof(string),
            typeof(DisplayWithTextBox),
            new UIPropertyMetadata("Type Here"));

        // Using a DependencyProperty as the backing store for WitdhDisplayText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WitdhDisplayTextProperty =
            DependencyProperty.Register(
                "WitdhDisplayText",
                typeof(GridLength),
                typeof(DisplayWithTextBox),
                new PropertyMetadata(new GridLength(0.5, GridUnitType.Star)));

        #endregion

        #region Fields

        private bool isRequired;

        private int? maxLength;

        private decimal maxNumber;


        //public Type FilterType { get; set; }

        private Type type;

        #endregion

        #region Constructors and Destructors


        public DisplayWithTextBox()
        {
            Manager.RegisterFormGrid(this);
            var rDictionary = new ResourceDictionary();
            rDictionary.Source =
                new Uri(
                    string.Format("/Core.Framework.Windows;component/Styles/Controls.DisplayWithTextbox.xaml"),
                    UriKind.Relative);
            this.Style = rDictionary["DisplayMetroInTextbox"] as Style;
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

        public Type FilterType
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
                if (this.type == typeof(Int16) || this.type == typeof(short?))
                {
                    this.MaxLength = 18;
                    this.MaxNumber = Int16.MaxValue;
                }
                else if (this.type == typeof(Byte) || this.type == typeof(byte?))
                {
                    this.MaxNumber = Byte.MaxValue;
                    this.MaxLength = 18;
                }
                else if (this.type == typeof(Int32) || this.type == typeof(Int32))
                {
                    this.MaxNumber = Int32.MaxValue;
                    this.MaxLength = 18;
                }
                else if (this.type == typeof(Int64) || this.type == typeof(Int64?))
                {
                    this.MaxNumber = Int64.MaxValue;
                    this.MaxLength = 18;
                }
                else if (this.type.FullName.Contains("double") || this.type.FullName.Contains("float")
                         || this.type.FullName.Contains("decimal"))
                {
                    this.MaxLength = 18;
                }
                else if (this.type.FullName.Contains("Char"))
                {
                    this.MaxLength = 1;
                }
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
                if (this.IsError == false)
                {
                    this.PartTextBox.IsError = this.IsError;
                }
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
                    ToolTipService.SetToolTip(this.PartTextBox, args.Message);
                    ToolTipService.SetToolTip(this.PartTextBlock, args.Message);
                    return true;
                }
                ToolTipService.SetToolTip(this.PartTextBox, null);
                ToolTipService.SetToolTip(this.PartTextBlock, null);
                return this.PartTextBox.IsNull;
            }
        }

        public bool IsReadOnly { get; set; }

        public bool IsRequired
        {
            get
            {
                if (this.PartTextBox == null)
                {
                    return this.isRequired;
                }
                return this.PartTextBox.IsRequired;
            }
            set
            {
                if (this.PartTextBox != null)
                {
                    this.PartTextBox.TextChanged -= this.PartTextBoxOnTextChanged;
                }
                if (this.PartTextBox != null)
                {
                    this.PartTextBox.IsRequired = value;
                }
                this.isRequired = value;
                if (this.PartTextBox != null)
                {
                    this.PartTextBox.TextChanged += this.PartTextBoxOnTextChanged;
                }
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
                return this.Name;
            }
        }

        public int? MaxLength
        {
            get
            {
                return this.maxLength;
            }
            set
            {
                this.maxLength = value;
                if (this.PartTextBox != null)
                {
                    if (value != null)
                    {
                        this.PartTextBox.MaxLength = value.Value;
                    }
                }
            }
        }

        public decimal MaxNumber
        {
            get
            {
                return this.maxNumber;
            }
            set
            {
                this.maxNumber = value;
                if (this.PartTextBox != null)
                {
                    this.PartTextBox.MaxNumber = this.maxNumber;
                }
            }
        }

        public TextBlock PartTextBlock { get; set; }

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
                if (this.PartTextBox == null)
                {
                    return this.Text;
                }
                return this.PartTextBox.Text;
            }
            set
            {
                this.Text = value == null ? "" : value.ToString();
            }
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
            this.Text = string.Empty;
            if (this.PartTextBox != null)
            {
                this.PartTextBox.ClearValueControl();
            }
        }

        public void FocusControl()
        {
            ThreadPool.QueueUserWorkItem(state => Manager.Timeout(this.Dispatcher, () => this.PartTextBox.Focus()));
        }

        public override void OnApplyTemplate()
        {
            this.PartTextBox = this.GetTemplateChild(PART_Texbox) as CoreTextBox;
            this.PartTextBlock = this.GetTemplateChild(PART_TextBlock) as TextBlock;
            base.OnApplyTemplate();
            var bindingText = new Binding("Text");
            bindingText.Mode = BindingMode.TwoWay;
            bindingText.Source = this;
            if (this.PartTextBox != null)
            {
                BindingOperations.SetBinding(this.PartTextBox, TextBox.TextProperty, bindingText);
                this.PartTextBox.TextChanged += this.PartTextBoxOnTextChanged;
                this.PartTextBox.LostFocus += this.PartTextBoxOnLostFocus;
                this.PartTextBox.IsRequired = this.isRequired;
                this.PartTextBox.IsEnabled = !this.IsReadOnly;
                this.PartTextBox.FilterType = this.FilterType;
                this.PartTextBox.MaxNumber = this.MaxNumber;
                this.PartTextBox.KeyDown += PartTextBoxOnKeyDown;
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

        public void OnBeforeValidate(HandleArgs e = null)
        {
            EventHandler<HandleArgs> handler = this.BeforeValidate;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        //private void PartTextBoxOnPreviewTextInput(object sender, TextCompositionEventArgs e)
        //{
        //    if (FilterType != null)
        //    {

        //        if (FilterType.FullName.ToLower().Contains("int") || FilterType.FullName.ToLower().Contains("byte"))
        //        {
        //            regex = new Regex(@"^[0-9]$");
        //        }
        //        else if (FilterType.FullName.Contains("double") || FilterType.FullName.Contains("float") || FilterType.FullName.Contains("decimal"))
        //        {
        //            regex = new Regex(@"^[0-9\.]$");
        //        }
        //        else if (FilterType.FullName.Contains("Char"))
        //        {
        //            MaxLength = 1;
        //            regex = null;
        //        }
        //        else
        //        {
        //            regex = null;
        //        }
        //    }
        //    if (regex != null && !regex.IsMatch(e.Text))
        //    {
        //        e.Handled = true;
        //    }
        //    else
        //    {
        //        base.OnPreviewTextInput(e);
        //    }
        //}

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
            var form = dependencyObject as DisplayWithTextBox;
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
            if (this.PartTextBox == null)
            {
                return;
            }
            this.PartTextBox.Text = this.PartTextBox.Text.ToUpper();
            if (this.MaxNumber != 0)
            {
                if (!string.IsNullOrEmpty(this.PartTextBox.Text))
                {
                    if (Convert.ToInt64(this.PartTextBox.Text) > this.MaxNumber)
                    {
                        this.PartTextBox.Text = this.MaxNumber.ToString(CultureInfo.InvariantCulture);
                    }
                }
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
    }
}