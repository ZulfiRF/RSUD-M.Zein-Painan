using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Core.Framework.Helper.Collections;
using Core.Framework.Helper.Contracts;

namespace Core.Framework.Model
{
    public class CoreObjectable<T> : ICoreQueryable<T> where T : class, ILoadModel
    {
        private List<T> list;

        public CoreObjectable(List<T> list)
        {
            this.list = list;
        }
        #region Implementation of IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICoreQueryable<T>

        
        public string BuildQuery()
        {
            throw new NotImplementedException();
        }

        public object Max(Expression<Func<T, object>> expression)
        {
            throw new NotImplementedException();
        }

        public object Min(Expression<Func<T, object>> expression)
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<T, object>> expression)
        {
            throw new NotImplementedException();
        }

        public ICoreQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public ICoreQueryable<T> Between(Expression<Func<T, int>> expression, string high, string low)
        {
            throw new NotImplementedException();
        }

        public T FirstOrDefault()
        {
            throw new NotImplementedException();
        }

        public T FirstOrDefault(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public bool Any(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public bool Any()
        {
            throw new NotImplementedException();
        }

        public ICoreQueryable<T> Top(int topData)
        {
            throw new NotImplementedException();
        }

        public ICoreQueryable<T> OrderBy(Expression<Func<T, object>> expression)
        {
            throw new NotImplementedException();
        }

        public ICoreQueryable<T> OrderByDescending(Expression<Func<T, object>> expression)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public bool Delete()
        {
            throw new NotImplementedException();
        }

        public ICoreQueryable<T> ThenBy(Expression<Func<T, object>> expression)
        {
            throw new NotImplementedException();
        }

        public ICoreQueryable<T> InsertInto<TDestination>() where TDestination : class, ILoadModel
        {
            throw new NotImplementedException();
        }

        public ICoreQueryable<T> Select()
        {
            throw new NotImplementedException();
        }

        public ICoreQueryable<T> Update<TResult>(Expression<Func<T, TResult>> func, object source)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression)
        {
            throw new NotImplementedException();
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }

        public AsycnModel<T> Asycn()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}