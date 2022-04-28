namespace Core.Framework.Web.Mvc.Impl
{
    /// <summary>
    /// Class ButtonTemplate
    /// </summary>
    public class ButtonTemplate : BaseTemplate
    {
        #region Implementation of ITemplate

        /// <summary>
        /// Renders this instance.
        /// </summary>
        /// <returns>System.String.</returns>
        public override string Render()
        {
            if (string.IsNullOrEmpty(Tag))
                return "<button " + Helper.Helper.ConvertToAttribut(HtmlAttribute) + " class=\"" + CssStyle + "\"> " + ContentTitle + "</button>";
            return Tag;
        }

        #endregion Implementation of ITemplate
    }
}