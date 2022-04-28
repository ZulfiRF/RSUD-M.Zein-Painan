namespace Core.Framework.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;

    [TemplateVisualState(Name = "Large", GroupName = "SizeStates")]
    [TemplateVisualState(Name = "Small", GroupName = "SizeStates")]
    [TemplateVisualState(Name = "Inactive", GroupName = "ActiveStates")]
    [TemplateVisualState(Name = "Active", GroupName = "ActiveStates")]
    public class ProgressRing : Control
    {
        #region Static Fields

        public static readonly DependencyProperty BindableWidthProperty = DependencyProperty.Register(
            "BindableWidth",
            typeof(double),
            typeof(ProgressRing),
            new PropertyMetadata(default(double), BindableWidthCallback));

        public static readonly DependencyProperty EllipseDiameterProperty =
            DependencyProperty.Register(
                "EllipseDiameter",
                typeof(double),
                typeof(ProgressRing),
                new PropertyMetadata(default(double)));

        public static readonly DependencyProperty EllipseOffsetProperty = DependencyProperty.Register(
            "EllipseOffset",
            typeof(Thickness),
            typeof(ProgressRing),
            new PropertyMetadata(default(Thickness)));

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
            "IsActive",
            typeof(bool),
            typeof(ProgressRing),
            new FrameworkPropertyMetadata(
                default(bool),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                IsActiveChanged));

        public static readonly DependencyProperty IsLargeProperty = DependencyProperty.Register(
            "IsLarge",
            typeof(bool),
            typeof(ProgressRing),
            new PropertyMetadata(true, IsLargeChangedCallback));

        public static readonly DependencyProperty MaxSideLengthProperty = DependencyProperty.Register(
            "MaxSideLength",
            typeof(double),
            typeof(ProgressRing),
            new PropertyMetadata(default(double)));

        #endregion

        #region Fields

        private List<Action> _deferredActions = new List<Action>();

        #endregion

        #region Constructors and Destructors

        static ProgressRing()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ProgressRing),
                new FrameworkPropertyMetadata(typeof(ProgressRing)));
        }

        public ProgressRing()
        {
            this.SizeChanged += this.OnSizeChanged;
        }

        #endregion

        #region Public Properties

        public double BindableWidth
        {
            get
            {
                return (double)this.GetValue(BindableWidthProperty);
            }
            private set
            {
                this.SetValue(BindableWidthProperty, value);
            }
        }

        public double EllipseDiameter
        {
            get
            {
                return (double)this.GetValue(EllipseDiameterProperty);
            }
            private set
            {
                this.SetValue(EllipseDiameterProperty, value);
            }
        }

        public Thickness EllipseOffset
        {
            get
            {
                return (Thickness)this.GetValue(EllipseOffsetProperty);
            }
            private set
            {
                this.SetValue(EllipseOffsetProperty, value);
            }
        }

        public bool IsActive
        {
            get
            {
                return (bool)this.GetValue(IsActiveProperty);
            }
            set
            {
                this.SetValue(IsActiveProperty, value);
            }
        }

        public bool IsLarge
        {
            get
            {
                return (bool)this.GetValue(IsLargeProperty);
            }
            set
            {
                this.SetValue(IsLargeProperty, value);
            }
        }

        public double MaxSideLength
        {
            get
            {
                return (double)this.GetValue(MaxSideLengthProperty);
            }
            private set
            {
                this.SetValue(MaxSideLengthProperty, value);
            }
        }

        #endregion

        #region Public Methods and Operators

        public override void OnApplyTemplate()
        {
            //make sure the states get updated
            this.UpdateLargeState();
            this.UpdateActiveState();
            base.OnApplyTemplate();
            if (this._deferredActions != null)
            {
                foreach (Action action in this._deferredActions)
                {
                    action();
                }
            }
            this._deferredActions = null;
        }

        #endregion

        #region Methods

        private static void BindableWidthCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var ring = dependencyObject as ProgressRing;
            if (ring == null)
            {
                return;
            }

            var action = new Action(
                () =>
                {
                    ring.SetEllipseDiameter((double)dependencyPropertyChangedEventArgs.NewValue);
                    ring.SetEllipseOffset((double)dependencyPropertyChangedEventArgs.NewValue);
                    ring.SetMaxSideLength((double)dependencyPropertyChangedEventArgs.NewValue);
                });

            if (ring._deferredActions != null)
            {
                ring._deferredActions.Add(action);
            }
            else
            {
                action();
            }
        }

        private static void IsActiveChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var ring = dependencyObject as ProgressRing;
            if (ring == null)
            {
                return;
            }

            ring.UpdateActiveState();
        }

        private static void IsLargeChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var ring = dependencyObject as ProgressRing;
            if (ring == null)
            {
                return;
            }

            ring.UpdateLargeState();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            this.BindableWidth = this.ActualWidth;
        }

        private void SetEllipseDiameter(double width)
        {
            if (width <= 60)
            {
                this.EllipseDiameter = 6.0;
            }
            else
            {
                this.EllipseDiameter = width * 0.1 + 6;
            }
        }

        private void SetEllipseOffset(double width)
        {
            if (width <= 60)
            {
                this.EllipseOffset = new Thickness(0, 24, 0, 0);
            }
            else
            {
                this.EllipseOffset = new Thickness(0, width * 0.4 + 24, 0, 0);
            }
        }

        private void SetMaxSideLength(double width)
        {
            this.MaxSideLength = width <= 60 ? 60.0 : width;
        }

        private void UpdateActiveState()
        {
            Action action;

            if (this.IsActive)
            {
                action = () => VisualStateManager.GoToState(this, "Active", true);
            }
            else
            {
                action = () => VisualStateManager.GoToState(this, "Inactive", true);
            }

            if (this._deferredActions != null)
            {
                this._deferredActions.Add(action);
            }

            else
            {
                action();
            }
        }

        private void UpdateLargeState()
        {
            Action action;

            if (this.IsLarge)
            {
                action = () => VisualStateManager.GoToState(this, "Large", true);
            }
            else
            {
                action = () => VisualStateManager.GoToState(this, "Small", true);
            }

            if (this._deferredActions != null)
            {
                this._deferredActions.Add(action);
            }

            else
            {
                action();
            }
        }

        #endregion
    }
}