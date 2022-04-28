using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Core.Framework.Helper.Collections
{
    public class AsycnModel : IList
    {
        public bool IsBusy { get; set; }
        protected Dictionary<int, object> ListModel = new Dictionary<int, object>();

        protected long count;

        protected bool isFixedSize;
        protected bool isReadOnly;
        protected bool isSynchronized;
        protected object syncRoot;
        public AsycnModel(IEnumerable<IDataRecord> sources, long countData = int.MaxValue)
        {
            Sources = sources;
            FromFramework = true;
            count = countData;
            lazyLoadEnumerator = Sources.GetEnumerator();
            dictionary.BeforeGetValueNull += DictionaryOnBeforeGetValueNull;
        }

        private void DictionaryOnBeforeGetValueNull(object sender, ItemEventArgs<CoreDictionary<int, object>.KeyItem> eventArgs)
        {
        }

        public AsycnModel(IEnumerable<IDataRecord> sources)
        {
            Sources = sources;
            FromFramework = true;
            int i = 0;
            foreach (var source in sources)
            {
                i++;
            }
            count = i;
            lazyLoadEnumerator = Sources.GetEnumerator();
            dictionary.BeforeGetValueNull += DictionaryOnBeforeGetValueNull;
        }

        public AsycnModel(IEnumerable sources, Type typeModel)
        {
            Sources = sources;
            TypeModel = typeModel;
            FromFramework = false;
            int i = 0;
            foreach (var source in sources)
            {
                i++;
            }
            count = i;
            lazyLoadEnumerator = Sources.GetEnumerator();
            dictionary.BeforeGetValueNull += DictionaryOnBeforeGetValueNull;
        }



        public bool FromFramework { get; protected set; }

        public IEnumerable Sources { get; protected set; }
        public Type TypeModel { get; set; }
        #region Implementation of IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < count; i++)
            {
                yield return this[i];
            }
            //return Sources.GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.ICollection"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public int Count
        {
            get { return Convert.ToInt32(count); }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
        /// </summary>
        /// <returns>
        /// true if access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe); otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsSynchronized
        {
            get { return isSynchronized; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <returns>
        /// An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object SyncRoot
        {
            get { return syncRoot; }
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"/>. The <see cref="T:System.Array"/> must have zero-based indexing. </param><param name="index">The zero-based index in <paramref name="array"/> at which copying begins. </param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null. </exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero. </exception><exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.-or- The number of elements in the source <see cref="T:System.Collections.ICollection"/> is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>. </exception><exception cref="T:System.ArgumentException">The type of the source <see cref="T:System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <paramref name="array"/>. </exception><filterpriority>2</filterpriority>
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Implementation of IList

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> has a fixed size.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.IList"/> has a fixed size; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsFixedSize
        {
            get { return isFixedSize; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.IList"/> is read-only; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsReadOnly
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
        private readonly IEnumerator lazyLoadEnumerator;
        protected Dictionary<int, IDataRecord> dictionaryRecord = new Dictionary<int, IDataRecord>();
        private readonly CoreDictionary<int, object> dictionary = new CoreDictionary<int, object>();
        protected int lastIndex;
        public object this[int index]
        {
            get
            {
                //return Sources.Cast<object>().ToArray()[index];
                if (index > count)
                    return null;
                object result = null;
                //  result = dictionary[index];

                //ManualResetEvent manualReset = new ManualResetEvent(false);
                //ThreadPool.QueueUserWorkItem(delegate(object state)
                //                                 {
                //var tempIndex = (int)state;
                var tempIndex = (int)index;
                //Thread.Sleep(2);
                if (!ListModel.TryGetValue(tempIndex, out result))
                {
                    if (FromFramework)
                    {
                        result = Activator.CreateInstance(TypeModel) as ILoadModel;
                        (result as ILoadModel).Skip = true;
                        int i = lastIndex + 1;
                        IDataRecord dataRecord;
                        if (dictionaryRecord.TryGetValue(tempIndex, out dataRecord))
                        {

                        }
                        else
                        {
                            var max = dictionaryRecord.Any() ? dictionaryRecord.Max(n => n.Key) : 0;
                            if (max + 1 != tempIndex && max != tempIndex)
                            {
                                if (lazyLoadEnumerator.Current != null)
                                {
                                    //    lazyLoadEnumerator = Sources.GetEnumerator();
                                }
                                while (true)
                                {
                                    lazyLoadEnumerator.MoveNext();
                                    if (!dictionaryRecord.TryGetValue(i, out dataRecord))
                                    {
                                        //  Thread.Sleep(10);
                                        dataRecord = lazyLoadEnumerator.Current as IDataRecord;
                                        if (i != count)
                                            dictionaryRecord.Add(i, dataRecord);
                                    }
                                    if (i == tempIndex)
                                    {
                                        break;
                                    }
                                    i++;
                                }
                            }
                            else
                            {
                                lastIndex = tempIndex;
                                lazyLoadEnumerator.MoveNext();
                                dataRecord = lazyLoadEnumerator.Current as IDataRecord;
                                if (tempIndex != count)
                                    dictionaryRecord.Add(tempIndex, dataRecord);
                            }

                        }
                        //                        dataRecord = Sources.Cast<IDataRecord>().ToArray()[tempIndex];
                        var loadModel = result as ILoadModel;
                        if (loadModel != null)
                        {
                            loadModel.OnInitLoad(dataRecord);
                            (result as ILoadModel).IsLoad = true;
                        }
                        OnLoadItem(new ItemEventArgs<object, IDataRecord>(result, dataRecord));
                        
                        object data;
                        if (!ListModel.TryGetValue(tempIndex, out data))
                            ListModel.Add(tempIndex, result);

                    }
                    else
                    {
                        ListModel.Add(tempIndex, Sources.Cast<object>().ToArray()[tempIndex]);
                    }
                }
                else
                {
                }
                return result;
            }
            set
            {
                object result;
                if (!ListModel.TryGetValue(index, out result))
                {
                    result = Activator.CreateInstance(TypeModel);
                    var loadModel = result as ILoadModel;
                    if (loadModel != null) loadModel.IsLoad = true;
                    ListModel.Add(index, result);
                }
                else
                {                    
                    ListModel[index] = value;
                }
            }
        }
        protected event EventHandler<ItemEventArgs<object, IDataRecord>> LoadItem;
        protected virtual void OnLoadItem(ItemEventArgs<object, IDataRecord> e)
        {
            EventHandler<ItemEventArgs<object, IDataRecord>> handler = LoadItem;
            if (handler != null) handler(this, e);
        }
        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <returns>
        /// The position into which the new element was inserted, or -1 to indicate that the item was not inserted into the collection,
        /// </returns>
        /// <param name="value">The object to add to the <see cref="T:System.Collections.IList"/>. </param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception><filterpriority>2</filterpriority>
        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only. </exception><filterpriority>2</filterpriority>
        public void Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.IList"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Object"/> is found in the <see cref="T:System.Collections.IList"/>; otherwise, false.
        /// </returns>
        /// <param name="value">The object to locate in the <see cref="T:System.Collections.IList"/>. </param><filterpriority>2</filterpriority>
        public bool Contains(object value)
        {
            return false;
        }
        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <returns>
        /// The index of <paramref name="value"/> if found in the list; otherwise, -1.
        /// </returns>
        /// <param name="value">The object to locate in the <see cref="T:System.Collections.IList"/>. </param><filterpriority>2</filterpriority>
        public int IndexOf(object value)
        {
            return -1;
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.IList"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="value"/> should be inserted. </param><param name="value">The object to insert into the <see cref="T:System.Collections.IList"/>. </param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>. </exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception><exception cref="T:System.NullReferenceException"><paramref name="value"/> is null reference in the <see cref="T:System.Collections.IList"/>.</exception><filterpriority>2</filterpriority>
        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <param name="value">The object to remove from the <see cref="T:System.Collections.IList"/>. </param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception><filterpriority>2</filterpriority>
        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.IList"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove. </param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>. </exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception><filterpriority>2</filterpriority>
        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }
        #endregion
    }


}