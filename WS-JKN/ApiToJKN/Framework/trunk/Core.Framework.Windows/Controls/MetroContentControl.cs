namespace Core.Framework.Windows.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class MetroContentControl : ContentControl
    {
        #region Static Fields

        public static readonly DependencyProperty OnlyLoadTransitionProperty =
            DependencyProperty.Register(
                "OnlyLoadTransition",
                typeof(bool),
                typeof(MetroContentControl),
                new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty ReverseTransitionProperty =
            DependencyProperty.Register(
                "ReverseTransition",
                typeof(bool),
                typeof(MetroContentControl),
                new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty TransitionsEnabledProperty =
            DependencyProperty.Register(
                "TransitionsEnabled",
                typeof(bool),
                typeof(MetroContentControl),
                new FrameworkPropertyMetadata(true));

        #endregion

        #region Fields

        private bool transitionLoaded;

        #endregion

        #region Constructors and Destructors

        public MetroContentControl()
        {
            this.DefaultStyleKey = typeof(MetroContentControl);

            this.Loaded += this.MetroContentControlLoaded;
            this.Unloaded += this.MetroContentControlUnloaded;
        }

        #endregion

        #region Public Properties

        public bool OnlyLoadTransition
        {
            get
            {
                return (bool)this.GetValue(OnlyLoadTransitionProperty);
            }
            set
            {
                this.SetValue(OnlyLoadTransitionProperty, value);
            }
        }

        public bool ReverseTransition
        {
            get
            {
                return (bool)this.GetValue(ReverseTransitionProperty);
            }
            set
            {
                this.SetValue(ReverseTransitionProperty, value);
            }
        }

        public bool TransitionsEnabled
        {
            get
            {
                return (bool)this.GetValue(TransitionsEnabledProperty);
            }
            set
            {
                this.SetValue(TransitionsEnabledProperty, value);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Reload()
        {
            if (!this.TransitionsEnabled || this.transitionLoaded)
            {
                return;
            }

            if (this.ReverseTransition)
            {
                VisualStateManager.GoToState(this, "BeforeLoaded", true);
                VisualStateManager.GoToState(this, "AfterUnLoadedReverse", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "BeforeLoaded", true);
                VisualStateManager.GoToState(this, "AfterLoaded", true);
            }
        }

        #endregion

        #region Methods

        private void MetroContentControlIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.TransitionsEnabled && !this.transitionLoaded)
            {
                if (!this.IsVisible)
                {
                    VisualStateManager.GoToState(
                        this,
                        this.ReverseTransition ? "AfterUnLoadedReverse" : "AfterUnLoaded",
                        false);
                }
                else
                {
                    VisualStateManager.GoToState(
                        this,
                        this.ReverseTransition ? "AfterLoadedReverse" : "AfterLoaded",
                        true);
                }
            }
        }

        private void MetroContentControlLoaded(object sender, RoutedEventArgs e)
        {
            if (this.TransitionsEnabled)
            {
                if (!this.transitionLoaded)
                {
                    this.transitionLoaded = this.OnlyLoadTransition;
                    VisualStateManager.GoToState(
                        this,
                        this.ReverseTransition ? "AfterLoadedReverse" : "AfterLoaded",
                        true);
                }
                this.IsVisibleChanged -= this.MetroContentControlIsVisibleChanged;
                this.IsVisibleChanged += this.MetroContentControlIsVisibleChanged;
            }
            else
            {
                var root = ((Grid)this.GetTemplateChild("root"));
                root.Opacity = 1.0;
                var transform = ((TranslateTransform)root.RenderTransform);
                if (transform.IsFrozen)
                {
                    TranslateTransform modifiedTransform = transform.Clone();
                    modifiedTransform.X = 0;
                    root.RenderTransform = modifiedTransform;
                }
                else
                {
                    transform.X = 0;
                }
            }
        }

        private void MetroContentControlUnloaded(object sender, RoutedEventArgs e)
        {
            if (this.TransitionsEnabled)
            {
                if (this.transitionLoaded)
                {
                    VisualStateManager.GoToState(
                        this,
                        this.ReverseTransition ? "AfterUnLoadedReverse" : "AfterUnLoaded",
                        false);
                }
                this.IsVisibleChanged -= this.MetroContentControlIsVisibleChanged;
            }
        }

        #endregion
    }
}