using System;

namespace Core.Framework.Model.Attr
{
    /// <summary>
    ///     Class CharecterSpesialAttribute
    /// </summary>
    public class CharecterSpesialAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CharecterSpesialAttribute" /> class.
        /// </summary>
        /// <param name="allow">if set to <c>true</c> [allow].</param>
        public CharecterSpesialAttribute(bool allow)
        {
            Allow = allow;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="CharecterSpesialAttribute" /> is allow.
        /// </summary>
        /// <value><c>true</c> if allow; otherwise, <c>false</c>.</value>
        public bool Allow { get; set; }
    }
}