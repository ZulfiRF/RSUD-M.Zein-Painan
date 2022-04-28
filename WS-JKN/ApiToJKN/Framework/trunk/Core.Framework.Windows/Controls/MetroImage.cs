namespace Core.Framework.Windows.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    [Obsolete("Control is broken in that it only works under some very specific circumstances. Will be removed in v1")]
    public class MetroImage : Control
    {
        #region Static Fields

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source",
            typeof(Visual),
            typeof(MetroImage),
            new PropertyMetadata(default(Visual)));

        #endregion

        #region Constructors and Destructors

        public MetroImage()
        {
            this.DefaultStyleKey = typeof(MetroImage);
        }

        #endregion

        #region Public Properties

        public Visual Source
        {
            get
            {
                return (Visual)this.GetValue(SourceProperty);
            }
            set
            {
                this.SetValue(SourceProperty, value);
            }
        }

        #endregion
    }
}