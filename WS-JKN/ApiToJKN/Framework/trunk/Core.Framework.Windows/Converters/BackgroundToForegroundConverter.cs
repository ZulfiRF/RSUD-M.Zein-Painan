namespace Core.Framework.Windows.Converters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;

    public class BackgroundToForegroundConverter : IValueConverter, IMultiValueConverter
    {
        #region Static Fields

        private static BackgroundToForegroundConverter _instance;

        #endregion

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit

        #region Constructors and Destructors

        private BackgroundToForegroundConverter()
        {
        }

        #endregion

        #region Public Properties

        public static BackgroundToForegroundConverter Instance
        {
            get
            {
                return _instance ?? (_instance = new BackgroundToForegroundConverter());
            }
        }

        #endregion

        #region Public Methods and Operators

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush)
            {
                Color idealForegroundColor = this.IdealTextColor(((SolidColorBrush)value).Color);
                var foreGroundBrush = new SolidColorBrush(idealForegroundColor);
                foreGroundBrush.Freeze();
                return foreGroundBrush;
            }
            return Brushes.White;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Brush bgBrush = values.Length > 0 ? values[0] as Brush : null;
            Brush titleBrush = values.Length > 1 ? values[1] as Brush : null;
            if (titleBrush != null)
            {
                return titleBrush;
            }
            return Convert(bgBrush, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return targetTypes.Select(t => DependencyProperty.UnsetValue).ToArray();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Determining Ideal Text Color Based on Specified Background Color
        ///     http://www.codeproject.com/KB/GDI-plus/IdealTextColor.aspx
        /// </summary>
        /// <param name="bg">The bg.</param>
        /// <returns></returns>
        private Color IdealTextColor(Color bg)
        {
            const int nThreshold = 105;
            int bgDelta = System.Convert.ToInt32((bg.R * 0.299) + (bg.G * 0.587) + (bg.B * 0.114));
            Color foreColor = (255 - bgDelta < nThreshold) ? Colors.Black : Colors.White;
            return foreColor;
        }

        #endregion
    }
}