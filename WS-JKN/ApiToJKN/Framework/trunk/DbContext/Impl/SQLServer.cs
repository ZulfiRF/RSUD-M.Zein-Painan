using System.Data.SqlClient;

namespace DbContext
{
    public class SQLServer : IConnection
    {
        private SqlTransaction transaction { get; set; }

        private SqlCommand command { get; set; }

        private SqlCommand reader { get; set; }

        private SqlConnection connection { get; set; }

        private string connectionString;

        public SQLServer(string conn)
        {
            connection = new SqlConnection(conn);
            connectionString = conn;
            command = new SqlCommand();
            command.Connection = connection;
            reader = new SqlCommand();
            reader.Connection = connection;
        }

        public void BeginTransaction()
        {
            transaction = connection.BeginTransaction();
            command.Transaction = transaction;
            reader.Transaction = transaction;
        }

        public void Commit()
        {
            transaction.Commit();
        }

        public void RollBack()
        {
            try
            {
                reader.Dispose();
                command.Dispose();
                transaction.Rollback();
            }
            catch (System.Exception)
            {
            }
        }

        public string CommandText
        {
            get
            {
                return command.CommandText;
            }
            set
            {
                command.CommandText = value;
            }
        }

        public void ExecuteNonQuery()
        {
            command.ExecuteNonQuery();
        }

        public string Connection
        {
            get
            {
                return connection.ConnectionString;
            }
            set
            {
                connection.ConnectionString = value;
            }
        }

        public void Close()
        {
            connection.Close();
        }

        public void Open()
        {
            connection = new SqlConnection(connectionString);
            command = new SqlCommand();
            command.Connection = connection;
            reader = new SqlCommand();
            reader.Connection = connection;
            connection.Open();
        }

        public string FormateDate
        {
            get
            {
                return "MM/dd/yyyy hh:mm:00";
            }
        }

        public System.Data.Common.DbDataReader ExecuteReader()
        {
            return reader.ExecuteReader();
        }

        #region IConnection Members

        public void CloseReader()
        {
            reader.CommandType = System.Data.CommandType.Text;
            reader.Dispose();
        }

        public string CommandReader
        {
            get
            {
                return reader.CommandText;
            }
            set
            {
                reader.CommandText = value;
            }
        }

        #endregion IConnection Members
    }
}