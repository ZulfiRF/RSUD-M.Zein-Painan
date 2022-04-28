using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Core.Framework.Helper;
using Core.Framework.Helper.Collections;
using Core.Framework.Model.Attr;
using Core.Framework.Model.Contract;
using Core.Framework.Model.Error;
using Core.Framework.Model.QueryBuilder.Clausa;

namespace Core.Framework.Model
{
    /// <summary>
    ///     Class CoreContext
    /// </summary>
    public class CoreContext
    {
        /// <summary>
        ///     The manager
        /// </summary>
        private ContextManager manager;

        /// <summary>
        ///     Gets or sets the manager.
        /// </summary>
        /// <value>The manager.</value>
        protected internal ContextManager Manager
        {
            get { return manager; }
            set
            {
                if (manager == null)
                    manager = value;
            }
        }

        /// <summary>
        ///     Gets or sets the domain.
        /// </summary>
        /// <value>The domain.</value>
        protected internal object Domain { get; set; }
    }

    /// <summary>
    ///     Class CoreContext
    /// </summary>
    /// <typeparam name="TEntity">The type of the T entity.</typeparam>
    public class CoreContext<TEntity> : CoreContext, ICoreContext<TEntity>, IDisposable
        where TEntity : TableItem
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CoreContext{TEntity}" /> class.
        /// </summary>
        public CoreContext()
        {
            skip = -1;
            take = -1;
            top = -1;
            Domain = Activator.CreateInstance<TEntity>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CoreContext{TEntity}" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public CoreContext(ContextManager context)
            : this()
        {
            Manager = context;
        }

        /// <summary>
        ///     Gets or sets the type load.
        /// </summary>
        /// <value>The type load.</value>
        protected RowType TypeLoad { get; set; }

        public object Model { get; set; }
        public WhereClause[] whereClouses { get; set; }
        public event EventHandler<QueryArgs> BeforeExecuteQuery;

        /// <summary>
        ///     Counts the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.Int32.</returns>
        public virtual int Count(Expression<Func<TEntity, bool>> expression)
        {
            using (var conn = Manager.ConnectionManager.Connection)
            {
                conn.Open();
                using (var command = Manager.ConnectionManager.Command)
                {
                    command.Connection = conn;
                    var entity = Activator.CreateInstance<TEntity>();
                    var table = entity.GetType().GetCustomAttributes(true).OfType<TableAttribute>().FirstOrDefault();
                    if (table == null) return 0;
                    command.CommandText = "Select * from " + table.TabelName;
                    if (Manager.UseCaching)
                    {
                        object link;
                        if (Manager.VirtualRow.TryGetValue("Count All " + expression + " " + command.CommandText,
                            out link))
                        {
                            return Convert.ToInt16(link);
                        }
                        if (
                            !Manager.VirtualRow.TryGetValue("Count All " + expression + " " + command.CommandText,
                                out link))
                        {
                            link = new TableItem();
                            Manager.VirtualRow.Add("Count All  " + expression + " " + command.CommandText, link);
                        }
                    }
                    var read = command.ExecuteReader();
                    var count = 0;
                    while (read.Read())
                    {
                        if (expression == null)
                        {
                            count++;
                            continue;
                        }
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
                                item.Dictionarys.Add(read.GetName(i).ToLower(), read[i]);
                                item.Previous.Add(read.GetName(i).ToLower(), read[i]);
                            }
                        }

                        if (expression.Compile().Invoke((TEntity)item))
                            count++;
                    }
                    return count;
                }
            }
        }

        /// <summary>
        ///     Counts this instance.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public virtual int Count()
        {
            try
            {
                using (var conn = Manager.ConnectionManager.Connection)
                {
                    conn.Open();
                    using (var command = Manager.ConnectionManager.Command)
                    {
                        command.Connection = conn;
                        var entity = Activator.CreateInstance<TEntity>();
                        var table = entity.GetType().GetCustomAttributes(true).OfType<TableAttribute>().FirstOrDefault();
                        if (table == null) return 0;
                        command.CommandText = "Select * from " + table.TabelName;
                        if (Manager.UseCaching)
                        {
                            object link;
                            if (Manager.VirtualRow.TryGetValue("Count All " + command.CommandText, out link))
                            {
                                return Convert.ToInt16(link);
                            }
                            if (!Manager.VirtualRow.TryGetValue("Count All " + command.CommandText, out link))
                            {
                                link = new TableItem();
                                Manager.VirtualRow.Add("Count All  " + command.CommandText, link);
                            }
                        }
                        var read = command.ExecuteReader();
                        var count = 0;
                        while (read.Read())
                        {
                            count++;
                        }
                        return count;
                    }
                }
            }
            catch (Exception exception)
            {
                Manager.OnError(new ErrorArgs(exception));
            }
            return 0;
        }

        /// <summary>
        ///     digunakan untuk mengambil satu data dari database
        /// </summary>
        /// <returns>TEntity.</returns>
        /// <example>
        ///     contoh yang digunakan untuk mengambil satu data dari database
        ///     <code>
        ///  var context = new DomainContext(connectionString);
        ///  var oneData = context.ContactPeopleInImmediatelies.FirstOrDefault();
        /// </code>
        /// </example>
        public virtual TEntity FirstOrDefault()
        {
            return FirstOrDefault(null);
        }

        /// <summary>
        ///     Firsts the or default. digunakan untuk mengambil satu data dari database
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>TEntity.</returns>
        /// <example>
        ///     contoh yang digunakan untuk mengambil satu data dari database
        ///     <code>
        ///  var context = new DomainContext(connectionString);
        ///  var oneData = context.ContactPeopleInImmediatelies.FirstOrDefault(n => n.EmployeID.Equals("0001"));
        /// </code>
        /// </example>
        public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> expression)
        {
            try
            {
                using (var conn = Manager.ConnectionManager.Connection)
                {
                    conn.Open();
                    using (var command = Manager.ConnectionManager.Command)
                    {
                        command.Connection = conn;
                        var entity = Activator.CreateInstance<TEntity>();
                        var table = entity.GetType().GetCustomAttributes(true).OfType<TableAttribute>().FirstOrDefault();
                        if (table == null) return null;
                        command.CommandText = "Select * from " + table.TabelName;
                        if (Manager.UseCaching)
                        {
                            object link;
                            if (Manager.VirtualRow.TryGetValue("First " + command.CommandText, out link))
                            {
                                return (TEntity)link;
                            }
                            if (!Manager.VirtualRow.TryGetValue("First " + command.CommandText, out link))
                            {
                                link = new TableItem();
                                Manager.VirtualRow.Add("First " + command.CommandText, link);
                            }
                        }
                        var read = command.ExecuteReader();
                        while (read.Read())
                        {
                            var item = Activator.CreateInstance(entity.GetType()) as TableItem;
                            item.Manager = Manager;
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
                                    item.Dictionarys.Add(read.GetName(i).ToLower(), read[i]);
                                    item.Previous.Add(read.GetName(i).ToLower(), read[i]);
                                }
                            }
                            if (expression == null)
                            {
                                Manager.FreezeUpdate = false;
                                item.OnInit();
                                Manager.FreezeUpdate = true;
                                return (TEntity)item;
                            }
                            if (expression.Compile().Invoke((TEntity)item))
                            {
                                Manager.FreezeUpdate = false;
                                item.OnInit();
                                Manager.FreezeUpdate = true;
                                return (TEntity)item;
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Manager.OnError(new ErrorArgs(exception));
            }
            return null;
        }

        /// <summary>
        ///     Ases the pararell.
        /// </summary>
        /// <returns>IEnumerable{`0}.</returns>
        public IEnumerable<TEntity> AsPararell()
        {
            return null;
            //var listTask = new List<Task<IEnumerable<TEntity>>>();
            //int maxCore = 8;
            //for (int core = 0; core < maxCore; core++)
            //{
            //    var task = Task<IEnumerable<TEntity>>.Factory.StartNew(val =>
            //    {
            //        #region Load

            //        List<TEntity> list = new List<TEntity>();
            //        using (var conn = Manager.ConnectionManager.Connection)
            //        {
            //            conn.Open();
            //            using (var command = Manager.ConnectionManager.Command)
            //            {
            //                var entity = Activator.CreateInstance<TEntity>();
            //                command.Connection = conn;

            //                var table = entity.GetType().GetCustomAttributes(true).OfType<TableAttribute>().FirstOrDefault();
            //                if (table == null) return list;
            //                command.CommandText = "Select * from " + table.TabelName;
            //                if (Manager.UseCaching)
            //                {
            //                    object result;
            //                    IEnumerable<TableItem> link;
            //                    if (Manager.VirtualRow.TryGetValue(command.CommandText, out result))
            //                    {
            //                        if (result is IEnumerable<TableItem>)
            //                        {
            //                            link = result as IEnumerable<TableItem>;
            //                            return link.Cast<TEntity>();
            //                        }
            //                    }
            //                    if (!Manager.VirtualRow.TryGetValue(command.CommandText, out result))
            //                    {
            //                        link = new List<TableItem>();
            //                        Manager.VirtualRow.Add(command.CommandText, link);
            //                    }
            //                }
            //                var read = command.ExecuteReader();
            //                int hasDownload = 0, hasTake = 0;
            //                while (read.Read())
            //                {
            //                    hasDownload++;
            //                    if (skip != -1)
            //                    {
            //                        if (skip > hasDownload) continue;
            //                    }
            //                    if (hasDownload % maxCore != (int)val) continue;
            //                    var item = new TableItem();
            //                    item = Activator.CreateInstance(entity.GetType()) as TableItem;
            //                    if (item == null)
            //                        continue;
            //                    item.IsNew = false;
            //                    item.Type = TypeLoad;
            //                    item.Manager = Manager;
            //                    item.Dictionarys = new CoreDictionary<string, object>();
            //                    item.Previous = new CoreDictionary<string, object>();
            //                    for (int i = 0; i < read.FieldCount; i++)
            //                    {
            //                        item.Dictionarys.Add(read.GetName(i).ToLower(), read[i]);
            //                        item.Previous.Add(read.GetName(i).ToLower(), read[i]);
            //                    }

            //                    list.Add((TEntity)item);
            //                    if (take != -1)
            //                    {
            //                        hasTake++;

            //                        if (take == hasTake) break;
            //                    }
            //                }
            //            }
            //        }
            //        return list;

            //        #endregion Load
            //    }, core);
            //    listTask.Add(task);
            //}
            //int count = 0;
            //List<Task> hasComplate = new List<Task>();
            //while (true)
            //{
            //    foreach (var task in listTask)
            //    {
            //        if (task.IsCompleted && hasComplate.FirstOrDefault(n => n == task) == null)
            //        {
            //            count++;
            //            hasComplate.Add(task);
            //            foreach (var entity in task.Result)
            //            {
            //                yield return entity;
            //            }
            //        }
            //    }
            //    if (count == listTask.Count)
            //        break;
            //}
        }

        /// <summary>
        ///     Renders this instance. digunakan untuk merender hasil dari data yang akan di munculkan
        /// </summary>
        /// <returns>IEnumerable{TEntity}.</returns>
        /// <example>
        ///     contoh yang digunakan untuk mengambil data dari database
        ///     <code>
        ///  var context = new DomainContext(connectionString);
        ///  var oneData = context.ContactPeopleInImmediatelies.Render();
        /// </code>
        ///     ///
        /// </example>
        public virtual IEnumerable<TEntity> Render()
        {
            return RenderData().Cast<TEntity>();
        }

        public virtual IEnumerable RenderData()
        {
            using (var conn = Manager.ConnectionManager.Connection)
            {
                conn.Open();
                using (var command = Manager.ConnectionManager.Command)
                {
                    var entity = Model ?? Activator.CreateInstance<TEntity>();
                    command.Connection = conn;

                    var table = entity.GetType().GetCustomAttributes(true).OfType<TableAttribute>().FirstOrDefault();
                    if (table == null) return null;
                    var query = "";
                    var tableName = table.TabelName + ",";
                    var sqlLeftJoin = "";
                    var clause = new List<WhereClause>();
                    WhereClause itemClause = null;
                    if (whereClouses != null)
                    {
                        clause.AddRange(whereClouses);
                        itemClause = whereClouses.FirstOrDefault();
                    }
                    foreach (var result in entity.GetType().GetProperties())
                    {
                        var property = result.GetCustomAttributes(true).OfType<ReferenceAttribute>();
                        var aliasTable = "";
                        var aliasTableName = "";
                        foreach (var referenceAttribute in property.Where(n => !string.IsNullOrEmpty(n.TableName)))
                        {
                            if (referenceAttribute.TableName == table.TabelName)
                            {
                                aliasTable = referenceAttribute.TableName + " as " + referenceAttribute.TableName + referenceAttribute.TableName;
                                aliasTableName = referenceAttribute.TableName + referenceAttribute.TableName;
                            }
                            tableName += (!string.IsNullOrEmpty(aliasTable) ? aliasTable : referenceAttribute.TableName) + ",";
                            sqlLeftJoin +=
                                " LEFT OUTER JOIN " + (!string.IsNullOrEmpty(aliasTable) ? aliasTable : referenceAttribute.TableName) + " ON ";
                            foreach (var reference in referenceAttribute.ReferenecKey)
                            {
                                var arrQuery = reference.Split('=');
                                sqlLeftJoin += (!string.IsNullOrEmpty(aliasTableName) ? aliasTableName : referenceAttribute.TableName) + "." + arrQuery[0] + " = " +
                                               table.TabelName + "." + arrQuery[1] + " AND ";
                            }

                            if (!string.IsNullOrEmpty(sqlLeftJoin))
                                sqlLeftJoin = sqlLeftJoin.Substring(0, sqlLeftJoin.Length - 4);
                            var propertyClauseWhere = result.GetCustomAttributes(true).OfType<SearchAttribute>();
                            //if (itemClause != null)
                            //    foreach (var clausaWhere in propertyClauseWhere)
                            //    {
                            //        clause.Add(new WhereClause(clausaWhere.PropertyPath, Core.Framework.Model.QueryBuilder.Enums.Comparison.Like, itemClause.Value));

                            //    }
                        }
                    }

                    if (!string.IsNullOrEmpty(sqlLeftJoin) && sqlLeftJoin.EndsWith(" AND "))
                    {
                        sqlLeftJoin = sqlLeftJoin.Substring(0, sqlLeftJoin.Length - 4);
                        whereClouses = clause.ToArray();
                    }

                    if (string.IsNullOrEmpty(query))
                        command.CommandText = "Select * from " + table.TabelName + " " + sqlLeftJoin;

                    if (whereClouses != null)
                    {
                        var resultWhereQuery = Manager.ConnectionManager.CreateFilterRow(whereClouses);
                        if (!string.IsNullOrEmpty(resultWhereQuery))
                            command.CommandText += " Where " + resultWhereQuery;
                    }
                    if (Manager.UseCaching)
                    {
                        object result;
                        IEnumerable<TableItem> link;
                        if (Manager.VirtualRow.TryGetValue(command.CommandText, out result))
                        {
                            if (result is IEnumerable<TableItem>)
                            {
                                link = result as IEnumerable<TableItem>;
                                return link.Cast<TEntity>();
                            }
                        }
                        if (!Manager.VirtualRow.TryGetValue(command.CommandText, out result))
                        {
                            link = new List<TableItem>();
                            Manager.VirtualRow.Add(command.CommandText, link);
                        }
                    }
                    var args = new QueryArgs(command.CommandText);
                    OnBeforeExecuteQuery(args);
                    command.CommandText = args.Command;
                    var model = new AsycnModel(Manager.ExecuteQueryAsync(command.CommandText));
                    if (Model != null) model.TypeModel = Model.GetType();
                    return model;
                }
            }
        }

        /// <summary>
        ///     Renders the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IEnumerable{`0}.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual IEnumerable<TEntity> Render(IQueryBuilder query)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Skips the specified value. digunakan untuk melewati beberapa row dalam database
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>ICoreContext{TEntity}.</returns>
        /// <example>
        ///     contoh yang digunakan untuk melewati data dari database
        ///     <code>
        ///  var context = new DomainContext(connectionString);
        ///  var oneData = context.ContactPeopleInImmediatelies.Skip(5);
        /// </code>
        /// </example>
        public virtual ICoreContext<TEntity> Skip(int value)
        {
            skip = value;
            return this;
        }

        /// <summary>
        ///     Takes the specified value.digunakan untuk hanya mengambil beberapa row dalam database
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>ICoreContext TEntity .</returns>
        /// <example>
        ///     contoh yang digunakan untuk hanya mengambil beberapa data dari database
        ///     <code>
        ///  var context = new DomainContext(connectionString);
        ///  var oneData = context.ContactPeopleInImmediatelies.Take(5).Render();
        /// </code>
        /// </example>
        public virtual ICoreContext<TEntity> Take(int value)
        {
            take = value;
            return this;
        }

        /// <summary>
        ///     Tops the specified value. digunakan untuk hanya mengambil top data dari database
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>ICoreContext{TEntity}.</returns>
        /// <example>
        ///     contoh yang digunakan untuk hanya mengambil top data dari database
        ///     <code>
        ///  var context = new DomainContext(connectionString);
        ///  var oneData = context.ContactPeopleInImmediatelies.Top(5).Render();
        /// </code>
        /// </example>
        public virtual ICoreContext<TEntity> Top(int value)
        {
            top = value;
            return this;
        }

        /// <summary>
        ///     Wheres the specified expression. digunakan untuk memfilter  data dari database
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>ICoreContext{TEntity}.</returns>
        /// <example>
        ///     contoh yang digunakan untuk memfilter  data dari database
        ///     <code>
        ///  var context = new DomainContext(connectionString);
        ///  var oneData = context.ContactPeopleInImmediatelies.Where(n => n.EmployeID.Equals("0001"));
        /// </code>
        /// </example>
        public virtual ICoreContext<TEntity> Where(Expression<Func<TEntity, bool>> expression)
        {
            whereExpression = expression;
            odataFilter = null;
            whereClouses = null;
            return this;
        }

        /// <summary>
        ///     Wheres the specified filter.
        /// </summary>
        /// <param name="functionToFilter"> </param>
        /// <param name="filter">The filter.</param>
        /// /// <param name="isOdata">The filter.</param>
        /// <returns>ICoreContext{`0}.</returns>
        public virtual ICoreContext<TEntity> Where(object functionToFilter, bool isOdata, params string[] filter)
        {
            if (functionToFilter is IEnumerable<WhereClause>)
            {
                odataFilter = null;
                whereExpression = null;
                whereClouses = functionToFilter as WhereClause[];
            }
            else
            {
                odataFilter = filter;
                whereExpression = null;
                whereClouses = null;
            }

            return this;
        }

        public ICoreContext<TEntity> Where(WhereClause[] query)
        {
            odataFilter = null;
            whereExpression = null;
            whereClouses = query;
            return this;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (Manager != null)
                Manager.Dispose();
        }

        public void OnBeforeExecuteQuery(QueryArgs e)
        {
            var handler = BeforeExecuteQuery;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        ///     Renders the specified expression.
        /// </summary>
        /// <typeparam name="TResult">The type of the T result.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>IEnumerable TEntity.</returns>
        /// <example>
        ///     contoh yang digunakan untuk mengambil data dari database
        ///     <code>
        ///  var context = new DomainContext(connectionString);
        ///  var oneData = context.ContactPeopleInImmediatelies.Render(n => n.EmployeID.Equals("0001"));
        /// </code>
        /// </example>
        public virtual IEnumerable<TResult> Render<TResult>(Expression<Func<TEntity, TResult>> expression)
            where TResult : TableItem
        {
            using (var conn = Manager.ConnectionManager.Connection)
            {
                conn.Open();
                using (var command = Manager.ConnectionManager.Command)
                {
                    var entity = Activator.CreateInstance<TEntity>();
                    command.Connection = conn;

                    var table = entity.GetType().GetCustomAttributes(true).OfType<TableAttribute>().FirstOrDefault();
                    if (table == null) return null;
                    command.CommandText = "Select * from " + table.TabelName;
                    if (Manager.UseCaching)
                    {
                        object result;
                        IEnumerable<TableItem> link;
                        if (Manager.VirtualRow.TryGetValue(command.CommandText, out result))
                        {
                            if (result is IEnumerable<TableItem>)
                            {
                                link = result as IEnumerable<TableItem>;
                                return link.OfType<TResult>();
                                //foreach (var linkItem in link)
                                //{
                                //    yield return expression.Compile().Invoke((TEntity)linkItem);
                                //}
                                //yield break;
                            }
                        }
                        if (!Manager.VirtualRow.TryGetValue(command.CommandText, out result))
                        {
                            link = new List<TableItem>();
                            Manager.VirtualRow.Add(command.CommandText, link);
                        }
                    }
                    var model = new AsycnModel<TResult>(Manager.ExecuteQueryAsync(command.CommandText));
                    return model;
                    //var read = command.ExecuteReader();
                    //int hasDownload = 0, hasTake = 0;
                    //while (read.Read())
                    //{
                    //    hasDownload++;
                    //    if (skip != -1)
                    //    {
                    //        if (skip > hasDownload) continue;
                    //    }

                    //    var item = new TableItem();
                    //    item = Activator.CreateInstance(entity.GetType()) as TableItem;
                    //    if (item == null)
                    //        yield break;
                    //    item.IsNew = false;
                    //    item.Type = TypeLoad;
                    //    item.Manager = Manager;
                    //    item.Dictionarys = new CoreDictionary<string, object>();
                    //    item.Previous = new CoreDictionary<string, object>();
                    //    for (int i = 0; i < read.FieldCount; i++)
                    //    {
                    //        if (read[i] is DBNull)
                    //        {
                    //            item.Dictionarys.Add(read.GetName(i).ToLower(), null);
                    //            item.Previous.Add(read.GetName(i).ToLower(), null);
                    //        }
                    //        else
                    //        {
                    //            item.Dictionarys.Add(read.GetName(i).ToLower(), read[i]);
                    //            item.Previous.Add(read.GetName(i).ToLower(), read[i]);
                    //        }
                    //    }
                    //    if (Manager.UseCaching)
                    //        Manager.AddVirtualRow(command.CommandText, item);
                    //    if (whereExpression != null)
                    //    {
                    //        if (whereExpression.Compile().Invoke((TEntity)item))
                    //            yield return expression.Compile().Invoke((TEntity)item);
                    //    }
                    //    else
                    //        yield return expression.Compile().Invoke((TEntity)item);
                    //    if (take != -1)
                    //    {
                    //        hasTake++;

                    //        if (take == hasTake) break;
                    //    }
                    //}
                }
            }
        }

        #region Variabel

        /// <summary>
        ///     The odata filter
        /// </summary>
        private string[] odataFilter;

        /// <summary>
        ///     The skip
        /// </summary>
        private int skip;

        /// <summary>
        ///     The take
        /// </summary>
        private int take;

        /// <summary>
        ///     The top
        /// </summary>
        private int top;

        /// <summary>
        ///     The where expression
        /// </summary>
        private Expression<Func<TEntity, bool>> whereExpression;

        #endregion Variabel
    }
}