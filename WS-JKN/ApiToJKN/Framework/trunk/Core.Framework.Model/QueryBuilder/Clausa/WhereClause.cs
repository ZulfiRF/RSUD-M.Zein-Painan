using System.Collections.Generic;
using Core.Framework.Model.QueryBuilder.Enums;

//
// Class: WhereClause
// Copyright 2006 by Ewout Stortenbeker
// Email: 4ewout@gmail.com
//
// This class is part of the CodeEngine Framework.
// You can download the framework DLL at http://www.code-engine.com/
//

namespace Core.Framework.Model.QueryBuilder.Clausa
{
    /// <summary>
    ///     Represents a WHERE clause on 1 database column, containing 1 or more comparisons on
    ///     that column, chained together by logic operators: eg (UserID=1 or UserID=2 or UserID&gt;100)
    ///     This can be achieved by doing this:
    ///     WhereClause myWhereClause = new WhereClause("UserID", Comparison.Equals, 1);
    ///     myWhereClause.AddClause(LogicOperator.Or, Comparison.Equals, 2);
    ///     myWhereClause.AddClause(LogicOperator.Or, Comparison.GreaterThan, 100);
    /// </summary>
    public class WhereClause
    {
        /// <summary>
        ///     The m_ comparison operator
        /// </summary>
        private Comparison m_ComparisonOperator;

        /// <summary>
        ///     The m_ field name
        /// </summary>
        private string m_FieldName;

        /// <summary>
        ///     The m_ value
        /// </summary>
        private object m_Value;

        /// <summary>
        ///     The sub clauses
        /// </summary>
        internal List<SubClause> SubClauses; // Array of SubClause

        /// <summary>
        ///     Initializes a new instance of the <see cref="WhereClause" /> struct.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="firstCompareOperator">The first compare operator.</param>
        /// <param name="firstCompareValue">The first compare value.</param>
        public WhereClause(string field, Comparison firstCompareOperator, object firstCompareValue)
        {
            m_FieldName = field;
            m_ComparisonOperator = firstCompareOperator;
            m_Value = firstCompareValue;
            SubClauses = new List<SubClause>();
        }

        public LogicOperator LogicOperator { get; set; }

        /// <summary>
        ///     Gets/sets the name of the database column this WHERE clause should operate on
        /// </summary>
        /// <value>The name of the field.</value>
        public string FieldName
        {
            get { return m_FieldName; }
            set { m_FieldName = value; }
        }

        /// <summary>
        ///     Gets/sets the comparison method
        /// </summary>
        /// <value>The comparison operator.</value>
        public Comparison ComparisonOperator
        {
            get { return m_ComparisonOperator; }
            set { m_ComparisonOperator = value; }
        }

        /// <summary>
        ///     Gets/sets the value that was set for comparison
        /// </summary>
        /// <value>The value.</value>
        public object Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        /// <summary>
        ///     Adds the clause.
        /// </summary>
        /// <param name="logic">The logic.</param>
        /// <param name="compareOperator">The compare operator.</param>
        /// <param name="compareValue">The compare value.</param>
        public void AddClause(LogicOperator logic, Comparison compareOperator, object compareValue)
        {
            var NewSubClause = new SubClause(logic, compareOperator, compareValue);
            SubClauses.Add(NewSubClause);
        }

        /// <summary>
        ///     Struct SubClause
        /// </summary>
        internal struct SubClause
        {
            /// <summary>
            ///     The comparison operator
            /// </summary>
            public Comparison ComparisonOperator;

            /// <summary>
            ///     The logic operator
            /// </summary>
            public LogicOperator LogicOperator;

            /// <summary>
            ///     The value
            /// </summary>
            public object Value;

            /// <summary>
            ///     Initializes a new instance of the <see cref="SubClause" /> struct.
            /// </summary>
            /// <param name="logic">The logic.</param>
            /// <param name="compareOperator">The compare operator.</param>
            /// <param name="compareValue">The compare value.</param>
            public SubClause(LogicOperator logic, Comparison compareOperator, object compareValue)
            {
                LogicOperator = logic;
                ComparisonOperator = compareOperator;
                Value = compareValue;
            }
        }
    }
}