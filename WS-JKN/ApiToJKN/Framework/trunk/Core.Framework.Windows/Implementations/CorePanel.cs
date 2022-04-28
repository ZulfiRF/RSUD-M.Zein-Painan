namespace Core.Framework.Windows.Implementations
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using Core.Framework.Windows.Helper;

    public class CorePanel : UserControl
    {
        #region Static Fields

        public static readonly DependencyProperty BackgroundTextColorProperty =
            DependencyProperty.Register(
                "BackgroundTextColor",
                typeof(Brush),
                typeof(CorePanel),
                new UIPropertyMetadata(new SolidColorBrush(Colors.White), PropertyChangedCallback));

        public static readonly DependencyProperty ForegroundTextColorProperty =
            DependencyProperty.Register(
                "ForegroundTextColor",
                typeof(Brush),
                typeof(CorePanel),
                new UIPropertyMetadata(new SolidColorBrush(Colors.Black), PropertyChangedCallbackForegroundTextColor));

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(CorePanel),
            new UIPropertyMetadata("", TitleCallBack));

        #endregion

        #region Fields

        private bool disableControl;

        #endregion

        #region Public Properties

        public Brush BackgroundTextColor
        {
            get
            {
                return (Brush)this.GetValue(BackgroundTextColorProperty);
            }
            set
            {
                this.SetValue(BackgroundTextColorProperty, value);
            }
        }

        public ContentPresenter Child { get; set; }

        public bool EnabledControl
        {
            get
            {
                return this.disableControl;
            }
            set
            {
                this.disableControl = value;
                this.IsEnabled = value;
                foreach (FrameworkElement control in Manager.FindVisualChildren<FrameworkElement>(this))
                {
                    control.IsEnabled = value;
                }
            }
        }

        public Brush ForegroundTextColor
        {
            get
            {
                return (Brush)this.GetValue(ForegroundTextColorProperty);
            }
            set
            {
                this.SetValue(ForegroundTextColorProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for BackgrounTextColor.  This enables animation, styling, binding, etc...

        //public string Title { get { return TextBlock.Text.Trim(); } set { TextBlock.Text = "   " + value + "   "; } }

        public string Title
        {
            get
            {
                return ((string)this.GetValue(TitleProperty)).Trim();
            }
            set
            {
                this.SetValue(TitleProperty, "  " + value + "   ");
            }
        }

        #endregion

        #region Properties

        internal Border Border { get; set; }
        internal TextBlock TextBlock { get; set; }

        #endregion

        #region Public Methods and Operators

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.TextBlock = this.GetTemplateChild("Tb_TextBlock") as TextBlock;
        }

        #endregion

        #region Methods

        private static void PropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var form = dependencyObject as CorePanel;
            if (form != null)
            {
                if (form.TextBlock != null)
                {
                    form.TextBlock.Background = dependencyPropertyChangedEventArgs.NewValue as Brush;
                }
            }
        }

        private static void PropertyChangedCallbackForegroundTextColor(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var form = dependencyObject as CorePanel;
            if (form != null)
            {
                if (form.TextBlock != null)
                {
                    form.TextBlock.Foreground = dependencyPropertyChangedEventArgs.NewValue as Brush;
                }
            }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...

        private static void TitleCallBack(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var form = dependencyObject as CorePanel;
            if (form != null)
            {
                if (form.TextBlock != null)
                {
                    form.TextBlock.Text = dependencyPropertyChangedEventArgs.NewValue as string;
                }
            }
        }

        #endregion
    }
}