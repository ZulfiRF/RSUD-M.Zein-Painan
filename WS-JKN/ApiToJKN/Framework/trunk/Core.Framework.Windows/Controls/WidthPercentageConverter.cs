namespace Core.Framework.Windows.Controls
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class WidthPercentageConverter : IValueConverter
    {
        #region Public Methods and Operators

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double percentage = Double.Parse(parameter.ToString(), new CultureInfo("en-US"));
            return ((double)value) * percentage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}