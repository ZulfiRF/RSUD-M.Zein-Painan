using System;
using System.Collections.Generic;
using Core.Framework.Model.Provider.Adapter;
using Core.Framework.Model.Provider.Builder;
using Core.Framework.Model.Provider.Resolver;
using Core.Framework.Model.Provider.ValueObjects;

namespace Core.Framework.Model.Provider
{
    public abstract class SqlLamBase
    {
        internal static ISqlAdapter _defaultAdapter = new SqlServer2012Adapter();
        internal SqlQueryBuilder _builder;
        internal LambdaResolver _resolver;

        public IDictionary<string, object> QueryParameters
        {
            get { return _builder.Parameters; }
        }

        public string QueryString
        {
            get { return _builder.QueryString; }
        }

        public string[] SplitColumns
        {
            get { return _builder.SplitColumns.ToArray(); }
        }

        public SqlQueryBuilder SqlBuilder
        {
            get { return _builder; }
        }

        private static ISqlAdapter GetAdapterInstance(SqlAdapter adapter)
        {
            switch (adapter)
            {
                case SqlAdapter.SqlServer2008:
                    return new SqlServer2008Adapter();

                case SqlAdapter.SqlServer2012:
                    return new SqlServer2012Adapter();
            }
            throw new ArgumentException("The specified Sql Adapter was not recognized");
        }

        public string QueryStringPage(int pageSize, int? pageNumber = new int?())
        {
            return _builder.QueryStringPage(pageSize, pageNumber);
        }

        public static void SetAdapter(SqlAdapter adapter)
        {
            _defaultAdapter = GetAdapterInstance(adapter);
        }
    }
}