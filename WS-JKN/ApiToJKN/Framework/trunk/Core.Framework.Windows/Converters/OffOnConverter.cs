namespace Core.Framework.Windows.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using Core.Framework.Windows.Controls;

    public class OffOnConverter : IValueConverter
    {
        #region Public Methods and Operators

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var t = (ToggleSwitch)parameter;

            return t.IsChecked == true ? t.OnLabel : t.OffLabel;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}