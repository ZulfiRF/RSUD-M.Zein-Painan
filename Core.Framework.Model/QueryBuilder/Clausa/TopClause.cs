using Core.Framework.Model.QueryBuilder.Enums;

//
// Class: TopClause
// Copyright 2006 by Ewout Stortenbeker
// Email: 4ewout@gmail.com
//
// This class is part of the CodeEngine Framework.
// You can download the framework DLL at http://www.code-engine.com/
//

namespace Core.Framework.Model.QueryBuilder.Clausa
{
    /// <summary>
    ///     Represents a TOP clause for SELECT statements
    /// </summary>
    public struct TopClause
    {
        /// <summary>
        ///     The quantity
        /// </summary>
        public int Quantity;

        /// <summary>
        ///     The unit
        /// </summary>
        public TopUnit Unit;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TopClause" /> struct.
        /// </summary>
        /// <param name="nr">The nr.</param>
        public TopClause(int nr)
        {
            Quantity = nr;
            Unit = TopUnit.Records;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TopClause" /> struct.
        /// </summary>
        /// <param name="nr">The nr.</param>
        /// <param name="aUnit">A unit.</param>
        public TopClause(int nr, TopUnit aUnit)
        {
            Quantity = nr;
            Unit = aUnit;
        }
    }
}