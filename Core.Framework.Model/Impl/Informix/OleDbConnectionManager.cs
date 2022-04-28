using System;
using System.Data;
using System.Data.OleDb;

namespace Core.Framework.Model.Impl.Informix
{
    /// <summary>
    ///     Class SqlConnectionManager
    /// </summary>
    public class OleDbConnectionManager : BaseInformixConnectionManager
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseConnectionManager" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public OleDbConnectionManager(string connectionString)
            : base(connectionString)
        {
            ConnectionString = connectionString;
        }

        public override string ConnectionString
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
                connection = new OleDbConnection(ConnectionString);
                return connection;
            }
        }

        public override IDbCommand Command
        {
            get { return new OleDbCommand(); }
        }

        public override IDbTransaction Transaction
        {
            get { return null; }
        }
    }
}