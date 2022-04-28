using System;
using System.Globalization;
using System.Windows.Data;

namespace Core.Framework.Windows.Implementations
{
    public class StatusEnabledConverBack : IValueConverter
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Converts a value.
        /// </summary>
        /// <returns>
        ///     A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (parameter != null)
                    return value.ToString().Equals(parameter.ToString());
                return value.ToString().Equals("1");
            }
            catch (Exception)
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
                if (string.IsNullOrEmpty(value.ToString()))
                    return 1;
                else
                    return 1;
            else if (value is bool)
                if ((bool)value)
                    return 1;
                else
                    return 0;
            return 3;
        }

        #endregion
    }
}