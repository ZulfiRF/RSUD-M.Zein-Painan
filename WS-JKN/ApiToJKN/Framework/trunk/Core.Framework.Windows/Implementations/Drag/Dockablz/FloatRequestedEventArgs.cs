using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Core.Framework.Windows.Implementations.Drag.Dockablz
{
    public class FloatRequestedEventArgs : DragablzItemEventArgs
    {
        public FloatRequestedEventArgs(RoutedEvent routedEvent, object source, DragablzItem dragablzItem) 
            : base(routedEvent, source, dragablzItem)
        { }

        public FloatRequestedEventArgs(RoutedEvent routedEvent, DragablzItem dragablzItem) 
            : base(routedEvent, dragablzItem)
        { }
    }
}
