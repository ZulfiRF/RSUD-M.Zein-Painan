namespace Core.Framework.Model.Provider.Adapter
{
    internal class SqlServer2008Adapter : SqlServerAdapterBase, ISqlAdapter
    {
        

        public string QueryStringPage(string source, string selection, string conditions, string order, int pageSize,
            int pageNumber)
        {
            var str = string.Format("SELECT {0},ROW_NUMBER() OVER ({1}) AS RN FROM {2} {3}", selection, order, source,
                conditions);
            return string.Format("SELECT TOP {0} * FROM ({1}) InnerQuery WHERE RN > {2} ORDER BY RN", pageSize, str,
                pageSize * (pageNumber - 1));
        }

        
    }
}