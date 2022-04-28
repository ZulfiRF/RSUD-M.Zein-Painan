using System;
using System.Globalization;
using System.Windows.Data;

namespace Core.Framework.Windows.Converters
{
    public class StatusEnabledConverter : IValueConverter
    {
        #region Public Methods and Operators

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value != null)
                {
                    var flag = false;
                    if (value is int)
                        flag = (int)value == 1;
                    else if (value is byte)
                        flag = (byte)value == 1;
                    else if (value is Int16)
                        flag = (Int16)value == 1;
                    else if (value is double)
                        flag = (double)value == 1;
                    else if (value is bool)
                        flag = (bool)value;
                    //if (parameter != null)
                    //{
                    //    {
                    //        string result = parameter.ToString();
                    //        if (result.Equals("1"))
                    //            flag = !flag;
                    //    }
                    //    return flag;
                    //}                
                    return flag;
                }
                return false;
            }
            catch (Exception e)
            {
                throw new Exception("Converter Error", e);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value)
                    return 1;
            }

            return value;
        }

        #endregion
    }
}