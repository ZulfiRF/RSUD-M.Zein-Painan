using System;
using System.Linq;
using Core.Framework.Model.Contract;
using Core.Framework.Model.QueryBuilder.Clausa;
using Core.Framework.Model.QueryBuilder.Enums;

//
// Class: SelectQueryBuilder
// Copyright 2006 by Ewout Stortenbeker
// Email: 4ewout@gmail.com
//
// This class is part of the CodeEngine Framework. This framework also contains
// the UpdateQueryBuilder, InsertQueryBuilder and DeleteQueryBuilder.
// You can download the framework DLL at http://www.code-engine.com/
//

namespace Core.Framework.Model.QueryBuilder
{
    /// <summary>
    ///     Class SqlServerQueryBuilder
    /// </summary>
    public class SqlServerQueryBuilder : BaseQueryBuilder, IQueryBuilder
    {
        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="IQueryBuilder" /> is distinct.
        /// </summary>
        /// <value><c>true</c> if distinct; otherwise, <c>false</c>.</value>
        public override bool Distinct { get; set; }

        /// <summary>
        ///     Gets or sets the top records.
        /// </summary>
        /// <value>The top records.</value>
        public override int TopRecords
        {
            get { return topClause.Quantity; }
            set
            {
                topClause.Quantity = value;
                topClause.Unit = TopUnit.Records;
            }
        }

        /// <summary>
        ///     Gets or sets the top clause.
        /// </summary>
        /// <value>The top clause.</value>
        public override TopClause TopClause
        {
            get { return topClause; }
            set { topClause = value; }
        }

        /// <summary>
        ///     Gets the selected columns.
        /// </summary>
        /// <value>The selected columns.</value>
        public override string[] SelectedColumns
        {
            get
            {
                if (selectedColumns.Count > 0)
                    return selectedColumns.ToArray();
                return new[] {"*"};
            }
        }

        /// <summary>
        ///     Gets the selected tables.
        /// </summary>
        /// <value>The selected tables.</value>
        public override string[] SelectedTables
        {
            get { return selectedTables.ToArray(); }
        }

        /// <summary>
        ///     Selects all columns.
        /// </summary>
        public override void SelectAllColumns()
        {
            selectedColumns.Clear();
        }

        /// <summary>
        ///     Selects the count.
        /// </summary>
        public override void SelectCount()
        {
            SelectColumn("count(1)");
        }

        /// <summary>
        ///     Selects the column.
        /// </summary>
        /// <param name="column">The column.</param>
        public override void SelectColumn(string column)
        {
            selectedColumns.Clear();
            selectedColumns.Add(column);
        }

        /// <summary>
        ///     Selects the columns.
        /// </summary>
        /// <param name="columns">The columns.</param>
        public override void SelectColumns(params string[] columns)
        {
            selectedColumns.Clear();
            foreach (var column in columns)
            {
                selectedColumns.Add(column);
            }
        }

        /// <summary>
        ///     Selects from table.
        /// </summary>
        /// <param name="table">The table.</param>
        public override void SelectFromTable(string table)
        {
            selectedTables.Clear();
            selectedTables.Add(table);
        }

        /// <summary>
        ///     Selects from tables.
        /// </summary>
        /// <param name="tables">The tables.</param>
        public override void SelectFromTables(params string[] tables)
        {
            selectedTables.Clear();
            foreach (var table in tables)
            {
                selectedTables.Add(table);
            }
        }

        /// <summary>
        ///     Gets or sets the where.
        /// </summary>
        /// <value>The where.</value>
        public override WhereStatement Where
        {
            get { return whereStatement; }
            set { whereStatement = value; }
        }

        /// <summary>
        ///     Adds the where.
        /// </summary>
        /// <param name="clause">The clause.</param>
        public override void AddWhere(WhereClause clause)
        {
            AddWhere(clause, 1);
        }

        /// <summary>
        ///     Adds the where.
        /// </summary>
        /// <param name="clause">The clause.</param>
        /// <param name="level">The level.</param>
        public override void AddWhere(WhereClause clause, int level)
        {
            whereStatement.Add(clause, level);
        }

        /// <summary>
        ///     Adds the where.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="operator">The operator.</param>
        /// <param name="compareValue">The compare value.</param>
        /// <returns>WhereClause.</returns>
        public override WhereClause AddWhere(string field, Comparison @operator, object compareValue)
        {
            return AddWhere(field, @operator, compareValue, 1);
        }

        /// <summary>
        ///     Adds the where.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="operator">The operator.</param>
        /// <param name="compareValue">The compare value.</param>
        /// <returns>WhereClause.</returns>
        public override WhereClause AddWhere(Enum field, Comparison @operator, object compareValue)
        {
            return AddWhere(field.ToString(), @operator, compareValue, 1);
        }

        /// <summary>
        ///     Adds the where.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="operator">The operator.</param>
        /// <param name="compareValue">The compare value.</param>
        /// <param name="level">The level.</param>
        /// <returns>WhereClause.</returns>
        public override WhereClause AddWhere(string field, Comparison @operator, object compareValue, int level)
        {
            var newWhereClause = new WhereClause(field, @operator, compareValue);
            whereStatement.Add(newWhereClause, level);
            return newWhereClause;
        }

        /// <summary>
        ///     Adds the order by.
        /// </summary>
        /// <param name="clause">The clause.</param>
        public override void AddOrderBy(OrderByClause clause)
        {
            OrderByStatement.Add(clause);
        }

