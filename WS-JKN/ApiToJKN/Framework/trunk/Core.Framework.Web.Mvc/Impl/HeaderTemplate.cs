using Core.Framework.Web.Mvc.Contract;

namespace Core.Framework.Web.Mvc.Impl
{
    /// <summary>
    /// Class HeaderTemplate
    /// </summary>
    public class HeaderTemplate : ITemplate
    {
        /// <summary>
        /// Gets or sets the header title.
        /// </summary>
        /// <value>The header title.</value>
        public string HeaderTitle { get; set; }

        /// <summary>
        /// Headers the specified header.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <returns>HeaderTemplate.</returns>
        public HeaderTemplate Header(string header)
        {
            HeaderTitle = header;
            return this;
        }

        #region Implementation of ITemplate

        /// <summary>
        /// Renders this instance.
        /// </summary>
        /// <returns>System.String.</returns>
        public string Render()
        {
            return HeaderTitle;
        }

        #endregion Implementation of ITemplate
    }
}