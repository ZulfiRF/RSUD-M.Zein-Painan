namespace Core.Framework.Windows.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public class VisibilityConverter : IValueConverter
    {
        #region Public Methods and Operators

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
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}