namespace Core.Framework.Windows.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;

    [TemplateVisualState(Name = NormalState, GroupName = CommonStates)]
    [TemplateVisualState(Name = DisabledState, GroupName = CommonStates)]
    [TemplateVisualState(Name = CheckedState, GroupName = CheckStates)]
    [TemplateVisualState(Name = DraggingState, GroupName = CheckStates)]
    [TemplateVisualState(Name = UncheckedState, GroupName = CheckStates)]
    [TemplatePart(Name = SwitchRootPart, Type = typeof(Grid))]
    [TemplatePart(Name = SwitchBackgroundPart, Type = typeof(UIElement))]
    [TemplatePart(Name = SwitchTrackPart, Type = typeof(Grid))]
    [TemplatePart(Name = SwitchThumbPart, Type = typeof(FrameworkElement))]
    public class ToggleSwitchButton : ToggleButton
    {
        #region Constants

        private const string CheckStates = "CheckStates";

        private const string CheckedState = "Checked";

        private const string CommonStates = "CommonStates";

        private const string DisabledState = "Disabled";

        private const string DraggingState = "Dragging";

        private const string NormalState = "Normal";

        private const string SwitchBackgroundPart = "SwitchBackground";

        private const string SwitchRootPart = "SwitchRoot";

        private const string SwitchThumbPart = "SwitchThumb";

        private const string SwitchTrackPart = "SwitchTrack";

        private const string UncheckedState = "Unchecked";

        #endregion

        #region Static Fields

        public static readonly DependencyProperty SwitchForegroundProperty =
            DependencyProperty.Register(
                "SwitchForeground",
                typeof(Brush),
                typeof(ToggleSwitchButton),
                new PropertyMetadata(null));

        #endregion

        #region Fields

        private TranslateTransform _backgroundTranslation;

        private bool _isDragging = false;

        private Grid _root;

        private FrameworkElement _thumb;

        private TranslateTransform _thumbTranslation;

        private Grid _track;

        #endregion

        #region Constructors and Destructors

        public ToggleSwitchButton()
        {
            this.DefaultStyleKey = typeof(ToggleSwitchButton);
        }

        #endregion

        #region Public Properties

        public Brush SwitchForeground
        {
            get
            {
                return (Brush)this.GetValue(SwitchForegroundProperty);
            }
            set
            {
                this.SetValue(SwitchForegroundProperty, value);
            }
        }

        #endregion

        #region Public Methods and Operators

        public override void OnApplyTemplate()
        {
            if (this._track != null)
            {
                this._track.SizeChanged -= this.SizeChangedHandler;
            }
            if (this._thumb != null)
            {
                this._thumb.SizeChanged -= this.SizeChangedHandler;
            }
            base.OnApplyTemplate();
            this._root = this.GetTemplateChild(SwitchRootPart) as Grid;
            var background = this.GetTemplateChild(SwitchBackgroundPart) as UIElement;
            this._backgroundTranslation = background == null ? null : background.RenderTransform as TranslateTransform;
            this._track = this.GetTemplateChild(SwitchTrackPart) as Grid;
            this._thumb = this.GetTemplateChild(SwitchThumbPart) as Border;
            this._thumbTranslation = this._thumb == null ? null : this._thumb.RenderTransform as TranslateTransform;
            if (this._root != null && this._track != null && this._thumb != null
                && (this._backgroundTranslation != null || this._thumbTranslation != null))
            {
                this._track.SizeChanged += this.SizeChangedHandler;
                this._thumb.SizeChanged += this.SizeChangedHandler;
            }
            this.ChangeVisualState(false);
        }

        #endregion

        #region Methods

        protected override void OnToggle()
        {
            this.IsChecked = this.IsChecked != true;
            this.ChangeVisualState(true);
        }

        private void ChangeVisualState(bool useTransitions)
        {
            VisualStateManager.GoToState(this, this.IsEnabled ? NormalState : DisabledState, useTransitions);

            if (this._isDragging)
            {
                // TODO: _isDragging is never set to true, so we never enter this state
                VisualStateManager.GoToState(this, DraggingState, useTransitions);
            }
            else if (this.IsChecked == true)
            {
                VisualStateManager.GoToState(this, CheckedState, useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, UncheckedState, useTransitions);
            }
        }

        private void SizeChangedHandler(object sender, SizeChangedEventArgs e)
        {
            this._track.Clip = new RectangleGeometry
                               {
                                   Rect =
                                       new Rect(
                                       0,
                                       0,
                                       this._track.ActualWidth,
                                       this._track.ActualHeight)
                               };
            // TODO: this value is being assigned on each callback but not used anywhere
            double checkedTranslation = this._track.ActualWidth - this._thumb.ActualWidth - this._thumb.Margin.Left
                                        - this._thumb.Margin.Right;
        }

        #endregion
    }
}