using System.Collections.Generic;
using System.Linq;

namespace Core.Framework.Helper.Date
{
    using System;
    using System.Globalization;

    public static class DateHelper
    {
        public static string ToInteger(this DateTime date)
        {
            var a = date.ToUniversalTime().Ticks;
            return ((date.ToUniversalTime().Ticks - new DateTime(1970, 1, 1).Ticks) / 10000).ToString();
        }
        public static int ToInteger(this DateTime? date)
        {
            try
            {
                if (date.HasValue)
                    return Convert.ToInt32((date.Value.ToUniversalTime().Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000);
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public static bool Between(this DateTime currentDate, DateTime from, DateTime until)
        {
            return currentDate >= from && currentDate <= until;
        }
        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }
        public static DateTimeFormatInfo GetDateFormat(CultureInfo culture)
        {
            if (culture == null) throw new ArgumentNullException("culture");

            if (culture.Calendar is GregorianCalendar)
            {
                return culture.DateTimeFormat;
            }

            GregorianCalendar foundCal = null;
            DateTimeFormatInfo dtfi = null;
            foreach (var cal in culture.OptionalCalendars.OfType<GregorianCalendar>())
            {
                // Return the first Gregorian calendar with CalendarType == Localized 
                // Otherwise return the first Gregorian calendar
                if (foundCal == null)
                {
                    foundCal = cal;
                }

                if (cal.CalendarType != GregorianCalendarTypes.Localized) continue;

                foundCal = cal;
                break;
            }


            if (foundCal == null)
            {
                // if there are no GregorianCalendars in the OptionalCalendars list, use the invariant dtfi 
                dtfi = ((CultureInfo)CultureInfo.InvariantCulture.Clone()).DateTimeFormat;
                dtfi.Calendar = new GregorianCalendar();
            }
            else
            {
                dtfi = ((CultureInfo)culture.Clone()).DateTimeFormat;
                dtfi.Calendar = foundCal;
            }

            return dtfi;
        }
        public static DateTime ConvertDateTimeFromString(string value)
        {
            return ConvertDateTimeFromString(value, "dd/MM/yyyy");
        }
        public static DateTime ConvertDateTimeFromString(string value, string format)
        {
            try
            {
                var dtfi = new DateTimeFormatInfo();
                dtfi.ShortDatePattern = format;
                dtfi.DateSeparator = "-";
                DateTime currentDate = Convert.ToDateTime(value, dtfi);
                return currentDate;
            }
            catch (Exception)
            {
                return DateTime.Now;
            }

        }
        #region Public Methods and Operators

        public static DateTime? ConvertDateTime(string dateStringValue, string format = "dd-MM-yyyy")
        {
            try
            {
                return DateTime.ParseExact(dateStringValue, format, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }

        public static Age CountAge(DateTime birthDate)
        {
            DateTime dateNow = DateTime.Now;

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
                int countDayInMonth = DateTime.DaysInMonth(fromDate.Year, fromDate.Month);
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

        public static Age CountDiffrenceAge(DateTime currentlyDate, DateTime birthDate)
        {
            var age = new Age();
            //var timespan = currentlyDate.Subtract(birthDate);
            //var totalDays = Convert.ToInt32(timespan.TotalDays);
            //int year = totalDays / 365;
            //totalDays = totalDays - (year * 365);
            //int month = totalDays / 30;
            //totalDays = totalDays - (month * 30);
            //int day = totalDays;

            // di komen, menggunakan time span
            //if (birthDate > currentlyDate)
            //{
            //    return age;
            //}
            //int year = currentlyDate.Year - birthDate.Year;
            //int month = currentlyDate.Month - birthDate.Month;

            //int day = currentlyDate.Day - birthDate.Day;
            //if (day < 0)
            //{
            //    int m = birthDate.Month;
            //    int y = currentlyDate.Year;
            //    if (m <= 0)
            //    {
            //        m = 12 + m;
            //        y--;
            //    }
            //    day = DateTime.DaysInMonth(y, m) + day;
            //    month--;
            //}
            //if (month < 0)
            //{
            //    month = 12 + month;
            //    year--;
            //}

            var years = currentlyDate.Year - birthDate.Year;
            var months = 0;
            var days = 0;

            if (currentlyDate < birthDate.AddYears(years) && years != 0)
            {
                years--;
            }

            // Calculate the number of months.
            birthDate = birthDate.AddYears(years);

            if (birthDate.Year == currentlyDate.Year)
            {
                months = currentlyDate.Month - birthDate.Month;
            }
            else
            {
                months = 12 - birthDate.Month + currentlyDate.Month;
            }

            if (currentlyDate < birthDate.AddMonths(months) && months != 0)
            {
                months--;
            }

            birthDate = birthDate.AddMonths(months);

            days = (currentlyDate - birthDate).Days;

            age.Day = days;
            age.Month = months;
            age.Year = years;

            return age;
        }

        #endregion

        public static string ConvertDateTimeToString(object o, string format = "dd-MM-yyyy")
        {
            if (o == null)
                return "-";
            if (o == DBNull.Value)
                return "-";
            if (o is DateTime)
            {
                if (((DateTime?)o).HasValue)
                    return ((DateTime?)o).Value.ToString(format);
            }
            return "-";
        }

        /// <summary>
        /// Untuk mengeset DatetimePicker Tanggal sesuai dengan data yang dimasukan pada TextBox
        /// </summary>
        /// <param name="coreTextBoxHari">string Hari</param>
        /// <param name="coreTextBoxTahun">string Tahun</param>
        /// <param name="coreTextBoxBulan">string Bulan</param>
        public static DateTime SetCalculateDatePicker(string coreTextBoxTahun, string coreTextBoxBulan, string coreTextBoxHari)
        {
            try
            {
                var dateTimeNow = DateTime.Now;

                if (string.IsNullOrEmpty(coreTextBoxTahun))
                    coreTextBoxTahun = "0";
                var dateYearCount = Convert.ToInt16(coreTextBoxTahun);
                dateTimeNow = dateTimeNow.AddYears(-dateYearCount);

                if (string.IsNullOrEmpty(coreTextBoxBulan))
                    coreTextBoxBulan = "0";
                var dateMonthCount = Convert.ToInt16(coreTextBoxBulan);
                dateTimeNow = dateTimeNow.AddMonths(-dateMonthCount);

                if (string.IsNullOrEmpty(coreTextBoxHari))
                    coreTextBoxHari = "0";
                var dateDayCount = Convert.ToInt16(coreTextBoxHari);
                dateTimeNow = dateTimeNow.AddDays(-dateDayCount);

                return dateTimeNow;
            }
            catch (Exception)
            {
                return new DateTime();
            }
        }
    }
}