﻿using System;
using System.Windows.Controls;
using System.Windows;

namespace Core.Framework.Windows.Implementations.Chart.Controls
{
    #if NETFX_CORE
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;
    using Windows.UI.Xaml.Markup;
    using Windows.UI.Xaml;
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Xaml.Media.Animation;
    using Windows.UI.Core;
#else

#endif

#if SILVERLIGHT
    public class ColumnSeriesPanel : Panel
#else
    public class ColumnSeriesPanel : Grid
#endif
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in Children)
                child.Measure(availableSize);

            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size cellSize = new Size(Math.Ceiling(finalSize.Width / Children.Count), finalSize.Height);
            int  col = 0;

            double bottomposition = finalSize.Height;
            foreach (UIElement child in Children)
            {
                double width= finalSize.Width;
                double height = child.DesiredSize.Height;
                double x = 0;
                double y = bottomposition - height;
                Rect rect = new Rect(x, y, width, height);
                child.Arrange(rect);

                bottomposition -= height;
                col++;
            }
            return finalSize;
        }
    }
}
