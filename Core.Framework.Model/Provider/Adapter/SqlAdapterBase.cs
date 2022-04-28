namespace Core.Framework.Model.Provider.Adapter
{
    internal class SqlAdapterBase
    {
        public string QueryString(string selection, string source, string conditions, string order, string grouping,
            string having, bool isExist = false, int top = int.MaxValue)
        {
            if (isExist)
                return string.Format("SELECT " +
                                     "CASE WHEN ( EXISTS (SELECT " +
                                     " 1 AS [C1]" +
                                     " FROM {1} {2} {3} {4} {5}" +
                                     " )) THEN cast(1 as bit) ELSE cast(0 as bit) END AS [C1]" +
                                     " FROM  ( SELECT 1 AS X ) AS [SingleRowTable1]", selection, source, conditions,
                    order, grouping, having);
            if (top != int.MaxValue)
                return string.Format("SELECT TOP {0} {1} FROM {2} {3} {4} {5} {6}", top, selection, source, conditions,
                    order, grouping, having);
            return string.Format("SELECT {0} FROM {1} {2} {3} {4} {5}", selection, source, conditions, order, grouping,
                having);
        }
    }
}