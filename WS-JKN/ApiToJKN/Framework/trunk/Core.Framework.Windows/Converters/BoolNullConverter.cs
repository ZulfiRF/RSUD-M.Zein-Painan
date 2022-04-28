using System;
using System.Globalization;
using System.Windows.Data;

namespace Core.Framework.Windows.Converters
{
    public class BoolNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {           
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}