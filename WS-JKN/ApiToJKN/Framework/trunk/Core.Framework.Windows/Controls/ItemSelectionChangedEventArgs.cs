using System.Windows;

namespace Core.Framework.Windows.Controls
{
    public class ItemSelectionChangedEventArgs : RoutedEventArgs
    {
        public bool IsSelected
        {
            get;
            private set;
        }
        public object Item
        {
            get;
            private set;
        }

        public ItemSelectionChangedEventArgs(RoutedEvent routedEvent, object source, object item, bool isSelected)
            : base(routedEvent, source)
        {
            Item = item;
            IsSelected = isSelected;
        }
    }
}