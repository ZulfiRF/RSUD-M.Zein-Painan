using System;

namespace Core.Framework.Model.Attr
{
    /// <summary>
    ///     Class TableAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TableAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TableAttribute" /> class.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        public TableAttribute(string tableName)
        {
            TabelName = tableName;
            AutoDrop = UpdateTableType.NothingAction;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TableAttribute" /> class.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="autoUpload">The auto upload.</param>
        public TableAttribute(string tableName, UpdateTableType autoUpload)
        {
            TabelName = tableName;
            AutoDrop = autoUpload;
        }

        public TableAttribute()
        {
        }

        /// <summary>
        ///     Gets or sets the name of the tabel.
        /// </summary>
        /// <value>The name of the tabel.</value>
        public string TabelName { get; set; }

        /// <summary>
        ///     Gets or sets the auto drop.
        /// </summary>
        /// <value>The auto drop.</value>
        public UpdateTableType AutoDrop { get; set; }
    }
}