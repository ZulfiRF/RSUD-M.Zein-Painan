namespace Core.Framework.Windows.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    public class MetroAnimatedSingleRowTabControl : TabControl
    {
        #region Static Fields

        public static readonly DependencyProperty TabStripMarginProperty = DependencyProperty.Register(
            "TabStripMargin",
            typeof(Thickness),
            typeof(MetroAnimatedSingleRowTabControl),
            new PropertyMetadata(new Thickness(0)));

        #endregion

        #region Constructors and Destructors

        public MetroAnimatedSingleRowTabControl()
        {
            this.DefaultStyleKey = typeof(MetroAnimatedSingleRowTabControl);
        }

        #endregion

        #region Public Properties

        public Thickness TabStripMargin
        {
            get
            {
                return (Thickness)this.GetValue(TabStripMarginProperty);
            }
            set
            {
                this.SetValue(TabStripMarginProperty, value);
            }
        }

        #endregion

        // Using a DependencyProperty as the backing store for TabStripMargin.  This enables animation, styling, binding, etc...
    }
}