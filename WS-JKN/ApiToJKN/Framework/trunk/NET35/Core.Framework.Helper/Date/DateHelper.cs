using System;
using System.Globalization;

namespace Core.Framework.Helper.Date
{
    public class DateHelper
    {
        public static Age CountAge(DateTime birthDate)
        {
            var dateNow = DateTime.Now;

            return CountAge(birthDate, dateNow);
        }

        public static Age CountAge(DateTime birthDate, DateTime fromDate)
        {
            var age = new Age();
            age.Year = fromDate.Year - birthDate.Year;
            age.Month = fromDate.Month - birthDate.Month;
            if (age.Month < 0)
            {
                age.Year--;
                age.Month = fromDate.Month + (12 - birthDate.Month);
            }
            age.Day = fromDate.Day - birthDate.Day;
            if (age.Day < 0)
            {
                var countDayInMonth = DateTime.DaysInMonth(fromDate.Year, fromDate.Month - 1);
                age.Month--;
                if (age.Month < 0)
                {
                    age.Year--;
                    age.Month = 0;

                }
                age.Day = countDayInMonth - age.Day;

            }
            return age;
        }

        public static DateTime? ConvertDateTime(string dateStringValue, string format)
        {
            try
            {
                if (string.IsNullOrEmpty(format))
                    format = "dd-MM-yyyy";
                return DateTime.ParseExact(dateStringValue, format, CultureInfo.InvariantCulture);

            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }
    }
}
