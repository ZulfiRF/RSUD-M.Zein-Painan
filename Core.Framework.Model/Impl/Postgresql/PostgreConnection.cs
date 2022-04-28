using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Core.Framework.Model.Helper.Odata;
using Npgsql;


namespace Core.Framework.Model.Impl.Postgresql
{
    [Serializable]
    public sealed class PostgreConnection : BaseConnectionManager
    {
        public PostgreConnection(string connectionString):base(connectionString)
        {
            ConnectionString = connectionString;
        }
        public override string CheckConnection()
        {
            throw new NotImplementedException();
        }

        public override string Equal(string input)
        {
            throw new NotImplementedException();
        }

        public override string Null(string input)
        {
            throw new NotImplementedException();
        }

        public override string NotEqual(string input)
        {
            throw new NotImplementedException();
        }

        public override string GreaterThan(string input)
        {
            throw new NotImplementedException();
        }

        public override string GreaterThanOrEqual(string input)
        {
            throw new NotImplementedException();
        }

        public override string LessThan(string input)
        {
            throw new NotImplementedException();
        }

        public override string LessThanOrEqual(string input)
        {
            throw new NotImplementedException();
        }

        public override string And(string input)
        {
            throw new NotImplementedException();
        }

        public override string Or(string input)
        {
            throw new NotImplementedException();
        }

        public override string Not(string input)
        {
            throw new NotImplementedException();
        }

        public override string Startswith(string input)
        {
            throw new NotImplementedException();
        }

        public override string Endwith(string input)
        {
            throw new NotImplementedException();
        }

        public override string Contains(string input)
        {
            throw new NotImplementedException();
        }

        public override ResultQuery CreateFilter(string input)
        {
            throw new NotImplementedException();
        }

        private IDbConnection connection;
        public override IDbConnection Connection
        {
            get
            {
                if (string.IsNullOrEmpty(ConnectionString))
                    throw new ArgumentNullException("Connection String is Null or Empty");
                connection = new NpgsqlConnection(ConnectionString);
                return connection;
            }
        }

        public override IDbCommand Command
        {
            get { return new NpgsqlCommand(); }
        }

        public override IDbTransaction Transaction
        {
            get
            {
                if (connection != null)
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    return connection.BeginTransaction();
                }
                throw new ArgumentNullException("Conection Manager is Null");
            }
        }

        public override string ConnectionString { get; set; }
        public override string FormateDate { get { return "yyyy/MM/dd HH:mm:ss"; } }
        public override string ConvertBoolean(bool value)
        {
            throw new NotImplementedException();
        }

        public override bool CreateTable(TableItem item)
        {
            throw new NotImplementedException();
        }

        public override bool CreateDatabase()
        {
            throw new NotImplementedException();
        }
    }
}
