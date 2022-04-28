namespace Core.Framework.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;

    [TemplatePart(Name = "PART_ScrollViewer", Type = typeof(ScrollViewer))]
    public class Panorama : ItemsControl
    {
        #region Static Fields

        public static readonly DependencyProperty GroupHeightProperty = DependencyProperty.Register(
            "GroupHeight",
            typeof(double),
            typeof(Panorama),
            new FrameworkPropertyMetadata(640.0));

        public static readonly DependencyProperty HeaderFontColorProperty =
            DependencyProperty.Register(
                "HeaderFontColor",
                typeof(Brush),
                typeof(Panorama),
                new FrameworkPropertyMetadata(Brushes.White));

        public static readonly DependencyProperty HeaderFontFamilyProperty =
            DependencyProperty.Register(
                "HeaderFontFamily",
                typeof(FontFamily),
                typeof(Panorama),
                new FrameworkPropertyMetadata(new FontFamily("Segoe UI Light")));

        public static readonly DependencyProperty HeaderFontSizeProperty = DependencyProperty.Register(
            "HeaderFontSize",
            typeof(double),
            typeof(Panorama),
            new FrameworkPropertyMetadata(40.0));

        public static readonly DependencyProperty ItemBoxProperty = DependencyProperty.Register(
            "ItemHeight",
            typeof(double),
            typeof(Panorama),
            new FrameworkPropertyMetadata(120.0));

        public static readonly DependencyProperty UseSnapBackScrollingProperty =
            DependencyProperty.Register(
                "UseSnapBackScrolling",
                typeof(bool),
                typeof(Panorama),
                new FrameworkPropertyMetadata(true));

        private static int PixelsToMoveToBeConsideredClick = 2;

        private static int PixelsToMoveToBeConsideredScroll = 5;

        #endregion

        #region Fields

        private readonly DispatcherTimer animationTimer = new DispatcherTimer(DispatcherPriority.DataBind);

        private double friction;

        private Point previousPoint;

        private Point scrollStartOffset;

        private Point scrollStartPoint;

        private Point scrollTarget;

        private ScrollViewer sv;

        private IPanoramaTile tile;

        private Vector velocity;

        #endregion

        #region Constructors and Destructors

        static Panorama()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Panorama), new FrameworkPropertyMetadata(typeof(Panorama)));
        }

        public Panorama()
        {
            this.friction = 0.85;

            this.animationTimer.Interval = new TimeSpan(0, 0, 0, 0, 20);
            this.animationTimer.Tick += this.HandleWorldTimerTick;

            this.Loaded += (sender, e) => { this.animationTimer.Start(); };

            this.Unloaded += (sender, e) => { this.animationTimer.Stop(); };
        }

        #endregion

        #region Public Properties

        public double Friction
        {
            get
            {
                return 1.0 - this.friction;
            }
            set
            {
                this.friction = Math.Min(Math.Max(1.0 - value, 0), 1.0);
            }
        }

        public double GroupHeight
        {
            get
            {
                return (double)this.GetValue(GroupHeightProperty);
            }
            set
            {
                this.SetValue(GroupHeightProperty, value);
            }
        }

        public Brush HeaderFontColor
        {
            get
            {
                return (Brush)this.GetValue(HeaderFontColorProperty);
            }
            set
            {
                this.SetValue(HeaderFontColorProperty, value);
            }
        }

        public FontFamily HeaderFontFamily
        {
            get
            {
                return (FontFamily)this.GetValue(HeaderFontFamilyProperty);
            }
            set
            {
                this.SetValue(HeaderFontFamilyProperty, value);
            }
        }

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

        public double ItemBox
        {
            get
            {
                return (double)this.GetValue(ItemBoxProperty);
            }
            set
            {
                this.SetValue(ItemBoxProperty, value);
            }
        }

        public bool UseSnapBackScrolling
        {
            get
            {
                return (bool)this.GetValue(UseSnapBackScrollingProperty);
            }
            set
            {
                this.SetValue(UseSnapBackScrollingProperty, value);
            }
        }

        #endregion

        #region Public Methods and Operators

        public override void OnApplyTemplate()
        {
            this.sv = (ScrollViewer)this.Template.FindName("PART_ScrollViewer", this);
            base.OnApplyTemplate();
        }

        #endregion

        #region Methods

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (this.sv.IsMouseOver)
            {
                this.tile = null;

                // Save starting point, used later when determining how much to scroll.
                this.scrollStartPoint = e.GetPosition(this);
                this.scrollStartOffset.X = this.sv.HorizontalOffset;
                this.scrollStartOffset.Y = this.sv.VerticalOffset;

                // Update the cursor if can scroll or not.
                this.Cursor = (this.sv.ExtentWidth > this.sv.ViewportWidth)
                              || (this.sv.ExtentHeight > this.sv.ViewportHeight)
                    ? Cursors.ScrollAll
                    : Cursors.Arrow;

                //store Control if one was found, so we can call its command later
                var x = TreeHelper.TryFindFromPoint<ListBoxItem>(this, this.scrollStartPoint);
                if (x != null)
                {
                    x.IsSelected = !x.IsSelected;
                    ItemsControl tiles = ItemsControlFromItemContainer(x);
                    object data = tiles.ItemContainerGenerator.ItemFromContainer(x);
                    if (data != null && data is IPanoramaTile)
                    {
                        this.tile = (IPanoramaTile)data;
                    }
                }
            }

            base.OnPreviewMouseDown(e);
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPoint = e.GetPosition(this);

                // Determine the new amount to scroll.
                var delta = new Point(
                    this.scrollStartPoint.X - currentPoint.X,
                    this.scrollStartPoint.Y - currentPoint.Y);

                if (Math.Abs(delta.X) < PixelsToMoveToBeConsideredScroll
                    && Math.Abs(delta.Y) < PixelsToMoveToBeConsideredScroll)
                {
                    return;
                }

                this.scrollTarget.X = this.scrollStartOffset.X + delta.X;
                this.scrollTarget.Y = this.scrollStartOffset.Y + delta.Y;

                // Scroll to the new position.
                this.sv.ScrollToHorizontalOffset(this.scrollTarget.X);
                this.sv.ScrollToVerticalOffset(this.scrollTarget.Y);
                this.CaptureMouse();
            }

            base.OnPreviewMouseMove(e);
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                this.ReleaseMouseCapture();
            }
            this.Cursor = Cursors.Arrow;
            Point currentPoint = e.GetPosition(this);

            // Determine the new amount to scroll.
            var delta = new Point(this.scrollStartPoint.X - currentPoint.X, this.scrollStartPoint.Y - currentPoint.Y);

            if (Math.Abs(delta.X) < PixelsToMoveToBeConsideredClick
                && Math.Abs(delta.Y) < PixelsToMoveToBeConsideredClick && this.tile != null)
            {
                if (this.tile.TileClickedCommand != null)
                {
                    //Ok, its a click ask the tile to do its job
                    if (this.tile.TileClickedCommand.CanExecute(null))
                    {
                        this.tile.TileClickedCommand.Execute(null);
                    }
                }
            }

            base.OnPreviewMouseUp(e);
        }

        private void DoStandardScrolling()
        {
            this.sv.ScrollToHorizontalOffset(this.scrollTarget.X);
            this.sv.ScrollToVerticalOffset(this.scrollTarget.Y);
            this.scrollTarget.X += this.velocity.X;
            this.scrollTarget.Y += this.velocity.Y;
            this.velocity *= this.friction;
        }

        private void HandleWorldTimerTick(object sender, EventArgs e)
        {
            if (this.sv == null)
            {
                return;
            }
            DependencyProperty prop = DesignerProperties.IsInDesignModeProperty;
            var isInDesignMode =
                (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;

            if (isInDesignMode)
            {
                return;
            }

            if (this.IsMouseCaptured)
            {
                Point currentPoint = Mouse.GetPosition(this);
                this.velocity = this.previousPoint - currentPoint;
                this.previousPoint = currentPoint;
            }
            else
            {
                if (this.velocity.Length > 1)
                {
                    this.DoStandardScrolling();
                }
                else
                {
                    if (this.UseSnapBackScrolling)
                    {
                        int mx = (int)this.sv.HorizontalOffset % (int)this.ActualWidth;
                        if (mx == 0)
                        {
                            return;
                        }
                        int ix = (int)this.sv.HorizontalOffset / (int)this.ActualWidth;
                        double snapBackX = mx > this.ActualWidth / 2
                            ? (ix + 1) * this.ActualWidth
                            : ix * this.ActualWidth;
                        this.sv.ScrollToHorizontalOffset(
                            this.sv.HorizontalOffset + (snapBackX - this.sv.HorizontalOffset) / 4.0);
                    }
                    else
                    {
                        this.DoStandardScrolling();
                    }
                }
            }
        }

        #endregion
    }
}