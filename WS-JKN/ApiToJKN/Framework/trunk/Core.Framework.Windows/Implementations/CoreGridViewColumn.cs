using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using Core.Framework.Windows.Controls;

namespace Core.Framework.Windows.Implementations
{
    public class CoreGridViewColumn : GridViewColumn
    {
        public string Display
        {
            get { return (string)GetValue(DisplayProperty); }
            set { SetValue(DisplayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Display.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayProperty =
            DependencyProperty.Register("Display", typeof(string), typeof(CoreGridViewColumn), new UIPropertyMetadata(null, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var form = dependencyObject as CoreGridViewColumn;
            if (form != null)
            {
                form.textFactory.SetValue(GridColumn.DisplayProperty, e.NewValue);
            }

        }

        FrameworkElementFactory textFactory;
        public CoreGridViewColumn()
        {
            var datatemplae = new DataTemplate();
            textFactory = new FrameworkElementFactory(typeof(GridColumn));
            datatemplae.VisualTree = textFactory;
            CellTemplate = datatemplae;
        }
    }
}