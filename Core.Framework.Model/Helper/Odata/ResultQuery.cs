using System.Collections.Generic;

namespace Core.Framework.Model.Helper.Odata
{
    /// <summary>
    ///     Class ResultQuery
    /// </summary>
    public class ResultQuery
    {
        /// <summary>
        ///     Gets or sets the filter.
        /// </summary>
        /// <value>The filter.</value>
        public string Filter { get; set; }

        /// <summary>
        ///     Gets or sets the relation.
        /// </summary>
        /// <value>The relation.</value>
        public List<string> Relation { get; set; }
    }
}