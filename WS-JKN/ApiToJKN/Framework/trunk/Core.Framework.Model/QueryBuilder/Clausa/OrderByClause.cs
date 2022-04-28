using Core.Framework.Model.QueryBuilder.Enums;

//
// Class: OrderByClause
// Copyright 2006 by Ewout Stortenbeker
// Email: 4ewout@gmail.com
//
// This class is part of the CodeEngine Framework.
// You can download the framework DLL at http://www.code-engine.com/
//

namespace Core.Framework.Model.QueryBuilder.Clausa
{
    /// <summary>
    ///     Represents a ORDER BY clause to be used with SELECT statements
    /// </summary>
    public struct OrderByClause
    {
        /// <summary>
        ///     The field name
        /// </summary>
        public string FieldName;

        /// <summary>
        ///     The sort order
        /// </summary>
        public Sorting SortOrder;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OrderByClause" /> struct.
        /// </summary>
        /// <param name="field">The field.</param>
        public OrderByClause(string field)
        {
            FieldName = field;
            SortOrder = Sorting.Ascending;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OrderByClause" /> struct.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="order">The order.</param>
        public OrderByClause(string field, Sorting order)
        {
            FieldName = field;
            SortOrder = order;
        }
    }
}