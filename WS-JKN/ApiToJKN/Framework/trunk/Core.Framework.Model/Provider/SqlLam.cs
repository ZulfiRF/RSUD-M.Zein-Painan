using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Core.Framework.Model.Provider.Builder;
using Core.Framework.Model.Provider.Resolver;
using Core.Framework.Model.Provider.ValueObjects;

namespace Core.Framework.Model.Provider
{
    public class SqlLam<T> : SqlLamBase
    {
        public SqlLam()
        {
            //   base._builder = new SqlQueryBuilder(LambdaResolver.GetTableName<T>(), _defaultAdapter);
            _resolver = new LambdaResolver(_builder);
        }

        public SqlLam(Expression<Func<T, bool>> expression) : this()
        {
            Where(expression);
        }

        internal SqlLam(SqlQueryBuilder builder, LambdaResolver resolver)
        {
            _builder = builder;
            _resolver = resolver;
        }

        public SqlLam<T> And(Expression<Func<T, bool>> expression)
        {
            _builder.And();
            _resolver.ResolveQuery(expression);
            return this;
        }

        public SqlLam<T> GroupBy(Expression<Func<T, object>> expression)
        {
            _resolver.GroupBy(expression);
            return this;
        }

        public SqlLam<T2> Join<T2>(Expression<Func<T, T2, bool>> expression)
        {
            var lam = new SqlLam<T2>(_builder, _resolver);
            _resolver.Join(expression);
            return lam;
        }

        public SqlLam<TResult> Join<T2, TKey, TResult>(SqlLam<T2> joinQuery,
            Expression<Func<T, TKey>> primaryKeySelector, Expression<Func<T, TKey>> foreignKeySelector,
            Func<T, T2, TResult> selection)
        {
            var lam = new SqlLam<TResult>(_builder, _resolver);
            _resolver.Join<T, T2, TKey>(primaryKeySelector, foreignKeySelector);
            return lam;
        }

        public SqlLam<T> Or(Expression<Func<T, bool>> expression)
        {
            _builder.Or();
            _resolver.ResolveQuery(expression);
            return this;
        }

        public SqlLam<T> OrderBy(Expression<Func<T, object>> expression)
        {
            _resolver.OrderBy(expression, false);
            return this;
        }

        public SqlLam<T> OrderByDescending(Expression<Func<T, object>> expression)
        {
            _resolver.OrderBy(expression, true);
            return this;
        }

        public SqlLam<T> Select(params Expression<Func<T, object>>[] expressions)
        {
            foreach (var expression in expressions)
            {
                _resolver.Select(expression);
            }
            return this;
        }

        public SqlLam<T> SelectAverage(Expression<Func<T, object>> expression)
        {
            _resolver.SelectWithFunction(expression, SelectFunction.AVG);
            return this;
        }

        public SqlLam<T> SelectCount(Expression<Func<T, object>> expression)
        {
            _resolver.SelectWithFunction(expression, SelectFunction.COUNT);
            return this;
        }

        public SqlLam<T> SelectDistinct(Expression<Func<T, object>> expression)
        {
            _resolver.SelectWithFunction(expression, SelectFunction.DISTINCT);
            return this;
        }

        public SqlLam<T> SelectMax(Expression<Func<T, object>> expression)
        {
            _resolver.SelectWithFunction(expression, SelectFunction.MAX);
            return this;
        }

        public SqlLam<T> SelectMin(Expression<Func<T, object>> expression)
        {
            _resolver.SelectWithFunction(expression, SelectFunction.MIN);
            return this;
        }

        public SqlLam<T> SelectSum(Expression<Func<T, object>> expression)
        {
            _resolver.SelectWithFunction(expression, SelectFunction.SUM);
            return this;
        }

        public SqlLam<T> Where(Expression<Func<T, bool>> expression)
        {
            return And(expression);
        }

        public SqlLam<T> WhereIsIn(Expression<Func<T, object>> expression, SqlLamBase sqlQuery)
        {
            _builder.And();
            _resolver.QueryByIsIn(expression, sqlQuery);
            return this;
        }

        public SqlLam<T> WhereIsIn(Expression<Func<T, object>> expression, IEnumerable<object> values)
        {
            _builder.And();
            _resolver.QueryByIsIn(expression, values);
            return this;
        }

        public SqlLam<T> WhereNotIn(Expression<Func<T, object>> expression, SqlLamBase sqlQuery)
        {
            _builder.And();
            _resolver.QueryByNotIn(expression, sqlQuery);
            return this;
        }

        public SqlLam<T> WhereNotIn(Expression<Func<T, object>> expression, IEnumerable<object> values)
        {
            _builder.And();
            _resolver.QueryByNotIn(expression, values);
            return this;
        }
    }
}