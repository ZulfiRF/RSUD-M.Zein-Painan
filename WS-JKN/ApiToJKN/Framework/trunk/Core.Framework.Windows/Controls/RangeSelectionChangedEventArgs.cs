namespace Core.Framework.Windows.Controls
{
    using System.Windows;

    public class RangeSelectionChangedEventArgs : RoutedEventArgs
    {
        #region Constructors and Destructors

        internal RangeSelectionChangedEventArgs(long newRangeStart, long newRangeStop)
        {
            this.NewRangeStart = newRangeStart;
            this.NewRangeStop = newRangeStop;
        }

        internal RangeSelectionChangedEventArgs(RangeSlider slider)
            : this(slider.RangeStartSelected, slider.RangeStopSelected)
        {
        }

        #endregion

        #region Public Properties

        public long NewRangeStart { get; set; }
        public long NewRangeStop { get; set; }

        #endregion
    }
}