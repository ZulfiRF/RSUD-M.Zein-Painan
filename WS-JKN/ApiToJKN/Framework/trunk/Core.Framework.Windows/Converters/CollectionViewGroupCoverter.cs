namespace Core.Framework.Windows.Converters
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;

    public class CollectionViewGroupCoverter : IValueConverter
    {
        #region Public Methods and Operators

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                int index = -1;
                object parent =
                    value.GetType()
                        .GetProperty(
                            "Parent",
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                            | BindingFlags.CreateInstance)
                        .GetValue(value, null);
                while (parent != null)
                {
                    index++;
                    parent =
                        parent.GetType()
                            .GetProperty(
                                "Parent",
                                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                | BindingFlags.CreateInstance)
                            .GetValue(parent, null);
                }
                if (parameter == null)
                {
                    return new Thickness(index * 25, 0, 0, 0);
                }
                return new Thickness(index * -25, 0, 0, 0);
            }
            catch (Exception)
            {
            }
            return new Thickness(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}