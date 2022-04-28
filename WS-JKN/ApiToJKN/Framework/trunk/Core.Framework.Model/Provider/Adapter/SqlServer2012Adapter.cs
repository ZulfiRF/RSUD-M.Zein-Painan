﻿namespace Core.Framework.Model.Provider.Adapter
{
    internal class SqlServer2012Adapter : SqlServerAdapterBase, ISqlAdapter
    {
        public string QueryStringPage(string source, string selection, string conditions, string order, int pageSize,
            int pageNumber)
        {
            return string.Format("SELECT {0} FROM {1} {2} {3} OFFSET {4} ROWS FETCH NEXT {5} ROWS ONLY", selection,
                source, conditions, order, pageSize*(pageNumber - 1), pageSize);
        }
    }
}