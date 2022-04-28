using System;

namespace Core.Framework.Model.Attr
{
    /// <summary>
    ///     Class ReferenceAttribute
    /// </summary>
    public class ReferenceApiAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ReferenceAttribute" /> class.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="items">berisikan table yang di relasikan</param>
        public ReferenceApiAttribute(string tableName, params ReferenceItem[] items)
        {
            TableName = tableName;
            Items = items;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReferenceAttribute" /> class.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="referenecKey">berisikan mapping ID  yang di relasikan</param>
        /// <example>
        ///     contoh yang digunakan untuk  merelasikan domain satu ke domain yang lain
        ///     <code>
        ///   [Reference("tTask", "ID=TaskRef")]
        /// </code>
        /// </example>
        public ReferenceApiAttribute(string tableName, params string[] referenecKey)
        {
            TableName = tableName;
            ReferenecKey = referenecKey;
        }

        /// <summary>
        ///     Gets or sets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName { get; set; }

        /// <summary>
        ///     Gets or sets the referenec key.
        /// </summary>
        /// <value>The referenec key.</value>
        public string[] ReferenecKey { get; set; }

        /// <summary>
        ///     Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public ReferenceItem[] Items { get; set; }
    }
}