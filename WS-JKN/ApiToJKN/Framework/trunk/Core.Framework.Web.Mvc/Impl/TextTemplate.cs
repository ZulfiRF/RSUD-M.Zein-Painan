namespace Core.Framework.Web.Mvc.Impl
{
    /// <summary>
    /// Class TextTemplate
    /// </summary>
    public class TextTemplate : BaseTemplate
    {
        #region Implementation of ITemplate

        /// <summary>
        /// Renders this instance.
        /// </summary>
        /// <returns>System.String.</returns>
        public override string Render()
        {
            if (string.IsNullOrEmpty(Tag))
                return "<span " + Helper.Helper.ConvertToAttribut(HtmlAttribute) + " class=\"" + CssStyle + "\"> " + ContentTitle + "</span>";
            return Tag;
        }

        #endregion Implementation of ITemplate
    }
}