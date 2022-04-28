namespace Core.Framework.Windows.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class PanoramaGroupHeightConverter : IMultiValueConverter
    {
        #region Public Methods and Operators

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double itemBox = double.Parse(values[0].ToString());
            double groupHeight = double.Parse(values[1].ToString());
            double headerHeight = double.Parse(values[2].ToString());

            return (Math.Floor((groupHeight - headerHeight) / itemBox) * itemBox);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}