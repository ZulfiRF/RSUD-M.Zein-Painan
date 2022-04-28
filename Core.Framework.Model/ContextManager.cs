using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using Core.Framework.Helper;
using Core.Framework.Helper.Contracts;
using Core.Framework.Helper.Logging;
using Core.Framework.Helper.Security;
using Core.Framework.Model.Attr;
using Core.Framework.Model.Contract;
using Core.Framework.Model.Error;
using Core.Framework.Model.Impl;
using Core.Framework.Model.Impl.Postgresql;
using Core.Framework.Model.Impl.SqlServer;

namespace Core.Framework.Model
{
    /// <summary>
    ///     Class ContextManager
    /// </summary>
    [Serializable]
    public class ContextManager : IDisposable
    {
        /// <summary>
        ///     The list auto incement
        /// </summary>
        private const string ListAutoIncement = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        ///     Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public static Dictionary<Guid, Exception> GetErrorContext = new Dictionary<Guid, Exception>();

        private IDbConnection connectionAuotmaticClose;
        private static string queryTamp;

        /// <summary>
        ///     The dictionary auto increment
        /// </summary>
        private Dictionary<string, object> dictionaryAutoIncrement;

        /// <summary>
        ///     The has auto increment
        /// </summary>
        private bool hasAutoIncrement;

        private readonly CoreDictionary<string, Int64> countQueryAsycn = new CoreDictionary<string, Int64>();

        /// <summary>
        ///     The list
        /// </summary>
        private readonly Dictionary<int, char> list = new CoreDictionary<int, char>();

        private readonly List<IDbConnection> listDbConnectionCreateByRuntime = new List<IDbConnection>();

        /// <summary>
        ///     Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [freeze update].
        /// </summary>
        /// <value><c>true</c> if [freeze update]; otherwise, <c>false</c>.</value>
        public bool FreezeUpdate { get; set; }
        public Exception CurrentException { get; set; }
        public Guid MyGuid { get; set; }
        public bool OfflineMode { get; set; }
        public string CurrentSql { get; set; }
        public IDbConnection CurrentConnection { get; set; }
        public bool FailedConnection { get; set; }

