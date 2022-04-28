using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Core.Framework.Windows.Controls
{
    public partial class ProgressIndicator
    {
        #region Static Fields

        public static readonly DependencyProperty ProgressColourProperty =
            DependencyProperty.RegisterAttached(
                "ProgressColour",
                typeof(Brush),
                typeof(ProgressIndicator),
                new UIPropertyMetadata(null));

        #endregion

        #region Constructors and Destructors

        public ProgressIndicator()
        {
            this.InitializeComponent();
            this.DataContext = this;
            this.IsVisibleChanged += (s, e) => ((ProgressIndicator)s).StartStopAnimation();
            DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(
                VisibilityProperty,
                this.GetType());
            dpd.AddValueChanged(this, (s, e) => ((ProgressIndicator)s).StartStopAnimation());
            this.Loaded += this.OnLoaded;
        }

        #endregion

        #region Public Properties

        public Brush ProgressColour
        {
            get
            {
                return (Brush)this.GetValue(ProgressColourProperty);
            }
            set
            {
                this.SetValue(ProgressColourProperty, value);
            }
        }

        public double WidthBlockAnimation { get; set; }

        #endregion

        #region Methods

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            this.R1.Width = this.WidthBlockAnimation;
            this.R2.Width = this.WidthBlockAnimation;
            this.R3.Width = this.WidthBlockAnimation;
            this.R4.Width = this.WidthBlockAnimation;
            this.R5.Width = this.WidthBlockAnimation;
        }

        private void StartStopAnimation()
        {
            bool shouldAnimate = (this.Visibility == Visibility.Visible && this.IsVisible);
            this.Dispatcher.BeginInvoke(
                new Action(
                    () =>
                    {
                        var s = this.Resources["animate"] as Storyboard;
                        if (s != null)
                        {
                            if (shouldAnimate)
                            {
                                s.Begin();
                            }
                            else
                            {
                                s.Stop();
                            }
                        }
                    }));
        }

        #endregion
    }
}