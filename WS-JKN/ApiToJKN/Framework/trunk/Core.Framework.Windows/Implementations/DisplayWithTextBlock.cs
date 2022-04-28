using Core.Framework.Windows.Helper;

namespace Core.Framework.Windows.Implementations
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    using Core.Framework.Windows.Contracts;

    /// <summary>
    ///     Interaction logic for DisplayWithTextBlock.xaml
    /// </summary>
    [TemplatePart(Name = PART_TextBlock, Type = typeof(TextBlock))]
    public class DisplayWithTextBlock : UserControl, IValidateControl, IValueElement
    {
        // Using a DependencyProperty as the backing store for ControlVerticalPositionAligment.  This enables animation, styling, binding, etc...

        #region Constants

        private const string PART_TextBlock = "TbContent";

        #endregion

        #region Static Fields

        public static readonly DependencyProperty ControlVerticalPositionAligmentProperty =
            DependencyProperty.Register(
                "ControlVerticalPositionAligment",
                typeof(VerticalAlignment),
                typeof(DisplayWithTextBlock),
                new UIPropertyMetadata(VerticalAlignment.Top));

        // Using a DependencyProperty as the backing store for DisplayText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register(
            "DisplayText",
            typeof(string),
            typeof(DisplayWithTextBlock),
            new UIPropertyMetadata("Title"));

        // Using a DependencyProperty as the backing store for IsError.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsErrorProperty = DependencyProperty.Register(
            "IsError",
            typeof(bool),
            typeof(DisplayWithTextBlock),
            new UIPropertyMetadata(false));

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(DisplayWithTextBlock),
            new UIPropertyMetadata("", PropertyChangedCallback));

        // Using a DependencyProperty as the backing store for TextVerticalPositionAligment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextVerticalPositionAligmentProperty =
            DependencyProperty.Register(
                "TextVerticalPositionAligment",
                typeof(VerticalAlignment),
                typeof(DisplayWithTextBlock),
                new UIPropertyMetadata(VerticalAlignment.Top));

        // Using a DependencyProperty as the backing store for Watermark.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register(
            "Watermark",
            typeof(string),
            typeof(DisplayWithTextBlock),
            new UIPropertyMetadata("Type Here"));

        // Using a DependencyProperty as the backing store for WitdhDisplayText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WitdhDisplayTextProperty =
            DependencyProperty.Register(
                "WitdhDisplayText",
                typeof(GridLength),
                typeof(DisplayWithTextBlock),
                new PropertyMetadata(new GridLength(0.5, GridUnitType.Star)));

        #endregion

        #region Fields

        private bool isRequired;

        #endregion

        #region Constructors and Destructors


        public DisplayWithTextBlock()
        {
            Manager.RegisterFormGrid(this);
            var rDictionary = new ResourceDictionary();
            rDictionary.Source =
                new Uri(
                    string.Format("/Core.Framework.Windows;component/Styles/Controls.DisplayWithTextBlock.xaml"),
                    UriKind.Relative);
            this.Style = rDictionary["DisplayMetroInTextBlock"] as Style;
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

        public bool IsReadOnly { get; set; }

        public bool IsRequired
        {
            get
            {
                return false;
            }
            set
            {
                this.isRequired = value;
            }
        }

        public bool CanFocus
        {
            get { return false; }
        }

        public string Key
        {
            get
            {
                return this.Name;
            }
        }

        public int? MaxLength { get; set; }

        public TextBlock PartTextBlock { get; set; }
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
        }

        public void FocusControl()
        {
        }

        public override void OnApplyTemplate()
        {
            this.PartTextBlock = this.GetTemplateChild(PART_TextBlock) as TextBlock;

            base.OnApplyTemplate();
            var bindingText = new Binding("Text");
            bindingText.Mode = BindingMode.TwoWay;
            bindingText.Source = this;
            if (this.PartTextBlock != null)
            {
                BindingOperations.SetBinding(this.PartTextBlock, TextBox.TextProperty, bindingText);
                this.PartTextBlock.LostFocus += this.PartTextBlockOnLostFocus;
                this.PartTextBlock.IsEnabled = !this.IsReadOnly;
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
            var form = dependencyObject as DisplayWithTextBlock;
            if (form != null)
            {
                if (form.PartTextBlock != null)
                {
                    form.PartTextBlock.Text = dependencyPropertyChangedEventArgs.NewValue as string;
                }
            }
        }

        private void PartTextBlockOnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            if (this.IsRequired)
            {
                if (this.PartTextBlock != null)
                {
                    this.IsError = string.IsNullOrEmpty(this.PartTextBlock.Text);
                }
            }
        }

        private void PartTextBlockOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            if (this.IsRequired)
            {
                if (this.PartTextBlock != null)
                {
                    this.IsError = string.IsNullOrEmpty(this.PartTextBlock.Text);
                }
            }
            this.OnTextChanged(textChangedEventArgs);
        }

        #endregion

        protected virtual void OnBeforeValidate(HandleArgs e)
        {
            var handler = BeforeValidate;
            if (handler != null) handler(this, e);
        }
    }
}