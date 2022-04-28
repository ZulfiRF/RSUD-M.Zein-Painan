using System;
using System.Globalization;
using System.Windows.Data;

namespace Core.Framework.Windows.Converters
{
    public class ListColoring : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var model = (byte)value;
            if (model == 1)
                return 1;
            if (model == 2)
                return 2;
            if (model == 3)
                return 3;
            if (model == 4)
                return 4;
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}