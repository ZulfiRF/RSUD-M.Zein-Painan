namespace Core.Framework.Windows.Implementations
{
    using System;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    using Core.Framework.Windows.Contracts;
    using Core.Framework.Windows.Helper;

    /// <summary>
    ///     Interaction logic for DisplayWithDateTime.xaml
    /// </summary>
    [TemplatePart(Name = PART_Texbox, Type = typeof(CoreDatePicker))]
    public class DisplayWithDateTime : UserControl, IValidateControl, IValueElement
    {
        // Using a DependencyProperty as the backing store for DisplayText.  This enables animation, styling, binding, etc...

        #region Constants

        private const string PART_Texbox = "TbContent";

        #endregion

        #region Static Fields

        public static readonly DependencyProperty ControlVerticalPositionAligmentProperty =
            DependencyProperty.Register(
                "ControlVerticalPositionAligment",
                typeof(VerticalAlignment),
                typeof(DisplayWithDateTime),
                new UIPropertyMetadata(VerticalAlignment.Top));

        public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register(
            "DisplayText",
            typeof(string),
            typeof(DisplayWithDateTime),
            new UIPropertyMetadata("Title"));

        // Using a DependencyProperty as the backing store for IsError.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsErrorProperty = DependencyProperty.Register(
            "IsError",
            typeof(bool),
            typeof(DisplayWithDateTime),
            new UIPropertyMetadata(false));

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(DisplayWithDateTime),
            new UIPropertyMetadata("", PropertyChangedCallback));

        // Using a DependencyProperty as the backing store for TextVerticalPositionAligment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextVerticalPositionAligmentProperty =
            DependencyProperty.Register(
                "TextVerticalPositionAligment",
                typeof(VerticalAlignment),
                typeof(DisplayWithDateTime),
                new UIPropertyMetadata(VerticalAlignment.Top));

        // Using a DependencyProperty as the backing store for Watermark.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register(
            "Watermark",
            typeof(string),
            typeof(DisplayWithDateTime),
            new UIPropertyMetadata("Type Here"));

        // Using a DependencyProperty as the backing store for WitdhDisplayText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WitdhDisplayTextProperty =
            DependencyProperty.Register(
                "WitdhDisplayText",
                typeof(GridLength),
                typeof(DisplayWithDateTime),
                new PropertyMetadata(new GridLength(0.5, GridUnitType.Star)));

        #endregion

        #region Fields

        private bool isRequired;

        #endregion

        #region Constructors and Destructors


        public DisplayWithDateTime()
        {
            try
            {
                Manager.RegisterFormGrid(this);
                var rDictionary = new ResourceDictionary();
                rDictionary.Source =
                    new Uri(
                        string.Format("/Core.Framework.Windows;component/Styles/Controls.DisplayWithDateTime.xaml"),
                        UriKind.Relative);
                this.Style = rDictionary["DisplayWithDateTime"] as Style;
            }
            catch (Exception)
            {
            }

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
                    return true;
                }
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
                //if (PartTextBox != null)
                //    PartTextBox.TextChanged -= PartTextBoxOnTextChanged;
                if (this.PartTextBox != null)
                {
                    this.PartTextBox.IsRequired = value;
                }
                this.isRequired = value;
                //if (PartTextBox != null)
                //    PartTextBox.TextChanged += PartTextBoxOnTextChanged;
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

        public CoreDatePicker PartTextBox { get; set; }
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

        //public bool IsReadOnly { get; set; }
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
                if (PartCheckbox != null)
                    if (PartCheckbox.Visibility == Visibility.Visible)
                    {
                        if (PartCheckbox.IsChecked != null && !PartCheckbox.IsChecked.Value)
                        {
                            return null;
                        }
                    }
                return this.Text;
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
            this.PartTextBox = this.GetTemplateChild(PART_Texbox) as CoreDatePicker;
            PartCheckbox = this.GetTemplateChild("CbUseData") as CoreCheckBox;
            base.OnApplyTemplate();
            var bindingText = new Binding("Text");
            bindingText.Mode = BindingMode.TwoWay;
            bindingText.Source = this;
            if (this.PartTextBox != null)
            {
                BindingOperations.SetBinding(this.PartTextBox, DatePicker.SelectedDateProperty, bindingText);
                //PartTextBox.TextChanged += PartTextBoxOnTextChanged;
                this.PartTextBox.LostFocus += this.PartTextBoxOnLostFocus;
                this.PartTextBox.IsRequired = this.isRequired;
                this.PartTextBox.IsEnabled = !this.IsReadOnly;
            }

            if (AllowNull)
            {
                if (PartCheckbox != null)
                {
                    //PartCheckbox.Visibility = Visibility.Visible;
                    if (PartTextBox != null) PartTextBox.Margin = new Thickness(0, 0, 0, 0);
                }
            }
            else
            {
                if (PartCheckbox != null)
                {
                    //PartCheckbox.Visibility = Visibility.Collapsed;
                    if (PartTextBox != null) PartTextBox.Margin = new Thickness(0, 0, 0, 0);
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

        private static bool Once = false;
        private static void PropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var form = dependencyObject as DisplayWithDateTime;
            if (form != null)
            {
                if (form.PartTextBox != null)
                {
                    if (!Once)
                    {
                        Once = true;
                        form.PartTextBox.Text = dependencyPropertyChangedEventArgs.NewValue as string;
                    }                    
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

        public CoreCheckBox PartCheckbox { get; set; }

        public bool AllowNull { get; set; }
    }
}