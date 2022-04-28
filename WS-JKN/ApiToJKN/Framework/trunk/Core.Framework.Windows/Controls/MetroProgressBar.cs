namespace Core.Framework.Windows.Controls
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Animation;

    public class MetroProgressBar : ProgressBar
    {
        #region Static Fields

        public static readonly DependencyProperty EllipseDiameterProperty =
            DependencyProperty.Register(
                "EllipseDiameter",
                typeof(double),
                typeof(MetroProgressBar),
                new PropertyMetadata(default(double)));

        public static readonly DependencyProperty EllipseOffsetProperty = DependencyProperty.Register(
            "EllipseOffset",
            typeof(double),
            typeof(MetroProgressBar),
            new PropertyMetadata(default(double)));

        #endregion

        #region Constructors and Destructors

        static MetroProgressBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(MetroProgressBar),
                new FrameworkPropertyMetadata(typeof(MetroProgressBar)));
        }

        public MetroProgressBar()
        {
            this.SizeChanged += this.SizeChangedHandler;
        }

        #endregion

        #region Public Properties

        public double EllipseDiameter
        {
            get
            {
                return (double)this.GetValue(EllipseDiameterProperty);
            }
            set
            {
                this.SetValue(EllipseDiameterProperty, value);
            }
        }

        public double EllipseOffset
        {
            get
            {
                return (double)this.GetValue(EllipseOffsetProperty);
            }
            set
            {
                this.SetValue(EllipseOffsetProperty, value);
            }
        }

        #endregion

        #region Public Methods and Operators

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.SizeChangedHandler(null, null);
        }

        #endregion

        #region Methods

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            // Update the Ellipse properties to their default values
            // only if they haven't been user-set.
            if (this.EllipseDiameter.Equals(0))
            {
                this.SetEllipseDiameter(this.ActualWidth);
            }
            if (this.EllipseOffset.Equals(0))
            {
                this.SetEllipseOffset(this.ActualWidth);
            }
        }

        private double CalcContainerAnimEnd(double width)
        {
            double firstPart = 0.4352 * width;
            if (width <= 180)
            {
                return firstPart - 25.731;
            }
            if (width <= 280)
            {
                return firstPart + 27.84;
            }

            return firstPart + 58.862;
        }

        private double CalcContainerAnimStart(double width)
        {
            if (width <= 180)
            {
                return -34;
            }
            if (width <= 280)
            {
                return -50.5;
            }

            return -63;
        }

        private double CalcEllipseAnimEnd(double width)
        {
            return width * 2.0 / 3.0;
        }

        private double CalcEllipseAnimWell(double width)
        {
            return width * 1.0 / 3.0;
        }

        private VisualState GetIndeterminate()
        {
            DependencyObject templateGrid = this.GetTemplateChild("ContainingGrid");
            IList groups = VisualStateManager.GetVisualStateGroups((FrameworkElement)templateGrid);
            return groups != null
                ? groups.Cast<VisualStateGroup>()
                    .SelectMany(@group => @group.States.Cast<VisualState>())
                    .FirstOrDefault(state => state.Name == "Indeterminate")
                : null;
        }

        private void ResetStoryboard(double width)
        {
            lock (this)
            {
                //perform calculations
                double containerAnimStart = this.CalcContainerAnimStart(width);
                double containerAnimEnd = this.CalcContainerAnimEnd(width);
                double ellipseAnimWell = this.CalcEllipseAnimWell(width);
                double ellipseAnimEnd = this.CalcEllipseAnimEnd(width);
                //reset the main double animation
                try
                {
                    VisualState indeterminate = this.GetIndeterminate();

                    if (indeterminate != null)
                    {
                        Storyboard newStoryboard = indeterminate.Storyboard.Clone();
                        Timeline doubleAnim = newStoryboard.Children.First(t => t.Name == "MainDoubleAnim");
                        doubleAnim.SetValue(DoubleAnimation.FromProperty, containerAnimStart);
                        //doubleAnim.InvalidateProperty(DoubleAnimation.FromProperty);
                        doubleAnim.SetValue(DoubleAnimation.ToProperty, containerAnimEnd);
                        //doubleAnim.InvalidateProperty(DoubleAnimation.ToProperty);
                        //indeterminate.Storyboard.Begin();

                        var namesOfElements = new[] { "E1", "E2", "E3", "E4", "E5" };
                        foreach (string elemName in namesOfElements)
                        {
                            var doubleAnimParent =
                                (DoubleAnimationUsingKeyFrames)
                                    newStoryboard.Children.First(t => t.Name == elemName + "Anim");
                            DoubleKeyFrame first, second, third;
                            if (elemName == "E1")
                            {
                                first = doubleAnimParent.KeyFrames[1];
                                second = doubleAnimParent.KeyFrames[2];
                                third = doubleAnimParent.KeyFrames[3];
                            }
                            else
                            {
                                first = doubleAnimParent.KeyFrames[2];
                                second = doubleAnimParent.KeyFrames[3];
                                third = doubleAnimParent.KeyFrames[4];
                            }

                            first.Value = ellipseAnimWell;
                            second.Value = ellipseAnimWell;
                            third.Value = ellipseAnimEnd;
                            first.InvalidateProperty(DoubleKeyFrame.ValueProperty);
                            second.InvalidateProperty(DoubleKeyFrame.ValueProperty);
                            third.InvalidateProperty(DoubleKeyFrame.ValueProperty);

                            doubleAnimParent.InvalidateProperty(Storyboard.TargetPropertyProperty);
                            doubleAnimParent.InvalidateProperty(Storyboard.TargetNameProperty);
                        }
                        indeterminate.Storyboard.Remove();
                        indeterminate.Storyboard = newStoryboard;
                        if (this.IsIndeterminate)
                        {
                            indeterminate.Storyboard.Begin((FrameworkElement)this.GetTemplateChild("ContainingGrid"));
                        }
                    }
                }
                catch (Exception)
                {
                    //we just ignore 
                }
            }
        }

        private void SetEllipseDiameter(double width)
        {
            if (width <= 180)
            {
                this.EllipseDiameter = 4;
                return;
            }
            if (width <= 280)
            {
                this.EllipseDiameter = 5;
                return;
            }

            this.EllipseDiameter = 6;
        }

        private void SetEllipseOffset(double width)
        {
            if (width <= 180)
            {
                this.EllipseOffset = 4;
                return;
            }
            if (width <= 280)
            {
                this.EllipseOffset = 7;
                return;
            }

            this.EllipseOffset = 9;
        }

        private void SizeChangedHandler(object sender, SizeChangedEventArgs e)
        {
            double actualWidth = this.ActualWidth;
            MetroProgressBar bar = this;

            bar.ResetStoryboard(actualWidth);
        }

        #endregion
    }
}