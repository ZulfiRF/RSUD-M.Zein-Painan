namespace Core.Framework.Web.Mvc.Impl
{
    /// <summary>
    /// Class HtmlTemplate
    /// </summary>
    public class HtmlTemplate : BaseTemplate
    {
        #region Implementation of ITemplate

        /// <summary>
        /// Renders this instance.
        /// </summary>
        /// <returns>System.String.</returns>
        public override string Render()
        {
            if (string.IsNullOrEmpty(Tag))
                return ContentTitle;
            return Tag;
        }

        #endregion Implementation of ITemplate
    }
}