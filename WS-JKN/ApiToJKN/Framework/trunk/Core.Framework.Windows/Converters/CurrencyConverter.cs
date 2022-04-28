namespace Core.Framework.Windows.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class CurrencyConverter : IValueConverter
    {
        #region Public Methods and Operators

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double)
            {
                string result = String.Format(new CultureInfo("id-ID"), "{0:N}", value);
                string rp = "Rp.";
                for (int i = result.Replace("Rp", "").Length; i < 15; i++)
                {
                    rp += " ";
                }
                return rp + result.Replace("Rp", "");
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