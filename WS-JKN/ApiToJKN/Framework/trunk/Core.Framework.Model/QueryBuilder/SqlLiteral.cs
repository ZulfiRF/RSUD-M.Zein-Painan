//
// Class: SqlLiteral
// Copyright 2006 by Ewout Stortenbeker
// Email: 4ewout@gmail.com
//
// This class is part of the CodeEngine Framework.
// You can download the framework DLL at http://www.code-engine.com/
//

namespace Core.Framework.Model.QueryBuilder
{
    /// <summary>
    ///     Class SqlLiteral
    /// </summary>
    public class SqlLiteral
    {
        /// <summary>
        ///     The statement rows affected
        /// </summary>
        public static string StatementRowsAffected = "SELECT @@ROWCOUNT";

        /// <summary>
        ///     The _value
        /// </summary>
        private string _value;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SqlLiteral" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public SqlLiteral(string value)
        {
            _value = value;
        }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}