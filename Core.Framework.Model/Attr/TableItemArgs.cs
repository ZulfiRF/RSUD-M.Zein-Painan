using System;

namespace Core.Framework.Model.Attr
{
    /// <summary>
    ///     Class TableItemArgs
    /// </summary>
    public class TableItemArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TableItemArgs" /> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public TableItemArgs(TableItem model)
        {
            Item = model;
        }

        /// <summary>
        ///     Gets or sets the item.
        /// </summary>
        /// <value>The item.</value>
        public TableItem Item { get; set; }
    }
}