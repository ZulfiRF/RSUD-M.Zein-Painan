//
// Enum: Comparison
// Copyright 2006 by Ewout Stortenbeker
// Email: 4ewout@gmail.com
//
// This enum is part of the CodeEngine Framework.
// You can download the framework DLL at http://www.code-engine.com/
//

namespace Core.Framework.Model.QueryBuilder.Enums
{
    /// <summary>
    ///     Represents comparison operators for WHERE, HAVING and JOIN clauses
    /// </summary>
    public enum Comparison
    {
        /// <summary>
        ///     The equals
        /// </summary>
        Equals,

        /// <summary>
        ///     The not equals
        /// </summary>
        NotEquals,

        /// <summary>
        ///     The like
        /// </summary>
        Like,

        /// <summary>
        ///     The not like
        /// </summary>
        NotLike,

        /// <summary>
        ///     The greater than
        /// </summary>
        GreaterThan,

        /// <summary>
        ///     The greater or equals
        /// </summary>
        GreaterOrEquals,

        /// <summary>
        ///     The less than
        /// </summary>
        LessThan,

        /// <summary>
        ///     The less or equals
        /// </summary>
        LessOrEquals,

        /// <summary>
        ///     The in
        /// </summary>
        In
    }
}