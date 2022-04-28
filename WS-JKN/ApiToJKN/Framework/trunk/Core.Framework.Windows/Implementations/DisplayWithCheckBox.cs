namespace Core.Framework.Windows.Implementations
{
    using System;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;

    using Core.Framework.Windows.Contracts;
    using Core.Framework.Windows.Helper;

    /// <summary>
    ///     Interaction logic for DisplayWithCheckBox.xaml
    /// </summary>
    [TemplatePart(Name = PART_Texbox, Type = typeof(DisplayWithCheckBox))]
    public class DisplayWithCheckBox : UserControl, IValidateControl, IValueElement
    {
        #region Constants

        private const string PART_Texbox = "TbContent";

        #endregion

        #region Static Fields

        public static readonly DependencyProperty ControlVerticalPositionAligmentProperty =
            DependencyProperty.Register(
                "ControlVerticalPositionAligment",
                typeof(VerticalAlignment),
                typeof(DisplayWithCheckBox),
                new UIPropertyMetadata(VerticalAlignment.Top));

        // Using a DependencyProperty as the backing store for DisplayText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register(
            "DisplayText",
            typeof(string),
            typeof(DisplayWithCheckBox),
            new UIPropertyMetadata("Title"));

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(
            "IsChecked",
            typeof(bool?),
            typeof(DisplayWithCheckBox),
            new UIPropertyMetadata(false, PropertyChangedCallback));

        public static readonly DependencyProperty IsErrorProperty = DependencyProperty.Register(
            "IsError",
            typeof(bool),
            typeof(DisplayWithCheckBox),
            new UIPropertyMetadata(false));

        public static readonly DependencyProperty TextVerticalPositionAligmentProperty =
            DependencyProperty.Register(
                "TextVerticalPositionAligment",
                typeof(VerticalAlignment),
                typeof(DisplayWithCheckBox),
                new UIPropertyMetadata(VerticalAlignment.Top));

        // Using a DependencyProperty as the backing store for Watermark.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register(
            "Watermark",
            typeof(string),
            typeof(DisplayWithCheckBox),
            new UIPropertyMetadata("Type Here"));

        // Using a DependencyProperty as the backing store for WitdhDisplayText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WitdhDisplayTextProperty =
            DependencyProperty.Register(
                "WitdhDisplayText",
                typeof(GridLength),
                typeof(DisplayWithCheckBox),
                new PropertyMetadata(new GridLength(0.5, GridUnitType.Star)));

        #endregion

        #region Fields

        private bool isRequired;

        #endregion

        #region Constructors and Destructors


        public DisplayWithCheckBox()
        {
            Manager.RegisterFormGrid(this);
            var rDictionary = new ResourceDictionary();
            rDictionary.Source =
                new Uri(
                    string.Format("/Core.Framework.Windows;component/Styles/Controls.DisplayWithCheckBox.xaml"),
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
                return this.Name;
            }
        }

        public CoreCheckBox PartCheckBox { get; set; }
        public bool SkipAutoFocus { get; set; }

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
                return this.IsChecked;
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
        }

        public void FocusControl()
        {
            ThreadPool.QueueUserWorkItem(state => Manager.Timeout(this.Dispatcher, () => this.PartCheckBox.Focus()));
        }

        public override void OnApplyTemplate()
        {
            this.PartCheckBox = this.GetTemplateChild(PART_Texbox) as CoreCheckBox;

            base.OnApplyTemplate();
            var bindingText = new Binding("IsChecked");
            bindingText.Mode = BindingMode.TwoWay;
            bindingText.Source = this;
            if (this.PartCheckBox != null)
            {
                BindingOperations.SetBinding(this.PartCheckBox, ToggleButton.IsCheckedProperty, bindingText);
                this.PartCheckBox.IsRequired = this.isRequired;
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
            var form = dependencyObject as DisplayWithCheckBox;
            if (form != null)
            {
                if (form.PartCheckBox != null)
                {
                    form.PartCheckBox.IsChecked = dependencyPropertyChangedEventArgs.NewValue as bool?;
                }
            }
        }

        #endregion

        protected virtual void OnBeforeValidate(HandleArgs e)
        {
            var handler = BeforeValidate;
            if (handler != null) handler(this, e);
        }
    }
}