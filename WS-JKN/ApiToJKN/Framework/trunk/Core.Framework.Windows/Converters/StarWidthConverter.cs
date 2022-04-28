using System;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace Core.Framework.Windows.Converters
{
    public class StarWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var listview = value as ListView;
            if (listview != null)
            {
                var width = listview.Width;
                var gv = listview.View as GridView;
                if (gv != null)
                    width = gv.Columns.Where(t => !Double.IsNaN(t.Width)).Aggregate(width, (current, t) => current - t.Width);
                return width - 5;// this is to take care of margin/padding
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}