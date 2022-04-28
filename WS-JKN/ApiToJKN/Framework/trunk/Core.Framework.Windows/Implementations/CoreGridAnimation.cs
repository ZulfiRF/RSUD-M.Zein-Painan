using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Core.Framework.Windows.Implementations
{
    public class CoreGridAnimation : Panel
    {
        public double StartAnimationFromX { get; set; }
        public double StartAnimationFromY { get; set; }
        //public Orientation OrientationType { get; set; }
        private Orientation orientationType;

        public Orientation OrientationType
        {
            get { return orientationType; }
            set
            {
                orientationType = value;
                if (value == Orientation.Vertical)
                {
                    ScrollViewer.SetVerticalScrollBarVisibility(this, ScrollBarVisibility.Auto);
                    ScrollViewer.SetHorizontalScrollBarVisibility(this, ScrollBarVisibility.Disabled);
                }
                else
                {
                    ScrollViewer.SetVerticalScrollBarVisibility(this, ScrollBarVisibility.Disabled);
                    ScrollViewer.SetHorizontalScrollBarVisibility(this, ScrollBarVisibility.Auto);
                }
            }
        }

        #region Fields

        private readonly TimeSpan animationLength = TimeSpan.FromMilliseconds(200);
        private readonly TimeSpan animationLengtOpacity = TimeSpan.FromMilliseconds(1500);

        #endregion

        #region Constructors and Destructors

        public CoreGridAnimation()
        {
            OrientationType = Orientation.Horizontal;
            this.TimeAnimation = this.animationLength;
        }

        #endregion

        #region Properties

        private TimeSpan TimeAnimation { get; set; }

        #endregion

        #region Methods

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.Children == null || this.Children.Count == 0)
            {
                return finalSize;
            }

            double curX = 0, curY = 0, curLineHeight = 0;

            foreach (UIElement child in this.Children)
            {
                var trans = child.RenderTransform as TranslateTransform;
                if (trans == null)
                {
                    child.RenderTransformOrigin = new Point(0, 0);
                    trans = new TranslateTransform();

                    child.RenderTransform = trans;
                }
                if (OrientationType == Orientation.Horizontal)
                {
                    if (curX + child.DesiredSize.Width > finalSize.Width)
                    {
                        //Wrap to next line
                        curY += curLineHeight;
                        curX = 0;
                        curLineHeight = 0;
                    }

                    trans.X = curX + 20;
                    child.Arrange(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));

                    trans.BeginAnimation(
                        TranslateTransform.XProperty,
                        new DoubleAnimation(curX, this.TimeAnimation),
                        HandoffBehavior.Compose);

                    trans.BeginAnimation(
                        TranslateTransform.YProperty,
                        new DoubleAnimation(curY, this.TimeAnimation),
                        HandoffBehavior.Compose);

                    curX += child.DesiredSize.Width;
                    if (child.DesiredSize.Height > curLineHeight)
                    {
                        curLineHeight = child.DesiredSize.Height;
                    }
                }
                else
                {
                    if (curY + child.DesiredSize.Height > finalSize.Height)
                    {
                        //Wrap to next line
                        curX += curLineHeight;
                        curY = 0;
                        curLineHeight = 0;
                    }

                    child.Arrange(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));

                    trans.BeginAnimation(
                        TranslateTransform.XProperty,
                        new DoubleAnimation(curX, this.TimeAnimation),
                        HandoffBehavior.SnapshotAndReplace);
                    trans.BeginAnimation(
                        TranslateTransform.YProperty,
                        new DoubleAnimation(curY, this.TimeAnimation),
                        HandoffBehavior.SnapshotAndReplace);

                    curY += child.DesiredSize.Height;
                    if (child.DesiredSize.Width > curLineHeight)
                    {
                        curLineHeight = child.DesiredSize.Width;
                    }
                }

            }

            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var infiniteSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            double curX = 0, curY = 0, curLineHeight = 0;
            foreach (UIElement child in this.Children)
            {
                child.Measure(infiniteSize);
                if (OrientationType == Orientation.Horizontal)
                {
                    if (curX + child.DesiredSize.Width > availableSize.Width)
                    {
                        //Wrap to next line
                        curY += curLineHeight;
                        curX = 0;
                        curLineHeight = 0;
                    }

                    curX += child.DesiredSize.Width;
                    if (child.DesiredSize.Height > curLineHeight)
                    {
                        curLineHeight = child.DesiredSize.Height;
                    }
                }
                else
                {
                    if (curY + child.DesiredSize.Height > availableSize.Height)
                    {
                        //Wrap to next line
                        curX += curLineHeight;
                        curY = 0;
                        curLineHeight = 0;
                    }

                    curY += child.DesiredSize.Height;
                    if (child.DesiredSize.Width > curLineHeight)
                    {
                        curLineHeight = child.DesiredSize.Width;
                    }
                }
            }
            if (OrientationType == Orientation.Horizontal)
            {
                curY += curLineHeight;
            }
            else
            {
                curX += curLineHeight;
            }
            if (OrientationType == Orientation.Horizontal)
            {
                var resultSize = new Size
                {
                    Width =
                        double.IsPositiveInfinity(availableSize.Width)
                            ? curX
                            : availableSize.Width,
                    Height =
                        double.IsPositiveInfinity(availableSize.Height)
                            ? curY
                            : availableSize.Height
                };

                return resultSize;
            }
            else
            {
                var resultSize = new Size
                {
                    Width =
                        double.IsPositiveInfinity(availableSize.Width)
                            ? curX
                            : availableSize.Width,
                    Height =
                        double.IsPositiveInfinity(availableSize.Height)
                            ? curY
                            : availableSize.Height
                };

                return resultSize;
            }
        }

        #endregion
    }
}