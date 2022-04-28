//
// Enum: JoinType
// Copyright 2006 by Ewout Stortenbeker
// Email: 4ewout@gmail.com
//
// This enum is part of the CodeEngine Framework.
// You can download the framework DLL at http://www.code-engine.com/
//

namespace Core.Framework.Model.QueryBuilder.Enums
{
    /// <summary>
    ///     Represents operators for JOIN clauses
    /// </summary>
    public enum JoinType
    {
        /// <summary>
        ///     The inner join
        /// </summary>
        InnerJoin,

        /// <summary>
        ///     The outer join
        /// </summary>
        OuterJoin,

        /// <summary>
        ///     The left join
        /// </summary>
        LeftJoin,

        /// <summary>
        ///     The right join
        /// </summary>
        RightJoin
    }
}