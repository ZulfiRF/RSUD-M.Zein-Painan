using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Core.Framework.Helper.Contracts;

namespace Core.Framework.Helper.Collections
{
    public class AsycnModel<T> : AsycnModel, IList<T>, IList where T : class, ILoadModel
    {
        // private readonly Dictionary<int, T> listModel = new Dictionary<int, T>();

        public event EventHandler<ItemEventArgs<T, IDataRecord>> LoadItemGeneric;

        public void OnLoadItemGeneric(ItemEventArgs<T, IDataRecord> e)
        {
            EventHandler<ItemEventArgs<T, IDataRecord>> handler = LoadItemGeneric;
            if (handler != null) handler(this, e);
        }

        protected override void OnLoadItem(ItemEventArgs<object, IDataRecord> e)
        {
            OnLoadItemGeneric(new ItemEventArgs<T, IDataRecord>(e.Item as T, e.Parameter));
            base.OnLoadItem(e);
        }
        public AsycnModel(IEnumerable<IDataRecord> sources)
            : base(sources)
        {
            TypeModel = typeof(T);
        }
        public AsycnModel(ICoreQueryable<T> sources)
            : base(sources, typeof(T))
        {

        }
        public AsycnModel(IEnumerable<T> sources)
            : base(sources, typeof(T))
        {

        }

        public AsycnModel(IEnumerable<IDataRecord> sources, long countItem)
            : base(sources, countItem)
        {
            TypeModel = typeof(T);
        }

        #region Implementation of IEnumerable


        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            for (int i = 0; i < count; i++)
            {
                yield return this[i] as T;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<T>

        #region IList Members

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.ICollection"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        int ICollection.Count
        {
            get { return Convert.ToInt32(count); }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.IList"/> is read-only; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        bool IList.IsReadOnly
        {
            get { return isReadOnly; }
        }




        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <param name="index">The zero-based index of the element to get or set. </param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>. </exception><exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.IList"/> is read-only. </exception><filterpriority>2</filterpriority>
        object IList.this[int index]
        {
            get { return this[index]; }
            set { }
        }



        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only. </exception><filterpriority>2</filterpriority>
        void IList.Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.IList"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove. </param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>. </exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception><filterpriority>2</filterpriority>
        void IList.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        #region PageSize

        private readonly int _pageSize = 100;

        /// <summary>
        /// Gets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        public int PageSize
        {
            get { return _pageSize; }
        }

        #endregion

        #region PageTimeout

        private readonly long _pageTimeout = 10000;

        /// <summary>
        /// Gets the page timeout.
        /// </summary>
        /// <value>The page timeout.</value>
        public long PageTimeout
        {
            get { return _pageTimeout; }
        }

        #endregion

        #region IList<T> Members

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        int ICollection<T>.Count
        {
            get { return Convert.ToInt32(count); }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        bool ICollection<T>.IsReadOnly
        {
            get { return isReadOnly; }
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        /// <summary>on
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {

        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
        void ICollection<T>.Clear()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }
        #endregion

        #endregion

        #region Implementation of IList<T>

        public new event EventHandler<ItemEventArgs<T, IDataRecord>> LoadItem;
        //private IEnumerable<IDataRecord> model;
        //private long count_2;

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <param name="index">The zero-based index of the element to get or set.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>        
        T IList<T>.this[int index]
        {
            get
            {
                return base[index] as T;
            }
            set { base[index] = value; }
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
        void IList<T>.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param><param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
        public void Insert(int index, T item)
        {
            var newDict = new Dictionary<int, object>();
            var i = 0;
            foreach (var o in ListModel)
            {
                if (i == index)
                { newDict.Add(index, item); i++; }
                newDict.Add(i, o.Value);
                i++;
            }
            i = 0;
            var dictionaryRecordTemp = new Dictionary<int, IDataRecord>();
            foreach (var dataRecord in dictionaryRecord)
            {
                if (i == index)
                {
                    dictionaryRecordTemp.Add(index, null); i++;
                }
                dictionaryRecordTemp.Add(i, dataRecord.Value);
                i++;
            }
            dictionaryRecord = dictionaryRecordTemp;
            ListModel = newDict;
            if (!FromFramework)
            {
                Sources = ListModel;
            }
            count = count + 1;
            lastIndex = lastIndex + 1;

        }
        protected void OnLoadItem(ItemEventArgs<T, IDataRecord> e)
        {
            EventHandler<ItemEventArgs<T, IDataRecord>> handler = LoadItem;
            if (handler != null) handler(this, e);
        }

        #endregion
        #endregion
    }
}
