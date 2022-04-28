namespace Core.Framework.Model.Attr
{
    /// <summary>
    ///     Class RealtionStringAutoIncrementAttribute
    /// </summary>
    public class RealtionStringAutoIncrementAttribute : SkipAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RealtionStringAutoIncrementAttribute" /> class.
        /// </summary>
        /// <param name="value">berisikan panjang maksimal dari aut increment number</param>
        public RealtionStringAutoIncrementAttribute(int value)
        {
            Value = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RealtionStringAutoIncrementAttribute" /> class.
        /// </summary>
        /// <param name="value">berisikan panjang maksimal dari aut increment number</param>
        /// <param name="relationProperty">berisikan property yang akan di gabungkan pada auto increment</param>
        public RealtionStringAutoIncrementAttribute(int value, params string[] relationProperty)
        {
            Value = value;
            RelationProperty = relationProperty;
        }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public int Value { get; set; }

        /// <summary>
        ///     Gets or sets the relation property.
        /// </summary>
        /// <value>The relation property.</value>
        public string[] RelationProperty { get; set; }
    }
}