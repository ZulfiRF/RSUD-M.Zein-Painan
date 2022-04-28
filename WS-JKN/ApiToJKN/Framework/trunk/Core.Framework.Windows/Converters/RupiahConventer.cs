using System;
using System.Globalization;
using Core.Framework.Model.Provider.ValueObjects;
using Core.Framework.Windows.Helper;

namespace Core.Framework.Windows.Converters
{
    public class RupiahConventer
    {
        /// <summary>
        /// Konversi dari bilangan ke pecahan uang rupiah
        /// </summary>
        /// <param name="value">nilai yang akan di konversi</param>
        /// <returns>string nilai</returns>
        public static string ToRupiahCurrency(double value)
        {
            try
            {
                var cultureInfo = new CultureInfo("id-ID");
                return value.ToString("C2", cultureInfo);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }            
        }
        /// <summary>
        /// Konveri pecahan uang rupiah ke bilangan
        /// </summary>
        /// <param name="value">nilai yang akan dikonversi</param>
        /// <returns>double nilai</returns>
        public static double ToCurrencyBack(string value)
        {
            try
            {
                if (value != null)
                {
                    if (string.IsNullOrEmpty(value))
                        value = "0";
                    var currency = Double.Parse(value, NumberStyles.Currency, new CultureInfo("id-ID"));
                    return currency;
                }
                return 0;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

       
    }
}