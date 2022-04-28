using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Core.Framework.Model.Attr
{
    /// <summary>
    ///     Class FieldAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FieldAttribute : RequiredAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FieldAttribute" /> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="length">berisikan panjang karakter maksimal</param>
        public FieldAttribute(string fieldName, int length)
            : this(fieldName, false, length, SpesicicationType.AllowNull)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FieldAttribute" /> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="isPrimary">if set to <c>true</c> [is primary].</param>
        /// <param name="length">berisikan panjang karakter maksimal</param>
        public FieldAttribute(string fieldName, bool isPrimary, int length)
            : this(fieldName, isPrimary, length, SpesicicationType.AllowNull)
        {
            if (isPrimary)
                IsAllowNull = SpesicicationType.NotAllowNull;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FieldAttribute" /> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="spesicication">The spesicication.</param>
        public FieldAttribute(string fieldName, SpesicicationType spesicication)
            : this(fieldName, false, 0, spesicication)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FieldAttribute" /> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="isPrimary">if set to <c>true</c> [is primary].</param>
        /// <param name="spesicication">The spesicication.</param>
        public FieldAttribute(string fieldName, bool isPrimary, SpesicicationType spesicication)
            : this(fieldName, isPrimary, 0, spesicication)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FieldAttribute" /> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="isPrimary">if set to <c>true</c> [is primary].</param>
        public FieldAttribute(string fieldName, bool isPrimary)
            : this(fieldName, isPrimary, 0, SpesicicationType.AllowNull)
        {
            if (isPrimary)
                IsAllowNull = SpesicicationType.NotAllowNull;
            ;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FieldAttribute" /> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="isPrimary">if set to <c>true</c> [is primary].</param>
        /// <param name="length">berisikan panjang karakter maksimal</param>
        /// <param name="isAllowNull">The is allow null.</param>
        public FieldAttribute(string fieldName, bool isPrimary, int length, SpesicicationType isAllowNull)
        {
            FieldName = fieldName;
            IsPrimary = isPrimary;
            Length = length;
            IsAllowNull = isAllowNull;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FieldAttribute" /> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        public FieldAttribute(string fieldName)
            : this(fieldName, false, 0, SpesicicationType.AllowNull)
        {
        }

        /// <summary>
        ///     Gets or sets the name of the field.
        /// </summary>
        /// <value>The name of the field.</value>
        public string FieldName { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is primary.
        /// </summary>
        /// <value><c>true</c> if this instance is primary; otherwise, <c>false</c>.</value>
        public bool IsPrimary { get; set; }

        /// <summary>
        ///     Gets or sets the length.
        /// </summary>
        /// <value>berisikan panjang karakter maksimal</value>
        public int? Length { get; set; }

        /// <summary>
        ///     Gets or sets the is allow null.
        /// </summary>
        /// <value>The is allow null.</value>
        public SpesicicationType IsAllowNull { get; set; }

        /// <summary>
        ///     Gets or sets the info.
        /// </summary>
        /// <value>The info.</value>
        public PropertyInfo Info { get; set; }

        public bool IsReference { get; internal set; }
    }
}