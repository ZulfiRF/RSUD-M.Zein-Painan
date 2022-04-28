using System.ComponentModel.DataAnnotations;

namespace Core.Framework.Model.Attr
{
    /// <summary>
    ///     Class EnumAttribute
    /// </summary>
    public class EnumAttribute : RequiredAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EnumAttribute" /> class.
        /// </summary>
        /// <param name="mappingEnum">The mapping enum.</param>
        public EnumAttribute(params string[] mappingEnum)
        {
            MappingEnum = mappingEnum;
        }

        /// <summary>
        ///     Gets or sets the mapping enum.
        /// </summary>
        /// <value>The mapping enum.</value>
        public string[] MappingEnum { get; set; }
    }
}