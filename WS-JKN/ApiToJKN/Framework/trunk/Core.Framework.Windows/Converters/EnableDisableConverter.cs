using System;
using System.Globalization;
using System.Windows.Data;

namespace Core.Framework.Windows.Converters
{
    public class EnableDisableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                var flag = (bool)value;
                if (parameter != null)
                {
                    string result = parameter.ToString();
                    if (result.Equals("1"))
                    {
                        flag = !flag;
                    }
                }
                if (flag)
                {
                    return true;
                }
                return false;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}