using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Core.Framework.Model.Attr;
using Core.Framework.Model.QueryBuilder.Clausa;

namespace Core.Framework.Model.Contract
{
    /// <summary>
    ///     Interface ICoreContext
    /// </summary>
    /// <typeparam name="TEntity">The type of the T entity.</typeparam>
    public interface ICoreContext<TEntity>
    {
        event EventHandler<QueryArgs> BeforeExecuteQuery;

        /// <summary>
        ///     Counts the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.Int32.</returns>
        int Count(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        ///     Counts this instance.
        /// </summary>
        /// <returns>System.Int32.</returns>
        int Count();

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
        TEntity FirstOrDefault();

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
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        ///     Ases the pararell.
        /// </summary>
        /// <returns>IEnumerable{TEntity}.</returns>
        IEnumerable<TEntity> AsPararell();

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
        /// </example>
        IEnumerable<TEntity> Render();

        IEnumerable RenderData();
        ///// <summary>
        ///// Renders the specified expression.
        ///// </summary>
        ///// <typeparam name="TResult">The type of the T result.</typeparam>
        ///// <param name="expression">The expression.</param>
        ///// <returns>IEnumerable TEntity.</returns>
        ///// <example> contoh yang digunakan untuk mengambil data dari database        
        ///// <code>
        ///// var context = new DomainContext(connectionString);
        ///// var oneData = context.ContactPeopleInImmediatelies.Render(n => n.EmployeID.Equals("0001"));
        /////</code>
        /////</example>
        //IEnumerable<TResult> Render<TResult>(Expression<Func<TEntity, TResult>> expression);

        /// <summary>
        ///     Renders the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IEnumerable{TEntity}.</returns>
        IEnumerable<TEntity> Render(IQueryBuilder query);

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
        ICoreContext<TEntity> Skip(int value);

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
        ICoreContext<TEntity> Take(int value);

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
        ICoreContext<TEntity> Top(int value);

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
        ICoreContext<TEntity> Where(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        ///     Wheres the specified filter.
        /// </summary>
        /// <param name="functionToFilter"> </param>
        /// <param name="filter">The filter.</param>
        /// ///
        /// <param name="isOdata">The filter.</param>
        /// <returns>ICoreContext{TEntity}.</returns>
        ICoreContext<TEntity> Where(object functionToFilter, bool isOdata, params string[] filter);

        /// <summary>
        ///     Wheres the specified expression. digunakan untuk memfilter  data dari database
        /// </summary>
        /// <param name="query">Query yang menampung data pencarian beserta operator pencarian.</param>
        /// <returns>ICoreContext{TEntity}.</returns>
        ICoreContext<TEntity> Where(WhereClause[] query);
    }
}