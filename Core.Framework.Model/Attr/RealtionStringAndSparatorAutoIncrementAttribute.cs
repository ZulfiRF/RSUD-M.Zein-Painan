using System;

namespace Core.Framework.Model.Attr
{
    /// <summary>
    ///     Class RealtionStringAndSparatorAutoIncrementAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RealtionStringAndSparatorAutoIncrementAttribute : SkipAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RealtionStringAndSparatorAutoIncrementAttribute" /> class.
        /// </summary>
        /// <param name="value">berisikan panjang maksimal dari aut increment number</param>
        public RealtionStringAndSparatorAutoIncrementAttribute(int value)
        {
            Value = value;
            Sparator = string.Empty;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RealtionStringAndSparatorAutoIncrementAttribute" /> class.
        /// </summary>
        /// <param name="value">berisikan panjang maksimal dari aut increment number</param>
        /// <param name="sparator">bersikian string pemisah</param>
        /// <param name="relationProperty">berisikan property yang akan di gabungkan pada auto increment</param>
        public RealtionStringAndSparatorAutoIncrementAttribute(int value, string sparator,
            params string[] relationProperty)
        {
            Value = value;
            RelationProperty = relationProperty;
            Sparator = sparator;
        }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public int Value { get; set; }

        /// <summary>
        ///     Gets or sets the sparator.
        /// </summary>
        /// <value>The sparator.</value>
        public string Sparator { get; set; }

        /// <summary>
        ///     Gets or sets the relation property.
        /// </summary>
        /// <value>The relation property.</value>
        public string[] RelationProperty { get; set; }
    }
}