        #region Implementation of IDisposable

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var dbConnection in listDbConnectionCreateByRuntime)
            {
                try
                {
                    dbConnection.Close();
                }
                catch (Exception exception)
                {
                    OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                }
            }
            TagUpdate.Clear();
            TagSave.Clear();
            TagDelete.Clear();
            if (VirtualRow != null) VirtualRow.Clear();
            VirtualRow = null;
        }

        #endregion Implementation of IDisposable

        public IDataReader ExecuteQueryAutomaticCloseConnection(string sql)
        {
            var conn = ConnectionManager.Connection;
            if (connectionAuotmaticClose == null)
            {
                connectionAuotmaticClose = conn;
            }

            var log = BaseDependency.Get<ILogRepository>();
            try
            {
                CurrentConnection = conn;
                if (connectionAuotmaticClose.State == ConnectionState.Open)
                    connectionAuotmaticClose.Close();
                if (!Current.ConnectionString.Equals(ConnectionString))
                {
                    Current = this;
                    FailedConnection = false;
                }
                if (FailedConnection) return default(IDataReader);
                connectionAuotmaticClose.Open();
                var command = ConnectionManager.Command;
                command.Connection = connectionAuotmaticClose;
                CurrentSql = sql;
                command.CommandText = sql;
                OnStreamingLog(new ItemEventArgs<string>(sql));
                return command.ExecuteReader();
            }
            catch (Exception exception)
            {
                CurrentException = exception;
                OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                OnError(new ErrorArgs(exception));

                if (log != null)
                    log.Error(exception.Message);
                return default(IDataReader);
            }
            finally
            {
                if (log != null)
                    log.Info(sql);
            }
        }

        public IDataReader ExecuteQuery(string sql)
        {
            if (sql == null)
                return default(IDataReader);
            var log = BaseDependency.Get<ILogRepository>();
            try
            {
                var conn = ConnectionManager.Connection;                
                var command = ConnectionManager.Command;
                ThreadPool.QueueUserWorkItem(state =>
                {
                    Thread.Sleep(2000);
                    try
                    {
                        var dbConnection = state as IDbConnection;
                        if (dbConnection != null)
                        {
                            //dbConnection.Dispose();
                            //listDbConnectionCreateByRuntime.Remove((state as IDbConnection));
                            command.Dispose();
                        }
                    }
                    catch (Exception exception)
                    {
                        Log.Error(exception);
                    }
                }, conn);
                CurrentConnection = conn;
                listDbConnectionCreateByRuntime.Add(conn);
                if (!Current.ConnectionString.Equals(ConnectionString))
                {
                    Current = this;
                    FailedConnection = false;
                }
                //if (FailedConnection) return default(IDataReader);
                conn.Open();

                command.Connection = conn;
                CurrentSql = sql;
                command.CommandText = sql;
                //command.CommandTimeout = 30;

                OnStreamingLog(new ItemEventArgs<string>(sql));
                return command.ExecuteReader();
            }
            catch (Exception exception)
            {
                Debug.Print(sql);
                Current.FailedConnection = true;
                FailedConnection = true;
                CurrentException = exception;
                OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                OnError(new ErrorArgs(exception));

                if (log != null)
                    log.Error(exception.Message);
                return default(IDataReader);
            }
            finally
            {
                if (log != null)
                    log.Info(sql);
            }
        }

        public event EventHandler<ItemEventArgs<string>> StreamingLog;
        public static event EventHandler<ItemEventArgs<string>> StreamingLogSingleTone;

        public void OnStreamingLog(ItemEventArgs<string> e, string log = "")
        {
            ThreadPool.QueueUserWorkItem(delegate (object state)
            {
                try
                {
                    OnStreamingLogSingleTone(new ItemEventArgs<string>(state.ToString()));
                    var handler = StreamingLog;
                    if (handler != null) handler(this, new ItemEventArgs<string>(state.ToString()));
                }
                catch (Exception)
                {
                }
            }, e.Item);
        }

        public IEnumerable<TEntity> ExecuteQuery<TEntity>(string sql, bool useCahe = true) where TEntity : BaseItem
        {
            if (sql == null)
                return null;
            try
            {
                if (useCahe)
                {
                    var data = CacheHelper.GetCache<List<TEntity>>(sql);
                    TableItem type = Activator.CreateInstance<TEntity>() as TableItem;
                    if (data != null && (type != null && !type.IsRebindFromServer))
                    {
                        return (List<TEntity>)data;
                    }
                }
                var conn = Current.ConnectionManager.Connection; // ConnectionManager.Connection;
                // listDbConnectionCreateByRuntime.Add(conn);
                try
                {
                    if (!Current.ConnectionString.Equals(ConnectionString))
                    {
                        Current = this;
                        FailedConnection = false;
                    }
                    if (FailedConnection && Current.FailedConnection) return null;
                    conn.Open();
                }
                catch (SqlException sqlException)
                {
                    Current.FailedConnection = true;
                    FailedConnection = true;
                    OnStreamingLog(new ItemEventArgs<string>(sqlException.ToString()));
                    OnError(new ErrorArgs(sqlException));
                    return null;
                }
                catch (Exception exception)
                {
                    Current.FailedConnection = true;
                    FailedConnection = true;
                    OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                    OnError(new ErrorArgs(exception));
                    return null;
                }

                var command = ConnectionManager.Command;
                command.Connection = conn;
                CurrentSql = sql;
                command.CommandText = sql;
                OnStreamingLog(new ItemEventArgs<string>(sql));
                var reader = command.ExecuteReader();
                var list = new List<TEntity>();
                while (reader.Read())
                {
                    var model = Activator.CreateInstance<TEntity>() as BaseItem;
                    model.OnInit(reader, this);
                    list.Add(model as TEntity);
                }
                CacheHelper.RegisterCache(sql, list);
                reader.Close();
                conn.Close();

                return list;

            }
            finally
            {
                var log = BaseDependency.Get<ILogRepository>();
                if (log != null)
                    log.Info(sql);
            }
        }

        public Int64 GetCountDataAsycn(string sql)
        {
            var str = sql.ToLower();
            var findKeyword = "";
            var temp = str;
            while (true)
            {
                if (temp.ToLower().Split(' ').Count(n => n.Equals("select")) != 1 ||
                    (temp.ToLower().Split(' ').Count(n => n.Equals("from")) != 1))
                    break;
                if (temp.ToLower().IndexOf("select", StringComparison.Ordinal) == -1 &&
                    temp.ToLower().IndexOf("from", StringComparison.Ordinal) == -1)
                {
                    break;
                }
                var tempSelect = temp.Substring(temp.IndexOf("select", StringComparison.Ordinal) + 7);
                var tempFrom = tempSelect.Substring(tempSelect.IndexOf(" from ", StringComparison.Ordinal) + 5);
                findKeyword = "select count(*) from " + tempFrom;
                break;
            }
            if (!string.IsNullOrEmpty(findKeyword))
            {
                Int64 result;
                if (!countQueryAsycn.TryGetValue(sql, out result))
                {
                    CurrentSql = findKeyword;
                    var readerCount = ExecuteQuery(findKeyword);
                    if (readerCount != null)
                    {
                        while (readerCount.Read())
                        {
                            countQueryAsycn.Add(sql, Convert.ToInt64(readerCount[0]));
                        }
                    }
                }
            }
            return countQueryAsycn[sql];
        }

        public IEnumerable<IDataRecord> ExecuteQueryAsync(string sql)
        {
            var conn = ConnectionManager.Connection;
            listDbConnectionCreateByRuntime.Add(conn);
            try
            {
                if (!Current.ConnectionString.Equals(ConnectionString))
                {
                    Current = this;
                    FailedConnection = false;
                }
                if (FailedConnection) return Enumerable.Empty<IDataRecord>();
                conn.Open();
            }
            catch (Exception exception)
            {
                Current.FailedConnection = true;
                FailedConnection = true;
                OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                OnError(new ErrorArgs(exception));
            }


            var command = ConnectionManager.Command;
            command.Connection = conn;
            command.CommandText = sql;
            var str = sql.ToLower();
            var findKeyword = "";
            var temp = str;
            while (true)
            {
                if (temp.ToLower().Contains("group by")) break;
                if (temp.ToLower().Split(' ').Count(n => n.Equals("select")) != 1 ||
                    (temp.ToLower().Split(' ').Count(n => n.Equals("from")) != 1))
                    break;
                if (temp.ToLower().IndexOf("select", StringComparison.Ordinal) == -1 &&
                    temp.ToLower().IndexOf("from", StringComparison.Ordinal) == -1)
                {
                    break;
                }
                var tempSelect = temp.Substring(temp.IndexOf("select", StringComparison.Ordinal) + 7);
                var tempFrom = tempSelect.Substring(tempSelect.IndexOf(" from ", StringComparison.Ordinal) + 5);
                findKeyword = "select count(*) from " + tempFrom;
                if (findKeyword.IndexOf("order ") != -1)
                    findKeyword = findKeyword.Substring(0, findKeyword.IndexOf("order "));
                break;
            }
            if (!string.IsNullOrEmpty(findKeyword))
            {
                Int64 result;
                if (!countQueryAsycn.TryGetValue(sql, out result))
                {
                    var readerCount = ExecuteQuery(findKeyword);
                    if (readerCount != null)
                    {
                        while (readerCount.Read())
                        {
                            countQueryAsycn.Add(sql, Convert.ToInt64(readerCount[0]));
                        }
                    }
                }
            }
            CurrentSql = sql;
            OnStreamingLog(new ItemEventArgs<string>(sql));
            var reader = command.ExecuteReader();
            var dataRecords = reader as IEnumerable;
            var @async = new List<IDataRecord>();
            if (dataRecords != null)
            {
                try
                {
                    foreach (IDataRecord dataRecord in dataRecords)
                    {
                        @async.Add(dataRecord);
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
            try
            {
                conn.Close();
            }
            catch (Exception exception)
            {
                OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                OnError(new ErrorArgs(exception));
            }
            return @async;
        }

        public IDataReader ExecuteQueryAsycn(string sql)
        {
            try
            {
                CurrentSql = sql;
                var resetEvent = new ManualResetEvent(false);
                var conn = Current.CurrentConnection;
                listDbConnectionCreateByRuntime.Add(conn);
                try
                {
                    if (!Current.ConnectionString.Equals(ConnectionString))
                    {
                        Current = this;
                        FailedConnection = false;
                    }
                    if (FailedConnection) return default(IDataReader);
                    conn.Open();
                }
                catch (Exception exception)
                {
                    Current.FailedConnection = true;
                    FailedConnection = true;
                    OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                    OnError(new ErrorArgs(exception));
                }
                var command = ConnectionManager.Command;
                IDataReader reader = null;
                ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        command.Connection = conn;
                        command.CommandText = sql;
                        OnStreamingLog(new ItemEventArgs<string>(sql));
                        reader = command.ExecuteReader();
                        resetEvent.Set();
                    }, command);
                resetEvent.WaitOne();
                return reader;
            }
            finally
            {
                var log = BaseDependency.Get<ILogRepository>();
                if (log != null)
                    log.Info(sql);
            }
        }

        public T CreateRow<T>(Dictionary<string, object> dic)
        {
            return default(T);
        }

        public object GetOneDataFromQuery(string sql)
        {
            var conn = ConnectionManager.Connection;
            listDbConnectionCreateByRuntime.Add(conn);
            try
            {
                if (!Current.ConnectionString.Equals(ConnectionString))
                {
                    Current = this;
                    FailedConnection = false;
                }
                if (FailedConnection) return null;
                conn.Open();
            }
            catch (Exception exception)
            {
                Current.FailedConnection = true;
                FailedConnection = true;
                OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                OnError(new ErrorArgs(exception));
            }
            var command = ConnectionManager.Command;
            command.Connection = conn;
            CurrentSql = sql;
            command.CommandText = sql;
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                return reader[0];
            }
            return "";
        }

        public void ExecuteNonQuery(string sql)
        {
            try
            {
                if (UseGlobalTransaction)
                {
                    queryTamp += sql;
                    return;
                }
                CurrentSql = sql;
                var conn = ConnectionManager.Connection;
                listDbConnectionCreateByRuntime.Add(conn);
                try
                {
                    if (!Current.ConnectionString.Equals(ConnectionString))
                    {
                        Current = this;
                        FailedConnection = false;
                    }
                    if (FailedConnection) return;
                    conn.Open();
                }
                catch (Exception exception)
                {
                    Current.FailedConnection = true;
                    FailedConnection = true;
                    OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                    OnError(new ErrorArgs(exception));
                }
                var command = ConnectionManager.Command;
                command.Connection = conn;
                command.CommandText = sql;
                OnStreamingLog(new ItemEventArgs<string>(sql));
                command.ExecuteNonQuery();
            }
            finally
            {
                var log = BaseDependency.Get<ILogRepository>();
                if (log != null)
                    log.Info(sql);
            }
        }

        #region Event

        /// <summary>
        ///     Occurs when [error].
        /// </summary>
        public event EventHandler<ErrorArgs> Error;

        public static event EventHandler<ErrorArgs> ErrorInformation;

        /// <summary>
        ///     Occurs when [auto increment complete].
        /// </summary>
        public event EventHandler<TableItemArgs> AutoIncrementComplete;


        /// <summary>
        ///     Occurs when [auto increment complete].
        /// </summary>
        public event EventHandler<QueryArgs> BeforeQueryCommit;

        /// <summary>
        ///     Called when [error].
        /// </summary>
        /// <param name="e">The e.</param>
        public virtual void OnError(ErrorArgs e)
        {
            if (Error != null)
                Error(this, e);
            if (ErrorInformation != null)
            {
                if (string.IsNullOrEmpty(ConnectionString))
                {
                    ErrorInformation(this,
                        new ErrorArgs(
                            new Exception(
                                "A network-related or instance-specific error occurred while establishing a connection to SQL Server. The server was not found or was not accessible. Verify that the instance name is correct and that SQL Server is configured to allow remote connections. (provider: SQL Network Interfaces, error: 26 - Error Locating Server/Instance Specified")));
                }
                else
                    ErrorInformation(this, e);
            }
            //if (GetErrorContext.TryGetValue(MyGuid, out result))
            //{
            //    GetErrorContext.Add(MyGuid, e.Exception);
            //}
            //else
            //{
            //    GetErrorContext[MyGuid] = e.Exception;
            //}
        }

        #endregion Event

        #region Method

        public void SetDatabaseManager(IConnectionManager manager)
        {
            ConnectionManager = manager;
        }

        /// <summary>
        ///     Autoes the index of the fill index.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="field">The field.</param>
        /// <param name="length">The length.</param>
        /// <returns>System.String.</returns>
        public string AutoFillIndexIndex(string table, string field, int length)
        {
            using (var context = ConnectionManager.Connection)
            {
                try
                {
                    if (!Current.ConnectionString.Equals(ConnectionString))
                    {
                        Current = this;
                        FailedConnection = false;
                    }
                    if (FailedConnection) return "";
                    context.Open();
                }
                catch (Exception exception)
                {
                    Current.FailedConnection = true;
                    FailedConnection = true;
                    OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                    OnError(new ErrorArgs(exception));
                }
                var s = "";
                using (var command = ConnectionManager.Command)
                {
                    command.Connection = context;
                    command.CommandText = "select " + field + " from " + table + " order by " + field;
                    var data = command.ExecuteReader();
                    long temp = 0;
                    var first = true;
                    while (data.Read())
                    {
                        s = data[0].ToString();
                        if (first)
                            if (s.Substring(s.Length - length) !=
                                Convert.ToInt64(1).ToString("D" + length.ToString(CultureInfo.InvariantCulture)))
                            {
                                first = false;
                                return DateTime.Now.ToString("yyMM") +
                                       Convert.ToInt64(0).ToString("D" + length.ToString(CultureInfo.InvariantCulture));
                            }
                        if (temp != 0)
                            if (Convert.ToInt64(s) - temp != 1)
                                return temp.ToString(CultureInfo.InvariantCulture);
                        temp = Convert.ToInt64(s);
                        first = false;
                    }
                }

                return s;
            }
        }

        /// <summary>
        ///     digunakan jika ingin menggunakan transaction di class yang berbeda
        /// </summary>
        public static bool UseGlobalTransaction { get; set; }

        /// <summary>
        ///     Commits this instance.
        /// </summary>
        /// <returns>
        ///     <c>true</c> jika berhasil melakakukan perubahan pada database , <c>false</c> jika terjadi kesalahan pada
        ///     proses penyimpanan pada database
        /// </returns>
        public bool Commit()
        {
            if (UseGlobalTransaction)
                return Commit(false);
            return Commit(true);
        }

        public bool Commit(bool status)
        {
            var TagDeleteLocal = TagDelete.ToList();
            var TagUpdateLocals = TagUpdate;// new Dictionary<string, object>(TagUpdate);
            var TagSaveLocal = new Dictionary<string, object>(TagSave);
            //var item = BaseDependency.Get<BaseItem>();
            try
            {
                dictionaryAutoIncrement = new CoreDictionary<string, object>();
                var contextManager = this;
                if (UseGlobalTransaction)
                    contextManager = Current;
                using (var context = contextManager.ConnectionManager.Connection)
                {
                    try
                    {
                        if (!Current.ConnectionString.Equals(ConnectionString))
                        {
                            Current = this;
                            FailedConnection = false;
                        }
                        if (FailedConnection && Current.FailedConnection) return false;
                        context.Open();
                    }
                    catch (Exception exception)
                    {
                        Current.FailedConnection = true;
                        FailedConnection = true;
                        OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                        OnError(new ErrorArgs(exception));
                    }
                    using (var transcation = contextManager.ConnectionManager.Transaction)
                    {
                        var query = new StringBuilder();

                        #region Delete

                        TagDeleteLocal.OfType<TableItem>().ToList().ForEach(n => n.OnBeforeDelete());
                        TagUpdateLocals.Where(n => n.Value is TableItem)
                            .Select(n => n.Value)
                            .Cast<TableItem>()
                            .ToList()
                            .
                            ForEach(n => n.OnBeforeUpdate());
                        TagDeleteLocal.OfType<TableItem>().ToList().ForEach(n => n.OnBeforeInsert());
                        foreach (var delete in TagDeleteLocal.Cast<TableItem>())
                        {
                            delete.WillCommit = true;
                            var str = new StringBuilder();
                            var linkItem = delete;

                            str.Append("Delete from " + linkItem.TableName + " Where ");
                            var cek = false;
                            foreach (var key in linkItem.PrimaryKeys)
                            {
                                if (!cek)
                                {
                                    str.Append(key + "='" + linkItem.Previous[key.ToLower()] + "'");
                                    cek = true;
                                }
                                else
                                {
                                    str.Append(" AND " + key + "='" +
                                               ((linkItem.Previous[key.ToLower()] is DateTime)
                                                   ? "" + ((DateTime)linkItem.Previous[key.ToLower()]).ToString(
                                                       ConnectionManager.FormateDate) + "' "
                                                   : "" + linkItem.Previous[key.ToLower()] + "' ") + "");
                                }
                            }
                            query.Append(str + ";");
                        }

                        #endregion Delete

                        #region Save

                        //query.Append(TagSaveLocal.Select(save => save.Value as TableItem).Aggregate(query, RecursiveInsert)) ;
                        foreach (var o in TagSaveLocal)
                        {
                            var str = RecursiveInsert(query, o.Value as TableItem).ToString();
                            query.Append(str);
                        }

                        #endregion Save

                        #region Update

                        TagUpdateLocals.Reverse();
                        foreach (var update in TagUpdateLocals)
                        {
                            var str = new StringBuilder();
                            var where = new StringBuilder();
                            var linkItem = update.Value as TableItem;
                            if (linkItem != null)
                            {
                                if (string.IsNullOrEmpty(linkItem.TableName))
                                    continue;
                                linkItem.WillCommit = true;
                                str.Append("UPDATE  " + linkItem.TableName + " SET ");
                                var cek = false;
                                foreach (var key in linkItem.PrimaryKeys)
                                {
                                    if (!cek)
                                    {
                                        if (linkItem.Previous != null)
                                        {
                                            where.Append(key + "='" + ((linkItem.Previous[key.ToLower()] is DateTime)
                                                ? "" +
                                                  ((DateTime)linkItem.Previous[key.ToLower()]).ToString(
                                                      ConnectionManager.FormateDate) + "' "
                                                : "" + linkItem.Previous[key.ToLower()] + "' ") + "");
                                        }
                                        cek = true;
                                    }
                                    else if (linkItem.Previous != null)
                                        @where.Append(" AND " + key + "='" +
                                                      ((linkItem.Previous[key.ToLower()] is DateTime)
                                                          ? "" +
                                                            ((DateTime)linkItem.Previous[key.ToLower()]).ToString(
                                                                ConnectionManager.FormateDate) + "' "
                                                          : "" + linkItem.Previous[key.ToLower()] + "' ") + "");
                                }
                                if (
                                    !(query.ToString().Contains(where.ToString()) &&
                                      query.ToString().Contains("UPDATE  " + linkItem.TableName + " SET ")))
                                {
                                    var field = new StringBuilder("");
                                    foreach (var fields in linkItem.Fields)
                                    {
                                        var cekField = false;
                                        if (fields.Info.GetCustomAttributes(true).OfType<SkipAttribute>().Count() != 0)
                                        {
                                            cekField = true;
                                            hasAutoIncrement = true;
                                            var subItem =
                                                fields.Info.GetCustomAttributes(true).OfType<SkipAttribute>().
                                                    FirstOrDefault();

                                            if (subItem is EncrypteAttribute)
                                            {
                                                var encrypteAttribute = subItem as EncrypteAttribute;
                                                var dataField = fields.Info.GetValue(linkItem, null);
                                                if (dataField != null)
                                                {
                                                    var encrypte = dataField.ToString();
                                                    //var password = "";
                                                    //foreach (var fieldAttribute in linkItem.Fields)
                                                    //    if (encrypteAttribute.RelationProperty != null)
                                                    //        foreach (
                                                    //            string relation in encrypteAttribute.RelationProperty)
                                                    //        {
                                                    //            if (fieldAttribute.Info.Name.Equals(relation))
                                                    //                password = password +
                                                    //                           fieldAttribute.Info.GetValue(linkItem,
                                                    //                                                        null);
                                                    //        }
                                                    //encrypte = encrypte.Encrypt(encrypteAttribute.Key + password);
                                                    encrypte = Cryptography.FuncAesEncrypt(encrypte);
                                                    field.Append(fields.FieldName + "='" + encrypte + "',");
                                                }
                                            }
                                        }
                                        if (!cekField)
                                        {
                                            if (fields.Info.PropertyType.IsEnum)
                                            {
                                                var values = Enum.GetValues(fields.Info.PropertyType);
                                                var count = 0;
                                                foreach (var val in values)
                                                {
                                                    if (
                                                        val.ToString()
                                                            .Equals(fields.Info.GetValue(linkItem, null).ToString()))
                                                        field.Append(fields.FieldName + "='" + count + "',");
                                                    count++;
                                                }
                                            }
                                            else if (fields.Info.PropertyType.Name.ToLower().Contains("bool"))
                                            {
                                                if (linkItem.Previous != null &&
                                                    linkItem.Dictionarys[fields.FieldName.ToLower()] != null &&
                                                    !linkItem.Dictionarys[fields.FieldName.ToLower()].Equals(
                                                        linkItem.Previous[fields.FieldName.ToLower()]))
                                                    field.Append(fields.FieldName + "=" + "'" +
                                                                 ConnectionManager.ConvertBoolean(
                                                                     Convert.ToBoolean(fields.Info.GetValue(linkItem,
                                                                         null))) + "',");
                                            }
                                            else if (
                                                fields.Info.PropertyType.ToString().ToLower().Split(' ')[0].
                                                    Contains("system.nullable`1[system.datetime]"))
                                            {
                                                if (
                                                    !(fields.IsAllowNull == SpesicicationType.AllowNull &&
                                                      fields.Info.GetValue(linkItem, null) == null))
                                                {
                                                    if (linkItem.Previous != null &&
                                                        linkItem.Dictionarys[fields.FieldName.ToLower()] != null &&
                                                        !linkItem.Dictionarys[fields.FieldName.ToLower()].Equals(
                                                            linkItem.Previous[fields.FieldName.ToLower()]))
                                                    {
                                                        field.Append(fields.FieldName + "=" +
                                                                     ((fields.Info.GetValue(linkItem, null) is DateTime)
                                                                         ? "'" +
                                                                           ((DateTime)
                                                                               fields.Info.GetValue(linkItem, null))
                                                                               .ToString(ConnectionManager.FormateDate) +
                                                                           "',"
                                                                         : "null,"));
                                                    }
                                                }
                                                else if (fields.IsAllowNull == SpesicicationType.AllowNull &&
                                                         fields.Info.GetValue(linkItem, null) == null)
                                                {
                                                    field.Append(fields.FieldName + "=" + "null,");
                                                }
                                            }
                                            else
                                            {
                                                if (
                                                    !(fields.IsAllowNull == SpesicicationType.AllowNull &&
                                                      fields.Info.GetValue(linkItem, null) == null))
                                                {
                                                    if (linkItem.Previous != null &&
                                                        linkItem.Dictionarys[fields.FieldName.ToLower()] != null &&
                                                        !linkItem.Dictionarys[fields.FieldName.ToLower()].Equals(
                                                            linkItem.Previous[fields.FieldName.ToLower()]))
                                                    {
                                                        //caun
                                                        if (fields.Info.PropertyType == typeof(double?) ||
                                                            fields.Info.PropertyType == typeof(float?))
                                                        {
                                                            if (fields.Info.GetValue(linkItem, null) != null)
                                                                field.Append(fields.FieldName + "=" +
                                                                             ((fields.Info.GetValue(linkItem, null) is
                                                                                 DateTime)
                                                                                 ? "'" +
                                                                                   ((DateTime)
                                                                                       fields.Info.GetValue(linkItem,
                                                                                           null)).ToString(
                                                                                               ConnectionManager
                                                                                                   .FormateDate) + "',"
                                                                                 : "'" +
                                                                                   fields.Info.GetValue(linkItem, null)
                                                                                       .ToString()
                                                                                       .Replace(",", ".") + "',"));
                                                            else
                                                                field.Append(fields.FieldName + "=" +
                                                                             ((fields.Info.GetValue(linkItem, null) is
                                                                                 DateTime)
                                                                                 ? "'" +
                                                                                   ((DateTime)
                                                                                       fields.Info.GetValue(linkItem,
                                                                                           null)).ToString(
                                                                                               ConnectionManager
                                                                                                   .FormateDate) + "',"
                                                                                 : "null,"));
                                                        }
                                                        else if (fields.Info.PropertyType == typeof(double) ||
                                                                 fields.Info.PropertyType == typeof(float))
                                                        {
                                                            field.Append(fields.FieldName + "=" +
                                                                         ((fields.Info.GetValue(linkItem, null) is
                                                                             DateTime)
                                                                             ? "'" +
                                                                               ((DateTime)
                                                                                   fields.Info.GetValue(linkItem,
                                                                                       null)).ToString(
                                                                                           ConnectionManager
                                                                                               .FormateDate) + "',"
                                                                             : "'" +
                                                                               fields.Info.GetValue(linkItem, null)
                                                                                   .ToString()
                                                                                   .Replace(",", ".") + "',"));
                                                        }
                                                        else
                                                        {
                                                            field.Append(fields.FieldName + "=" +
                                                                         ((fields.Info.GetValue(linkItem, null) is
                                                                             DateTime)
                                                                             ? "'" +
                                                                               ((DateTime)
                                                                                   fields.Info.GetValue(linkItem,
                                                                                       null)).ToString(
                                                                                           ConnectionManager
                                                                                               .FormateDate) + "',"
                                                                             : "'" +
                                                                               fields.Info.GetValue(linkItem, null) +
                                                                               "',"));
                                                        }
                                                    }
                                                }
                                                else if (fields.IsAllowNull == SpesicicationType.AllowNull &&
                                                         fields.Info.GetValue(linkItem, null) == null)
                                                {
                                                    field.Append(fields.FieldName + "=NULl,");
                                                }
                                            }
                                        }
                                    }
                                    var field_ = field.ToString();
                                    if (field_ != string.Empty)
                                    {
                                        field_ = field_.Substring(0, field_.Length - 1);
                                    }
                                    if (!string.IsNullOrEmpty(field_))
                                    {
                                        str.Append(field_ + " Where ");
                                        str.Append(where);
                                        var sql = str.ToString();
                                        if (!query.ToString().Contains(sql))
                                            query.Append(sql + ";");
                                    }
                                }
                            }
                        }

                        #endregion Update

                        var cmd = contextManager.ConnectionManager.Command;
                        cmd.Connection = context;
                        if (transcation != null)
                            cmd.Transaction = transcation;
                        queryTamp += query.ToString().Replace(";;", ";");

                        cmd.CommandText += queryTamp;



                        if (!string.IsNullOrEmpty(cmd.CommandText))
                        {
                            CurrentSql = cmd.CommandText;
                            var userLog = BaseDependency.Get<ISetting>();
                            if (userLog != null)
                            if (userLog.GetValue("streamSql").Equals("true"))
                                Log.Info(CurrentSql);
                            OnStreamingLog(new ItemEventArgs<string>(cmd.CommandText));

                            if (BeforeQueryCommit != null)
                                BeforeQueryCommit(this, new QueryArgs(cmd.CommandText));
                            if (status)
                            {
                                var log = BaseDependency.Get<ILogRepository>();
                                if (log != null)
                                    log.Info(cmd.CommandText);
                                cmd.ExecuteNonQuery();
                                if (transcation != null)
                                    transcation.Commit();
                                TagSave.Clear();
                                TagUpdate.Clear();
                                TagDelete.Clear();


                                queryTamp = null;
                            }
                            else
                            {
                                if (UseGlobalTransaction)
                                {
                                    TagSave.Clear();
                                    TagUpdate.Clear();
                                    TagDelete.Clear();
                                }
                                return true;
                            }
                        }
                    }
                }
            }
            catch (SqlException exception)
            {
                queryTamp = null;
                TagUpdate.Clear();
                TagSave.Clear();
                TagDelete.Clear();
                hasAutoIncrement = false;

                if (exception.Number == 1205)
                {
                    Thread.Sleep(100);
                    return Commit();
                }

                var log = BaseDependency.Get<ILogRepository>();
                OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                Trace.Write(exception);
                if (log != null)
                    log.Error(exception.GetBaseException().ToString());
                if (exception.Message.Contains("Driver not capable."))
                    return true;
                if (hasAutoIncrement)
                    if (exception.Message.ToLower().Contains("duplicate"))
                    {
                        Thread.Sleep(100);
                        return Commit();
                    }

                Log.ThrowError(exception, "100");

                if (Error != null)
                    Error(this, new ErrorArgs(exception));
                else
                    throw;
            }
            catch (Exception exception)
            {
                if (exception.ToString().Contains("deadlock"))
                {
                    throw;
                }
                OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                Trace.Write(exception);
                var log = BaseDependency.Get<ILogRepository>();
                if (log != null)
                    log.Error(exception.GetBaseException().ToString());
                if (exception.Message.Contains("Driver not capable."))
                    return true;
                if (hasAutoIncrement)
                    if (exception.Message.ToLower().Contains("duplicate"))
                    {
                        return Commit();
                    }
                if (Error != null)
                    Error(this, new ErrorArgs(exception));
                else
                    throw;
            }
            finally
            {
                try
                {
                    if (status)
                    {
                        TagUpdate.Clear();
                        TagSave.Clear();
                        TagDelete.Clear();
                        hasAutoIncrement = false;
                    }
                }
                catch (Exception exception)
                {
                    Log.Error(exception);
                }
            }
            return true;
        }

        /// digunakan untuk mengambil data yang akan di save
        public IEnumerable<object> ListInsertObject()
        {
            return TagSave.Select(n => n.Value);
        }

        /// <summary>
        ///     Deletes the object.digunakan untuk menghapus item pada database
        /// </summary>
        /// <param name="items">The items.</param>
        /// <example>
        ///     contoh yang digunakan untuk menghapus item pada database
        ///     <code>
        ///  var context = new ContextManager(connectionString);
        ///  context.DeleteObject(new[]{new TabelItem(),new TabelItem()});
        ///  context.Commit();
        /// </code>
        /// </example>
        public void DeleteObject(IEnumerable<TableItem> items)
        {
            foreach (var item in items)
            {
                if (item != null)
                    if (!item.IsNew)
                        TagDelete.Add(item);
            }
        }

        /// <summary>
        ///     Deletes the object.digunakan untuk menghapus item pada database
        /// </summary>
        /// <param name="item">The item.</param>
        /// <example>
        ///     contoh yang digunakan untuk menghapus item pada database
        ///     <code>
        ///  var context = new ContextManager(connectionString);
        ///  context.DeleteObject(new TabelItem());
        ///  context.Commit();
        /// </code>
        /// </example>
        public void DeleteObject(TableItem item)
        {
            if (item != null)
                if (!item.IsNew)
                    TagDelete.Add(item);
        }

        /// <summary>
        ///     Inserts the object.digunakan untuk menyimpan item pada database
        /// </summary>
        /// <param name="items">The items.</param>
        /// <example>
        ///     contoh yang digunakan untuk menyimpan item pada database
        ///     <code>
        ///  var context = new ContextManager(connectionString);
        ///  context.InsertObject(new[]{new TabelItem(),new TabelItem()});
        ///  context.Commit();
        /// </code>
        /// </example>
        public void InsertObject(IEnumerable<TableItem> items)
        {
            foreach (var item in items)
            {
                item.GuidId = Guid.NewGuid().ToString();
                TagSave.Add(item.GuidId, item);
            }
        }

        /// <summary>
        ///     Inserts the object.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <example>
        ///     contoh yang digunakan untuk menghapus item pada database
        ///     <code>
        ///  var context = new ContextManager(connectionString);
        ///  context.DeleteObject(new TabelItem());
        ///  context.Commit();
        /// </code>
        /// </example>
        public void InsertObject(TableItem item)
        {
            if (string.IsNullOrEmpty(item.GuidId))
            {
                item.GuidId = Guid.NewGuid().ToString();
                TagSave.Add(item.GuidId, item);
            }
            else
                TagSave[item.GuidId] = item;
        }

        /// <summary>
        ///     Lasts the index of the record. digunakan untuk mengambil data terkahir dari record
        /// </summary>
        /// <param name="sql">berisikan Sql query</param>
        /// <returns>System.String.</returns>
        /// <example>
        ///     contoh yang digunakan untuk menghapus item pada database
        ///     <code>
        ///  var context = new ContextManager(connectionString);
        ///  var last=context.LastRecordIndex("Select * from tTable");
        /// </code>
        /// </example>
        public string LastRecordIndex(string sql)
        {
            using (var context = ConnectionManager.Connection)
            {
                try
                {
                    if (!Current.ConnectionString.Equals(ConnectionString))
                    {
                        Current = this;
                        FailedConnection = false;
                    }
                    if (FailedConnection) return "";
                    context.Open();
                }
                catch (Exception exception)
                {
                    Current.FailedConnection = true;
                    FailedConnection = true;
                    OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                    OnError(new ErrorArgs(exception));
                }
                try
                {
                    var s = "";
                    using (var command = ConnectionManager.Command)
                    {
                        command.Connection = context;
                        command.CommandText = sql;
                        var data = command.ExecuteReader();

                        while (data.Read())
                        {
                            s = data[0].ToString();
                        }

                        return s;
                    }
                }
                catch (Exception exception)
                {
                    OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                    if (Error != null)
                        Error(this, new ErrorArgs(exception));
                    return string.Empty;
                }
            }
        }

        /// <summary>
        ///     Lasts the index of the record.digunakan untuk mengambil data terkahir dari record berdasarkan table dan field
        /// </summary>
        /// <param name="table">berisikian nama table.</param>
        /// <param name="field">berisikan nama field</param>
        /// <param name="date"></param>
        /// <returns>System.String.</returns>
        /// <example>
        ///     contoh yang digunakan untuk menghapus item pada database
        ///     <code>
        ///  var context = new ContextManager(connectionString);
        ///  var last=context.LastRecordIndex("tTable","kode");
        /// </code>
        /// </example>
        public string LastRecordIndex(string table, string field, DateTime date)
        {
            using (var context = ConnectionManager.Connection)
            {
                try
                {
                    if (!Current.ConnectionString.Equals(ConnectionString))
                    {
                        Current = this;
                        FailedConnection = false;
                    }
                    if (FailedConnection) return string.Empty;
                    context.Open();
                }
                catch (Exception exception)
                {
                    Current.FailedConnection = true;
                    FailedConnection = true;
                    OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                    OnError(new ErrorArgs(exception));
                }
                var s = "";
                using (var command = ConnectionManager.Command)
                {
                    command.Connection = context;
                    command.CommandText = "select " + field + " from " + table + " WHERE Substring(" + field +
                                          ",1,4) = '" + date.ToString("yyMM") + "' order by " + field;
                    var data = command.ExecuteReader();
                    while (data.Read())
                    {
                        s = data[0].ToString();
                    }
                }
                return s;
            }
        }
        public string LastRecordIndexChar(string table, string field,string code, DateTime date)
        {
            using(var context = ConnectionManager.Connection)
            {
                try
                {
                    if(!Current.ConnectionString.Equals(ConnectionString))
                    {
                        Current = this;
                        FailedConnection = false;
                    }
                    if(FailedConnection)
                        return string.Empty;
                    context.Open();
                }
                catch(Exception exception)
                {
                    Current.FailedConnection = true;
                    FailedConnection = true;
                    OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                    OnError(new ErrorArgs(exception));
                }
                var s = "";
                using(var command = ConnectionManager.Command)
                {
                    command.Connection = context;
                    command.CommandText = "select " + field + " from " + table + " WHERE Substring(" + field +
                                          ",1,4) = '" + date.ToString("yyMM") + "' AND Substring(" + field + ",10,1) = '" + code + "' order by " + field;
                    var data = command.ExecuteReader();
                    while(data.Read())
                    {
                        s = data[0].ToString();
                    }
                }
                return s;
            }
        }

        public string LastRecordIntIndex(string table, string field)
        {
            using (var context = ConnectionManager.Connection)
            {
                try
                {
                    if (!Current.ConnectionString.Equals(ConnectionString))
                    {
                        Current = this;
                        FailedConnection = false;
                    }
                    if (FailedConnection) return string.Empty;
                    context.Open();
                }
                catch (Exception exception)
                {
                    Current.FailedConnection = true;

                    FailedConnection = true;
                    OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                    OnError(new ErrorArgs(exception));
                }
                var s = "";
                using (var command = ConnectionManager.Command)
                {
                    command.Connection = context;
                    command.CommandText = "select " + field + " from " + table + " order by " + field;
                    var data = command.ExecuteReader();
                    while (data.Read())
                    {
                        s = data[0].ToString();
                    }
                }

                return s;
            }
        }

        /// <summary>
        ///     Lasts the index of the record.digunakan untuk mengambil data terkahir dari record berdasarkan table dan field
        /// </summary>
        /// <param name="table">berisikian nama table.</param>
        /// <param name="field">berisikan nama field</param>
        /// <param name="startWith">bersisikan keyword yang dicari</param>
        /// <param name="lengtWordAfterField">berisikan batas panjang karakter setelah kata yang di cari</param>
        /// <returns>System.String.</returns>
        /// <example>
        ///     contoh yang digunakan untuk menghapus item pada database
        ///     <code>
        ///  var context = new ContextManager(connectionString);
        ///  var last=context.LastRecordIndex("tTable","kode","AB",3);
        /// </code>
        /// </example>
        public string LastRecordIndex(string table, string field, string startWith, int lengtWordAfterField)
        {
            using (var context = ConnectionManager.Connection)
            {
                try
                {
                    if (!Current.ConnectionString.Equals(ConnectionString))
                    {
                        Current = this;
                        FailedConnection = false;
                    }
                    if (FailedConnection) return string.Empty;
                    context.Open();
                }
                catch (Exception exception)
                {
                    Current.FailedConnection = true;
                    FailedConnection = true;
                    OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                    OnError(new ErrorArgs(exception));
                }
                var s = "";
                using (var command = ConnectionManager.Command)
                {
                    command.Connection = context;
                    command.CommandText = "select " + field + " from " + table + " where " + field + " like '" +
                                          startWith +
                                          "%' order by " + field;
                    var data = command.ExecuteReader();
                    while (data.Read())
                    {
                        var temp = data[0].ToString().Substring(startWith.Length);
                        if (temp.Length == lengtWordAfterField || temp.Length == 0)
                            s = data[0].ToString();
                    }

                    return s;
                }
            }
        }

        /// <summary>
        ///     Lasts the index of the record.digunakan untuk mengambil data terkahir dari record berdasarkan table dan field
        /// </summary>
        /// <param name="table">berisikian nama table.</param>
        /// <param name="field">berisikan nama field</param>
        /// <param name="startWith">bersisikan keyword yang dicari</param>
        /// <returns>System.String.</returns>
        /// <example>
        ///     contoh yang digunakan untuk menghapus item pada database
        ///     <code>
        ///  var context = new ContextManager(connectionString);
        ///  var last=context.LastRecordIndex("tTable","kode","AB");
        /// </code>
        /// </example>
        public string LastRecordIndex(string table, string field, string startWith)
        {
            using (var context = ConnectionManager.Connection)
            {
                try
                {
                    if (!Current.ConnectionString.Equals(ConnectionString))
                    {
                        Current = this;
                        FailedConnection = false;
                    }
                    if (FailedConnection) return string.Empty;
                    context.Open();
                }
                catch (Exception exception)
                {
                    Current.FailedConnection = true;
                    FailedConnection = true;
                    OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                    OnError(new ErrorArgs(exception));
                }
                var s = "";
                using (var command = ConnectionManager.Command)
                {
                    command.Connection = context;
                    command.CommandText = "select " + field + " from " + table + " where " + field + " like '" +
                                          startWith +
                                          "%' order by " + field;
                    var data = command.ExecuteReader();
                    while (data.Read())
                    {
                        s = data[0].ToString();
                    }

                    return s;
                }
            }
        }

        /// <summary>
        ///     Lasts the index of the record. digunakan untuk mengambil data terkahir dari record
        /// </summary>
        /// <param name="sql">berisikan Sql query</param>
        /// <returns>System.String.</returns>
        /// <example>
        ///     contoh yang digunakan untuk menghapus item pada database
        ///     <code>
        ///  var context = new ContextManager(connectionString);
        ///  var last=context.LastRecordIndex("Select * from tTable");
        /// </code>
        /// </example>
        public object LastRecordIndexObject(string sql)
        {
            using (var context = ConnectionManager.Connection)
            {
                try
                {
                    if (!Current.ConnectionString.Equals(ConnectionString))
                    {
                        Current = this;
                        FailedConnection = false;
                    }
                    if (FailedConnection) return null;
                    context.Open();
                }
                catch (Exception exception)
                {
                    Current.FailedConnection = true;
                    FailedConnection = true;
                    OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                    OnError(new ErrorArgs(exception));
                }
                object s = "";
                using (var command = ConnectionManager.Command)
                {
                    command.Connection = context;
                    command.CommandText = sql;
                    var data = command.ExecuteReader();
                    while (data.Read())
                    {
                        s = data[0];
                    }
                }
                return s;
            }
        }

        /// <summary>
        ///     Updates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Update(BaseItem item)
        {
            if (FreezeUpdate)
            {
                object obj;
                if (!TagUpdate.TryGetValue(item.GuidId, out obj))
                    TagUpdate.Add(item.GuidId, item);
                else
                    TagUpdate[item.GuidId] = item;
            }
        }

        /// <summary>
        ///     Autoes the increment last record char. digunakan untuk auto generate numbering
        /// </summary>
        /// <param name="iEnumerable">The i enumerable.</param>
        /// <returns>IEnumerable{System.Char}.</returns>
        private IEnumerable<char> AutoIncrementLastRecordChar(IEnumerable<char> iEnumerable)
        {
            var listChar = iEnumerable.ToArray();
            for (int i = 0; i < listChar.Length;)
            {
                if (listChar[i].Equals(list.Last().Value))
                {
                    listChar[i] = '0';
                    if (listChar[i + 1].Equals(list.Last().Value))
                    {
                        var tempList = AutoIncrementLastRecordChar(listChar.Skip(1)).ToArray();
                        for (var j = 0; j < tempList.Length; j++)
                        {
                            listChar[i + 1 + j] = tempList[j];
                        }
                    }
                    else
                        listChar[i + 1] = list[list.FirstOrDefault(n => n.Value.Equals(listChar[i + 1])).Key + 1];
                    break;
                }
                listChar[i] = list[list.FirstOrDefault(n => n.Value.Equals(listChar[i])).Key + 1];
                break;
            }
            return listChar;
        }


        public void GenerateAutoNumber()
        {
            dictionaryAutoIncrement = new CoreDictionary<string, object>();
            foreach (var n1 in TagSave)
            {
                object o = n1.Value;
                TableItem linkItem = o as TableItem;
                if (linkItem != null)
                {
                    foreach (var fields in linkItem.Fields)
                    {
                        if (fields.Info.GetCustomAttributes(true).OfType<SkipAttribute>().Count() != 0)
                        {
                            var subItem = fields.Info.GetCustomAttributes(true).OfType<SkipAttribute>().FirstOrDefault();

                            #region AutoGenerateDateTimeYYMMAttribute

                            object result;

                            if (subItem is AutoGenerateDateTimeYYMMAttribute)
                            {
                                hasAutoIncrement = true;
                                var attr = subItem as AutoGenerateDateTimeYYMMAttribute;
                                attr.Property = fields.Info.Name;
                                DateTime? date = DateTime.Now;
                                try
                                {
                                    var property = linkItem.GetType().GetProperty("DefaultTime");
                                    if (property != null)
                                    {
                                        date = (DateTime?)property.GetValue(linkItem, null);
                                        if (date == DateTime.MinValue || date == null)
                                        {
                                            date = DateTime.Now;
                                        }
                                    }
                                }
                                catch (Exception exception)
                                {
                                    OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                                }
                                if (date == null)
                                {
                                    date = DateTime.Now;
                                }
                                try
                                {
                                    var lastRecord = "";
                                    switch (attr.TypeGenerate)
                                    {
                                        case AutoGenerateDateTimeYYMMAttribute.TypeAutoGenerate.AutoFill:
                                            lastRecord = AutoFillIndexIndex(linkItem.TableName, fields.FieldName, attr.Length);
                                            break;

                                        case AutoGenerateDateTimeYYMMAttribute.TypeAutoGenerate.LastIndex:
                                            lastRecord = LastRecordIndex(linkItem.TableName, fields.FieldName, date.Value);
                                            break;
                                        case AutoGenerateDateTimeYYMMAttribute.TypeAutoGenerate.CanGenerateManual:
                                            if (fields.Info.GetValue(linkItem, null) != null && !string.IsNullOrEmpty(fields.Info.GetValue(linkItem, null).ToString()))
                                                lastRecord = fields.Info.GetValue(linkItem, null).ToString();
                                            else
                                            {
                                                lastRecord = LastRecordIndex(linkItem.TableName, fields.FieldName, date.Value);
                                            }
                                            break;
                                    }
                                    if (attr.TypeGenerate != AutoGenerateDateTimeYYMMAttribute.TypeAutoGenerate.CanGenerateManual || (attr.TypeGenerate != AutoGenerateDateTimeYYMMAttribute.TypeAutoGenerate.CanGenerateManual && fields.Info.GetValue(linkItem, null).ToString() == string.Empty))
                                    {
                                        if (dictionaryAutoIncrement.TryGetValue(linkItem.TableName + fields.FieldName, out result))
                                            lastRecord = result.ToString();
                                        int countLasRecort;
                                        var lengsubstring = lastRecord.Length - attr.Length;
                                        if (lengsubstring < 0) lengsubstring = 1;
                                        if (lastRecord != string.Empty && lastRecord.ToString(CultureInfo.InvariantCulture).Substring(lengsubstring).ToCharArray().Count(n => n.Equals('9')) == attr.Length)
                                            throw new OverloadPrimaryException("Over load key");
                                        if (lastRecord == string.Empty) countLasRecort = 1;
                                        else
                                        {
                                            //if (!lastRecord.Substring(0, lengsubstring).Equals(date.Value.ToString("yyMM")))
                                            //{
                                            //    countLasRecort = 1;
                                            //}
                                            if (!lastRecord.Substring(0, lengsubstring).Equals(date.Value.ToString("yyMM")))
                                            {
                                                countLasRecort = 1;
                                            }
                                            else countLasRecort = Convert.ToInt32(lastRecord.Substring(lengsubstring)) + 1;
                                        }

                                        var hasil = date.Value.ToString("yyMM") + countLasRecort.ToString("D" + attr.Length);
                                        //var hasil = lastRecord.Substring(0, lengsubstring) + countLasRecort.ToString("D" + attr.Length);
                                        linkItem[fields.FieldName] = hasil;
                                        if (!dictionaryAutoIncrement.TryGetValue(linkItem.TableName + fields.FieldName, out result))
                                            dictionaryAutoIncrement.Add(linkItem.TableName + fields.FieldName, hasil);
                                        else dictionaryAutoIncrement[linkItem.TableName + fields.FieldName] = hasil;
                                    }
                                }
                                catch (Exception exception)
                                {
                                    OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                                    if (Error != null)
                                        Error(this, new ErrorArgs(exception));
                                }
                            }

                            #endregion AutoGenerateDateTimeYYMMAttribute

                            #region AutoGenerateDateTimeYYMMAttributeCode

                            else if(subItem is AutoGenerateDateTimeYYMMAttributeCode)
                            {
                                hasAutoIncrement = true;
                                var attr = subItem as AutoGenerateDateTimeYYMMAttributeCode;
                                attr.Property = fields.Info.Name;
                                DateTime? date = DateTime.Now;
                                try
                                {
                                    var property = linkItem.GetType().GetProperty("DefaultTime");
                                    if(property != null)
                                    {
                                        date = (DateTime?)property.GetValue(linkItem, null);
                                        if(date == DateTime.MinValue || date == null)
                                        {
                                            date = DateTime.Now;
                                        }
                                    }
                                }
                                catch(Exception exception)
                                {
                                    OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                                }
                                if(date == null)
                                {
                                    date = DateTime.Now;
                                }
                                try
                                {
                                    var lastRecord = "";
                                    switch(attr.TypeGenerate)
                                    {
                                        case AutoGenerateDateTimeYYMMAttributeCode.TypeAutoGenerate.AutoFill:
                                            lastRecord = AutoFillIndexIndex(linkItem.TableName, fields.FieldName, attr.Length);
                                            break;

                                        case AutoGenerateDateTimeYYMMAttributeCode.TypeAutoGenerate.LastIndex:
                                            lastRecord = LastRecordIndex(linkItem.TableName, fields.FieldName, date.Value);
                                            break;
                                        case AutoGenerateDateTimeYYMMAttributeCode.TypeAutoGenerate.CanGenerateManual:
                                            if(fields.Info.GetValue(linkItem, null) != null && !string.IsNullOrEmpty(fields.Info.GetValue(linkItem, null).ToString()))
                                                lastRecord = fields.Info.GetValue(linkItem, null).ToString();
                                            else
                                            {
                                                lastRecord = LastRecordIndex(linkItem.TableName, fields.FieldName, date.Value);
                                            }
                                            break;
                                    }
                                    if(attr.TypeGenerate != AutoGenerateDateTimeYYMMAttributeCode.TypeAutoGenerate.CanGenerateManual || (attr.TypeGenerate != AutoGenerateDateTimeYYMMAttributeCode.TypeAutoGenerate.CanGenerateManual && fields.Info.GetValue(linkItem, null).ToString() == string.Empty))
                                    {
                                       
                                        if(dictionaryAutoIncrement.TryGetValue(linkItem.TableName + fields.FieldName, out result))
                                            lastRecord = result.ToString();
                                        int countLasRecort;
                                        var lengsubstring = lastRecord.Length - attr.Length;
                                        if(lengsubstring < 0)
                                            lengsubstring = 1;
                                        if(lastRecord != string.Empty && lastRecord.ToString(CultureInfo.InvariantCulture).Substring(lengsubstring).ToCharArray().Count(n => n.Equals('9')) == attr.Length)
                                            throw new OverloadPrimaryException("Over load key");
                                        if(lastRecord == string.Empty)
                                            countLasRecort = 1;
                                        else
                                        {
                                            var dates = date.Value.ToString("yyMM");
                                            var last = lengsubstring -1;
                                            var leng = lastRecord.Length - attr.Code.Length;
                                            var fixLength = lastRecord.Substring(0, leng);
                                            if(lastRecord == string.Empty)
                                                countLasRecort = 1;
                                            else
                                            {

                                                if(!fixLength.Substring(0, last).Equals(dates))
                                                {
                                                    countLasRecort = 1;
                                                }
                                                else
                                                    countLasRecort = Convert.ToInt32(fixLength.Substring(lengsubstring)) + 1;
                                            }
                                        }

                                        var hasil =date.Value.ToString("yyMM") + countLasRecort.ToString("D" + attr.Length) +  attr.Code ;
                                        linkItem[fields.FieldName] = hasil;
                                        if(!dictionaryAutoIncrement.TryGetValue(linkItem.TableName + fields.FieldName, out result))
                                            dictionaryAutoIncrement.Add(linkItem.TableName + fields.FieldName, hasil);
                                        else
                                            dictionaryAutoIncrement[linkItem.TableName + fields.FieldName] = hasil;
                                    }
                                }
                                catch(Exception exception)
                                {
                                    OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                                    if(Error != null)
                                        Error(this, new ErrorArgs(exception));
                                }
                            }

                            #endregion AutoGenerateDateTimeYYMMAttribute

                            #region CustomIntAutoIncrement

                            else if (subItem is CustomIntAutoIncrement)
                            {
                                hasAutoIncrement = true;
                                try
                                {
                                    var lastRecordResult = LastRecordIntIndex(linkItem.TableName, fields.FieldName);
                                    if (dictionaryAutoIncrement.TryGetValue(linkItem.TableName + fields.FieldName, out result))
                                        lastRecordResult = result.ToString();
                                    var lastRecord = 0;
                                    if (!string.IsNullOrEmpty(lastRecordResult))
                                        lastRecord = Convert.ToInt32(lastRecordResult);
                                    lastRecord++;
                                    linkItem[fields.FieldName] = Convert.ChangeType(lastRecord, fields.Info.PropertyType);
                                    if (!dictionaryAutoIncrement.TryGetValue(linkItem.TableName + fields.FieldName, out result))
                                        dictionaryAutoIncrement.Add(linkItem.TableName + fields.FieldName, lastRecord);
                                    else
                                        dictionaryAutoIncrement[linkItem.TableName + fields.FieldName] = Convert.ChangeType(lastRecord, fields.Info.PropertyType);
                                }
                                catch (Exception exception)
                                {
                                    OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                                }
                            }

                            #endregion CustomIntAutoIncrement

                            #region CustomIntAutoIncrement

                            else if (subItem is CustomCharAutoIncrement)
                            {
                                hasAutoIncrement = true;
                                try
                                {
                                    var lastRecordResult = LastRecordIntIndex(linkItem.TableName, fields.FieldName);
                                    if (dictionaryAutoIncrement.TryGetValue(linkItem.TableName + fields.FieldName, out result))
                                        lastRecordResult = result.ToString();
                                    var lastRecord = 0;
                                    if (!string.IsNullOrEmpty(lastRecordResult))
                                        lastRecord = lastRecordResult[0];
                                    else
                                        lastRecord = 'A';
                                    lastRecord++;
                                    linkItem[fields.FieldName] = Convert.ChangeType(lastRecord, fields.Info.PropertyType);
                                    if (!dictionaryAutoIncrement.TryGetValue(linkItem.TableName + fields.FieldName, out result))
                                        dictionaryAutoIncrement.Add(linkItem.TableName + fields.FieldName, lastRecord);
                                    else
                                        dictionaryAutoIncrement[linkItem.TableName + fields.FieldName] = Convert.ChangeType(lastRecord, fields.Info.PropertyType);
                                }
                                catch (Exception exception)
                                {
                                    OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                                }
                            }

                            #endregion CustomIntAutoIncrement

                            #region CustomAutoIncrementAttribute

                            else if (subItem is RealtionStringAutoIncrementAttribute)
                            {
                                hasAutoIncrement = true;
                                var listChart = ListAutoIncement.ToCharArray();
                                if (list.Count == 0)
                                    for (var i = 0; i < listChart.Length; i++)
                                    {
                                        list.Add(i, listChart[i]);
                                    }
                                var attr = subItem as RealtionStringAutoIncrementAttribute;

                                string lastRecord;
                                var tempHasil = "";
                                if (attr.RelationProperty != null)
                                    foreach (var realtionProperty in attr.RelationProperty)
                                    {
                                        tempHasil += linkItem.GetType().GetProperty(realtionProperty).GetValue(linkItem, null).ToString();
                                    }
                                lastRecord = LastRecordIndex(linkItem.TableName, fields.FieldName, tempHasil);
                                if (dictionaryAutoIncrement.TryGetValue(linkItem.TableName + fields.FieldName, out result))
                                    lastRecord = result.ToString();
                                if (lastRecord.ToString(CultureInfo.InvariantCulture).ToCharArray().Count(n => n.Equals('Z')) == attr.Value - tempHasil.Length)
                                    throw new OverloadPrimaryException("Over load key");
                                if (lastRecord == string.Empty)
                                    lastRecord = Convert.ToInt16(0).ToString("D" + (attr.Value - tempHasil.Length).ToString(CultureInfo.InvariantCulture));
                                else
                                    lastRecord = lastRecord.Substring(tempHasil.Length);
                                //var countLasRecort = 0;
                                if (lastRecord == string.Empty)
                                {
                                } //countLasRecort = 'A';
                                else
                                {
                                    var index = list.FirstOrDefault(n => n.Value.ToString(CultureInfo.InvariantCulture).ToUpper().Equals(lastRecord[lastRecord.Length - 1].ToString(CultureInfo.InvariantCulture).ToUpper()));
                                    if (index.Key == list.Count - 1)
                                    {
                                    }

                                    var tempChar = lastRecord.ToCharArray();
                                    lastRecord = "";
                                    for (var i = 0; i < tempChar.Length - 1; i++)
                                    {
                                        lastRecord += tempChar[i];
                                    }

                                    if (index.Key == list.Max(n => n.Key))
                                    {
                                        var tempLastRecord = lastRecord;
                                        lastRecord = "";
                                        AutoIncrementLastRecordChar(tempLastRecord.ToCharArray().Reverse()).Reverse().ToList().ForEach(n => lastRecord += n);
                                        lastRecord += list[0].ToString(CultureInfo.InvariantCulture);
                                    }
                                    else
                                        lastRecord += list[index.Key + 1].ToString(CultureInfo.InvariantCulture);
                                }
                                if (fields.Info.ToString().Contains("Char"))
                                {
                                    linkItem[fields.FieldName] = Convert.ToChar(lastRecord);
                                    if (!dictionaryAutoIncrement.TryGetValue(linkItem.TableName + fields.FieldName, out result))
                                        dictionaryAutoIncrement.Add(linkItem.TableName + fields.FieldName, lastRecord);
                                    else
                                        dictionaryAutoIncrement[linkItem.TableName + fields.FieldName] = lastRecord;
                                }
                                else
                                {
                                    if (!dictionaryAutoIncrement.TryGetValue(linkItem.TableName + fields.FieldName, out result))
                                        dictionaryAutoIncrement.Add(linkItem.TableName + fields.FieldName, (tempHasil + lastRecord));
                                    else
                                        dictionaryAutoIncrement[linkItem.TableName + fields.FieldName] = (tempHasil + lastRecord);
                                    linkItem[fields.FieldName] = tempHasil + lastRecord;
                                }
                            }

                            #endregion CustomAutoIncrementAttribute

                            #region EncrypteAttribute

                            else if (subItem is EncrypteAttribute)
                            {
                            }

                            #endregion EncrypteAttribute

                            #region RealtionStringAndSparatorAutoIncrementAttribute

                            else if (subItem is RealtionStringAndSparatorAutoIncrementAttribute)
                            {
                                hasAutoIncrement = true;
                                var listChart = ListAutoIncement.ToCharArray();
                                if (list.Count == 0)
                                    for (var i = 0; i < listChart.Length; i++)
                                    {
                                        list.Add(i, listChart[i]);
                                    }
                                var attr = subItem as RealtionStringAndSparatorAutoIncrementAttribute;

                                string lastRecord;
                                var tempHasil = "";
                                if (attr.RelationProperty != null)
                                    foreach (var realtionProperty in attr.RelationProperty)
                                    {
                                        var data = linkItem.GetType().GetProperty(realtionProperty).GetValue(linkItem, null).ToString();
                                        if (!string.IsNullOrEmpty(data))
                                            tempHasil += data + attr.Sparator;
                                    }
                                lastRecord = LastRecordIndex(linkItem.TableName, fields.FieldName, tempHasil, attr.Value);
                                if (dictionaryAutoIncrement.TryGetValue(linkItem.TableName + fields.FieldName, out result))
                                    lastRecord = result.ToString();
                                if (!string.IsNullOrEmpty(lastRecord) && lastRecord.ToString(CultureInfo.InvariantCulture).ToCharArray().Count(n => n.Equals('Z')) == attr.Value - tempHasil.Length)
                                    throw new OverloadPrimaryException("Over load key");
                                if (lastRecord == string.Empty)
                                    lastRecord = Convert.ToInt16(0).ToString("D" + (attr.Value).ToString(CultureInfo.InvariantCulture));
                                else
                                    lastRecord = lastRecord.Substring(tempHasil.Length);
                                //var countLasRecort = 0;
                                if (lastRecord == string.Empty)
                                {
                                    //countLasRecort = 'A';
                                }
                                else
                                {
                                    var index = list.FirstOrDefault(n => n.Value.ToString(CultureInfo.InvariantCulture).ToUpper().Equals(lastRecord[lastRecord.Length - 1].ToString(CultureInfo.InvariantCulture).ToUpper()));
                                    if (index.Key == list.Count - 1)
                                    {
                                    }

                                    var tempChar = lastRecord.ToCharArray();
                                    lastRecord = "";
                                    for (var i = 0; i < tempChar.Length - 1; i++)
                                    {
                                        lastRecord += tempChar[i];
                                    }

                                    if (index.Key == list.Max(n => n.Key))
                                    {
                                        var tempLastRecord = lastRecord;
                                        lastRecord = "";
                                        AutoIncrementLastRecordChar(tempLastRecord.ToCharArray().Reverse()).Reverse().ToList().ForEach(n => lastRecord += n);
                                        lastRecord += list[0].ToString(CultureInfo.InvariantCulture);
                                    }
                                    else
                                        lastRecord += list[index.Key + 1].ToString(CultureInfo.InvariantCulture);
                                }
                                if (fields.Info.ToString().Contains("Char"))
                                {
                                    linkItem[fields.FieldName] = Convert.ToChar(lastRecord);
                                    if (!dictionaryAutoIncrement.TryGetValue(linkItem.TableName + fields.FieldName, out result))
                                        dictionaryAutoIncrement.Add(linkItem.TableName + fields.FieldName, (lastRecord));
                                    else
                                        dictionaryAutoIncrement[linkItem.TableName + fields.FieldName] = (lastRecord);
                                }
                                else
                                {
                                    linkItem[fields.FieldName] = tempHasil + lastRecord;
                                    if (!dictionaryAutoIncrement.TryGetValue(linkItem.TableName + fields.FieldName, out result))
                                        dictionaryAutoIncrement.Add(linkItem.TableName + fields.FieldName, (tempHasil + lastRecord));
                                    else
                                        dictionaryAutoIncrement[linkItem.TableName + fields.FieldName] = (tempHasil + lastRecord);
                                }
                            }

                            #endregion RealtionStringAndSparatorAutoIncrementAttribute

                            if (AutoIncrementComplete != null)
                                AutoIncrementComplete(this, new TableItemArgs(linkItem));
                        }
                    }
                }
            }
        }






        /// <summary>
        ///     Creates the inser query.
        /// </summary>
        /// <param name="linkItem">The link item.</param>
        /// <returns>StringBuilder.</returns>
        /// <exception cref="OverloadPrimaryException">
        ///     Over load key
        ///     or
        ///     Over load key
        ///     or
        ///     Over load key
        /// </exception>        
        private StringBuilder CreateInserQuery(TableItem linkItem)
        {
            //var str = new StringBuilder();
            if (linkItem.IsAutoIncrement)
                hasAutoIncrement = true;
            //str.Append("Insert " + linkItem.TableName + " Where ");
            var field = new StringBuilder("(");
            var value = new StringBuilder("(");
            foreach (var fields in linkItem.Fields)
            {
                if (fields.Info.GetCustomAttributes(true).OfType<SkipAttribute>().Count() != 0)
                {
                    var subItem = fields.Info.GetCustomAttributes(true).OfType<SkipAttribute>().FirstOrDefault();

                    #region AutoGenerateDateTimeYYMMAttribute

                    object result;

                    if (subItem is AutoGenerateDateTimeYYMMAttribute)
                    {
                        hasAutoIncrement = true;
                        field.Append(fields.FieldName + ",");
                        var attr = subItem as AutoGenerateDateTimeYYMMAttribute;
                        attr.Property = fields.Info.Name;
                        DateTime? date = DateTime.Now;
                        try
                        {
                            var property = linkItem.GetType().GetProperty("DefaultTime");
                            if (property != null)
                            {
                                date = (DateTime?)property.GetValue(linkItem, null);
                                if (date == DateTime.MinValue || date == null)
                                {
                                    date = DateTime.Now;
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                        }
                        if (date == null)
                        {
                            date = DateTime.Now;
                        }
                        try
                        {
                            var lastRecord = "";
                            var fixLast = "";
                            switch (attr.TypeGenerate)
                            {
                                case AutoGenerateDateTimeYYMMAttribute.TypeAutoGenerate.AutoFill:
                                    lastRecord = AutoFillIndexIndex(linkItem.TableName, fields.FieldName, attr.Length);
                                    break;

                                case AutoGenerateDateTimeYYMMAttribute.TypeAutoGenerate.LastIndex:
                                    lastRecord = LastRecordIndex(linkItem.TableName, fields.FieldName, date.Value);
                                    fixLast = lastRecord;
                                    break;
                                case AutoGenerateDateTimeYYMMAttribute.TypeAutoGenerate.CanGenerateManual:
                                    if (fields.Info.GetValue(linkItem, null) != null &&
                                        !string.IsNullOrEmpty(fields.Info.GetValue(linkItem, null).ToString()))
                                        lastRecord = fields.Info.GetValue(linkItem, null).ToString();
                                    else
                                    {
                                        lastRecord = LastRecordIndex(linkItem.TableName, fields.FieldName, date.Value);
                                    }
                                    break;
                            }

                            if (attr.TypeGenerate !=
                               AutoGenerateDateTimeYYMMAttribute.TypeAutoGenerate.CanGenerateManual ||
                               (attr.TypeGenerate !=
                                AutoGenerateDateTimeYYMMAttribute.TypeAutoGenerate.CanGenerateManual &&
                                fields.Info.GetValue(linkItem, null).ToString() == string.Empty))
                            {
                                if (dictionaryAutoIncrement.TryGetValue(
                                    linkItem.TableName + fields.FieldName, out result)) lastRecord = result.ToString();
                                int countLasRecort;
                                var lengsubstring = lastRecord.Length - attr.Length;
                                if (lengsubstring < 0) lengsubstring = 1;
                                if (lastRecord != string.Empty
                                    &&
                                    lastRecord.ToString(CultureInfo.InvariantCulture).Substring(lengsubstring).
                                        ToCharArray().Count(n => n.Equals('9')) == attr.Length)
                                    throw new OverloadPrimaryException("Over load key");
                                if (lastRecord == string.Empty) countLasRecort = 1;
                                else
                                {
                                    //if (!lastRecord.Substring(0, lengsubstring).Equals(date.Value.ToString("yyMM")))
                                    //{
                                    //    countLasRecort = 1;
                                    //}
                                    if (!lastRecord.Substring(0, lengsubstring).Equals(date.Value.ToString("yyMM")))
                                    {
                                        countLasRecort = 1;
                                    }
                                    else countLasRecort = Convert.ToInt32(lastRecord.Substring(lengsubstring)) + 1;
                                }

                                var hasil = date.Value.ToString("yyMM") + countLasRecort.ToString("D" + attr.Length);
                                //var hasil = lastRecord.Substring(0, lengsubstring) + countLasRecort.ToString("D" + attr.Length);

                                if (fixLast == hasil)
                                {
                                    countLasRecort = Convert.ToInt32(fixLast.Substring(lengsubstring)) + 1;
                                    hasil = date.Value.ToString("yyMM") + countLasRecort.ToString("D" + attr.Length);
                                }

                                value.Append("'" + hasil + "',");
                                linkItem[fields.FieldName] = hasil;
                                if (
                                    !dictionaryAutoIncrement.TryGetValue(
                                        linkItem.TableName + fields.FieldName, out result))
                                    dictionaryAutoIncrement.Add(linkItem.TableName + fields.FieldName, hasil);
                                else dictionaryAutoIncrement[linkItem.TableName + fields.FieldName] = hasil;
                            }

                        }
                        catch (Exception exception)
                        {
                            OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                            if (Error != null)
                                Error(this, new ErrorArgs(exception));
                        }
                    }

                    #endregion AutoGenerateDateTimeYYMMAttribute

                    #region AutoGenerateDateTimeYYMMAttributeCode

                    else if(subItem is AutoGenerateDateTimeYYMMAttributeCode)
                    {
                        hasAutoIncrement = true;
                        field.Append(fields.FieldName + ",");
                        var attr = subItem as AutoGenerateDateTimeYYMMAttributeCode;
                        attr.Property = fields.Info.Name;
                        DateTime? date = DateTime.Now;
                        try
                        {
                            var property = linkItem.GetType().GetProperty("DefaultTime");
                            if(property != null)
                            {
                                date = (DateTime?)property.GetValue(linkItem, null);
                                if(date == DateTime.MinValue || date == null)
                                {
                                    date = DateTime.Now;
                                }
                            }
                        }
                        catch(Exception exception)
                        {
                            OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                        }
                        if(date == null)
                        {
                            date = DateTime.Now;
                        }
                        try
                        {
                            var lastRecord = "";
                            var fixLast = "";
                            switch(attr.TypeGenerate)
                            {
                                case AutoGenerateDateTimeYYMMAttributeCode.TypeAutoGenerate.AutoFill:
                                    lastRecord = AutoFillIndexIndex(linkItem.TableName, fields.FieldName, attr.Length);
                                    break;

                                case AutoGenerateDateTimeYYMMAttributeCode.TypeAutoGenerate.LastIndex:
                                    lastRecord = LastRecordIndexChar(linkItem.TableName, fields.FieldName,attr.Code, date.Value);
                                    fixLast = lastRecord;
                                    break;
                                case AutoGenerateDateTimeYYMMAttributeCode.TypeAutoGenerate.CanGenerateManual:
                                    if(fields.Info.GetValue(linkItem, null) != null &&
                                        !string.IsNullOrEmpty(fields.Info.GetValue(linkItem, null).ToString()))
                                        lastRecord = fields.Info.GetValue(linkItem, null).ToString();
                                    else
                                    {
                                        lastRecord = LastRecordIndexChar(linkItem.TableName, fields.FieldName,attr.Code, date.Value);
                                    }
                                    break;
                            }

                            if(attr.TypeGenerate !=
                               AutoGenerateDateTimeYYMMAttributeCode.TypeAutoGenerate.CanGenerateManual ||
                               (attr.TypeGenerate !=
                                AutoGenerateDateTimeYYMMAttributeCode.TypeAutoGenerate.CanGenerateManual &&
                                fields.Info.GetValue(linkItem, null).ToString() == string.Empty))
                            {
                                if(dictionaryAutoIncrement.TryGetValue(
                                    linkItem.TableName + fields.FieldName, out result))
                                    lastRecord = result.ToString();
                                int countLasRecort;
                                var lengsubstring = lastRecord.Length - attr.Length;
                                if(lengsubstring < 0)
                                    lengsubstring = 1;
                                if(lastRecord != string.Empty
                                    &&
                                    lastRecord.ToString(CultureInfo.InvariantCulture).Substring(lengsubstring).
                                        ToCharArray().Count(n => n.Equals('9')) == attr.Length)
                                    throw new OverloadPrimaryException("Over load key");
                                var dates = date.Value.ToString("yyMM");
                                var last = lengsubstring -1;
                                var leng = lastRecord.Length - attr.Code.Length;
                                var fixLength = lastRecord.Substring(0, leng);
                                if(lastRecord == string.Empty)
                                    countLasRecort = 1;
                                else
                                {
                                   
                                    if(!fixLength.Substring(0, last).Equals(dates))
                                    {
                                        countLasRecort = 1;
                                    }
                                    else
                                        countLasRecort = Convert.ToInt32(fixLength.Substring(lengsubstring)) + 1;
                                }
                              
                                var hasil = date.Value.ToString("yyMM") + countLasRecort.ToString("D" + attr.Length) + attr.Code;
                                //var hasil = lastRecord.Substring(0, lengsubstring) + countLasRecort.ToString("D" + attr.Length);

                                if(fixLast == hasil)
                                {
                                    countLasRecort = Convert.ToInt32(fixLength.Substring(lengsubstring)) + 1;
                                    hasil = date.Value.ToString("yyMM") + countLasRecort.ToString("D" + attr.Length) + attr.Code;
                                }

                                value.Append("'" + hasil + "',");
                                linkItem[fields.FieldName] = hasil;
                                if(
                                    !dictionaryAutoIncrement.TryGetValue(
                                        linkItem.TableName + fields.FieldName, out result))
                                    dictionaryAutoIncrement.Add(linkItem.TableName + fields.FieldName, hasil);
                                else
                                    dictionaryAutoIncrement[linkItem.TableName + fields.FieldName] = hasil;
                            }

                        }
                        catch(Exception exception)
                        {
                            OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                            if(Error != null)
                                Error(this, new ErrorArgs(exception));
                        }
                    }

                    #endregion AutoGenerateDateTimeYYMMAttribute

                    #region CustomIntAutoIncrement

                    else if (subItem is CustomIntAutoIncrement)
                    {
                        hasAutoIncrement = true;
                        field.Append(fields.FieldName + ",");
                        try
                        {
                            var lastRecordResult = LastRecordIntIndex(linkItem.TableName, fields.FieldName);
                            if (dictionaryAutoIncrement.TryGetValue(linkItem.TableName + fields.FieldName, out result))
                                lastRecordResult = result.ToString();
                            var lastRecord = 0;
                            if (!string.IsNullOrEmpty(lastRecordResult))
                                lastRecord = Convert.ToInt32(lastRecordResult);
                            lastRecord++;
                            value.Append("'" + lastRecord + "',");
                            linkItem[fields.FieldName] = Convert.ChangeType(lastRecord, fields.Info.PropertyType);
                            if (!dictionaryAutoIncrement.TryGetValue(linkItem.TableName + fields.FieldName, out result))
                                dictionaryAutoIncrement.Add(linkItem.TableName + fields.FieldName, lastRecord);
                            else
                                dictionaryAutoIncrement[linkItem.TableName + fields.FieldName] =
                                    Convert.ChangeType(lastRecord,
                                        fields.Info.
                                            PropertyType);
                        }
                        catch (Exception exception)
                        {
                            OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                        }
                    }

                    #endregion CustomIntAutoIncrement

                    #region CustomIntAutoIncrement

                    else if (subItem is CustomCharAutoIncrement)
                    {
                        hasAutoIncrement = true;
                        field.Append(fields.FieldName + ",");
                        try
                        {
                            var lastRecordResult = LastRecordIntIndex(linkItem.TableName, fields.FieldName);
                            if (dictionaryAutoIncrement.TryGetValue(linkItem.TableName + fields.FieldName, out result))
                                lastRecordResult = result.ToString();
                            var lastRecord = 0;
                            if (!string.IsNullOrEmpty(lastRecordResult))
                                lastRecord = lastRecordResult[0];
                            else
                                lastRecord = 'A';
                            lastRecord++;
                            value.Append("'" + Convert.ToChar(lastRecord) + "',");
                            linkItem[fields.FieldName] = Convert.ChangeType(lastRecord, fields.Info.PropertyType);
                            if (!dictionaryAutoIncrement.TryGetValue(linkItem.TableName + fields.FieldName, out result))
                                dictionaryAutoIncrement.Add(linkItem.TableName + fields.FieldName, lastRecord);
                            else
                                dictionaryAutoIncrement[linkItem.TableName + fields.FieldName] =
                                    Convert.ChangeType(lastRecord,
                                        fields.Info.
                                            PropertyType);
                        }
                        catch (Exception exception)
                        {
                            OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                        }
                    }

                    #endregion CustomIntAutoIncrement

                    #region CustomAutoIncrementAttribute

                    else if (subItem is RealtionStringAutoIncrementAttribute)
                    {
                        hasAutoIncrement = true;
                        field.Append(fields.FieldName + ",");
                        var listChart = ListAutoIncement.ToCharArray();
                        if (list.Count == 0)
                            for (var i = 0; i < listChart.Length; i++)
                            {
                                list.Add(i, listChart[i]);
                            }
                        var attr = subItem as RealtionStringAutoIncrementAttribute;

                        string lastRecord;
                        var tempHasil = "";
                        if (attr.RelationProperty != null)
                            foreach (var realtionProperty in attr.RelationProperty)
                            {
                                tempHasil +=
                                    linkItem.GetType().GetProperty(realtionProperty).GetValue(linkItem, null).ToString();
                            }
                        lastRecord = LastRecordIndex(linkItem.TableName, fields.FieldName, tempHasil);
                        if (dictionaryAutoIncrement.TryGetValue(linkItem.TableName + fields.FieldName, out result))
                            lastRecord = result.ToString();
                        if (lastRecord.ToString(CultureInfo.InvariantCulture).ToCharArray().Count(n => n.Equals('Z')) ==
                            attr.Value - tempHasil.Length)
                            throw new OverloadPrimaryException("Over load key");
                        if (lastRecord == string.Empty)
                            lastRecord =
                                Convert.ToInt16(0).ToString("D" +
                                                            (attr.Value - tempHasil.Length).ToString(
                                                                CultureInfo.InvariantCulture));
                        else
                            lastRecord = lastRecord.Substring(tempHasil.Length);
                        //var countLasRecort = 0;
                        if (lastRecord == string.Empty)
                        {

                        }//countLasRecort = 'A';
                        else
                        {
                            var index =
                                list.FirstOrDefault(
                                    n =>
                                        n.Value.ToString(CultureInfo.InvariantCulture).ToUpper().Equals(
                                            lastRecord[lastRecord.Length - 1].ToString(CultureInfo.InvariantCulture)
                                                .ToUpper
                                                ()));
                            if (index.Key == list.Count - 1)
                            {
                            }

                            var tempChar = lastRecord.ToCharArray();
                            lastRecord = "";
                            for (var i = 0; i < tempChar.Length - 1; i++)
                            {
                                lastRecord += tempChar[i];
                            }

                            if (index.Key == list.Max(n => n.Key))
                            {
                                var tempLastRecord = lastRecord;
                                lastRecord = "";
                                AutoIncrementLastRecordChar(tempLastRecord.ToCharArray().Reverse()).Reverse().ToList().
                                    ForEach(
                                        n => lastRecord += n);
                                lastRecord += list[0].ToString(CultureInfo.InvariantCulture);
                            }
                            else
                                lastRecord += list[index.Key + 1].ToString(CultureInfo.InvariantCulture);
                        }
                        if (fields.Info.ToString().Contains("Char"))
                        {
                            value.Append("'" + Convert.ToChar(lastRecord) + "',");
                            linkItem[fields.FieldName] = Convert.ToChar(lastRecord);
                            if (!dictionaryAutoIncrement.TryGetValue(linkItem.TableName + fields.FieldName, out result))
                                dictionaryAutoIncrement.Add(linkItem.TableName + fields.FieldName, lastRecord);
                            else
                                dictionaryAutoIncrement[linkItem.TableName + fields.FieldName] = lastRecord;
                        }
                        else
                        {
                            if (!dictionaryAutoIncrement.TryGetValue(linkItem.TableName + fields.FieldName, out result))
                                dictionaryAutoIncrement.Add(linkItem.TableName + fields.FieldName,
                                    (tempHasil + lastRecord));
                            else
                                dictionaryAutoIncrement[linkItem.TableName + fields.FieldName] = (tempHasil + lastRecord);
                            value.Append("'" + (tempHasil + lastRecord) + "',");
                            linkItem[fields.FieldName] = tempHasil + lastRecord;
                        }
                    }

                    #endregion CustomAutoIncrementAttribute

                    #region EncrypteAttribute

                    else if (subItem is EncrypteAttribute)
                    {
                    }

                    #endregion EncrypteAttribute

                    #region RealtionStringAndSparatorAutoIncrementAttribute

                    else if (subItem is RealtionStringAndSparatorAutoIncrementAttribute)
                    {
                        hasAutoIncrement = true;
                        field.Append(fields.FieldName + ",");
                        var listChart = ListAutoIncement.ToCharArray();
                        if (list.Count == 0)
                            for (var i = 0; i < listChart.Length; i++)
                            {
                                list.Add(i, listChart[i]);
                            }
                        var attr = subItem as RealtionStringAndSparatorAutoIncrementAttribute;

                        string lastRecord;
                        var tempHasil = "";
                        if (attr.RelationProperty != null)
                            foreach (var realtionProperty in attr.RelationProperty)
                            {
                                var data =
                                    linkItem.GetType().GetProperty(realtionProperty).GetValue(linkItem, null).ToString();
                                if (!string.IsNullOrEmpty(data))
                                    tempHasil += data + attr.Sparator;
                            }
                        lastRecord = LastRecordIndex(linkItem.TableName, fields.FieldName, tempHasil, attr.Value);
                        if (dictionaryAutoIncrement.TryGetValue(linkItem.TableName + fields.FieldName, out result))
                            lastRecord = result.ToString();
                        if (!string.IsNullOrEmpty(lastRecord) &&
                            lastRecord.ToString(CultureInfo.InvariantCulture).ToCharArray().Count(n => n.Equals('Z')) ==
                            attr.Value - tempHasil.Length)
                            throw new OverloadPrimaryException("Over load key");
                        if (lastRecord == string.Empty)
                            lastRecord =
                                Convert.ToInt16(0).ToString("D" + (attr.Value).ToString(CultureInfo.InvariantCulture));
                        else
                            lastRecord = lastRecord.Substring(tempHasil.Length);
                        //var countLasRecort = 0;
                        if (lastRecord == string.Empty)
                        {
                            //countLasRecort = 'A';
                        }
                        else
                        {
                            var index =
                                list.FirstOrDefault(
                                    n =>
                                        n.Value.ToString(CultureInfo.InvariantCulture).ToUpper().Equals(
                                            lastRecord[lastRecord.Length - 1].ToString(CultureInfo.InvariantCulture)
                                                .ToUpper
                                                ()));
                            if (index.Key == list.Count - 1)
                            {
                            }

                            var tempChar = lastRecord.ToCharArray();
                            lastRecord = "";
                            for (var i = 0; i < tempChar.Length - 1; i++)
                            {
                                lastRecord += tempChar[i];
                            }

                            if (index.Key == list.Max(n => n.Key))
                            {
                                var tempLastRecord = lastRecord;
                                lastRecord = "";
                                AutoIncrementLastRecordChar(tempLastRecord.ToCharArray().Reverse()).Reverse().ToList().
                                    ForEach(
                                        n => lastRecord += n);
                                lastRecord += list[0].ToString(CultureInfo.InvariantCulture);
                            }
                            else
                                lastRecord += list[index.Key + 1].ToString(CultureInfo.InvariantCulture);
                        }
                        if (fields.Info.ToString().Contains("Char"))
                        {
                            value.Append("'" + Convert.ToChar(lastRecord) + "',");
                            linkItem[fields.FieldName] = Convert.ToChar(lastRecord);
                            if (!dictionaryAutoIncrement.TryGetValue(linkItem.TableName + fields.FieldName, out result))
                                dictionaryAutoIncrement.Add(linkItem.TableName + fields.FieldName, (lastRecord));
                            else
                                dictionaryAutoIncrement[linkItem.TableName + fields.FieldName] = (lastRecord);
                        }
                        else
                        {
                            value.Append("'" + (tempHasil + lastRecord) + "',");
                            linkItem[fields.FieldName] = tempHasil + lastRecord;
                            if (!dictionaryAutoIncrement.TryGetValue(linkItem.TableName + fields.FieldName, out result))
                                dictionaryAutoIncrement.Add(linkItem.TableName + fields.FieldName,
                                    (tempHasil + lastRecord));
                            else
                                dictionaryAutoIncrement[linkItem.TableName + fields.FieldName] = (tempHasil + lastRecord);
                        }
                    }

                    #endregion RealtionStringAndSparatorAutoIncrementAttribute

                    if (Current.AutoIncrementComplete != null)
                        Current.AutoIncrementComplete(this, new TableItemArgs(linkItem));

                    if (AutoIncrementComplete != null)
                        AutoIncrementComplete(this, new TableItemArgs(linkItem));
                }
                else
                {
                    if (
                        fields.Info.GetCustomAttributes(true).OfType<FieldAttribute>().Count(
                            n => n.IsAllowNull == SpesicicationType.AutoIncrement) == 0)
                    {
                        if (fields.Info.PropertyType.IsEnum)
                        {
                            var values = Enum.GetValues(fields.Info.PropertyType);
                            var count = 0;
                            foreach (var val in values)
                            {
                                var da = (int)val;
                                if (val.ToString().Equals(fields.Info.GetValue(linkItem, null).ToString()))
                                {
                                    field.Append(fields.FieldName + ",");
                                    value.Append("'" + count + "',");
                                }
                                count++;
                            }
                        }
                        else if (fields.Info.PropertyType.Name.ToLower().Contains("bool"))
                        {
                            field.Append(fields.FieldName + ",");
                            value.Append("'" +
                                         ConnectionManager.ConvertBoolean(
                                             Convert.ToBoolean(fields.Info.GetValue(linkItem, null))) +
                                         "',");
                        }
                        else
                        {
                            field.Append(fields.FieldName + ",");

                            if (fields.Info.PropertyType == typeof(DateTime?))
                            {
                                value.Append((fields.Info.GetValue(linkItem, null) is DateTime)
                                    ? "'" +
                                      ((DateTime)fields.Info.GetValue(linkItem, null)).ToString(
                                          ConnectionManager.FormateDate) + "',"
                                    : "null,");
                            }
                            else if (fields.Info.PropertyType == typeof(double?) ||
                                     fields.Info.PropertyType == typeof(float?))
                            {
                                if (fields.Info.GetValue(linkItem, null) != null)
                                    value.Append("'" +
                                                 fields.Info.GetValue(linkItem, null).ToString().Replace(",", ".") +
                                                 "',");
                                else
                                    value.Append("null,");
                            }
                            else if (fields.Info.PropertyType == typeof(double) ||
                                     fields.Info.PropertyType == typeof(float))
                            {
                                value.Append("'" +
                                             fields.Info.GetValue(linkItem, null).ToString().Replace(",", ".") +
                                             "',");
                            }
                            else
                            {
                                value.Append((fields.Info.GetValue(linkItem, null) is DateTime)
                                    ? "'" +
                                      ((DateTime)fields.Info.GetValue(linkItem, null)).ToString(
                                          ConnectionManager.FormateDate) + "',"
                                    : "'" + fields.Info.GetValue(linkItem, null) + "',");
                            }
                        }
                    }
                }
            }
            foreach (var fields in linkItem.Fields)
            {
                if (fields.Info.GetCustomAttributes(true).OfType<SkipAttribute>().Count() != 0)
                {
                    var subItem = fields.Info.GetCustomAttributes(true).OfType<SkipAttribute>().FirstOrDefault();
                    if (subItem is EncrypteAttribute)
                    {
                        field.Append(fields.FieldName + ",");

                        var encrypteAttribute = subItem as EncrypteAttribute;
                        var dataEncrypte = fields.Info.GetValue(linkItem, null);
                        if (dataEncrypte != null)
                        {
                            var encrypte = dataEncrypte.ToString();
                            //var password =
                            //    linkItem.Fields.Where(fieldAttribute => encrypteAttribute.RelationProperty != null).
                            //        Aggregate("",
                            //                  (current1, fieldAttribute) =>
                            //                  encrypteAttribute.RelationProperty.Where(
                            //                      relation => fieldAttribute.Info.Name.Equals(relation)).Aggregate(
                            //                          current1,
                            //                          (current, relation) =>
                            //                          current + fieldAttribute.Info.GetValue(linkItem, null)));
                            encrypte = Cryptography.FuncAesEncrypt(encrypte);
                            //encrypte = encrypte.Encrypt(encrypteAttribute.Key + password);
                            if (dataEncrypte is double || dataEncrypte is float)
                                value.Append("'" + encrypte.Replace(",", ".") + "',");
                            else
                                value.Append("'" + encrypte + "',");
                        }
                        else
                        {
                            value.Append("null,");
                        }
                    }
                    if (AutoIncrementComplete != null)
                        AutoIncrementComplete(this, new TableItemArgs(linkItem));
                }
            }
            var field_ = field.ToString();
            var value_ = value.ToString();
            if (field_ != string.Empty)
            {
                field_ = field_.Substring(0, field_.Length - 1) + ")";
            }
            if (value_ != string.Empty)
            {
                value_ = value_.Substring(0, value_.Length - 1) + ")";
            }
            var sql = new StringBuilder("Insert into " + linkItem.TableName + " " + field_ + " VALUES " + value_);
            sql = sql.Replace("''", "null");
            return sql;
        }

        /// <summary>
        ///     Recursives the insert.
        /// </summary>
        /// <param name="querys">The query.</param>
        /// <param name="linkItem">The link item.</param>
        /// <returns>StringBuilder.</returns>
        private StringBuilder RecursiveInsert(StringBuilder querys, TableItem linkItem)
        {
            var query = new StringBuilder();
            if (linkItem != null)
            {
                linkItem.WillCommit = true;
                //if (linkItem.TagBeforeInsert != null)
                //    foreach (var before in linkItem.TagBeforeInsert)
                //    {
                //        var temp = CreateInserQuery(before as TableItem) + ";";
                //        if (!query.ToString().Contains(temp))
                //            query.Append(temp);
                //        query = RecursiveInsert(query, (before as TableItem));
                //    }
                var str = CreateInserQuery(linkItem) + ";";
                if (!query.ToString().Contains(str))
                    query.Append(str + ";");
                //if (linkItem.TagAfterInsert != null)
                //    foreach (var after in linkItem.TagAfterInsert)
                //    {
                //        var temp = CreateInserQuery(after as TableItem) + ";";
                //        if (!query.ToString().Contains(temp))
                //            query.Append(temp);

                //        //query = RecursiveInsert(query, (after as LinkItem));
                //    }
            }
            return query;
        }

        #endregion Method

        #region Property

        /// <summary>
        ///     Gets or sets the current.
        /// </summary>
        /// <value>The current.</value>
        public static ContextManager Current { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [use caching].
        /// </summary>
        /// <value><c>true</c> if [use caching]; otherwise, <c>false</c>.</value>
        public bool UseCaching { get; set; }

        /// <summary>
        ///     Gets or sets the connection manager.
        /// </summary>
        /// <value>The connection manager.</value>
        public IConnectionManager ConnectionManager { get; protected set; }

        /// <summary>
        ///     Gets or sets the tag delete.
        /// </summary>
        /// <value>The tag delete.</value>
        internal IList<object> TagDelete { get; set; }

        /// <summary>
        ///     Gets or sets the tag save.
        /// </summary>
        /// <value>The tag save.</value>
        internal Dictionary<string, object> TagSave { get; set; }

        /// <summary>
        ///     Gets or sets the tag update.
        /// </summary>
        /// <value>The tag update.</value>
        internal Dictionary<string, object> TagUpdate { get; set; }

        public void ClearUpdate()
        {
            TagUpdate.Clear();
        }

        /// <summary>
        ///     Creates the database. digunakan untuk membuat database sesuai dengan model yang telah di deskripsikan
        /// </summary>
        /// <returns>Exception.</returns>
        public Exception CreateDatabase()
        {
            try
            {
                if (!DatabaseExist())
                {
                    ConnectionManager.CreateDatabase();
                }
                return CreateTable();
            }
            catch (Exception exception)
            {
                OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                if (exception.Message.Contains("No process is on the other end of the pipe."))
                    return CreateTable();
                return exception;
            }
        }

        /// <summary>
        ///     Creates the table. digunkan untuk membuat table sesuai dengan domain yang telah di registrasikan di context manager
        /// </summary>
        /// <returns>Exception.</returns>
        /// <example>
        ///     contoh yang digunakan untuk  memanggil  Add Item with Single Item
        ///     <code>
        ///   public class PresentationContext : ContextManager
        ///   {
        ///      public CoreContext<![CDATA[<Table>]]> Tables { get; set; }
        ///   }
        ///  
        ///   public class Main
        ///   {
        ///      public Main()
        ///      {
        ///          var context= new PresentationContext ();
        ///          var error=context.CreateTable();
        ///          if(erro!=null) throw;
        ///      }
        ///   }
        /// </code>
        /// </example>
        private Exception CreateTable()
        {
            try
            {
                foreach (var property in GetType().GetProperties())
                {
                    if (!property.PropertyType.IsClass || !property.PropertyType.IsPublic) continue;
                    var data = property.GetValue(this, null);
                    if (data == null) continue;
                    var table = data as CoreContext;
                    if (table == null) continue;
                    if (ConnectionManager.CreateTable(table.Domain as TableItem))
                        if (table.Domain is IPrepareTable)
                        {
                            InsertObject((table.Domain as IPrepareTable).InitializeData().Cast<TableItem>());
                        }
                }
                Commit();
                return null;
            }
            catch (Exception exception)
            {
                OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                Log.Error(exception);
                if (exception.Message.Contains("No process is on the other end of the pipe."))
                    return CreateTable();
                return exception;
            }
        }

        /// <summary>
        ///     Creates the table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns>Exception.</returns>
        /// ///
        /// <summary>
        ///     Creates the table. digunkan untuk membuat table sesuai dengan domain yang telah di registrasikan di context manager
        /// </summary>
        /// <returns>Exception.</returns>
        /// <example>
        ///     contoh yang digunakan untuk  memanggil  Add Item with Single Item
        ///     <code>
        ///   public class PresentationContext : ContextManager
        ///   {
        ///      public CoreContext<![CDATA[<Table>]]> Tables { get; set; }
        ///   }
        ///  
        ///   public class Main
        ///   {
        ///      public Main()
        ///      {
        ///          var context= new PresentationContext ();
        ///          var error=context.CreateTable(new Table());
        ///          if(erro!=null) throw;
        ///      }
        ///   }
        /// </code>
        /// </example>
        public Exception CreateTable(TableItem table)
        {
            try
            {
                ConnectionManager.CreateTable(table);
                Commit();
                return null;
            }
            catch (Exception exception)
            {
                OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                if (exception.Message.Contains("already exists in database"))
                    return null;
                if (exception.Message.Contains("Cannot create a user thread. "))
                {
                    ThreadPool.QueueUserWorkItem(Replace, table);
                }
                return exception;
            }
        }


        /// <summary>
        ///     Replaces the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        private void Replace(object sender)
        {
            CreateTable(sender as TableItem);
        }

        /// <summary>
        ///     Databases the exist. digunkan untuk pengecekan database pa server database
        /// </summary>
        /// <returns><c>true</c> jika database ada di dalam server <c>false</c> jika database tidak ada di dalam server</returns>
        public bool DatabaseExist()
        {
            try
            {
                using (var conn = ConnectionManager.Connection)
                {
                    if (!Current.ConnectionString.Equals(ConnectionString))
                    {
                        Current = this;
                        FailedConnection = false;
                    }
                    if (FailedConnection) return false;
                    conn.Open();
                    return true;
                }
            }
            catch (Exception exception)
            {
                Current.FailedConnection = true;
                FailedConnection = true;
                OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                OnError(new ErrorArgs(exception));
                return false;
            }
        }

        #endregion Property

        #region Constractor       

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContextManager" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public ContextManager(string connectionString)
            : this(connectionString, new SqlConnectionManager(connectionString))
        {
            //ConnectionString = connectionString;
            //{Current = this;FailedConnection = false;}
            //ConnectionManager = new SqlConnectionManager(connectionString);
            //TagUpdate = new CoreDictionary<string, object>();
            //TagSave = new CoreDictionary<string, object>();
            //TagDelete = new List<object>();
            //VirtualRow = new CoreDictionary<string, object>();
            //foreach (var property in GetType().GetProperties())
            //{
            //    if (!property.PropertyType.IsClass || !property.PropertyType.IsPublic || property.PropertyType.Name.Equals("String")) continue;
            //    var data = property.GetValue(this, null);
            //    if (data != null) continue;
            //    if (property.PropertyType == null) continue;
            //    var createProperty = Activator.CreateInstance(property.PropertyType);
            //    var coreContext = createProperty as CoreContext;
            //    if (coreContext != null) coreContext.Manager = this;
            //    property.SetValue(this, createProperty, null);
            //}

            //var setting = BaseDependency.Get<ISetting>();
            //if (setting != null)
            //{
            //    var isPostgree = setting.GetValue("DB_Adapter");
            //    if (isPostgree != null)
            //    {
            //        if (isPostgree == "Postgre")
            //        {
            //            ConnectionManager = new PostgreConnection(connectionString);
            //        }
            //    }
            //}
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContextManager" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="context">The context.</param>
        public ContextManager(string connectionString, BaseConnectionManager context)
        {
            //if (string.IsNullOrEmpty(connectionString))
            //    throw new ArgumentNullException("connectionString Is Null");
            MyGuid = Guid.NewGuid();
            ConnectionString = connectionString;
            if (Current == null)
            {
                Current = this;
                FailedConnection = false;
            }
            else if (string.IsNullOrEmpty(context.ConnectionString))
            {
                //  {Current = this;FailedConnection = false;}
            }
            else if (!context.ConnectionString.Equals(Current.ConnectionString))
            {
                {
                    Current = this;
                    FailedConnection = false;
                }
            }
            context.ConnectionString = connectionString;
            ConnectionManager = context;
            TagUpdate = new CoreDictionary<string, object>();
            TagSave = new CoreDictionary<string, object>();
            TagDelete = new List<object>();
            VirtualRow = new CoreDictionary<string, object>();
            foreach (var property in GetType().GetProperties())
            {
                if (!property.PropertyType.IsClass || !property.PropertyType.IsPublic ||
                    property.PropertyType.Name.Equals("String")) continue;
                var data = property.GetValue(this, null);
                if (data != null) continue;
                var createProperty = Activator.CreateInstance(property.PropertyType);
                var coreContext = createProperty as CoreContext;
                if (coreContext != null) coreContext.Manager = this;
                property.SetValue(this, createProperty, null);
            }
        }

        public void ChangeCurrentContext()
        {
            {
                Current = this;
                FailedConnection = false;
            }
        }

        #endregion Constractor

        #region Get Json

        /// <summary>
        ///     Gets the json. mengambil data dari database namun balikan dari mehtod ini berupa format json
        /// </summary>
        /// <typeparam name="TEntity">berisikan entity yang akan di ambil di dalam database</typeparam>
        /// <returns>System.String.</returns>
        public string GetJson<TEntity>() where TEntity : TableItem
        {
            return GetJson<TEntity>(new string[] { });
        }

        /// <summary>
        ///     Gets the json. mengambil data dari database namun balikan dari mehtod ini berupa format json
        /// </summary>
        /// <typeparam name="TEntity">berisikan entity yang akan di ambil di dalam database</typeparam>
        /// <param name="include">berisikan property object yang akan di masukan ke dalam hasil json</param>
        /// <returns>System.String.</returns>
        public string GetJson<TEntity>(params string[] include) where TEntity : TableItem
        {
            return GetJson(Activator.CreateInstance<TEntity>().GetType(), "", -1, "", -1, "", -1, include);
        }

        /// <summary>
        ///     Gets the json. mengambil data dari database namun balikan dari mehtod ini berupa format json
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="select">list field yang akan di masukan kedalam result.</param>
        /// <param name="include">berisikan property object yang akan di masukan ke dalam hasil json</param>
        /// <returns>System.String.</returns>
        public string GetJson(Type entityType, string select, params string[] include)
        {
            return GetJson(entityType, select, -1, "", -1, "", -1, include);
        }

        /// <summary>
        ///     Gets the json. mengambil data dari database namun balikan dari mehtod ini berupa format json
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="top">Type of the entity.</param>
        /// <param name="include">berisikan property object yang akan di masukan ke dalam hasil json</param>
        /// <returns>System.String.</returns>
        public string GetJson(Type entityType, int top, params string[] include)
        {
            return GetJson(entityType, "", top, "", -1, "", -1, include);
        }

        /// <summary>
        ///     Gets the json. mengambil data dari database namun balikan dari mehtod ini berupa format json
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="select">list field yang akan di masukan kedalam result.</param>
        /// <param name="top">berisikan jumlah data yang akan diambil dari row pertama</param>
        /// <param name="orderBy">berisikan hasil akan di order menurut field mana</param>
        /// <param name="take">berisikan jumlah data yang akan di ambil</param>
        /// <param name="filter">berisikan query filter </param>
        /// <param name="skip">berisikan jumlah data yang akan di skip</param>
        /// <param name="include">berisikan property object yang akan di masukan ke dalam hasil json</param>
        /// <returns>System.String.</returns>
        public string GetJson(Type entityType, string select, int top, string orderBy, int take, string filter, int skip,
            params string[] include)
        {
            var str = new StringBuilder();
            using (var conn = ConnectionManager.Connection)
            {
                str.Append("[");
                try
                {
                    if (!Current.ConnectionString.Equals(ConnectionString))
                    {
                        Current = this;
                        FailedConnection = false;
                    }
                    if (FailedConnection) return "[]";
                    conn.Open();
                }
                catch (Exception exception)
                {
                    Current.FailedConnection = true;
                    FailedConnection = true;
                    OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                    OnError(new ErrorArgs(exception));
                }
                using (var command = ConnectionManager.Command)
                {
                    command.Connection = conn;
                    var item = Activator.CreateInstance(entityType);
                    var tableItem = item as TableItem;
                    if (tableItem != null)
                    {
                        var queryFilter = ConnectionManager.CreateFilter(filter.ToLower());
                        var encryteList =
                            tableItem.Fields.Where(
                                n => n.Info.GetCustomAttributes(true).OfType<EncrypteAttribute>().Count() != 0).Select(
                                    n => n.FieldName);
                        var table = item.GetType().GetCustomAttributes(true).OfType<TableAttribute>().FirstOrDefault();
                        if (table == null) return "";
                        if (string.IsNullOrEmpty(select))
                            select = "*";
                        else
                        {
                            var temp = select;
                            select = "";
                            foreach (var property in temp.Split(','))
                            {
                                var field = tableItem.Fields.FirstOrDefault(n => n.Info.Name.Equals(property));
                                select += field.FieldName + ",";
                            }
                            select = select.Substring(0, select.Length - 1);
                        }
                        if (!string.IsNullOrEmpty(orderBy))
                        {
                            var temp = orderBy;
                            orderBy = "";
                            foreach (var property in temp.Split(','))
                            {
                                var prop = property.Split(' ');
                                var field = tableItem.Fields.FirstOrDefault(n => n.Info.Name.Equals(prop[0]));

                                if (field != null)
                                    orderBy += field.FieldName + ((prop.Length == 2) ? " " + prop[1] : "") + ",";
                            }
                            orderBy = " Order By " + orderBy.Substring(0, orderBy.Length - 1);
                        }
                        if (!string.IsNullOrEmpty(queryFilter.Filter))
                        {
                            filter = queryFilter.Filter;
                            filter = " Where " + filter;
                        }
                        else
                        {
                            filter = "";
                        }
                        command.CommandText = "Select " + select + " from " + table.TabelName + filter + orderBy;
                        var keyField = "";
                        var read = command.ExecuteReader();
                        var hasRetrive = 0;
                        var dictionaryField =
                            tableItem.Fields.ToDictionary(fieldAttribute => fieldAttribute.FieldName.ToLower(),
                                fieldAttribute => fieldAttribute.Info.Name);
                        while (read.Read())
                        {
                            var IsValid = true;
                            var strResult = new StringBuilder();
                            if (skip != -1)
                            {
                                if (hasRetrive < skip)
                                {
                                    hasRetrive++;
                                    continue;
                                }
                                if (hasRetrive - skip == take)
                                    break;
                            }
                            else if (hasRetrive == take)
                                break;
                            strResult.Append("{");

                            for (var i = 0; i < read.FieldCount; i++)
                            {
                                if (read[i] is bool)
                                {
                                    keyField = read.GetName(i).ToLower();
                                    if (dictionaryField.TryGetValue(keyField, out keyField))
                                        strResult.Append("\"" + dictionaryField[read.GetName(i).ToLower()] + "\":" +
                                                         read[i].ToString().ToLower() + ",");
                                }
                                else if (read[i] is string)
                                {
                                    if (!read[i].ToString().Contains("="))
                                    {
                                        keyField = read.GetName(i).ToLower();
                                        if (dictionaryField.TryGetValue(keyField, out keyField))
                                            strResult.Append("\"" + dictionaryField[read.GetName(i).ToLower()] + "\":\"" +
                                                             read[i] + "\",");
                                    }
                                    else
                                    {
                                        try
                                        {
                                            keyField = read.GetName(i).ToLower();
                                            var firstOrDefault = tableItem.Fields.FirstOrDefault(
                                                n =>
                                                    n.FieldName.Equals(read.GetName(i)) &&
                                                    n.Info.GetCustomAttributes(true).OfType<EncrypteAttribute>().Count() !=
                                                    0);
                                            if (firstOrDefault != null)
                                            {
                                                var fieldEncrypte = firstOrDefault.Info;
                                                if (fieldEncrypte != null)
                                                {
                                                    var encrypteAttribute =
                                                        fieldEncrypte.GetCustomAttributes(true).OfType
                                                            <EncrypteAttribute>().FirstOrDefault();
                                                    var descrypte = read[i].ToString();
                                                    var password = "";
                                                    if (encrypteAttribute != null)
                                                    {
                                                        password = encrypteAttribute.RelationProperty.Aggregate(
                                                            password, (current, relation) => current + read[relation]);

                                                        //password = encrypteAttribute.RelationProperty.Aggregate(password, (current, relation) => ((Previous[relation] != null) ? current + Previous[relation] : current + Fields.FirstOrDefault(n => n.FieldName.Equals(relation)).Info.GetValue(this, null).ToString()));
                                                        descrypte = descrypte.Decrypt(encrypteAttribute.Key + password);
                                                        keyField = read.GetName(i).ToLower();
                                                        if (dictionaryField.TryGetValue(keyField, out keyField))
                                                            strResult.Append("\"" +
                                                                             dictionaryField[read.GetName(i).ToLower()] +
                                                                             "\":\"" + descrypte + "\",");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (dictionaryField.TryGetValue(keyField, out keyField))
                                                    strResult.Append("\"" + dictionaryField[read.GetName(i).ToLower()] +
                                                                     "\":***\"" + read[i] + "\"***,");
                                            }
                                        }
                                        catch (Exception exception)
                                        {
                                            Log.Error(exception);
                                            keyField = read.GetName(i).ToLower();
                                            if (dictionaryField.TryGetValue(keyField, out keyField))
                                                strResult.Append("\"" + dictionaryField[read.GetName(i).ToLower()] +
                                                                 "\":\"" + read[i] + "\",");
                                        }
                                    }
                                }

                                else if (read[i] is DateTime)
                                {
                                    keyField = read.GetName(i).ToLower();
                                    if (dictionaryField.TryGetValue(keyField, out keyField))
                                        strResult.Append("\"" + dictionaryField[read.GetName(i).ToLower()] + "\":\"" +
                                                         ((DateTime?)ConnectionManager.ConverterToObject(read[i], null))
                                                             .Value.ToString("yyyy-MM-ddT00:00:00").
                                                             ToString(CultureInfo.InvariantCulture) + "\",");
                                }
                                else
                                {
                                    keyField = read.GetName(i).ToLower();
                                    if (dictionaryField.TryGetValue(keyField, out keyField))
                                        strResult.Append("\"" + dictionaryField[read.GetName(i).ToLower()] + "\":\"" +
                                                         read[i] + "\",");
                                }
                            }
                            if (include != null)
                            {
                                foreach (var reference in include)
                                {
                                    try
                                    {
                                        PropertyInfo relatedType;
                                        var hasChild = false;
                                        if (reference.Contains("."))
                                        {
                                            hasChild = true;
                                            relatedType = item.GetType().GetProperty(reference.Split('.')[0]);
                                        }
                                        else
                                            relatedType = item.GetType().GetProperty(reference);
                                        var atrs = relatedType.GetCustomAttributes(true).OfType<ReferenceApiAttribute>();
                                        var rel = atrs.FirstOrDefault();
                                        if (rel == null) continue;
                                        var sql =
                                            ConnectionManager.CreateRelationQuery(
                                                rel.ReferenecKey.Select(s => s.Split('=')).Select(
                                                    temp => temp[0] + "='" + read[temp[1]] + "'").ToArray());

                                        // 1 != null
                                        // 2 == null
                                        byte doaminEmpty = 0;
                                        foreach (var referenceAttribute in queryFilter.Relation)
                                        {
                                            if (referenceAttribute.ToLower().Contains(reference.ToLower()))
                                            {
                                                if (referenceAttribute.Trim().ToLower().StartsWith("or"))
                                                {
                                                    doaminEmpty = 0;
                                                    continue;
                                                }
                                                if (referenceAttribute.Trim().ToLower().StartsWith("and"))
                                                {
                                                    if (referenceAttribute.ToLower().Contains(reference.ToLower() + "."))
                                                    {
                                                        doaminEmpty = 1;
                                                        sql += " " + referenceAttribute.Replace(reference + ".", "");
                                                    }
                                                    else
                                                    {
                                                        if (referenceAttribute.Contains("<>"))
                                                            doaminEmpty = 1;
                                                        else doaminEmpty = 2;
                                                    }
                                                }
                                                else if (referenceAttribute.ToLower().Trim().StartsWith("not") ||
                                                         referenceAttribute.StartsWith(reference))
                                                {
                                                    if (
                                                        referenceAttribute.ToLower().Contains(reference.ToLower() +
                                                                                              "."))
                                                    {
                                                        doaminEmpty = 1;
                                                        sql += " and " +
                                                               referenceAttribute.Replace(reference + ".", "");
                                                    }
                                                    else
                                                    {
                                                        if (referenceAttribute.Contains("<>") ||
                                                            referenceAttribute.Contains("not"))
                                                            doaminEmpty = 1;
                                                        else doaminEmpty = 2;
                                                    }
                                                }
                                                else
                                                {
                                                    IsValid = false;
                                                }
                                            }
                                        }
                                        if (IsValid)
                                        {
                                            object referenceItem;
                                            if (!hasChild)
                                                referenceItem = GetOneDataJson(relatedType.PropertyType, sql);
                                            else
                                            {
                                                referenceItem = GetOneDataJsonByType(relatedType.PropertyType, sql,
                                                    reference.Substring(
                                                        reference.Split('.')[0].
                                                            Length + 1));
                                            }
                                            if (referenceItem == null || referenceItem.Equals("[]"))
                                            {
                                                if (doaminEmpty == 1)
                                                {
                                                    IsValid = false;
                                                }
                                                strResult.Append("\"" + relatedType.Name + "\":[],");
                                            }
                                            else
                                            {
                                                if (doaminEmpty == 2)
                                                    IsValid = false;
                                                strResult.Append("\"" + relatedType.Name + "\":" + referenceItem + ",");
                                            }
                                        }
                                    }
                                    catch (Exception exception)
                                    {
                                        OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                                        if (!string.IsNullOrEmpty(reference))
                                            strResult.Append("\"" + reference + "\":[],");
                                    }
                                }
                            }
                            strResult.Remove(strResult.Length - 1, 1);
                            strResult.Append("},");
                            if (hasRetrive + 1 == top)
                                break;
                            if (top != -1 || take != -1 || skip != -1)
                                hasRetrive++;
                            if (IsValid)
                                str.Append(strResult);
                        }
                    }
                    if (str.Length != 1)
                    {
                        str.Remove(str.Length - 1, 1);
                    }
                    str.Append("]");
                }
            }
            return str.ToString();
        }

        /// <summary>
        ///     Gets the one data json. digunakan untuk mengambil satu object json yang bersama  relasinya
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="query">berisikan nilai filter untuk sql query yang akan ditampilkan</param>
        /// <param name="child">The child.</param>
        /// <returns>System.Object.</returns>
        public object GetOneDataJsonByType(Type tableName, string query, string child)
        {
            var field = "";
            using (var conn = ConnectionManager.Connection)
            {
                var tableItem = Activator.CreateInstance(tableName) as TableItem;
                try
                {
                    if (!Current.ConnectionString.Equals(ConnectionString))
                    {
                        Current = this;
                        FailedConnection = false;
                    }
                    if (FailedConnection) return null;
                    conn.Open();
                }
                catch (Exception exception)
                {
                    Current.FailedConnection = true;
                    FailedConnection = true;
                    OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                    OnError(new ErrorArgs(exception));
                }
                using (var command = ConnectionManager.Command)
                {
                    command.Connection = conn;

                    if (string.IsNullOrEmpty(tableItem.TableName))
                        return null;
                    command.CommandText = "Select * from " + tableItem.TableName + " Where " + query;
                    var read = command.ExecuteReader();
                    var dictionaryField = tableItem.Fields.ToDictionary(fieldAttribute => fieldAttribute.FieldName,
                        fieldAttribute => fieldAttribute.Info.Name);
                    while (read.Read())
                    {
                        var str = new StringBuilder();
                        str.Append("{");
                        for (var i = 0; i < read.FieldCount; i++)
                        {
                            if (read[i] is string)
                            {
                                if (!read[i].ToString().Contains("="))
                                {
                                    field = read.GetName(i).ToLower();
                                    if (dictionaryField.TryGetValue(field, out field))
                                        str.Append("\"" + dictionaryField[read.GetName(i)] + "\":\"" + read[i] + "\",");
                                }
                                else
                                {
                                    try
                                    {
                                        var firstOrDefault = tableItem.Fields.FirstOrDefault(
                                            n =>
                                                n.FieldName.Equals(read.GetName(i).ToLower()) &&
                                                n.Info.GetCustomAttributes(true).OfType<EncrypteAttribute>().Count() !=
                                                0);
                                        if (firstOrDefault != null)
                                        {
                                            var fieldEncrypte = firstOrDefault.Info;
                                            if (fieldEncrypte != null)
                                            {
                                                var encrypteAttribute =
                                                    fieldEncrypte.GetCustomAttributes(true).OfType<EncrypteAttribute>().
                                                        FirstOrDefault();
                                                var descrypte = read[i].ToString();
                                                var password = "";
                                                if (encrypteAttribute != null)
                                                {
                                                    foreach (var relation in encrypteAttribute.RelationProperty)
                                                    {
                                                        password += read[relation];
                                                    }

                                                    //password = encrypteAttribute.RelationProperty.Aggregate(password, (current, relation) => ((Previous[relation] != null) ? current + Previous[relation] : current + Fields.FirstOrDefault(n => n.FieldName.Equals(relation)).Info.GetValue(this, null).ToString()));
                                                    descrypte = descrypte.Decrypt(encrypteAttribute.Key + password);
                                                    field = read.GetName(i).ToLower();
                                                    if (dictionaryField.TryGetValue(field, out field))
                                                        str.Append("\"" + dictionaryField[read.GetName(i)] + "\":\"" +
                                                                   descrypte + "\",");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (dictionaryField.TryGetValue(field, out field))
                                                str.Append("\"" + dictionaryField[read.GetName(i)] + "\":***\"" +
                                                           read[i] + "\"***,");
                                        }
                                    }
                                    catch (Exception exception)
                                    {
                                        OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                                        field = read.GetName(i).ToLower();
                                        if (dictionaryField.TryGetValue(field, out field))
                                            str.Append("\"" + dictionaryField[read.GetName(i).ToLower()] + "\":\"" +
                                                       read[i].ToString().Replace("\"", "\\\"") + "\",");
                                    }
                                }
                            }
                            else if (read[i] is bool)
                            {
                                field = read.GetName(i).ToLower();
                                if (dictionaryField.TryGetValue(field, out field))
                                    str.Append("\"" + dictionaryField[read.GetName(i).ToLower()] + "\":" +
                                               read[i].ToString().ToLower() + ",");
                            }
                            else if (read[i] is DateTime)
                            {
                                field = read.GetName(i).ToLower();
                                if (dictionaryField.TryGetValue(field, out field))
                                    str.Append("\"" + dictionaryField[read.GetName(i)] + "\":\"" +
                                               ((DateTime?)ConnectionManager.ConverterToObject(read[i], null)).Value
                                                   .ToString("yyyy-MM-ddT00:00:00") + "\",");
                            }
                            else if (read[i] is DBNull)
                            {
                                if (dictionaryField.TryGetValue(read.GetName(i).ToLower(), out field))
                                    str.Append("\"" + dictionaryField[read.GetName(i)] + "\":\"\",");
                            }
                            else
                            {
                                field = read.GetName(i).ToLower();
                                if (dictionaryField.TryGetValue(field, out field))
                                    str.Append("\"" + dictionaryField[read.GetName(i)] + "\":\"" + read[i] + "\",");
                            }
                        }
                        object referenceItem;
                        if (child.Contains("."))
                        {
                            //   referenceItem = GetOneDataJson(relatedType.PropertyType, sql, reference.Substring(reference.Split(new[] { '.' })[0].Length + 1));
                        }

                        //else
                        {
                            PropertyInfo relatedType;
                            var hasChild = false;
                            if (child.Contains("."))
                            {
                                hasChild = true;
                                relatedType = tableItem.GetType().GetProperty(child.Split('.')[0]);
                            }
                            else
                                relatedType = tableItem.GetType().GetProperty(child);
                            var atrs = relatedType.GetCustomAttributes(true).OfType<ReferenceApiAttribute>();
                            var rel = atrs.FirstOrDefault();
                            if (rel == null) continue;
                            var sql =
                                ConnectionManager.CreateRelationQuery(
                                    rel.ReferenecKey.Select(s => s.Split('=')).Select(
                                        temp => temp[0] + "='" + read[temp[1]] + "'").ToArray());

                            // 1 != null
                            // 2 == null
                            byte doaminEmpty = 0;
                            if (!hasChild)
                                referenceItem = GetOneDataJson(relatedType.PropertyType, sql);
                            else
                            {
                                referenceItem = GetOneDataJsonByType(relatedType.PropertyType, sql,
                                    child.Substring(child.Split('.')[0].Length + 1));
                            }
                            var IsValid = true;
                            var strResult = new StringBuilder();
                            if (referenceItem == null || referenceItem.Equals("[]"))
                            {
                                if (doaminEmpty == 1)
                                {
                                    IsValid = false;
                                }
                                strResult.Append("\"" + relatedType.Name + "\":[],");
                            }
                            else
                            {
                                if (doaminEmpty == 2)
                                    IsValid = false;
                                strResult.Append("\"" + relatedType.Name + "\":" + referenceItem + ",");
                            }
                            strResult.Remove(strResult.Length - 1, 1);

                            //strResult.Append("},");

                            if (IsValid)
                                str.Append(strResult);
                        }

                        //else
                        //{
                        //    referenceItem = GetOneDataJson(relatedType.PropertyType, sql, reference.Substring(reference.Split(new[] { '.' })[0].Length + 1));
                        //}
                        str.Append("}");
                        return str.ToString();
                    }
                }
            }
            return "[]";
        }

        /// <summary>
        ///     Gets the one data json. digunkakan untuk mengambil satu object json menurut table name
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="query">berisikan nilai filter untuk sql query yang akan ditampilkan</param>
        /// <returns>System.Object.</returns>
        public object GetOneDataJsonByName(string tableName, string query)
        {
            using (var conn = ConnectionManager.Connection)
            {
                try
                {
                    if (!Current.ConnectionString.Equals(ConnectionString))
                    {
                        Current = this;
                        FailedConnection = false;
                    }
                    if (FailedConnection) return null;
                    conn.Open();
                }
                catch (Exception exception)
                {
                    Current.FailedConnection = true;
                    FailedConnection = true;
                    OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                    OnError(new ErrorArgs(exception));
                }
                using (var command = ConnectionManager.Command)
                {
                    command.Connection = conn;

                    if (string.IsNullOrEmpty(tableName))
                        return null;
                    command.CommandText = "Select * from " + tableName + " Where " + query;
                    var read = command.ExecuteReader();
                    while (read.Read())
                    {
                        var str = new StringBuilder();
                        str.Append("{");
                        for (var i = 0; i < read.FieldCount; i++)
                        {
                            if (read[i] is string)
                            {
                                str.Append("\"" + read.GetName(i) + "\":\"" + read[i].ToString().Replace("\"", "\\\"") +
                                           "\",");
                            }
                            else if (read[i] is DateTime)
                                str.Append("\"" + read.GetName(i) + "\":\"" +
                                           ((DateTime?)ConnectionManager.ConverterToObject(read[i], null)).Value
                                               .ToString("yyyy-MM-ddT00:00:00") + "\",");
                            else
                                str.Append("\"" + read.GetName(i) + "\":\"" + read[i] + "\",");
                        }
                        str.Remove(str.Length - 1, 1);
                        str.Append("}");
                        return str.ToString();
                    }
                }
            }
            return "[]";
        }

        /// <summary>
        ///     Gets the one data json.digunkakan untuk mengambil satu object json menurut type domain name
        /// </summary>
        /// <param name="table">berisikan type table dari domain yang akan di ambil</param>
        /// <param name="query">berisikan nilai filter untuk sql query yang akan ditampilkan</param>
        /// <returns>System.Object.</returns>
        private object GetOneDataJson(Type table, string query)
        {
            var field = "";
            using (var conn = ConnectionManager.Connection)
            {
                object obj = null;
                if (table.IsInterface)
                {
                    obj = Activator.CreateInstance(table.GetGenericArguments()[0]);

                    //return null;
                }
                else
                    obj = Activator.CreateInstance(table);
                var tableItem = new TableItem();
                tableItem = obj as TableItem;
                try
                {
                    if (!Current.ConnectionString.Equals(ConnectionString))
                    {
                        Current = this;
                        FailedConnection = false;
                    }
                    if (FailedConnection) return null;
                    conn.Open();
                }
                catch (Exception exception)
                {
                    Current.FailedConnection = true;
                    FailedConnection = true;
                    OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                    OnError(new ErrorArgs(exception));
                }
                using (var command = ConnectionManager.Command)
                {
                    command.Connection = conn;

                    if (string.IsNullOrEmpty(tableItem.TableName))
                        return null;
                    command.CommandText = "Select * from " + tableItem.TableName + " Where " + query;
                    var read = command.ExecuteReader();
                    var dictionaryField =
                        tableItem.Fields.ToDictionary(fieldAttribute => fieldAttribute.FieldName.ToLower(),
                            fieldAttribute => fieldAttribute.Info.Name);
                    var str = new StringBuilder();
                    if (table.IsGenericType)
                        str.Append("[");
                    while (read.Read())
                    {
                        str.Append("{");
                        for (var i = 0; i < read.FieldCount; i++)
                        {
                            if (read[i] is string)
                            {
                                if (!read[i].ToString().Contains("="))
                                {
                                    field = read.GetName(i).ToLower();
                                    if (dictionaryField.TryGetValue(field, out field))
                                        str.Append("\"" + dictionaryField[read.GetName(i).ToLower()] + "\":\"" + read[i] +
                                                   "\",");
                                }
                                else
                                {
                                    try
                                    {
                                        var firstOrDefault = tableItem.Fields.FirstOrDefault(
                                            n =>
                                                n.FieldName.Equals(read.GetName(i)) &&
                                                n.Info.GetCustomAttributes(true).OfType<EncrypteAttribute>().Count() !=
                                                0);
                                        if (firstOrDefault != null)
                                        {
                                            var fieldEncrypte = firstOrDefault.Info;
                                            if (fieldEncrypte != null)
                                            {
                                                var encrypteAttribute =
                                                    fieldEncrypte.GetCustomAttributes(true).OfType<EncrypteAttribute>().
                                                        FirstOrDefault();
                                                var descrypte = read[i].ToString();
                                                var password = "";
                                                if (encrypteAttribute != null)
                                                {
                                                    foreach (var relation in encrypteAttribute.RelationProperty)
                                                    {
                                                        password += read[relation];
                                                    }

                                                    //password = encrypteAttribute.RelationProperty.Aggregate(password, (current, relation) => ((Previous[relation] != null) ? current + Previous[relation] : current + Fields.FirstOrDefault(n => n.FieldName.Equals(relation)).Info.GetValue(this, null).ToString()));
                                                    descrypte = descrypte.Decrypt(encrypteAttribute.Key + password);
                                                    field = read.GetName(i).ToLower();
                                                    if (dictionaryField.TryGetValue(field, out field))
                                                        str.Append("\"" + dictionaryField[read.GetName(i).ToLower()] +
                                                                   "\":\"" + descrypte + "\",");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            field = read.GetName(i).ToLower();
                                            if (dictionaryField.TryGetValue(field, out field))
                                                str.Append("\"" + dictionaryField[read.GetName(i).ToLower()] +
                                                           "\":***\"" + read[i] + "\"***,");
                                        }
                                    }
                                    catch (Exception exception)
                                    {
                                        OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                                        field = read.GetName(i).ToLower();
                                        if (dictionaryField.TryGetValue(field, out field))
                                            str.Append("\"" + dictionaryField[read.GetName(i).ToLower()] + "\":\"" +
                                                       read[i].ToString().Replace("\"", "\\\"") + "\",");
                                    }
                                }
                            }
                            else if (read[i] is bool)
                            {
                                field = read.GetName(i).ToLower();
                                if (dictionaryField.TryGetValue(field, out field))
                                    str.Append("\"" + dictionaryField[read.GetName(i).ToLower()] + "\":" +
                                               read[i].ToString().ToLower() + ",");
                            }
                            else if (read[i] is DateTime)
                            {
                                field = read.GetName(i).ToLower();
                                if (dictionaryField.TryGetValue(field, out field))
                                    str.Append("\"" + dictionaryField[read.GetName(i).ToLower()] + "\":\"" +
                                               ((DateTime?)ConnectionManager.ConverterToObject(read[i], null)).Value
                                                   .ToString("yyyy-MM-ddT00:00:00") + "\",");
                            }
                            else if (read[i] is DBNull)
                            {
                                field = read.GetName(i).ToLower();
                                if (dictionaryField.TryGetValue(read.GetName(i).ToLower(), out field))
                                    str.Append("\"" + dictionaryField[read.GetName(i).ToLower()] + "\":\"\",");
                            }
                            else
                            {
                                field = read.GetName(i).ToLower();
                                if (dictionaryField.TryGetValue(field, out field))
                                    str.Append("\"" + dictionaryField[read.GetName(i).ToLower()] + "\":\"" + read[i] +
                                               "\",");
                            }
                        }
                        str.Remove(str.Length - 1, 1);
                        str.Append("}");
                        if (table.IsGenericType)
                            str.Append(",");
                    }
                    if (table.IsGenericType)
                    {
                        if (!str.ToString().Equals("["))
                            str.Remove(str.ToString().Length - 1, 1);
                        str.Append("]");
                    }
                    if (string.IsNullOrEmpty(str.ToString()))
                        return "[]";
                    return str.ToString();
                }
            }
            //  return "[]";
        }

        #endregion Get Json

        #region Get One Data

        /// <summary>
        ///     The virtual row
        /// </summary>
        protected internal Dictionary<string, object> VirtualRow;

        /// <summary>
        ///     Adds the virtual row. digunkan untuk menyimpan data yang telah di load ke dalam memory
        /// </summary>
        /// <param name="key">berisikan key untuk indexing hasil di memory</param>
        /// <param name="item">berisikan data yang akan di simpan di memory</param>
        protected internal void AddVirtualRow(string key, TableItem item)
        {
            var data = VirtualRow[key]; // as List<LinkItem>;
            if (data is List<TableItem>)
            {
                (data as List<TableItem>).Add(item);
            }
            else if (data is TableItem)
            {
                VirtualRow[key] = data;
            }
        }

        /// <summary>
        ///     Gets the first row. digunakan untuk mengambil data pertama di dalam database
        /// </summary>
        /// <param name="entity">berisikan object yang akan di ambil</param>
        /// <param name="function">berisikan fungsi untuk memfilter data yang akan di ambil</param>
        /// <returns>System.Object.</returns>
        public object GetFirstRow(object entity, Expression<Func<object, bool>> function)
        {
            using (var conn = ConnectionManager.Connection)
            {
                try
                {
                    if (!Current.ConnectionString.Equals(ConnectionString))
                    {
                        Current = this;
                        FailedConnection = false;
                    }
                    if (FailedConnection) return null;
                    conn.Open();
                }
                catch (Exception exception)
                {
                    Current.FailedConnection = true;
                    FailedConnection = true;
                    OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                    OnError(new ErrorArgs(exception));
                }
                using (var command = ConnectionManager.Command)
                {
                    command.Connection = conn;
                    var table = entity.GetType().GetCustomAttributes(true).OfType<TableAttribute>().FirstOrDefault();
                    if (table == null) return null;
                    command.CommandText = "Select * from " + table.TabelName;
                    if (UseCaching)
                    {
                        object link;
                        if (VirtualRow.TryGetValue("Top One " + command.CommandText, out link))
                        {
                            return link;
                        }
                        if (!VirtualRow.TryGetValue("Top One " + command.CommandText, out link))
                        {
                            link = new TableItem();
                            VirtualRow.Add("Top One " + command.CommandText, link);
                        }
                    }
                    var read = command.ExecuteReader();
                    while (read.Read())
                    {
                        var item = Activator.CreateInstance(entity.GetType()) as TableItem;
                        item.IsNew = false;
                        item.Dictionarys = new CoreDictionary<string, object>();
                        item.Previous = new CoreDictionary<string, object>();
                        for (var i = 0; i < read.FieldCount; i++)
                        {
                            if (read[i] is DBNull)
                            {
                                item.Dictionarys.Add(read.GetName(i).ToLower(), null);
                                item.Previous.Add(read.GetName(i).ToLower(), null);
                            }
                            else
                            {
                                item.Dictionarys.Add(read.GetName(i).ToLower(),
                                    ConnectionManager.ConverterToObject(read[i], item, read.GetName(i)));
                                item.Previous.Add(read.GetName(i).ToLower(),
                                    ConnectionManager.ConverterToObject(read[i], item, read.GetName(i)));
                            }
                        }
                        if (UseCaching)
                            AddVirtualRow("Top One " + command.CommandText, item);
                        if (function == null)
                            return item;
                        if (function.Compile().Invoke(item))
                            return item;
                    }
                }
            }
            return null;
        }

        /// <summary>
        ///     Gets the first row. digunakan untuk mengambil data pertama di dalam database
        /// </summary>
        /// <param name="entity">berisikan object yang akan di ambil</param>
        /// <returns>System.Object.</returns>
        public object GetFirstRow(object entity)
        {
            return GetFirstRow(entity, null);
        }

        /// <summary>
        ///     Gets the first row. digunakan untuk mengambil data pertama di dalam database
        /// </summary>
        /// <typeparam name="TEntity">berisikan entity yang akan di ambil di dalam database</typeparam>
        /// <returns>``0.</returns>
        public TEntity GetFirstRow<TEntity>()
            where TEntity : TableItem
        {
            return GetFirstRow<TEntity>(null);
        }

        /// <summary>
        ///     Gets the first row. digunakan untuk mengambil data pertama di dalam database
        /// </summary>
        /// <typeparam name="TEntity">berisikan entity yang akan di ambil di dalam database</typeparam>
        /// <param name="function">berisikan fungsi untuk memfilter data yang akan di ambil</param>
        /// <returns>``0.</returns>
        public TEntity GetFirstRow<TEntity>(Func<TEntity, bool> function)
            where TEntity : TableItem
        {
            using (var conn = ConnectionManager.Connection)
            {
                try
                {
                    if (!Current.ConnectionString.Equals(ConnectionString))
                    {
                        Current = this;
                        FailedConnection = false;
                    }
                    if (FailedConnection) return default(TEntity);
                    conn.Open();
                }
                catch (Exception exception)
                {
                    Current.FailedConnection = true;
                    FailedConnection = true;
                    OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                    OnError(new ErrorArgs(exception));
                }
                using (var command = ConnectionManager.Command)
                {
                    command.Connection = conn;
                    var table =
                        Activator.CreateInstance<TEntity>().GetType().GetCustomAttributes(true).OfType<TableAttribute>()
                            .FirstOrDefault();
                    if (table == null) return null;
                    command.CommandText = "Select * from " + table.TabelName;
                    OnStreamingLog(new ItemEventArgs<string>(command.CommandText));
                    if (UseCaching)
                    {
                        object link;
                        if (VirtualRow.TryGetValue("Top One " + command.CommandText, out link))
                        {
                            return link as TEntity;
                        }
                        if (!VirtualRow.TryGetValue("Top One " + command.CommandText, out link))
                        {
                            link = new TableItem();
                            VirtualRow.Add("Top One " + command.CommandText, link);
                        }
                    }
                    var read = command.ExecuteReader();
                    while (read.Read())
                    {
                        var item = Activator.CreateInstance<TEntity>();
                        item.IsNew = false;

                        item.Dictionarys = new CoreDictionary<string, object>();
                        item.Previous = new CoreDictionary<string, object>();
                        for (var i = 0; i < read.FieldCount; i++)
                        {
                            if (read[i] is DBNull)
                            {
                                item.Dictionarys.Add(read.GetName(i).ToLower(), null);
                                item.Previous.Add(read.GetName(i).ToLower(), null);
                            }
                            else
                            {
                                item.Dictionarys.Add(read.GetName(i).ToLower(),
                                    ConnectionManager.ConverterToObject(read[i], item, read.GetName(i)));
                                item.Previous.Add(read.GetName(i).ToLower(),
                                    ConnectionManager.ConverterToObject(read[i], item, read.GetName(i)));
                            }
                        }

                        if (UseCaching)
                            AddVirtualRow("Top One " + command.CommandText, item);
                        item.Manager = this;
                        if (function == null)
                            return item;
                        if (function.Invoke(item))
                            return item;
                    }
                }
            }
            return null;
        }

        public object GetOneDataByFieldPrimary(object linkItem)
        {
            var query = "";
            var tableItem = linkItem as TableItem;
            if (tableItem != null)
                foreach (var primaryKey in tableItem.PrimaryKeys)
                {
                    if (tableItem[primaryKey] != null) // <<== Di Tambahkan 8/5/2014
                    {
                        if (tableItem[primaryKey].GetType() == typeof(DateTime))
                        {
                            query += " " + primaryKey + "='" + ((DateTime)tableItem[primaryKey]).ToString(
                                ConnectionManager.FormateDate) + "' AND ";
                        }
                        else
                            query += " " + primaryKey + "='" + tableItem[primaryKey] + "' AND ";
                    }
                }
            query = query.Substring(0, query.Length - 4);
            return GetOneData(linkItem, query);
        }

        /// <summary>
        ///     Gets the one data. digunakan untuk mengambil satu data di database
        /// </summary>
        /// <param name="linkItem">berisikan object yang akan di ambil dari database</param>
        /// <param name="query">berisikan query filter berupa sintak sql</param>
        /// <returns>System.Object.</returns>
        public object GetOneData(object linkItem, string query)
        {
            try
            {
                // if (!OfflineMode)
                using (var conn = ConnectionManager.Connection)
                {
                    try
                    {
                        if (!Current.ConnectionString.Equals(ConnectionString))
                        {
                            Current = this;
                            FailedConnection = false;
                        }
                        if (FailedConnection)
                        {
                            Debug.Print("==>>>> Connection Close / Failed! <<<<==");
                            return null;
                        }
                        conn.Open();
                    }
                    catch (Exception exception)
                    {
                        Current.FailedConnection = true;
                        FailedConnection = true;
                        OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                        OnError(new ErrorArgs(exception));
                    }
                    using (var command = ConnectionManager.Command)
                    {
                        command.Connection = conn;

                        var model = linkItem as BaseItem;
                        if (model == null)
                            return null;
                        if (string.IsNullOrEmpty(model.GetTableName()))
                            return null;
                        command.CommandText = ConnectionManager.ConvertToQuery(model.GetTableName(), query);
                        command.CommandText = command.CommandText.Replace("=''", " is null ");
                        OnStreamingLog(new ItemEventArgs<string>(command.CommandText));
                        var read = command.ExecuteReader();
                        while (read.Read())
                        {
                            var item = Activator.CreateInstance(model.GetType());
                            if (!(item is BaseItem))
                                return null;
                            (item as BaseItem).IsNew = false;
                            (item as BaseItem).Manager = this;
                            (item as BaseItem).Dictionarys = new CoreDictionary<string, object>();
                            (item as BaseItem).Previous = new CoreDictionary<string, object>();
                            for (var i = 0; i < read.FieldCount; i++)
                            {
                                if (read[i] is DBNull)
                                {
                                    (item as BaseItem).Dictionarys.Add(read.GetName(i).ToLower(), null);
                                    (item as BaseItem).Previous.Add(read.GetName(i).ToLower(), null);
                                }
                                else
                                {
                                    (item as BaseItem).Dictionarys.Add(read.GetName(i).ToLower(),
                                        ConnectionManager.ConverterToObject(read[i], item, read.GetName(i)));
                                    (item as BaseItem).Previous.Add(read.GetName(i).ToLower(),
                                        ConnectionManager.ConverterToObject(read[i], item, read.GetName(i)));
                                }
                            }
                            conn.Close();
                            return item;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                OfflineMode = true;
                Debug.Write(exception);
            }
            return null;
        }

        /// <summary>
        ///     Gets the row. digunakan untuk mengambil data pada database
        /// </summary>
        /// <param name="entity">berisikan entity yang akan di ambil di dalam database</param>
        /// <returns>IEnumerable{System.Object}.</returns>
        public IEnumerable<TableItem> GetRow(object entity)
        {
            return GetRow(entity, RowType.Recursive);
        }

        /// <summary>
        ///     Gets the row. digunakan untuk mengambil data pada database
        /// </summary>
        /// <param name="entity">berisikan entity yang akan di ambil di dalam database</param>
        /// <param name="type">The type.</param>
        /// <returns>IEnumerable{System.Object}.</returns>
        public IEnumerable<TableItem> GetRow(object entity, RowType type)
        {
            using (var conn = ConnectionManager.Connection)
            {
                try
                {
                    if (!Current.ConnectionString.Equals(ConnectionString))
                    {
                        Current = this;
                        FailedConnection = false;
                    }
                    if (FailedConnection) yield break;
                    conn.Open();
                }
                catch (SqlException exception)
                {
                    Log.Error(exception);
                    Current.FailedConnection = true;
                    FailedConnection = true;
                }
                catch (Exception exception)
                {
                    Current.FailedConnection = true;
                    FailedConnection = true;
                    OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                    OnError(new ErrorArgs(exception));
                }
                using (var command = ConnectionManager.Command)
                {
                    command.Connection = conn;
                    if (entity is string)
                    {
                        command.CommandText = "Select * from " + entity;
                    }
                    else
                    {
                        var table = entity.GetType().GetCustomAttributes(true).OfType<TableAttribute>().FirstOrDefault();
                        if (table == null) yield break;
                        command.CommandText = "Select * from " + table.TabelName;
                    }
                    if (UseCaching)
                    {
                        object result;
                        IEnumerable<TableItem> link;
                        if (VirtualRow.TryGetValue(command.CommandText, out result))
                        {
                            if (result is IEnumerable<TableItem>)
                            {
                                link = result as IEnumerable<TableItem>;

                                foreach (var linkItem in link)
                                {
                                    yield return linkItem;
                                }
                                yield break;
                            }
                        }
                        if (!VirtualRow.TryGetValue(command.CommandText, out result))
                        {
                            link = new List<TableItem>();
                            VirtualRow.Add(command.CommandText, link);
                        }
                    }

                    var read = command.ExecuteReader();
                    var listTemp = new List<TableItem>();
                    while (read.Read())
                    {
                        var item = new TableItem();
                        if (entity is string)
                        {
                            var info = new FileInfo(Assembly.GetExecutingAssembly().Location);
                            var pathDirectory = Directory.GetCurrentDirectory();
                            foreach (var pathLibrary in Directory.GetFiles(pathDirectory).Where(n => n.EndsWith(".dll"))
                                )
                            {
                                var mustBreak = false;
                                var asm = Assembly.LoadFrom(pathLibrary);
                                foreach (var t in asm.GetExportedTypes())
                                {
                                    if (t.BaseType == typeof(TableItem))
                                    {
                                        var newTable = Activator.CreateInstance(t) as TableItem;
                                        if (newTable.TableName.Equals(entity))
                                        {
                                            item = newTable;
                                            mustBreak = true;
                                            break;
                                        }
                                    }
                                }
                                if (mustBreak)
                                    break;
                            }
                        }
                        else
                        {
                            item = Activator.CreateInstance(entity.GetType()) as TableItem;
                        }
                        if (item == null)
                            yield break;
                        item.IsNew = false;
                        item.Type = type;
                        item.Manager = this;
                        item.Dictionarys = new CoreDictionary<string, object>();
                        item.Previous = new CoreDictionary<string, object>();
                        for (var i = 0; i < read.FieldCount; i++)
                        {
                            try
                            {
                                if (read[i] is DBNull)
                                {
                                    item.Dictionarys.Add(read.GetName(i).ToLower(), null);
                                    item.Previous.Add(read.GetName(i).ToLower(), null);
                                }
                                else
                                {
                                    item.Dictionarys.Add(read.GetName(i).ToLower(),
                                        ConnectionManager.ConverterToObject(read[i], item, read.GetName(i)));
                                    item.Previous.Add(read.GetName(i).ToLower(),
                                        ConnectionManager.ConverterToObject(read[i], item, read.GetName(i)));
                                }
                            }
                            catch (Exception exception)
                            {
                                OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                            }
                        }
                        if (UseCaching)
                            AddVirtualRow(command.CommandText, item);
                        yield return item;
                        //listTemp.Add(item);
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the row. digunakan untuk mengambil data pada database
        /// </summary>
        /// <typeparam name="TEntity">berisikan entity yang akan di ambil di dalam database</typeparam>
        /// <param name="type">berisikan type entity yang akan di ambil di dalam database</param>
        /// <returns>CoreQueryable{``0}.</returns>
        public CoreQueryable<TEntity> GetRow<TEntity>(RowType type)
            where TEntity : TableItem
        {
            return new CoreQueryable<TEntity>(GetRow(Activator.CreateInstance<TEntity>(), type).OfType<TEntity>());
        }

        /// <summary>
        ///     Gets the row. digunakan untuk mengambil data pada database
        /// </summary>
        /// <typeparam name="TEntity">berisikan entity yang akan di ambil di dalam database</typeparam>
        /// <returns>IEnumerable{``0}.</returns>
        public IEnumerable<TEntity> GetRow<TEntity>()
        {
            return (GetRow(Activator.CreateInstance<TEntity>()).Cast<TEntity>());
        }

        /// <summary>
        ///     Gets the row. digunakan untuk mengambil data pada database
        /// </summary>
        /// <param name="linkItem">berisikan object yang akan di ambil dari database</param>
        /// <param name="query">berisikan query filter berupa sintak sql</param>
        /// <returns>CoreQueryable{System.Object}.</returns>
        internal IEnumerable<TableItem> GetRow(object linkItem, string query)
        {
            using (var conn = ConnectionManager.Connection)
            {
                try
                {
                    if (!Current.ConnectionString.Equals(ConnectionString))
                    {
                        Current = this;
                        FailedConnection = false;
                    }
                    if (FailedConnection) yield break;
                    conn.Open();
                }
                catch (Exception exception)
                {
                    OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                    OnError(new ErrorArgs(exception));
                }
                using (var command = ConnectionManager.Command)
                {
                    command.Connection = conn;
                    var model = linkItem as TableItem;
                    command.CommandText = "Select * from " + model.TableName + " Where " + query;
                    var read = command.ExecuteReader();
                    var listTemp = new List<TableItem>();
                    while (read.Read())
                    {
                        var item = Activator.CreateInstance(model.GetType());
                        if (!(item is TableItem))
                            yield break;
                        (item as TableItem).Skip = true;
                        (item as TableItem).IsNew = false;
                        (item as TableItem).Manager = this;
                        (item as TableItem).Dictionarys = new CoreDictionary<string, object>();
                        (item as TableItem).Previous = new CoreDictionary<string, object>();
                        for (var i = 0; i < read.FieldCount; i++)
                        {
                            if (read[i] is DBNull)
                            {
                                (item as TableItem).Dictionarys.Add(read.GetName(i).ToLower(), null);
                                (item as TableItem).Previous.Add(read.GetName(i).ToLower(), null);
                            }
                            else
                            {
                                (item as TableItem).Dictionarys.Add(read.GetName(i).ToLower(),
                                    ConnectionManager.ConverterToObject(read[i], item, read.GetName(i)));
                                (item as TableItem).Previous.Add(read.GetName(i).ToLower(),
                                    ConnectionManager.ConverterToObject(read[i], item, read.GetName(i)));
                            }
                        }
                        yield return (TableItem)item;
                        //listTemp.Add(item);
                    }
                    //return new CoreQueryable<TableItem>(new EnumerableQuery<TableItem>(listTemp));
                }
            }
            //return null;
        }

        #endregion Get One Data

        public bool CheckConnection()
        {
            try
            {
                return ExecuteQuery(ConnectionManager.CheckConnection()) != null;
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                return false;
            }
        }

        public void ClearTable(Type clearData)
        {
            try
            {
                var firstOrDefault = clearData.GetCustomAttributes(true).OfType<TableAttribute>().FirstOrDefault();
                if (firstOrDefault != null)
                    CurrentSql = "delete from " + firstOrDefault.TabelName;
                var conn = ConnectionManager.Connection;
                listDbConnectionCreateByRuntime.Add(conn);
                try
                {
                    if (!Current.ConnectionString.Equals(ConnectionString))
                    {
                        Current = this;
                        FailedConnection = false;
                    }
                    if (FailedConnection) return;
                    conn.Open();
                }
                catch (Exception exception)
                {
                    Current.FailedConnection = true;
                    FailedConnection = true;
                    OnStreamingLog(new ItemEventArgs<string>(exception.ToString()));
                    OnError(new ErrorArgs(exception));
                }
                var command = ConnectionManager.Command;
                command.Connection = conn;
                command.CommandText = CurrentSql;
                OnStreamingLog(new ItemEventArgs<string>(CurrentSql));
                command.ExecuteNonQuery();
                conn.Close();
            }
            finally
            {

                var log = BaseDependency.Get<ILogRepository>();
                if (log != null)
                    log.Info(CurrentSql);
            }
        }

        private static void OnStreamingLogSingleTone(ItemEventArgs<string> e)
        {
            if (StreamingLogSingleTone != null) StreamingLogSingleTone.Invoke(null, e);
        }
    }
}