using System;
using System.Data;
using System.Data.Odbc;

namespace Core.Framework.Model.Impl.Informix
{
    /// <summary>
    ///     Class SqlConnectionManager
    /// </summary>
    public class OdbcConnectionManager : BaseInformixConnectionManager
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseConnectionManager" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public OdbcConnectionManager(string connectionString)
            : base(connectionString)
        {
            ConnectionString = connectionString;
        }

        public override sealed string ConnectionString
        {
            get { return base.ConnectionString; }
            set { base.ConnectionString = value; }
        }

        public override IDbConnection Connection
        {
            get
            {
                if (string.IsNullOrEmpty(ConnectionString))
                    throw new ArgumentNullException("Connection String is Null or Empty");
                connection = new OdbcConnection(ConnectionString);
                return connection;
            }
        }

        public override IDbCommand Command
        {
            get { return new OdbcCommand(); }
        }

        public override IDbTransaction Transaction
        {
            get
            {
                if (connection != null)
                    return connection.BeginTransaction();
                throw new ArgumentNullException("Conection Manager is Null");
            }
        }
    }
}