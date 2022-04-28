namespace Core.Framework.Windows.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    [Obsolete("Control is broken in that it only works under some very specific circumstances. Will be removed in v1")]
    public class AppBarButton : Button
    {
        #region Static Fields

        public static readonly DependencyProperty MetroImageSourceProperty =
            DependencyProperty.Register(
                "MetroImageSource",
                typeof(Visual),
                typeof(AppBarButton),
                new PropertyMetadata(default(Visual)));

        #endregion

        #region Constructors and Destructors

        public AppBarButton()
        {
            this.DefaultStyleKey = typeof(AppBarButton);
        }

        #endregion

        #region Public Properties

        public Visual MetroImageSource
        {
            get
            {
                return (Visual)this.GetValue(MetroImageSourceProperty);
            }
            set
            {
                this.SetValue(MetroImageSourceProperty, value);
            }
        }

        #endregion
    }
}