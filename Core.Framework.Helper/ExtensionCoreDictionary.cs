namespace Core.Framework.Helper
{
    using System.Collections.Generic;

    public static class ExtensionCoreDictionary
    {
        #region Public Methods and Operators

        public static CoreDictionary<TKey, TElement> CoreDictionary<TKey, TElement>(
            this IDictionary<TKey, TElement> source)
        {
            return new CoreDictionary<TKey, TElement>(source);
        }

        #endregion
    }
}