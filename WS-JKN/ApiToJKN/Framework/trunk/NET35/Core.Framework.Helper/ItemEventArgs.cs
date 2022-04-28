using System;

namespace Core.Framework.Helper
{
   
    public class ItemEventArgs<T> : EventArgs
    {
        public T Item { get; set; }

        public ItemEventArgs(T item)
        {
            Item = item;
        }
    }
    public class ItemEventArgs : EventArgs
    {
        public object Item { get; set; }

        public ItemEventArgs(object item)
        {
            Item = item;
        }
    }
}