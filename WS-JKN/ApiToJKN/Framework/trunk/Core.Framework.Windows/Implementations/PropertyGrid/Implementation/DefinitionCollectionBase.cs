using System;
using System.Collections.ObjectModel;
using Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Definitions;

namespace Core.Framework.Windows.Implementations.PropertyGrid.Implementation
{
    public abstract class DefinitionCollectionBase<T> : ObservableCollection<T> where T : DefinitionBase
    {
        internal DefinitionCollectionBase() { }

        protected override void InsertItem(int index, T item)
        {
            if (item == null)
                throw new InvalidOperationException(@"Cannot insert null items in the collection.");

            item.Lock();
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, T item)
        {
            if (item == null)
                throw new InvalidOperationException(@"Cannot insert null items in the collection.");

            item.Lock();
            base.SetItem(index, item);
        }
    }
}