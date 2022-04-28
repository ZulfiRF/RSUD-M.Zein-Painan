namespace Core.Framework.Windows.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    public class ScrollViewerOffsetMediator : FrameworkElement
    {
        #region Static Fields

        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register(
                "HorizontalOffset",
                typeof(double),
                typeof(ScrollViewerOffsetMediator),
                new PropertyMetadata(default(double), OnHorizontalOffsetChanged));

        public static readonly DependencyProperty ScrollViewerProperty = DependencyProperty.Register(
            "ScrollViewer",
            typeof(ScrollViewer),
            typeof(ScrollViewerOffsetMediator),
            new PropertyMetadata(default(ScrollViewer), OnScrollViewerChanged));

        #endregion

        #region Public Properties

        public double HorizontalOffset
        {
            get
            {
                return (double)this.GetValue(HorizontalOffsetProperty);
            }
            set
            {
                this.SetValue(HorizontalOffsetProperty, value);
            }
        }

        public ScrollViewer ScrollViewer
        {
            get
            {
                return (ScrollViewer)this.GetValue(ScrollViewerProperty);
            }
            set
            {
                this.SetValue(ScrollViewerProperty, value);
            }
        }

        #endregion

        #region Methods

        private static void OnHorizontalOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var mediator = (ScrollViewerOffsetMediator)o;
            if (mediator.ScrollViewer != null)
            {
                mediator.ScrollViewer.ScrollToHorizontalOffset((double)(e.NewValue));
            }
        }

        private static void OnScrollViewerChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var mediator = (ScrollViewerOffsetMediator)o;
            var scrollViewer = (ScrollViewer)(e.NewValue);
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToHorizontalOffset(mediator.HorizontalOffset);
            }
        }

        #endregion
    }
}