        /// <summary>
        ///     Adds the order by.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="order">The order.</param>
        public override void AddOrderBy(Enum field, Sorting order)
        {
            AddOrderBy(field.ToString(), order);
        }

        /// <summary>
        ///     Adds the order by.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="order">The order.</param>
        public override void AddOrderBy(string field, Sorting order)
        {
            var newOrderByClause = new OrderByClause(field, order);
            OrderByStatement.Add(newOrderByClause);
        }

        /// <summary>
        ///     Groups the by.
        /// </summary>
        /// <param name="columns">The columns.</param>
        public override void GroupBy(params string[] columns)
        {
            foreach (var column in columns)
            {
                GroupByColumns.Add(column);
            }
        }

        /// <summary>
        ///     Gets or sets the having.
        /// </summary>
        /// <value>The having.</value>
        public override WhereStatement Having
        {
            get { return HavingStatement; }
            set { HavingStatement = value; }
        }

        /// <summary>
        ///     Adds the having.
        /// </summary>
        /// <param name="clause">The clause.</param>
        public override void AddHaving(WhereClause clause)
        {
            AddHaving(clause, 1);
        }

        /// <summary>
        ///     Adds the having.
        /// </summary>
        /// <param name="clause">The clause.</param>
        /// <param name="level">The level.</param>
        public override void AddHaving(WhereClause clause, int level)
        {
            HavingStatement.Add(clause, level);
        }

        /// <summary>
        ///     Adds the having.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="operator">The operator.</param>
        /// <param name="compareValue">The compare value.</param>
        /// <returns>WhereClause.</returns>
        public override WhereClause AddHaving(string field, Comparison @operator, object compareValue)
        {
            return AddHaving(field, @operator, compareValue, 1);
        }

        /// <summary>
        ///     Adds the having.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="operator">The operator.</param>
        /// <param name="compareValue">The compare value.</param>
        /// <returns>WhereClause.</returns>
        public override WhereClause AddHaving(Enum field, Comparison @operator, object compareValue)
        {
            return AddHaving(field.ToString(), @operator, compareValue, 1);
        }

        /// <summary>
        ///     Adds the having.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="operator">The operator.</param>
        /// <param name="compareValue">The compare value.</param>
        /// <param name="level">The level.</param>
        /// <returns>WhereClause.</returns>
        public override WhereClause AddHaving(string field, Comparison @operator, object compareValue, int level)
        {
            var newWhereClause = new WhereClause(field, @operator, compareValue);
            HavingStatement.Add(newWhereClause, level);
            return newWhereClause;
        }

        /// <summary>
        ///     Builds the select query
        /// </summary>
        /// <returns>Returns a string containing the query, or a DbCommand containing a command with parameters</returns>
        /// <exception cref="System.Exception">Having statement was set without Group By</exception>
        public override string BuildQuery()
        {
            var query = "SELECT ";

            // Output Distinct
            if (Distinct)
            {
                query += "DISTINCT ";
            }

            // Output Top clause
            if (!(topClause.Quantity == 100 & topClause.Unit == TopUnit.Percent))
            {
                query += "TOP " + topClause.Quantity;
                if (topClause.Unit == TopUnit.Percent)
                {
                    query += " PERCENT";
                }
                query += " ";
            }

            // Output column names
            if (selectedColumns.Count == 0)
            {
                if (selectedTables.Count == 1)
                    query += selectedTables[0] + ".";
                        // By default only select * from the table that was selected. If there are any joins, it is the responsibility of the user to select the needed columns.

                query += "*";
            }
            else
            {
                query = selectedColumns.Aggregate(query, (current, columnName) => current + (columnName + ','));
                query = query.TrimEnd(','); // Trim de last comma inserted by foreach loop
                query += ' ';
            }

            // Output table names
            if (selectedTables.Count > 0)
            {
                query += " FROM ";
                query = selectedTables.Aggregate(query, (current, tableName) => current + (tableName + ','));
                query = query.TrimEnd(','); // Trim de last comma inserted by foreach loop
                query += ' ';
            }

            // Output joins

            // Output where statement
            if (whereStatement.ClauseLevels > 0)
                query += " WHERE " + whereStatement.BuildWhereStatement();

            // Output GroupBy statement
            if (GroupByColumns.Count > 0)
            {
                query += " GROUP BY ";
                query = GroupByColumns.Aggregate(query, (current, column) => current + (column + ','));
                query = query.TrimEnd(',');
                query += ' ';
            }

            // Output having statement
            if (HavingStatement.ClauseLevels > 0)
            {
                // Check if a Group By Clause was set
                if (GroupByColumns.Count == 0)
                {
                    throw new Exception("Having statement was set without Group By");
                }

                query += " HAVING " + HavingStatement.BuildWhereStatement();
            }

            // Output OrderBy statement
            if (OrderByStatement.Count > 0)
            {
                query += " ORDER BY ";
                foreach (var clause in OrderByStatement)
                {
                    var orderByClause = "";
                    switch (clause.SortOrder)
                    {
                        case Sorting.Ascending:
                            orderByClause = clause.FieldName + " ASC";
                            break;
                        case Sorting.Descending:
                            orderByClause = clause.FieldName + " DESC";
                            break;
                    }
                    query += orderByClause + ',';
                }
                query = query.TrimEnd(','); // Trim de last AND inserted by foreach loop
                query += ' ';
            }

            // Return the built query
            return query;
        }
    }
}