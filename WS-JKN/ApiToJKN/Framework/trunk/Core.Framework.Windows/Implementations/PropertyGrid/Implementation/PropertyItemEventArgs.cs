using System.Windows;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation
{
    public class PropertyItemEventArgs : RoutedEventArgs
    {
        public PropertyItemBase PropertyItem
        {
            get;
            private set;
        }

        public object Item
        {
            get;
            private set;
        }

        public PropertyItemEventArgs(RoutedEvent routedEvent, object source, PropertyItemBase propertyItem, object item)
            : base(routedEvent, source)
        {
            this.PropertyItem = propertyItem;
            this.Item = item;
        }
    }
}