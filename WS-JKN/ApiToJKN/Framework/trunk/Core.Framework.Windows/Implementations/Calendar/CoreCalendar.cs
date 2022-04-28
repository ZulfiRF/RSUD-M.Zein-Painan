/*
    Jarloo
    http://www.jarloo.com
 
    This work is licensed under a Creative Commons Attribution-ShareAlike 3.0 Unported License  
    http://creativecommons.org/licenses/by-sa/3.0/ 

*/

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Core.Framework.Windows.Implementations.Calendar
{
    [TemplatePart(Name = "PART_TextBlockDate", Type = typeof(TextBlock))]
    public class CoreCalendar : Control
    {


        public ObservableCollection<Day> Days { get; set; }
        public ObservableCollection<string> DayNames { get; set; }
        public static readonly DependencyProperty CurrentDateProperty = DependencyProperty.Register("CurrentDate", typeof(DateTime), typeof(CoreCalendar));

        public event EventHandler<DayChangedEventArgs> DayChanged;

        public DateTime CurrentDate
        {
            get { return (DateTime)GetValue(CurrentDateProperty); }
            set { SetValue(CurrentDateProperty, value); }
        }

        static CoreCalendar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CoreCalendar), new FrameworkPropertyMetadata(typeof(CoreCalendar)));
        }

        public CoreCalendar()
        {
            try
            {
                var rDictionary = new ResourceDictionary
                {
                    Source = new Uri(
                        string.Format("/Core.Framework.Windows;component/Styles/Controls.Calendar.xaml"),
                        UriKind.Relative)
                };

                Style = rDictionary["StyleContainer"] as Style;

                DataContext = this;
                CurrentDate = DateTime.Today;

                DayNames = new ObservableCollection<string> { "Minggu", "Senin", "Selasa", "Rabu", "Kamis", "Jumat", "Sabtu" };

                Days = new ObservableCollection<Day>();
                BuildCalendar(DateTime.Today);
            }
            catch (Exception)
            {
            }
        }

        public void BuildCalendar(DateTime targetDate)
        {
            Days.Clear();

            //Calculate when the first day of the month is and work out an 
            //offset so we can fill in any boxes before that.
            var d = new DateTime(targetDate.Year, targetDate.Month, 1);
            int offset = DayOfWeekNumber(d.DayOfWeek);
            //if (offset != 1) 
            d = d.AddDays(-offset);

            //Show 6 weeks each with 7 days = 42
            for (int box = 1; box <= 42; box++)
            {
                var day = new Day
                              {
                                  Date = d,
                                  Enabled = true,
                                  Notes = null,
                                  IsTargetMonth = targetDate.Month == d.Month
                              };

                day.PropertyChanged += Day_Changed;
                day.IsToday = d == DateTime.Today;
                Days.Add(day);
                d = d.AddDays(1);
            }
        }

        private void Day_Changed(object sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName != "Notes") return;
            if (DayChanged == null) return;

            DayChanged(this, new DayChangedEventArgs(sender as Day));
        }

        private static int DayOfWeekNumber(DayOfWeek dow)
        {
            return Convert.ToInt32(dow.ToString("D"));
        }
    }
}