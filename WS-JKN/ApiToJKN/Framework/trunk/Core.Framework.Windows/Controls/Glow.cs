namespace Core.Framework.Windows.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class Glow : Control
    {
        #region Static Fields

        public static readonly DependencyProperty GlowColorProperty = DependencyProperty.Register(
            "GlowColor",
            typeof(Color),
            typeof(Glow),
            new UIPropertyMetadata(Colors.Transparent));

        public static readonly DependencyProperty IsGlowProperty = DependencyProperty.Register(
            "IsGlow",
            typeof(bool),
            typeof(Glow),
            new UIPropertyMetadata(true));

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation",
            typeof(Orientation),
            typeof(Glow),
            new UIPropertyMetadata(Orientation.Vertical));

        #endregion

        #region Constructors and Destructors

        static Glow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Glow), new FrameworkPropertyMetadata(typeof(Glow)));
        }

        #endregion

        #region Public Properties

        public Color GlowColor
        {
            get
            {
                return (Color)this.GetValue(GlowColorProperty);
            }
            set
            {
                this.SetValue(GlowColorProperty, value);
            }
        }

        public bool IsGlow
        {
            get
            {
                return (bool)this.GetValue(IsGlowProperty);
            }
            set
            {
                this.SetValue(IsGlowProperty, value);
            }
        }

        public Orientation Orientation
        {
            get
            {
                return (Orientation)this.GetValue(OrientationProperty);
            }
            set
            {
                this.SetValue(OrientationProperty, value);
            }
        }

        #endregion
    }
}