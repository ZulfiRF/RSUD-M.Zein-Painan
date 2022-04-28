using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using Core.Framework.Helper;
using Core.Framework.Model.Attr;
using Core.Framework.Model.Contract;
using Core.Framework.Model.Helper.Odata;
using Core.Framework.Model.QueryBuilder.Clausa;
using Core.Framework.Model.QueryBuilder.Enums;

namespace Core.Framework.Model.Impl
{
    /// <summary>
    ///     Class BaseConnectionManager
    /// </summary>
    [Serializable]
    public abstract class BaseConnectionManager : IConnectionManager
    {
        public BaseConnectionManager()
        {
            ConnectionString = ConfigurationManager.AppSettings["ConnectionCore"];
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseConnectionManager" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public BaseConnectionManager(string connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        ///     Gets the connection. digunakan untuk Connection saat mengakses database
        /// </summary>
        /// <value>The connection.</value>
        public abstract IDbConnection Connection { get; }

        /// <summary>
        ///     Gets the command. digunakan untuk mengelola sql query
        /// </summary>
        /// <value>The command.</value>
        public abstract IDbCommand Command { get; }

        /// <summary>
        ///     Gets the transaction.  digunakan untuk melakukan transaction dan roll back
        /// </summary>
        /// <value>The transaction.</value>
        public abstract IDbTransaction Transaction { get; }

        /// <summary>
        ///     Gets or sets the connection string. digunakan untuk menyeting connection string
        /// </summary>
        /// <value>The connection string.</value>
        public abstract string ConnectionString { get; set; }

        /// <summary>
        ///     Gets the formate date. digunakan untuk konversi date time ke bentuk string
        /// </summary>
        /// <value>The formate date.</value>
        public abstract string FormateDate { get; }

        /// <summary>
        ///     Converts the boolean. digunakan untuk konversi boolean ke bentuk string
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns>System.String.</returns>
        public abstract string ConvertBoolean(bool value);

        /// <summary>
        ///     Creates the table.digunakan untuk membuat table
        /// </summary>
        /// <param name="item">berisikan object yang akan di buat table</param>
        /// <returns>
        ///     <c>true</c> jika berhasil melakakukan penyimpanan pada database, <c>false</c> jika terjadi kesalahan pada
        ///     proses penyimpanan pada database
        /// </returns>
        public abstract bool CreateTable(TableItem item);

        /// <summary>
        ///     Creates the database.
        /// </summary>
        /// <returns>
        ///     <c>true</c> jika berhasil melakakukan pembuatan pada database, <c>false</c> jika terjadi kesalahan pada proses
        ///     pembuatan database
        /// </returns>
        public abstract bool CreateDatabase();

        /// <summary>
        ///     Creates the relation query. digunakan untuk konversi dari atrribute <see cref="ReferenceAttribute" /> ke bentuk sql
        /// </summary>
        /// <param name="related">The related.</param>
        /// <returns>System.String.</returns>
        public virtual string CreateRelationQuery(string[] related)
        {
            var query = related.Aggregate("", (current, key) => current + (key + " AND "));
            if (query.Length - 5 > 0)
                query = query.Substring(0, query.Length - 5);
            return query;
        }

        /// <summary>
        ///     Create Filter Row digunakan untuk konversi dari WhereClause <see cref="ReferenceAttribute" /> ke bentuk sql
        /// </summary>
        /// <param name="whereClouses"> berisikan where clouse yang di gunakan untuk melakukan filtering.</param>
        /// <returns>System.String.</returns>
        public virtual string CreateFilterRow(WhereClause[] whereClouses)
        {
            var stringBuilder = new StringBuilder();
            foreach (var whereClouse in whereClouses)
            {
                switch (whereClouse.ComparisonOperator)
                {
                    case Comparison.Equals:
                        if (whereClouse.Value == null)
                            stringBuilder.Append(" " + whereClouse.FieldName + " is null");
                        else
                            stringBuilder.Append(" " + whereClouse.FieldName + " = '" + whereClouse.Value + "'");
                        break;
                    case Comparison.NotEquals:
                        if (whereClouse.Value == null)
                            stringBuilder.Append(" " + whereClouse.FieldName + " is not null");
                        else
                            stringBuilder.Append(" " + whereClouse.FieldName + " <> '" + whereClouse.Value + "'");
                        break;
                    case Comparison.Like:
                        stringBuilder.Append(" " + whereClouse.FieldName + " Like '%" + whereClouse.Value + "%'");
                        break;
                    case Comparison.NotLike:
                        break;
                    case Comparison.GreaterThan:
                        stringBuilder.Append(" " + whereClouse.FieldName + " > '" + whereClouse.Value + "'");
                        break;
                    case Comparison.GreaterOrEquals:
                        stringBuilder.Append(" " + whereClouse.FieldName + " >= '" + whereClouse.Value + "'");
                        break;
                    case Comparison.LessThan:
                        stringBuilder.Append(" " + whereClouse.FieldName + " < '" + whereClouse.Value + "'");
                        break;
                    case Comparison.LessOrEquals:
                        stringBuilder.Append(" " + whereClouse.FieldName + " <= '" + whereClouse.Value + "'");
                        break;
                    case Comparison.In:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                stringBuilder.Append((whereClouse.LogicOperator == LogicOperator.Or) ? " Or " : " And ");
            }
            var queryResult = stringBuilder.ToString();
            if (queryResult.EndsWith(" And "))
                queryResult = queryResult.Substring(0, queryResult.Length - 5);
            else if (queryResult.EndsWith(" Or "))
                queryResult = queryResult.Substring(0, queryResult.Length - 4);
            return queryResult;
        }

        public virtual string ConvertToQuery(string table, string query)
        {
            return "Select * from " + table + " Where " + query;
        }

        public virtual object ConverterToObject(object value, object modelItem, string member)
        {
            return value;
        }

        public override string ToString()
        {
            return "Hello";
        }

        public virtual string ConvertField(string field)
        {
            return field;
        }

        public abstract string CheckConnection();

        public virtual string CreateConnectioString(ConnectionDomain connectionDomain)
        {
            string connectionString = null;
            if (!string.IsNullOrEmpty(connectionDomain.ConnectionTimeOut))
            {
                connectionString = @"Data Source=" + connectionDomain.ServerName + ";Initial Catalog=" +
                                   connectionDomain.Database + ";Persist Security Info=True;User ID=" +
                                   connectionDomain.UserName + ";Password=" + connectionDomain.Password + ";Connection Timeout=" + connectionDomain.ConnectionTimeOut + "";

            }
            else
            {
                connectionString = @"Data Source=" + connectionDomain.ServerName + ";Initial Catalog=" +
                                   connectionDomain.Database + ";Persist Security Info=True;User ID=" +
                                   connectionDomain.UserName + ";Password=" + connectionDomain.Password;

            }
            return connectionString;
        }

        #region Implementation of IOdataQuery

        /// <summary>
        ///     Equals the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public abstract string Equal(string input);

        /// <summary>
        ///     Nulls the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public abstract string Null(string input);

        /// <summary>
        ///     Nots the equal.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public abstract string NotEqual(string input);

        /// <summary>
        ///     Greaters the than.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public abstract string GreaterThan(string input);

        /// <summary>
        ///     Greaters the than or equal.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public abstract string GreaterThanOrEqual(string input);

        /// <summary>
        ///     Lesses the than.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public abstract string LessThan(string input);

        /// <summary>
        ///     Lesses the than or equal.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public abstract string LessThanOrEqual(string input);

        /// <summary>
        ///     Ands the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public abstract string And(string input);

        /// <summary>
        ///     Ors the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public abstract string Or(string input);

        /// <summary>
        ///     Nots the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public abstract string Not(string input);

        /// <summary>
        ///     Startswithes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public abstract string Startswith(string input);

        /// <summary>
        ///     Endwithes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public abstract string Endwith(string input);

        /// <summary>
        ///     Determines whether [contains] [the specified input].
        /// </summary>
        /// <param name="input">The input.</param>
        public abstract string Contains(string input);

        /// <summary>
        ///     Creates the filter.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>ResultQuery.</returns>
        public abstract ResultQuery CreateFilter(string input);

        #endregion Implementation of IOdataQuery
    }
}