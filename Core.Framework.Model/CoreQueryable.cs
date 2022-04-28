using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Core.Framework.Helper;
using Core.Framework.Helper.Collections;
using Core.Framework.Helper.Contracts;
using Core.Framework.Helper.Logging;
using Core.Framework.Model.Attr;
using Core.Framework.Model.Provider.Adapter;
using Core.Framework.Model.Provider.Builder;
using Core.Framework.Model.Provider.Resolver;
using Core.Framework.Model.Provider.ValueObjects;

namespace Core.Framework.Model
{
    public class CoreQueryable<T> : IQueryProvider, ICoreQueryable<T> where T : TableItem
        // : IQueryable<T>, IQueryProvider where T : TableItem
    {
        internal ISqlAdapter _defaultAdapter = new SqlServer2012Adapter();
        internal SqlQueryBuilder _builder;
        internal LambdaResolver _resolver;
        private readonly Dictionary<string, ChaceTemporary> anyDictionary = new Dictionary<string, ChaceTemporary>();
        private readonly IEnumerable<T> iEnumerable;
        private string tableDestination;

        private CoreQueryable()
        {
            var model = Activator.CreateInstance<T>();
            _builder = new SqlQueryBuilder(model.TableName, _defaultAdapter);
            var type = typeof(T);
            _resolver = new LambdaResolver(_builder);
            while (type != null)
            {
                if (!_resolver.TableNamesDictionary.ContainsKey(type.Name))
                    _resolver.TableNamesDictionary.Add(type.Name, model.TableName);
                var tableItem = Activator.CreateInstance(type) as TableItem;
                if (tableItem != null)
                    if (!_resolver.TableNamesDictionary.ContainsKey(tableItem.TableName))
                        _resolver.TableNamesDictionary.Add(tableItem.TableName, model.TableName);

                if (type == null)
                    break;
                type = type.BaseType;
            }

            _resolver.TablePrimary = model.TableName;
            _resolver.TypeTablePrimary = typeof(T);
        }

        public CoreQueryable(IEnumerable<T> iEnumerable)
        {
            this.iEnumerable = iEnumerable;
        }

        public CoreQueryable(ContextManager context, bool useCahe = true)
            : this()
        {
            UseCache = useCahe;
            if (context == null)
            {
                var contextManager = ContextManager.Current;
                ContextManager = contextManager;
            }
            else
                ContextManager = context;
        }

        public CoreQueryable(ContextManager manager, TableItem parent, params string[] param)
            : this(manager)
        {
            _builder.And();

            if (_resolver.TablePrimary != parent.TableName)
                _builder.JoinMultiple(_resolver.TablePrimary, parent.TableName, param);
            foreach (var s in param)
            {
                var temp = s.Split('=');
                if (temp.Length == 2)
                {
                    try
                    {
                        var firstOrDefault =
                            parent.GetType()
                                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                .FirstOrDefault(
                                    n =>
                                        n.GetCustomAttributes(true).OfType<FieldAttribute>().Count(
                                            x => x.FieldName.Equals(temp[1])) != 0);
                        if (firstOrDefault != null)
                        {
                            _builder.And();
                            if (firstOrDefault.GetValue(parent, null) is DateTime ||
                                firstOrDefault.GetValue(parent, null) is DateTime?)
                                _builder.QueryByField(parent.TableName, temp[0], "=",
                                    "" + _defaultAdapter.ConvertDateTime(firstOrDefault.GetValue(parent, null)) + "");
                            else
                            {
                                _builder.QueryByField(parent.TableName, temp[0], "=",
                                    "" + firstOrDefault.GetValue(parent, null) + "");
                            }
                        }
                        else
                        {
                            _builder.And();
                            _builder.QueryByField(parent.TableName, temp[0], "=", "" + temp[1] + "");
                        }
                    }
                    catch (Exception exception)
                    {
                        Log.Error(exception);
                    }
                }
            }
        }



        public ContextManager ContextManager { get; set; }
        public Expression Expression { get; set; }

        public Type ElementType
        {
            get { return typeof(T); }
        }

        public IQueryProvider Provider
        {
            get { return this; }
        }


        public bool UseCache { get; set; }
        public IEnumerator<T> GetEnumerator()
        {
            try
            {
                var enumerable = iEnumerable;
                if (enumerable == null)
                {
                    var sql = BuildQuery();
                    enumerable = ContextManager.ExecuteQuery<T>(sql, UseCache);
                }
                if (enumerable == null)
                    return null;
                return enumerable.GetEnumerator();
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }

        //public int Count()
        //{
        //    _resolver.SelectFunctionOnly<T>(SelectFunction.COUNT);
        //    var sql = BuildQuery();
        //    var reader = ContextManager.ExecuteQuery(sql);
        //    var tempList = new List<object>();
        //    while (reader.Read())
        //    {
        //        for (int i = 0; i < reader.FieldCount; i++)
        //        {
        //            tempList.Add(reader[i]);
        //        }

        //    }
        //    if (tempList.Count == 1)
        //        return tempList.Count;
        //    return tempList.Count;
        //}

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            throw new NotImplementedException();
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            throw new NotImplementedException();
        }

        public AsycnModel<T> Asycn()
        {
            var sql = BuildQuery();
            return new AsycnModel<T>(ContextManager.ExecuteQueryAsync(sql));
        }

        public object Max(Expression<Func<T, object>> expression)
        {
            try
            {
                _resolver.SelectWithFunction(expression, SelectFunction.MAX);
                var sql = BuildQuery();
                var reader = ContextManager.ExecuteQuery(sql);
                var tempList = new List<object>();
                if (reader == null) return null;
                while (reader.Read())
                {
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        tempList.Add(reader[i]);
                    }
                }
                if (tempList.Count == 1)
                    return tempList.FirstOrDefault();
                return tempList;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }

        public object Min(Expression<Func<T, object>> expression)
        {
            try
            {
                _resolver.SelectWithFunction(expression, SelectFunction.MAX);
                var sql = BuildQuery();
                var reader = ContextManager.ExecuteQuery(sql);
                var tempList = new List<object>();
                while (reader.Read())
                {
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        tempList.Add(reader[i]);
                    }
                }
                if (tempList.Count == 1)
                    return tempList.FirstOrDefault();
                return tempList;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }

        private string BuildQueryCount()
        {
            try
            {
                var localbuilder = _builder;
                var tableItem = Activator.CreateInstance<T>() as TableItem;

                var firstOrDefault = tableItem.Fields.FirstOrDefault(n => n.IsPrimary);
                if (firstOrDefault != null)
                    localbuilder.Select(tableItem.TableName, firstOrDefault.FieldName, SelectFunction.COUNT);
                var sql = _builder.Parameters.Aggregate(_builder.QueryString, (current, parameter) =>
                {
                    if (parameter.Value == null)
                        return current.Replace("@" + parameter.Key, "NULL");
                    if (parameter.Value.ToString().ToLower().Contains("convert"))
                    {
                        var temp = current.Replace("@" + parameter.Key, "" + parameter.Value + "");
                        return temp;
                    }
                    if (parameter.Value.ToString().StartsWith("(") &&
                        parameter.Value.ToString().EndsWith(")"))
                        return current.Replace("@" + parameter.Key, "" + parameter.Value + "");
                    return current.Replace("@" + parameter.Key, "'" + parameter.Value + "'");
                });

                return sql;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }


        public string BuildQuery()
        {
            try
            {
                var localParam = new Dictionary<string, object>(_builder.Parameters);
                if (_builder.SelectionList != null && _builder.SelectionList.Count == 0)
                {
                    var result = new FilterQuery()
                    {
                        Source = typeof(T),
                    };
                    CoreQueryHelper.OnFilterQueryEventHandler(this, new ItemEventArgs<FilterQuery>(result));
                    if (result.Query != null && result.Query.Count != 0)
                        _builder.SelectionList = result.Query;
                }
                var query = _builder.QueryString;

                if (IsDelete)
                    query = _builder.QueryDelete;
                else if (IsUpdate)
                    query = _builder.QueryUpdate;
                else if (IsInsertInto)
                {
                    query = _builder.QueryInsertInto(tableDestination) + query;
                }
                var sql = localParam.Aggregate(query, (current, parameter) =>
                {
                    if (parameter.Value == null)
                        return current.Replace("@" + parameter.Key, "NULL");
                    if (parameter.Value.ToString().ToLower().Contains("convert"))
                    {
                        var temp = current.Replace("@" + parameter.Key, "" + parameter.Value + "");
                        return temp;
                    }
                    if (parameter.Value.ToString().StartsWith("(") &&
                        parameter.Value.ToString().EndsWith(")"))
                        return current.Replace("@" + parameter.Key, "" + parameter.Value + "");
                    if (parameter.Value.ToString().Contains("and") && !parameter.Value.ToString().Contains("%"))
                        return current.Replace("@" + parameter.Key, "" + parameter.Value + "");
                    if (parameter.Value is Int16 || parameter.Value is Int32 || parameter.Value is Int64 || parameter.Value is double || parameter.Value is Single)
                        return current.Replace("@" + parameter.Key, "" + parameter.Value + "");
                    return current.Replace("@" + parameter.Key, "'" + parameter.Value + "'");
                });
                Debug.Print(sql);
                return sql;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }
        protected bool IsInsertInto { get; set; }

        protected bool IsUpdate { get; set; }

        public int Count()
        {
            var sql = BuildQueryCount();
            //var reader = ContextManager.ExecuteQuery(sql);
            //var tempList = new List<object>();
            var reader = ContextManager.ExecuteQueryAutomaticCloseConnection(sql);

            while (reader.Read())
            {
                return Convert.ToInt32(reader[0]);
            }
            return 0;
        }

        public int Count(Expression<Func<T, object>> expression)
        {
            try
            {
                _resolver.SelectWithFunction(expression, SelectFunction.COUNT);
                var sql = BuildQuery();
                _resolver.RemoveExpression(expression, SelectFunction.COUNT);
                //var reader = ContextManager.ExecuteQuery(sql);
                var reader = ContextManager.ExecuteQueryAutomaticCloseConnection(sql);
                while (reader.Read())
                {
                    try
                    {
                        return Convert.ToInt16(reader[0]);
                    }

                    catch (Exception exception)
                    {
                        Log.Error(exception);
                        return 0;
                    }
                    finally
                    {
                        reader.Close();
                    }

                    //for (int i = 0; i < reader.FieldCount; i++)
                    //{
                    //    tempList.Add(reader[i]);
                    //}
                }
                //reader.Dispose();
                reader.Close();
                return 0;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return 0;
            }
        }

        public CoreQueryable<T> Clone()
        {
            return (CoreQueryable<T>)MemberwiseClone();
        }

        //public IEnumerable<TResult> Select<TSource, TResult>(
        //    Func<TSource, TResult> selector)
        //{
        //    yield break;
        //}
        //public ICoreQueryable<TSource> Select<TSource>(Expression<Func<TSource, object>> expression) where TSource : class

        //{
        //    try
        //    {
        //        Expression = expression;

        //        _resolver.Select(expression);
        //        var sql = BuildQuery();
        //        var enumerable = ContextManager.ExecuteQuery(sql);
        //        var list = new List<TSource>();
        //        while (enumerable.Read())
        //        {
        //            var type = Activator.CreateInstance<TSource>();
        //            list.Add(type);
        //        }
        //        return (ICoreQueryable<TSource>)this;

        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error(e);

        //    }
        //    return (ICoreQueryable<TSource>)this;
        //}

        public ICoreQueryable<T> Select()
        {
            return this;
        }

        public IEnumerable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression)
        {
            try
            {
                Expression = expression;

                _resolver.Select(expression);
                var sql = BuildQuery();
                if (IsInsertInto)
                {
                    ContextManager.ExecuteNonQuery(sql);
                    return null;
                }
                else
                {
                    var enumerable = ContextManager.ExecuteQuery<T>(sql);
                    var list = new List<TResult>();
                    foreach (var item in enumerable)
                    {
                        var data = expression.Compile().Invoke(item);
                        list.Add(data);
                    }

                    return list;
                }

            }
            catch (Exception e)
            {
                Log.Error(e);

            }
            return null;
        }

        public void Execute()
        {
            var sql = BuildQuery();
            ContextManager.ExecuteNonQuery(sql);
        }

        public ICoreQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            try
            {
                if (_builder == null) return this;
                Expression = expression;
                _builder.And();
                _resolver.ResolveQuery(expression);

                return (ICoreQueryable<T>)this;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return this;
            }
        }

        public ICoreQueryable<T> Between(Expression<Func<T, int>> expression, string high, string low)
        {
            try
            {
                if (_builder == null) return this;
                Expression = expression;
                _builder.And();                
                _resolver.ResolveQuery(expression, high, low);
                return (ICoreQueryable<T>)this;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return this;
            }
        }

        public ICoreQueryable<T> OrderBy(Expression<Func<T, object>> expression)
        //public ICoreQueryable<T> OrderBy<T,TKey>(Func<T, TKey> keySelector)
        {
            try
            {

                if (_builder == null) return this;
                //_builder.OrderBy();
                _resolver.OrderBy(expression);
                return (ICoreQueryable<T>)this;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return this;
            }
        }

        public ICoreQueryable<T> OrderByDescending(Expression<Func<T, object>> expression)
        //public ICoreQueryable<T> OrderBy<T,TKey>(Func<T, TKey> keySelector)
        {
            try
            {

                if (_builder == null) return this;
                //_builder.OrderBy();
                _resolver.OrderBy(expression, true);
                return (ICoreQueryable<T>)this;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return this;
            }
        }

        public bool Delete(Expression<Func<T, bool>> expression)
        {
            try
            {
                if (_builder == null) return false;
                Expression = expression;
                _builder.And();
                _resolver.ResolveQuery(expression);
                IsDelete = true;
                var sql = BuildQuery();
                ContextManager.ExecuteQuery<T>(sql);
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return false;
            }
        }

        protected bool IsDelete { get; set; }

        public bool Delete()
        {
            try
            {
                ContextManager.ExecuteNonQuery("delete from " + (Activator.CreateInstance<T>() as TableItem).TableName);
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return false;
            }
        }

        public ICoreQueryable<T> ThenBy(Expression<Func<T, object>> expression)
        {
            try
            {

                if (_builder == null) return this;
                //_builder.OrderBy();
                _resolver.OrderBy(expression);
                return (ICoreQueryable<T>)this;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return this;
            }
        }

        public ICoreQueryable<T> InsertInto<TDestination>() where TDestination : class, ILoadModel
        {
            IsInsertInto = true;
            var tableItem = Activator.CreateInstance<TDestination>() as TableItem;
            if (tableItem != null)
                tableDestination = tableItem.TableName;
            return this;
        }

        public T FirstOrDefault()
        {
            try
            {
                if (_builder == null)
                {
                    if (iEnumerable != null)
                        return iEnumerable.FirstOrDefault();
                    return default(T);
                }
                //_builder.And();
                _builder.Top(1);
                var enumerable = iEnumerable;
                if (enumerable == null)
                {
                    var sql = BuildQuery();
                    enumerable = ContextManager.ExecuteQuery<T>(sql);
                }
                return enumerable.FirstOrDefault();
            }
            finally
            {
                if (_builder != null) _builder.Top(int.MaxValue);
            }
        }

        public T FirstOrDefault(Expression<Func<T, bool>> expression)
        {
            T model = null;
            string sql = "";
            try
            {

                if (_builder == null)
                {
                    if (iEnumerable != null)
                        return iEnumerable.FirstOrDefault(expression.Compile());
                    return default(T);
                }
                Expression = expression;
                _builder.And();

                _resolver.ResolveQuery(expression);
                _builder.Top(1);
                var enumerable = iEnumerable;

                if (enumerable == null)
                {
                    sql = BuildQuery();
                    if (UseCache)
                    {
                        try
                        {
                            var cache = CacheHelper.GetCache(sql);
                            if (cache != null && !cache.Equals(string.Empty))
                                return (T)cache;
                            else
                                enumerable = ContextManager.ExecuteQuery<T>(sql);
                        }
                        catch (Exception)
                        {
                            var cache = CacheHelper.GetCache(sql);
                            if (cache != null && !cache.Equals(string.Empty))
                                return (T)cache;
                        }
                    }
                    else
                        enumerable = ContextManager.ExecuteQuery<T>(sql, UseCache);
                }
                return model = enumerable.FirstOrDefault();
            }
            finally
            {
                CacheHelper.RegisterCache(sql, model, -1);
                if (_builder != null) _builder.Top(int.MaxValue);
            }
        }


        public bool Any(Expression<Func<T, bool>> expression)
        {
            try
            {
                Expression = expression;
                _builder.Exists();
                _builder.And();
                _resolver.ResolveQuery(expression);
                var sql = BuildQuery();
                var reader = ContextManager.ExecuteQueryAutomaticCloseConnection(sql);
                var valid = false;
                while (reader.Read())
                {
                    valid = (bool)reader[0];
                }
                reader.Close();
                reader.Dispose();

                return valid;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return false;
            }
        }

        public bool Any()
        {
            try
            {
                _builder.Exists();
                var sql = BuildQuery();


                var temp = new ChaceTemporary
                {
                    CreateDate = DateTime.Now
                };
                ChaceTemporary tempChace;
                if (anyDictionary.TryGetValue(sql, out tempChace))
                {
                    if (DateTime.Now - temp.CreateDate < TimeSpan.FromSeconds(3))
                    {
                        return tempChace.Result;
                    }
                    anyDictionary.Remove(sql);
                }
                var reader = ContextManager.ExecuteQueryAutomaticCloseConnection(sql);
                var valid = false;
                while (reader.Read())
                {
                    valid = (bool)reader[0];
                }
                reader.Close();
                reader.Dispose();
                if (!anyDictionary.TryGetValue(sql, out tempChace))
                {
                    temp.Result = valid;
                    anyDictionary.Add(sql, temp);
                }

                return valid;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return false;
            }
        }

        public ICoreQueryable<T> Top(int topData)
        {
            _builder.Top(topData);
            var sql = BuildQuery();
            var reader = ContextManager.ExecuteQuery<T>(sql);
            return new CoreQueryable<T>(reader);
        }

        private class ChaceTemporary
        {
            public bool Result { get; set; }
            public DateTime CreateDate { get; set; }
        }


        public ICoreQueryable<T> Update<TResult>(Expression<Func<T, TResult>> func, object source)
        {
            var body = func.Body;
            if (body.NodeType == ExpressionType.MemberAccess)
            {
                var fieldName = LambdaResolver.GetColumnName(func.Body);
                _builder.Update(fieldName, source.ToString());
            }
            IsUpdate = true;
            return this;
        }
    }

    public class CoreQueryHelper
    {
        public static event EventHandler<ItemEventArgs<FilterQuery>> FilterQueryEventHandler;

        public static void OnFilterQueryEventHandler(object obj, ItemEventArgs<FilterQuery> e)
        {
            if (FilterQueryEventHandler != null) FilterQueryEventHandler.Invoke(obj, e);
        }
        public static bool NotSysAdmin { get; set; }
    }
}