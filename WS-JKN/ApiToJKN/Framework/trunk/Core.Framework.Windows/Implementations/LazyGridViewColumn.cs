using System.Windows;
using System.Windows.Controls;

namespace Core.Framework.Windows.Implementations
{
    public class LazyGridViewColumn : GridViewColumn
    {


        public string Display
        {
            get { return (string)GetValue(DisplayProperty); }
            set { SetValue(DisplayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Display.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayProperty =
            DependencyProperty.Register("Display", typeof(string), typeof(LazyGridViewColumn), new UIPropertyMetadata("", PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var form = dependencyObject as LazyGridViewColumn;
            if (form != null)
            {
                form.textFactory.SetValue(LazyContent.DisplayProperty, e.NewValue);
            }
        }

        FrameworkElementFactory textFactory;
        public LazyGridViewColumn()
        {
            var datatemplae = new DataTemplate();
            textFactory = new FrameworkElementFactory(typeof(LazyContent));
            textFactory.SetValue(LazyContent.CurrentListViewProperty, this);
            datatemplae.VisualTree = textFactory;
            CellTemplate = datatemplae;

        }
    }
}