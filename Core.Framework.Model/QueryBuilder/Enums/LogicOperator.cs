//
// Enum: LogicOperator
// Copyright 2006 by Ewout Stortenbeker
// Email: 4ewout@gmail.com
//
// This enum is part of the CodeEngine Framework.
// You can download the framework DLL at http://www.code-engine.com/
//

namespace Core.Framework.Model.QueryBuilder.Enums
{
    /// <summary>
    ///     Represents logic operators for chaining WHERE and HAVING clauses together in a statement
    /// </summary>
    public enum LogicOperator
    {
        /// <summary>
        ///     The and
        /// </summary>
        And,

        /// <summary>
        ///     The or
        /// </summary>
        Or
    }
}