using System.Windows;

namespace Core.Framework.Windows.Implementations
{
    public class NumericUpDownMetroChangedRoutedEventArgs : RoutedEventArgs
    {
        public double Interval { get; set; }

        public NumericUpDownMetroChangedRoutedEventArgs(RoutedEvent routedEvent, double interval)
            : base(routedEvent)
        {
            Interval = interval;
        }
    }
}