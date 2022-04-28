using System;
using System.Data.Common;

namespace DbContext
{
    public class Connection : IConnection, IDisposable
    {
        public IConnection Provider { get; private set; }

        public IConnection Reader { get; private set; }

        public static string ConnectionString { get; set; }

        public enum DatabaseProvider
        {
            SQLServer,
            MySql
        }

        private DatabaseProvider _dbProvider;

        public DatabaseProvider DbProvider
        {
            get
            {
                return _dbProvider;
            }
            set
            {
                _dbProvider = value;
                switch (value)
                {
                    case DatabaseProvider.SQLServer:
                        Provider = new SQLServer(ConnectionString);
                        Reader = new SQLServer(ConnectionString);
                        break;

                    case DatabaseProvider.MySql:

                        //Provider = new MySql(ConnectionString);
                        //Reader = new MySql(ConnectionString);
                        break;

                    default:
                        Provider = new SQLServer(ConnectionString);
                        Reader = new SQLServer(ConnectionString);
                        break;
                }
            }
        }

        public Connection()
            : this(System.Configuration.ConfigurationManager.AppSettings["Connection"], DatabaseProvider.SQLServer)
        {
            DbProvider = DatabaseProvider.SQLServer;
        }

        public Connection(DatabaseProvider dbProvider)
            : this(System.Configuration.ConfigurationManager.AppSettings["Connection"], dbProvider)
        {
        }

        public Connection(string conn)
            : this(conn, DatabaseProvider.SQLServer)
        {
        }

        public Connection(string conn, DatabaseProvider dbProvider)
        {
            ConnectionString = conn;
            DbProvider = dbProvider;
        }

        #region IConnection

        public void BeginTransaction()
        {
            if (Provider != null)
            {
                Provider.BeginTransaction();
            }
        }

        public void Commit()
        {
            if (Provider != null)
            {
                Provider.Commit();
            }
        }

        public void RollBack()
        {
            if (Provider != null)
            {
                Provider.RollBack();
            }
        }

        public string CommandText
        {
            get
            {
                return Provider.CommandText;
            }
            set
            {
                Provider.CommandText = value;
            }
        }

        public void ExecuteNonQuery()
        {
            if (Provider != null)
            {
                Provider.ExecuteNonQuery();
            }
        }

        public void Dispose()
        {
            if (Provider != null)
            {
                Provider.Close();
            }
        }

        public void Close()
        {
            if (Provider != null)
            {
                Provider.Close();
            }
        }

        public void Open()
        {
            if (Provider != null)
            {
                Provider.Open();
            }
        }

        public string FormateDate
        {
            get
            {
                if (Provider != null)
                {
                    return Provider.FormateDate;
                }
                else
                {
                    return "";
                }
            }
        }

        #endregion IConnection

        public DbDataReader ExecuteReader()
        {
            if (Provider != null)
            {
                return Provider.ExecuteReader();
            }
            else
            {
                return null;
            }
        }

        #region IConnection Members

        public void CloseReader()
        {
            if (Provider != null)
            {
                Provider.CloseReader();
            }
        }

        #endregion IConnection Members

        #region IConnection Members

        public string CommandReader
        {
            get
            {
                return Provider.CommandReader;
            }
            set
            {
                Provider.CommandReader = value;
            }
        }

        #endregion IConnection Members
    }
}