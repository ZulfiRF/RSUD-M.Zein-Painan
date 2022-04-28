using System;
using Core.Framework.Model.QueryBuilder.Clausa;
using Core.Framework.Model.QueryBuilder.Enums;

namespace Core.Framework.Model.Contract
{
    /// <summary>
    ///     Interface IQueryBuilder
    /// </summary>
    public interface IQueryBuilder
    {
        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="IQueryBuilder" /> is distinct.
        /// </summary>
        /// <value><c>true</c> if distinct; otherwise, <c>false</c>.</value>
        bool Distinct { get; set; }

        /// <summary>
        ///     Gets or sets the top records.
        /// </summary>
        /// <value>The top records.</value>
        int TopRecords { get; set; }

        /// <summary>
        ///     Gets or sets the top clause.
        /// </summary>
        /// <value>The top clause.</value>
        TopClause TopClause { get; set; }

        /// <summary>
        ///     Gets the selected columns.
        /// </summary>
        /// <value>The selected columns.</value>
        string[] SelectedColumns { get; }

        /// <summary>
        ///     Gets the selected tables.
        /// </summary>
        /// <value>The selected tables.</value>
        string[] SelectedTables { get; }

        /// <summary>
        ///     Gets or sets the where.
        /// </summary>
        /// <value>The where.</value>
        WhereStatement Where { get; set; }

        /// <summary>
        ///     Gets or sets the having.
        /// </summary>
        /// <value>The having.</value>
        WhereStatement Having { get; set; }

        /// <summary>
        ///     Gets or sets the where statement.
        /// </summary>
        /// <value>The where statement.</value>
        WhereStatement WhereStatement { get; set; }

        /// <summary>
        ///     Selects all columns.
        /// </summary>
        void SelectAllColumns();

        /// <summary>
        ///     Selects the count.
        /// </summary>
        void SelectCount();

        /// <summary>
        ///     Selects the column.
        /// </summary>
        /// <param name="column">The column.</param>
        void SelectColumn(string column);

        /// <summary>
        ///     Selects the columns.
        /// </summary>
        /// <param name="columns">The columns.</param>
        void SelectColumns(params string[] columns);

        /// <summary>
        ///     Selects from table.
        /// </summary>
        /// <param name="table">The table.</param>
        void SelectFromTable(string table);

        /// <summary>
        ///     Selects from tables.
        /// </summary>
        /// <param name="tables">The tables.</param>
        void SelectFromTables(params string[] tables);

        /// <summary>
        ///     Adds the where.
        /// </summary>
        /// <param name="clause">The clause.</param>
        void AddWhere(WhereClause clause);

        /// <summary>
        ///     Adds the where.
        /// </summary>
        /// <param name="clause">The clause.</param>
        /// <param name="level">The level.</param>
        void AddWhere(WhereClause clause, int level);

        /// <summary>
        ///     Adds the where.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="operator">The operator.</param>
        /// <param name="compareValue">The compare value.</param>
        /// <returns>WhereClause.</returns>
        WhereClause AddWhere(string field, Comparison @operator, object compareValue);

        /// <summary>
        ///     Adds the where.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="operator">The operator.</param>
        /// <param name="compareValue">The compare value.</param>
        /// <returns>WhereClause.</returns>
        WhereClause AddWhere(Enum field, Comparison @operator, object compareValue);

        /// <summary>
        ///     Adds the where.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="operator">The operator.</param>
        /// <param name="compareValue">The compare value.</param>
        /// <param name="level">The level.</param>
        /// <returns>WhereClause.</returns>
        WhereClause AddWhere(string field, Comparison @operator, object compareValue, int level);

        /// <summary>
        ///     Adds the order by.
        /// </summary>
        /// <param name="clause">The clause.</param>
        void AddOrderBy(OrderByClause clause);

        /// <summary>
        ///     Adds the order by.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="order">The order.</param>
        void AddOrderBy(Enum field, Sorting order);

        /// <summary>
        ///     Adds the order by.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="order">The order.</param>
        void AddOrderBy(string field, Sorting order);

        /// <summary>
        ///     Groups the by.
        /// </summary>
        /// <param name="columns">The columns.</param>
        void GroupBy(params string[] columns);

        /// <summary>
        ///     Adds the having.
        /// </summary>
        /// <param name="clause">The clause.</param>
        void AddHaving(WhereClause clause);

        /// <summary>
        ///     Adds the having.
        /// </summary>
        /// <param name="clause">The clause.</param>
        /// <param name="level">The level.</param>
        void AddHaving(WhereClause clause, int level);

        /// <summary>
        ///     Adds the having.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="operator">The operator.</param>
        /// <param name="compareValue">The compare value.</param>
        /// <returns>WhereClause.</returns>
        WhereClause AddHaving(string field, Comparison @operator, object compareValue);

        /// <summary>
        ///     Adds the having.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="operator">The operator.</param>
        /// <param name="compareValue">The compare value.</param>
        /// <returns>WhereClause.</returns>
        WhereClause AddHaving(Enum field, Comparison @operator, object compareValue);

        /// <summary>
        ///     Adds the having.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="operator">The operator.</param>
        /// <param name="compareValue">The compare value.</param>
        /// <param name="level">The level.</param>
        /// <returns>WhereClause.</returns>
        WhereClause AddHaving(string field, Comparison @operator, object compareValue, int level);

        /// <summary>
        ///     Builds the query.
        /// </summary>
        /// <returns>System.String.</returns>
        string BuildQuery();
    }
}