namespace Core.Framework.Windows.Implementations
{
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;

    public class TransparentAdorner : Adorner
    {
        #region Constructors and Destructors

        public TransparentAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }

        #endregion

        #region Methods

        protected override void OnRender(DrawingContext drawingContext)
        {
            var brush = new SolidColorBrush();
            brush.Color = Colors.Transparent;

            drawingContext.DrawRectangle(brush, null, new Rect(new Point(0, 0), this.DesiredSize));
            base.OnRender(drawingContext);
        }

        #endregion
    }
}