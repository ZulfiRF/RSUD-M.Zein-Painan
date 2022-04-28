using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Framework.Helper.Extention
{
    public static class CollectionHelper
    {
        public static T One<T>(this IEnumerable collection, Int16 index = 0)
        {
            try
            {
                var arrr = collection.OfType<T>().ToArray();
                if (arrr.Length < index)
                    return default(T);
                return arrr[index];
            }
            catch (Exception e)
            {
                return default(T);
                throw new Exception(e.Message);
            }
        }
    }
}
