namespace Core.Framework.Helper
{
    using System;

    public class ItemEventArgs<T,TU>:ItemEventArgs<T>
    {
        public TU Parameter { get; set; }

        public ItemEventArgs(T item,TU parameter) : base(item)
        {
            Parameter = parameter;
            Item = item;
        }
    }
    public class ItemEventArgs<T> : EventArgs
    {
        #region Constructors and Destructors

        public ItemEventArgs(T item)
        {
            Item = item;
        }

        public bool HasRetrive { get; set; }
        #endregion

        #region Public Properties

        public T Item { get; set; }

        #endregion
    }

    public class ItemEventArgs : EventArgs
    {
        #region Constructors and Destructors

        public ItemEventArgs(object item)
        {
            Item = item;
        }

        #endregion

        #region Public Properties

        public object Item { get; set; }

        #endregion
    }
}