namespace System.Windows.Controls
{
}

namespace Core.Framework.Windows.Controls
{
    using System.Windows;


    public class MetroTabItem : CoreTabItem
    {
        #region Static Fields

        public static readonly DependencyProperty HeaderFontSizeProperty = DependencyProperty.Register(
            "HeaderFontSize",
            typeof(double),
            typeof(MetroTabItem),
            new PropertyMetadata(12));

        #endregion

        #region Constructors and Destructors

        public MetroTabItem()
        {
            this.DefaultStyleKey = typeof(MetroTabItem);
        }

        #endregion

        #region Public Properties

        public double HeaderFontSize
        {
            get
            {
                return (double)this.GetValue(HeaderFontSizeProperty);
            }
            set
            {
                this.SetValue(HeaderFontSizeProperty, value);
            }
        }

        #endregion

        // Using a DependencyProperty as the backing store for HeaderSize.  This enables animation, styling, binding, etc...
    }
}