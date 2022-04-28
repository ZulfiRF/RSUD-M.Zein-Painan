using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Core.Framework.Helper.Collections;

namespace Core.Framework.Helper.Contracts
{
    public interface ICoreQueryable<T> : IEnumerable<T> where T : class, ILoadModel
    {        
        string BuildQuery();
        AsycnModel<T> Asycn();
        object Max(Expression<Func<T, object>> expression);
        object Min(Expression<Func<T, object>> expression);
        int Count();
        int Count(Expression<Func<T, object>> expression);
        ICoreQueryable<T> Where(Expression<Func<T, bool>> expression);
        ICoreQueryable<T> Between(Expression<Func<T, int>> expression, string high, string low);
        T FirstOrDefault();
        T FirstOrDefault(Expression<Func<T, bool>> expression);
        bool Any(Expression<Func<T, bool>> expression);
        bool Any();
        ICoreQueryable<T> Top(int topData);
        ICoreQueryable<T> OrderBy(Expression<Func<T, object>> expression);
        ICoreQueryable<T> OrderByDescending(Expression<Func<T, object>> expression);
        bool Delete(Expression<Func<T, bool>> expression);
        bool Delete();
        ICoreQueryable<T> ThenBy(Expression<Func<T, object>> expression);
        ICoreQueryable<T> InsertInto<TDestination>() where TDestination : class, ILoadModel;

        ICoreQueryable<T> Select();

        //ICoreQueryable<T> Select<TResult>(
        //    Func<T, TResult> selector);
        ICoreQueryable<T> Update<TResult>(Expression<Func<T, TResult>> func, object source);
        IEnumerable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression);
        void Execute();
    }
}