namespace Core.Framework.Windows.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class ToUpperConverter : MarkupConverter
    {
        #region Methods

        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as string;
            return val != null ? val.ToUpper() : value;
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        #endregion
    }
}