using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Framework.Helper;

namespace System.Linq
{
}
namespace Core.Framework.Helper
{
    [Serializable]
    public class CoreDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        #region Constructors and Destructors

        public CoreDictionary()
        {
        }

        public CoreDictionary(IDictionary<TKey, TValue> value)
            : base(value)
        {
        }

        #endregion

        #region Public Events

        public event EventHandler<ItemEventArgs<KeyItem>> BeforeGetValueNull;

        #endregion

        #region Public Indexers

        public new TValue this[TKey key]
        {
            get
            {
                TValue value;
                if (TryGetValue(key, out value))
                {
                    return value;
                }
                var data = new KeyItem { Key = key, Value = default(TValue) };
                OnBeforeGetValueNull(new ItemEventArgs<KeyItem>(data));
                return data.Value;
            }
            set
            {
                TValue temp;
                if (TryGetValue(key, out temp))
                {
                    base[key] = value;
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        public void OnBeforeGetValueNull(ItemEventArgs<KeyItem> e)
        {
            if (BeforeGetValueNull != null)
            {
                BeforeGetValueNull(this, e);
            }
        }

        #endregion

        public class KeyItem
        {
            #region Public Properties

            public TKey Key { get; set; }
            public TValue Value { get; set; }

            #endregion
        }
    }


}
