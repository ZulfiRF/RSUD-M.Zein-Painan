using System;
using System.Collections.Generic;
using Core.Framework.Model.Contract;
using Core.Framework.Model.QueryBuilder.Clausa;
using Core.Framework.Model.QueryBuilder.Enums;

namespace Core.Framework.Model.QueryBuilder
{
    /// <summary>
    ///     Class BaseQueryBuilder
    /// </summary>
    public abstract class BaseQueryBuilder : IQueryBuilder
    {
        /// <summary>
        ///     The group by columns
        /// </summary>
        protected List<string> GroupByColumns = new List<string>(); // array of string

        /// <summary>
        ///     The having statement
        /// </summary>
        protected WhereStatement HavingStatement = new WhereStatement();

        /// <summary>
        ///     The order by statement
        /// </summary>
        protected List<OrderByClause> OrderByStatement = new List<OrderByClause>(); // array of OrderByClause

        /// <summary>
        ///     The selected columns
        /// </summary>
        protected List<string> selectedColumns = new List<string>(); // array of string

        /// <summary>
        ///     The selected tables
        /// </summary>
        protected List<string> selectedTables = new List<string>(); // array of string

        /// <summary>
        ///     The top clause
        /// </summary>
        protected TopClause topClause = new TopClause(100, TopUnit.Percent);

        /// <summary>
        ///     The where statement
        /// </summary>
        protected WhereStatement whereStatement = new WhereStatement();

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="IQueryBuilder" /> is distinct.
        /// </summary>
        /// <value><c>true</c> if distinct; otherwise, <c>false</c>.</value>
        public abstract bool Distinct { get; set; }

        /// <summary>
        ///     Gets or sets the top records.
        /// </summary>
        /// <value>The top records.</value>
        public abstract int TopRecords { get; set; }

        /// <summary>
        ///     Gets or sets the top clause.
        /// </summary>
        /// <value>The top clause.</value>
        public abstract TopClause TopClause { get; set; }

        /// <summary>
        ///     Gets the selected columns.
        /// </summary>
        /// <value>The selected columns.</value>
        public abstract string[] SelectedColumns { get; }

        /// <summary>
        ///     Gets the selected tables.
        /// </summary>
        /// <value>The selected tables.</value>
        public abstract string[] SelectedTables { get; }

        /// <summary>
        ///     Gets or sets the where.
        /// </summary>
        /// <value>The where.</value>
        public abstract WhereStatement Where { get; set; }

        /// <summary>
        ///     Gets or sets the having.
        /// </summary>
        /// <value>The having.</value>
        public abstract WhereStatement Having { get; set; }

        /// <summary>
        ///     Gets or sets the where statement.
        /// </summary>
        /// <value>The where statement.</value>
        public WhereStatement WhereStatement
        {
            get { return whereStatement; }
            set { whereStatement = value; }
        }

        /// <summary>
        ///     Selects all columns.
        /// </summary>
        public abstract void SelectAllColumns();

        /// <summary>
        ///     Selects the count.
        /// </summary>
        public abstract void SelectCount();

        /// <summary>
        ///     Selects the column.
        /// </summary>
        /// <param name="column">The column.</param>
        public abstract void SelectColumn(string column);

        /// <summary>
        ///     Selects the columns.
        /// </summary>
        /// <param name="columns">The columns.</param>
        public abstract void SelectColumns(params string[] columns);

        /// <summary>
        ///     Selects from table.
        /// </summary>
        /// <param name="table">The table.</param>
        public abstract void SelectFromTable(string table);

        /// <summary>
        ///     Selects from tables.
        /// </summary>
        /// <param name="tables">The tables.</param>
        public abstract void SelectFromTables(params string[] tables);

        /// <summary>
        ///     Adds the where.
        /// </summary>
        /// <param name="clause">The clause.</param>
        public abstract void AddWhere(WhereClause clause);

        /// <summary>
        ///     Adds the where.
        /// </summary>
        /// <param name="clause">The clause.</param>
        /// <param name="level">The level.</param>
        public abstract void AddWhere(WhereClause clause, int level);

        /// <summary>
        ///     Adds the where.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="operator">The operator.</param>
        /// <param name="compareValue">The compare value.</param>
        /// <returns>WhereClause.</returns>
        public abstract WhereClause AddWhere(string field, Comparison @operator, object compareValue);

        /// <summary>
        ///     Adds the where.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="operator">The operator.</param>
        /// <param name="compareValue">The compare value.</param>
        /// <returns>WhereClause.</returns>
        public abstract WhereClause AddWhere(Enum field, Comparison @operator, object compareValue);

        /// <summary>
        ///     Adds the where.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="operator">The operator.</param>
        /// <param name="compareValue">The compare value.</param>
        /// <param name="level">The level.</param>
        /// <returns>WhereClause.</returns>
        public abstract WhereClause AddWhere(string field, Comparison @operator, object compareValue, int level);

        /// <summary>
        ///     Adds the order by.
        /// </summary>
        /// <param name="clause">The clause.</param>
        public abstract void AddOrderBy(OrderByClause clause);

        /// <summary>
        ///     Adds the order by.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="order">The order.</param>
        public abstract void AddOrderBy(Enum field, Sorting order);

        /// <summary>
        ///     Adds the order by.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="order">The order.</param>
        public abstract void AddOrderBy(string field, Sorting order);

        /// <summary>
        ///     Groups the by.
        /// </summary>
        /// <param name="columns">The columns.</param>
        public abstract void GroupBy(params string[] columns);

        /// <summary>
        ///     Adds the having.
        /// </summary>
        /// <param name="clause">The clause.</param>
        public abstract void AddHaving(WhereClause clause);

        /// <summary>
        ///     Adds the having.
        /// </summary>
        /// <param name="clause">The clause.</param>
        /// <param name="level">The level.</param>
        public abstract void AddHaving(WhereClause clause, int level);

        /// <summary>
        ///     Adds the having.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="operator">The operator.</param>
        /// <param name="compareValue">The compare value.</param>
        /// <returns>WhereClause.</returns>
        public abstract WhereClause AddHaving(string field, Comparison @operator, object compareValue);

        /// <summary>
        ///     Adds the having.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="operator">The operator.</param>
        /// <param name="compareValue">The compare value.</param>
        /// <returns>WhereClause.</returns>
        public abstract WhereClause AddHaving(Enum field, Comparison @operator, object compareValue);

        /// <summary>
        ///     Adds the having.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="operator">The operator.</param>
        /// <param name="compareValue">The compare value.</param>
        /// <param name="level">The level.</param>
        /// <returns>WhereClause.</returns>
        public abstract WhereClause AddHaving(string field, Comparison @operator, object compareValue, int level);

        /// <summary>
        ///     Builds the query.
        /// </summary>
        /// <returns>System.String.</returns>
        public abstract string BuildQuery();
    }
}