using System;
using System.Collections.Generic;

namespace Core.Framework.Helper.Extention
{
    public static class QueryExtention
    {
        public static bool In(this object obj, IEnumerable<short> data = null)
        {
            return true;
        }
        public static bool In(this object obj, IEnumerable<byte> data = null)
        {
            return true;
        }
        public static bool In(this object obj, IEnumerable<string> data = null)
        {
            return true;
        }
        public static bool In(this object obj, IEnumerable<int> data = null)
        {
            return true;
        }

        public static bool NotIn(this object obj, IEnumerable<dynamic> data = null)
        {
            return true;
        }
    }
}