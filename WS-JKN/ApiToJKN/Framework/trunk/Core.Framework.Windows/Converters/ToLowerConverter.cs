using System;
using System.Globalization;
using System.Windows.Data;

namespace Core.Framework.Windows.Converters
{
    public class ToLowerConverter : MarkupConverter
    {
        #region Methods

        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as string;
            return val != null ? val.ToLower() : value;
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        #endregion
    }
}