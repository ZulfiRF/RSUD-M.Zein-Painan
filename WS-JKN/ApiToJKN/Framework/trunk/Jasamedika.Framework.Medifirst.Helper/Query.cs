using Medifirst.QueryBuilder;
using Medifirst.QueryBuilder.Impl;

namespace Medifirst.SistemAdministrasi
{
    internal class Query
    {
        public static IQueryBuilder query = QueryBuilderDB.CreateDatabaseQuery(Database.SQLSERVER);
    }
}
