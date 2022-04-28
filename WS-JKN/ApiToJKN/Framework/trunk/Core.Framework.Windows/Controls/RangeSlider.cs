namespace Core.Framework.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    [DefaultEvent("RangeSelectionChanged")]
    [TemplatePart(Name = "PART_RangeSliderContainer", Type = typeof(StackPanel))]
    [TemplatePart(Name = "PART_LeftEdge", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "PART_RightEdge", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "PART_LeftThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_MiddleThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_RightThumb", Type = typeof(Thumb))]
    public sealed class RangeSlider : Control
    {
        #region Constants

        private const double DefaultSplittersThumbWidth = 10;

        private const double RepeatButtonMoveRatio = 0.1;

        #endregion

        #region Static Fields

        public static readonly DependencyProperty MinRangeProperty = DependencyProperty.Register(
            "MinRange",
            typeof(long),
            typeof(RangeSlider),
            new UIPropertyMetadata((long)0, MinRangeChanged));

        public static readonly RoutedEvent RangeSelectionChangedEvent =
            EventManager.RegisterRoutedEvent(
                "RangeSelectionChanged",
                RoutingStrategy.Bubble,
                typeof(RangeSelectionChangedEventHandler),
                typeof(RangeSlider));

        public static readonly DependencyProperty RangeStartProperty = DependencyProperty.Register(
            "RangeStart",
            typeof(long),
            typeof(RangeSlider),
            new UIPropertyMetadata((long)0, RangeChanged));

        public static readonly DependencyProperty RangeStartSelectedProperty =
            DependencyProperty.Register(
                "RangeStartSelected",
                typeof(long),
                typeof(RangeSlider),
                new UIPropertyMetadata((long)0, RangesChanged));

        public static readonly DependencyProperty RangeStopProperty = DependencyProperty.Register(
            "RangeStop",
            typeof(long),
            typeof(RangeSlider),
            new UIPropertyMetadata((long)1, RangeChanged));

        public static readonly DependencyProperty RangeStopSelectedProperty =
            DependencyProperty.Register(
                "RangeStopSelected",
                typeof(long),
                typeof(RangeSlider),
                new UIPropertyMetadata((long)1, RangesChanged));

        public static RoutedUICommand MoveAllBack = new RoutedUICommand(
            "MoveAllBack",
            "MoveAllBack",
            typeof(RangeSlider),
            new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.B, ModifierKeys.Alt) }));

        public static RoutedUICommand MoveAllForward = new RoutedUICommand(
            "MoveAllForward",
            "MoveAllForward",
            typeof(RangeSlider),
            new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.F, ModifierKeys.Alt) }));

        public static RoutedUICommand MoveBack = new RoutedUICommand(
            "MoveBack",
            "MoveBack",
            typeof(RangeSlider),
            new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.B, ModifierKeys.Control) }));

        public static RoutedUICommand MoveForward = new RoutedUICommand(
            "MoveForward",
            "MoveForward",
            typeof(RangeSlider),
            new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.F, ModifierKeys.Control) }));

        #endregion

        #region Fields

        private Thumb _centerThumb;

        private bool _internalUpdate;

        private RepeatButton _leftButton;

        private Thumb _leftThumb;

        private long _movableRange;

        private double _movableWidth;

        private RepeatButton _rightButton;

        private Thumb _rightThumb;

        private StackPanel _visualElementsContainer;

        #endregion

        #region Constructors and Destructors

        static RangeSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(RangeSlider),
                new FrameworkPropertyMetadata(typeof(RangeSlider)));
        }

        public RangeSlider()
        {
            this.CommandBindings.Add(new CommandBinding(MoveBack, this.MoveBackHandler));
            this.CommandBindings.Add(new CommandBinding(MoveForward, this.MoveForwardHandler));
            this.CommandBindings.Add(new CommandBinding(MoveAllForward, this.MoveAllForwardHandler));
            this.CommandBindings.Add(new CommandBinding(MoveAllBack, this.MoveAllBackHandler));

            DependencyPropertyDescriptor.FromProperty(ActualWidthProperty, typeof(RangeSlider))
                .AddValueChanged(this, delegate { this.ReCalculateWidths(); });
        }

        #endregion

        #region Public Events

        public event RangeSelectionChangedEventHandler RangeSelectionChanged
        {
            add
            {
                this.AddHandler(RangeSelectionChangedEvent, value);
            }
            remove
            {
                this.RemoveHandler(RangeSelectionChangedEvent, value);
            }
        }

        #endregion

        #region Public Properties

        public long MinRange
        {
            get
            {
                return (long)this.GetValue(MinRangeProperty);
            }
            set
            {
                this.SetValue(MinRangeProperty, value);
            }
        }

        public long RangeStart
        {
            get
            {
                return (long)this.GetValue(RangeStartProperty);
            }
            set
            {
                this.SetValue(RangeStartProperty, value);
            }
        }

        public long RangeStartSelected
        {
            get
            {
                return (long)this.GetValue(RangeStartSelectedProperty);
            }
            set
            {
                this.SetValue(RangeStartSelectedProperty, value);
            }
        }

        public long RangeStop
        {
            get
            {
                return (long)this.GetValue(RangeStopProperty);
            }
            set
            {
                this.SetValue(RangeStopProperty, value);
            }
        }

        public long RangeStopSelected
        {
            get
            {
                return (long)this.GetValue(RangeStopSelectedProperty);
            }
            set
            {
                this.SetValue(RangeStopSelectedProperty, value);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void MoveSelection(bool isLeft)
        {
            double widthChange = RepeatButtonMoveRatio * (this.RangeStopSelected - this.RangeStartSelected)
                                 * this._movableWidth / this._movableRange;

            widthChange = isLeft ? -widthChange : widthChange;
            MoveThumb(this._leftButton, this._rightButton, widthChange);
            this.ReCalculateRangeSelected(true, true);
        }

        public void MoveSelection(long span)
        {
            if (span > 0)
            {
                if (this.RangeStopSelected + span > this.RangeStop)
                {
                    span = this.RangeStop - this.RangeStopSelected;
                }
            }
            else
            {
                if (this.RangeStartSelected + span < this.RangeStart)
                {
                    span = this.RangeStart - this.RangeStartSelected;
                }
            }

            if (span == 0)
            {
                return;
            }

            this._internalUpdate = true; //set flag to signal that the properties are being set by the object itself
            this.RangeStartSelected += span;
            this.RangeStopSelected += span;
            this.ReCalculateWidths();
            this._internalUpdate = false; //set flag to signal that the properties are being set by the object itself

            this.OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(this));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._visualElementsContainer = this.EnforceInstance<StackPanel>("PART_RangeSliderContainer");
            this._centerThumb = this.EnforceInstance<Thumb>("PART_MiddleThumb");
            this._leftButton = this.EnforceInstance<RepeatButton>("PART_LeftEdge");
            this._rightButton = this.EnforceInstance<RepeatButton>("PART_RightEdge");
            this._leftThumb = this.EnforceInstance<Thumb>("PART_LeftThumb");
            this._rightThumb = this.EnforceInstance<Thumb>("PART_RightThumb");
            this.InitializeVisualElementsContainer();
            this.ReCalculateWidths();
        }

        public void ResetSelection(bool isStart)
        {
            double widthChange = this.RangeStop - this.RangeStart;
            widthChange = isStart ? -widthChange : widthChange;

            MoveThumb(this._leftButton, this._rightButton, widthChange);
            this.ReCalculateRangeSelected(true, true);
        }

        public void SetSelectedRange(long selectionStart, long selectionStop)
        {
            long start = Math.Max(this.RangeStart, selectionStart);
            long stop = Math.Min(selectionStop, this.RangeStop);
            start = Math.Min(start, this.RangeStop - this.MinRange);
            stop = Math.Max(this.RangeStart + this.MinRange, stop);
            if (stop < start + this.MinRange)
            {
                return;
            }

            this._internalUpdate = true; //set flag to signal that the properties are being set by the object itself
            this.RangeStartSelected = start;
            this.RangeStopSelected = stop;
            this.ReCalculateWidths();
            this._internalUpdate = false; //set flag to signal that the properties are being set by the object itself
            this.OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(this));
        }

        public void ZoomToSpan(long span)
        {
            this._internalUpdate = true; //set flag to signal that the properties are being set by the object itself
            // Ensure new span is within the valid range
            span = Math.Min(span, this.RangeStop - this.RangeStart);
            span = Math.Max(span, this.MinRange);
            if (span == this.RangeStopSelected - this.RangeStartSelected)
            {
                return; // No change
            }

            // First zoom half of it to the right
            long rightChange = (span - (this.RangeStopSelected - this.RangeStartSelected)) / 2;
            long leftChange = rightChange;

            // If we will hit the right edge, spill over the leftover change to the other side
            if (rightChange > 0 && this.RangeStopSelected + rightChange > this.RangeStop)
            {
                leftChange += rightChange - (this.RangeStop - this.RangeStopSelected);
            }
            this.RangeStopSelected = Math.Min(this.RangeStopSelected + rightChange, this.RangeStop);
            rightChange = 0;

            // If we will hit the left edge and there is space on the right, add the leftover change to the other side
            if (leftChange > 0 && this.RangeStartSelected - leftChange < this.RangeStart)
            {
                rightChange = this.RangeStart - (this.RangeStartSelected - leftChange);
            }
            this.RangeStartSelected = Math.Max(this.RangeStartSelected - leftChange, this.RangeStart);
            if (rightChange > 0) // leftovers to the right
            {
                this.RangeStopSelected = Math.Min(this.RangeStopSelected + rightChange, this.RangeStop);
            }

            this.ReCalculateWidths();
            this._internalUpdate = false; //set flag to signal that the properties are being set by the object itself
            this.OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(this));
        }

        #endregion

        #region Methods

        private static double GetChangeKeepPositive(double width, double increment)
        {
            return Math.Max(width + increment, 0) - width;
        }

        private static void MinRangeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if ((long)e.NewValue < 0)
            {
                throw new ArgumentOutOfRangeException("value", "value for MinRange cannot be less than 0");
            }

            var slider = (RangeSlider)sender;
            if (slider._internalUpdate)
            {
                return;
            }

            slider._internalUpdate = true;
            slider.RangeStopSelected = Math.Max(slider.RangeStopSelected, slider.RangeStartSelected + (long)e.NewValue);
            slider.RangeStop = Math.Max(slider.RangeStop, slider.RangeStopSelected);
            slider._internalUpdate = false;

            slider.ReCalculateRanges();
            slider.ReCalculateWidths();
        }

        private static void MoveThumb(FrameworkElement x, FrameworkElement y, double horizonalChange)
        {
            double change = 0;
            if (horizonalChange < 0) //slider went left
            {
                change = GetChangeKeepPositive(x.Width, horizonalChange);
            }
            else if (horizonalChange > 0) //slider went right if(horizontal change == 0 do nothing)
            {
                change = -GetChangeKeepPositive(y.Width, -horizonalChange);
            }

            x.Width += change;
            y.Width -= change;
        }

        private static void RangeChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var slider = (RangeSlider)dependencyObject;
            if (slider._internalUpdate)
            {
                return;
            }

            slider.ReCalculateRanges();
            slider.ReCalculateWidths();
        }

        private static void RangesChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var slider = (RangeSlider)dependencyObject;
            if (slider._internalUpdate)
            {
                return;
            }

            slider.ReCalculateWidths();
            slider.OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(slider));
        }

        private void CenterThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            MoveThumb(this._leftButton, this._rightButton, e.HorizontalChange);
            this.ReCalculateRangeSelected(true, true);
        }

        private T EnforceInstance<T>(string partName) where T : FrameworkElement, new()
        {
            T element = this.GetTemplateChild(partName) as T ?? new T();
            return element;
        }

        //adds all visual element to the conatiner
        private void InitializeVisualElementsContainer()
        {
            this._visualElementsContainer.Orientation = Orientation.Horizontal;
            this._leftThumb.Width = DefaultSplittersThumbWidth;
            this._leftThumb.Tag = "left";
            this._rightThumb.Width = DefaultSplittersThumbWidth;
            this._rightThumb.Tag = "right";

            //handle the drag delta
            this._centerThumb.DragDelta += this.CenterThumbDragDelta;
            this._leftThumb.DragDelta += this.LeftThumbDragDelta;
            this._rightThumb.DragDelta += this.RightThumbDragDelta;
            this._leftButton.Click += this.LeftButtonClick;
            this._rightButton.Click += this.RightButtonClick;
        }

        private void LeftButtonClick(object sender, RoutedEventArgs e)
        {
            this.MoveSelection(true);
        }

        private void LeftThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            MoveThumb(this._leftButton, this._centerThumb, e.HorizontalChange);
            this.ReCalculateRangeSelected(true, false);
        }

        private void MoveAllBackHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.ResetSelection(true);
        }

        private void MoveAllForwardHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.ResetSelection(false);
        }

        private void MoveBackHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.MoveSelection(true);
        }

        private void MoveForwardHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.MoveSelection(false);
        }

        private void OnRangeSelectionChanged(RangeSelectionChangedEventArgs e)
        {
            e.RoutedEvent = RangeSelectionChangedEvent;
            this.RaiseEvent(e);
        }

        private void ReCalculateRangeSelected(bool reCalculateStart, bool reCalculateStop)
        {
            this._internalUpdate = true; //set flag to signal that the properties are being set by the object itself
            if (reCalculateStart)
            {
                // Make sure to get exactly rangestart if thumb is at the start
                this.RangeStartSelected = this._leftButton.Width == 0.0
                    ? this.RangeStart
                    : Math.Max(
                        this.RangeStart,
                        (long)(this.RangeStart + this._movableRange * this._leftButton.Width / this._movableWidth));
            }

            if (reCalculateStop)
            {
                // Make sure to get exactly rangestop if thumb is at the end
                this.RangeStopSelected = this._rightButton.Width == 0.0
                    ? this.RangeStop
                    : Math.Min(
                        this.RangeStop,
                        (long)(this.RangeStop - this._movableRange * this._rightButton.Width / this._movableWidth));
            }

            this._internalUpdate = false; //set flag to signal that the properties are being set by the object itself

            if (reCalculateStart || reCalculateStop)
            {
                //raise the RangeSelectionChanged event
                this.OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(this));
            }
        }

        private void ReCalculateRanges()
        {
            this._movableRange = this.RangeStop - this.RangeStart - this.MinRange;
        }

        private void ReCalculateWidths()
        {
            if (this._leftButton != null && this._rightButton != null && this._centerThumb != null)
            {
                this._movableWidth =
                    Math.Max(
                        this.ActualWidth - this._rightThumb.ActualWidth - this._leftThumb.ActualWidth
                        - this._centerThumb.MinWidth,
                        1);
                this._leftButton.Width =
                    Math.Max(this._movableWidth * (this.RangeStartSelected - this.RangeStart) / this._movableRange, 0);
                this._rightButton.Width =
                    Math.Max(this._movableWidth * (this.RangeStop - this.RangeStopSelected) / this._movableRange, 0);
                this._centerThumb.Width =
                    Math.Max(
                        this.ActualWidth - this._leftButton.Width - this._rightButton.Width
                        - this._rightThumb.ActualWidth - this._leftThumb.ActualWidth,
                        0);
            }
        }

        private void RightButtonClick(object sender, RoutedEventArgs e)
        {
            this.MoveSelection(false);
        }

        private void RightThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            MoveThumb(this._centerThumb, this._rightButton, e.HorizontalChange);
            this.ReCalculateRangeSelected(false, true);
        }

        #endregion
    }
}