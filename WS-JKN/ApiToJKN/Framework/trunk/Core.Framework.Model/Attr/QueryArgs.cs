using System;

namespace Core.Framework.Model.Attr
{
    /// <summary>
    ///     Class TableItemArgs
    /// </summary>
    public class QueryArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TableItemArgs" /> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public QueryArgs(string model)
        {
            Command = model;
        }

        /// <summary>
        ///     Gets or sets the item.
        /// </summary>
        /// <value>The item.</value>
        public string Command { get; set; }
    }
}