using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Framework.Helper;

namespace System.Linq
{
    public static class ExtensionLinq
    {
        public static CoreDictionary<TKey, TElement> ToCoreDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            var result=new CoreDictionary<TKey, TElement>();
            foreach (var source1 in source)
            {
                result.Add(keySelector.Invoke(source1), elementSelector.Invoke(source1));
            }
            return result;
        }
    }

}
namespace Core.Framework.Helper
{
    public class CoreDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public CoreDictionary()
        {
            
        }
        public CoreDictionary(IDictionary<string, object> cloneDictionary)
        {
            
        }
        public new TValue this[TKey key]
        {
            get
            {
                TValue result;
                if (!TryGetValue(key, out result))
                {
                    Add(key, default(TValue));
                }
                return base[key];
            }
            set
            {
                TValue result;
                if (!TryGetValue(key, out result))
                {
                    Add(key, value);
                }
                else
                    base[key] = value;
            }
        }
    }
}
