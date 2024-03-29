namespace Core.Framework.Windows.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    public class LayoutInvalidationCatcher : Decorator
    {
        #region Public Properties

        public Planerator PlaParent
        {
            get
            {
                return this.Parent as Planerator;
            }
        }

        #endregion

        #region Methods

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            Planerator pl = this.PlaParent;
            if (pl != null)
            {
                pl.InvalidateArrange();
            }
            return base.ArrangeOverride(arrangeSize);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Planerator pl = this.PlaParent;
            if (pl != null)
            {
                pl.InvalidateMeasure();
            }
            return base.MeasureOverride(constraint);
        }

        #endregion
    }
}