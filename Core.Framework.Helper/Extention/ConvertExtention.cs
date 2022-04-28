using System;
using System.Globalization;

namespace Core.Framework.Helper.Extention
{
    public static class ConvertExtention
    {
        public static T Cast<T>(this object data)
        {
            return (T)data;
        }
        public static Int16 ToInt16(this object data)
        {
            try
            {
                if (data == DBNull.Value)
                    return 0;
                if (data == null)
                    return 0;
                return Convert.ToInt16(data);
            }
            catch (Exception)
            {
                return 0;
            }

        }
        public static char ToChar(this object data)
        {
            if (data == null)
                return default(char);
            if (data == DBNull.Value)
                return default(char);
            if (data is char)
                return (char)data;
            return Convert.ToChar(data);
        }

        public static byte ToByte(this object data)
        {
            try
            {



                if (data == null)
                    return 0;
                if (data == DBNull.Value)
                    return 0;
                if (data is string)
                    if (string.IsNullOrEmpty(data.ToString()))
                        return 0;
                return Convert.ToByte(data);
            }
            catch (Exception)
            {

                return 0;
            }
        }
        public static Int32 ToInt32(this object data)
        {
            try
            {


                if (data == null)
                    return 0;
                if (data == DBNull.Value)
                    return 0;
                if (data is string)
                    if (string.IsNullOrEmpty(data.ToString()))
                        return 0;
                return Convert.ToInt32(data);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static Int64 ToInt64(this object data)
        {
            try
            {


                if (data == null)
                    return 0;
                if (data == DBNull.Value)
                    return 0;
                if (data is string)
                    if (string.IsNullOrEmpty(data.ToString()))
                        return 0;
                return Convert.ToInt64(data);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static DateTime ToDatetTime(this object data)
        {
            try
            {


                if (data == null)
                    return DateTime.Now;
                if (data == DBNull.Value)
                    return DateTime.Now;
                if (data is long)
                    return (new DateTime(1970, 1, 1)).AddMilliseconds((long)data);
                if (data is string)
                    if (string.IsNullOrEmpty(data.ToString()))
                        return DateTime.Now;
                return Convert.ToDateTime(data);
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }

        public static double ToDouble(this object data, string thousandDecimal = "")
        {
            try
            {
                if (data is DBNull)
                    return 0;
                if (data is string)
                    if (data.ToString() == string.Empty)
                        return 0;
                if (data == null)
                    return Convert.ToDouble(0);
                if (string.IsNullOrEmpty(thousandDecimal))
                    return Convert.ToDouble(data);
                var number = data.ToString().Split(new[] { CultureInfo.CurrentUICulture.NumberFormat.CurrencyGroupSeparator[0] });
                if (number.Length == 1)
                {
                    if (string.IsNullOrEmpty(thousandDecimal))
                        return Convert.ToDouble(data);
                    else if (data is DBNull)
                    {
                        return 0;
                    }
                    else
                    {
                        return Math.Round(Convert.ToDouble(data), thousandDecimal.ToInt16());
                    }
                }
                var str = "";
                if (number[1].Length > thousandDecimal.ToInt16())
                    str = number[0] + CultureInfo.CurrentUICulture.NumberFormat.CurrencyGroupSeparator[0] +
                          number[1].Substring(0, thousandDecimal.ToInt16());
                else
                    str = number[0] + CultureInfo.CurrentUICulture.NumberFormat.CurrencyGroupSeparator[0] +
                          number[1];
                return Convert.ToDouble(str);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static float ToFloat(this object data)
        {
            try
            {
                if (data is string)
                    if (data.ToString() == string.Empty)
                        return 0;
                if (data == null)
                    return 0;
                if (data == DBNull.Value)
                    return 0;
                return Convert.ToSingle(data);
            }
            catch (Exception)
            {
                return 0;
            }

        }
    }
}