using System.Data;
using Core.Framework.Model.Attr;
using Core.Framework.Model.Helper.Odata;
using Core.Framework.Model.QueryBuilder.Clausa;

namespace Core.Framework.Model.Contract
{
    /// <summary>
    ///     Interface IConnectionManager
    /// </summary>
    public interface IConnectionManager : IOdataQuery
    {
        string CheckConnection();
        /// <summary>
        ///     Gets the connection. digunakan untuk Connection saat mengakses database
        /// </summary>
        /// <value>The connection.</value>
        IDbConnection Connection { get; }

        /// <summary>
        ///     Gets the command. digunakan untuk mengelola sql query
        /// </summary>
        /// <value>The command.</value>
        IDbCommand Command { get; }

        /// <summary>
        ///     Gets the transaction.  digunakan untuk melakukan transaction dan roll back
        /// </summary>
        /// <value>The transaction.</value>
        IDbTransaction Transaction { get; }

        /// <summary>
        ///     Gets or sets the connection string. digunakan untuk menyeting connection string
        /// </summary>
        /// <value>The connection string.</value>
        string ConnectionString { get; set; }

        /// <summary>
        ///     Gets the formate date. digunakan untuk konversi date time ke bentuk string
        /// </summary>
        /// <value>The formate date.</value>
        string FormateDate { get; }

        /// <summary>
        ///     Converts the boolean. digunakan untuk konversi boolean ke bentuk string
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns>System.String.</returns>
        string ConvertBoolean(bool value);

        /// <summary>
        ///     Creates the table.digunakan untuk membuat table
        /// </summary>
        /// <param name="item">berisikan object yang akan di buat table</param>
        /// <returns>
        ///     <c>true</c> jika berhasil melakakukan penyimpanan pada database, <c>false</c> jika terjadi kesalahan pada
        ///     proses penyimpanan pada database
        /// </returns>
        bool CreateTable(TableItem item);

        /// <summary>
        ///     Creates the database.
        /// </summary>
        /// <returns>
        ///     <c>true</c> jika berhasil melakakukan pembuatan pada database, <c>false</c> jika terjadi kesalahan pada proses
        ///     pembuatan database
        /// </returns>
        bool CreateDatabase();

        /// <summary>
        ///     Creates the relation query. digunakan untuk konversi dari atrribute <see cref="ReferenceAttribute" /> ke bentuk sql
        /// </summary>
        /// <param name="related">The related.</param>
        /// <returns>System.String.</returns>
        string CreateRelationQuery(string[] related);

        /// <summary>
        ///     Create Filter Row digunakan untuk konversi dari WhereClause <see cref="ReferenceAttribute" /> ke bentuk sql
        /// </summary>
        /// <param name="whereClouses"> berisikan where clouse yang di gunakan untuk melakukan filtering.</param>
        /// <returns>System.String.</returns>
        string CreateFilterRow(WhereClause[] whereClouses);

        string ConvertToQuery(string table, string query);
        object ConverterToObject(object value, object modelItem, string member = "");
    }
}