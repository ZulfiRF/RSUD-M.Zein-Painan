/*
 * Copyright (c) 2010, Sergey Loktin (mailto://shadowconsp@gmail.com)
 * Licensed under The MIT License (http://www.opensource.org/licenses/mit-license.php)
*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Core.Framework.Windows.Clock
{
    /// <summary>
    /// Interaction logic for Clock.xaml
    /// </summary>
    public partial class MoonyClockCtrl : UserControl
    {
        private int _dayOfYear = -1;
        private int _year = -1;
        private int _colorNum = 0;

        public static readonly DependencyProperty SecondsProperty = DependencyProperty.Register(
          "Seconds", typeof(string), typeof(MoonyClockCtrl), new PropertyMetadata("00"));
        public string Seconds
        {
            get { return (string)this.GetValue(SecondsProperty); }
            set { this.SetValue(SecondsProperty, value); }
        }

        public static readonly DependencyProperty MinutesProperty = DependencyProperty.Register(
          "Minutes", typeof(string), typeof(MoonyClockCtrl), new PropertyMetadata("00"));
        public string Minutes
        {
            get { return (string)this.GetValue(MinutesProperty); }
            set { this.SetValue(MinutesProperty, value); }
        }

        public static readonly DependencyProperty HoursProperty = DependencyProperty.Register(
          "Hours", typeof(string), typeof(MoonyClockCtrl), new PropertyMetadata("00"));
        public string Hours
        {
            get { return (string)this.GetValue(HoursProperty); }
            set { this.SetValue(HoursProperty, value); }
        }

        public static readonly DependencyProperty PmAmProperty = DependencyProperty.Register(
          "PmAm", typeof(string), typeof(MoonyClockCtrl), new PropertyMetadata(string.Empty));
        public string PmAm
        {
            get { return (string)this.GetValue(PmAmProperty); }
            set { this.SetValue(PmAmProperty, value); }
        }

        public static readonly DependencyProperty Hours24Property = DependencyProperty.Register(
          "Hours24", typeof(bool), typeof(MoonyClockCtrl), new PropertyMetadata(true));
        public bool Hours24
        {
            get { return (bool)this.GetValue(Hours24Property); }
            set { this.SetValue(Hours24Property, value); }
        }

        public static readonly DependencyProperty DateProperty = DependencyProperty.Register(
          "Date", typeof(string), typeof(MoonyClockCtrl), new PropertyMetadata(String.Empty));
        public string Date
        {
            get { return (string)this.GetValue(DateProperty); }
            set { this.SetValue(DateProperty, value); }
        }

        public static readonly DependencyProperty DayOfWeekProperty = DependencyProperty.Register(
          "DayOfWeek", typeof(string), typeof(MoonyClockCtrl), new PropertyMetadata(String.Empty));
        public string DayOfWeek
        {
            get { return (string)this.GetValue(DayOfWeekProperty); }
            set { this.SetValue(DayOfWeekProperty, value); }
        }

        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
          "Angle", typeof(int), typeof(MoonyClockCtrl), new PropertyMetadata(0));
        public int Angle
        {
            get { return (int)this.GetValue(AngleProperty); }
            set { this.SetValue(AngleProperty, value); }
        }

        public static readonly DependencyProperty MainColorProperty = DependencyProperty.Register(
          "MainColor", typeof(Color), typeof(MoonyClockCtrl), new PropertyMetadata(Color.FromArgb(255, 255, 255, 255)));
        public Color MainColor
        {
            get { return (Color)this.GetValue(MainColorProperty); }
            set { this.SetValue(MainColorProperty, value); }
        }

        public static readonly DependencyProperty MainColor2Property = DependencyProperty.Register(
          "MainColor2", typeof(Color), typeof(MoonyClockCtrl), new PropertyMetadata(Color.FromArgb(255, 180, 180, 180)));
        public Color MainColor2
        {
            get { return (Color)this.GetValue(MainColor2Property); }
            set { this.SetValue(MainColor2Property, value); }
        }

        public static readonly DependencyProperty ShadowColorProperty = DependencyProperty.Register(
          "ShadowColor", typeof(Color), typeof(MoonyClockCtrl), new PropertyMetadata(Color.FromArgb(187, 0, 0, 0)));
        public Color ShadowColor
        {
            get { return (Color)this.GetValue(ShadowColorProperty); }
            set { this.SetValue(ShadowColorProperty, value); }
        }

        public int ColorNum 
        {
            get { return _colorNum; }
            set 
            { 
                _colorNum = value;
                if (_colorNum == 0)
                    SetLightColors();
                else
                    SetDarkColors();
            } 
        }

        public MoonyClockCtrl()
        {
            InitializeComponent();
            LoadSettings();
            this.DataContext = this;
        }

        private void LoadSettings()
        {
            _colorNum = Properties.Settings.Default.color;
            if (_colorNum == 0)
                SetLightColors();
            else
                SetDarkColors();
            Hours24 = Properties.Settings.Default.hours24;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            int tier = (RenderCapability.Tier >> 16);
            if (tier != 2)//allow shadows only on pcs with hardware acceleration
            {
                MainGrid.Children.Remove(shadow);
            }

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.1);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            SetCurTime();
        }

        /// <summary>
        /// Set current time
        /// </summary>
        private void SetCurTime()
        {
            DateTime now = DateTime.Now;
            SetDeg((now.Second + now.Millisecond / 1000.0) * 6);
            Seconds = now.Second.ToString("00");
            Minutes = now.Minute.ToString("00");
            if (Hours24)
            {
                Hours = now.Hour.ToString("00");
                PmAm = string.Empty;
            }
            else
            {
                Hours = now.ToString("hh");
                PmAm = now.ToString("tt");
            }
            if (now.DayOfYear != _dayOfYear || now.Year != _year)
            {
                Date = now.ToString("d MMMM yyyy");
                DayOfWeek = now.ToString("dddd");
                _dayOfYear = now.DayOfYear;
                _year = now.Year;
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            SetCurTime();
        }

        /// <summary>
        /// Set seconds arc degree
        /// </summary>
        /// <param name="degree"></param>
        private void SetDeg(double degree)
        {
            double offset = pathF.StartPoint.X;
            double x = Math.Cos((degree - 90) * Math.PI / 180) * 100.0 + offset;
            double y = Math.Sin((degree - 90) * Math.PI / 180) * 100.0 + offset;
            arc.Point = new Point(x, y);
            arc.IsLargeArc = (degree > 180);
        }

        public void SetLightColors()
        {
            _colorNum = 0;
            this.MainColor = Color.FromArgb(255, 255, 255, 255);
            this.MainColor2 = Color.FromArgb(255, 180, 180, 180);
            this.ShadowColor = Color.FromArgb(187, 0, 0, 0);
        }

        public void SetDarkColors()
        {
            _colorNum = 1;
            this.MainColor = Color.FromArgb(255, 0, 0, 0);
            this.MainColor2 = Color.FromArgb(255, 136, 136, 136);
            this.ShadowColor = Color.FromArgb(187, 221, 221, 221);
        }
    }
}
