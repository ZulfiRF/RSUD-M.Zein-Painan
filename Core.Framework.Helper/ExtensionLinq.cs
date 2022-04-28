using System;
using System.Collections.Generic;

namespace Core.Framework.Helper
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