using Core.Framework.Web.Mvc.Contract;

namespace Core.Framework.Web.Mvc.Impl
{
    /// <summary>
    /// Class BaseTemplate
    /// </summary>
    public class BaseTemplate : ITemplate
    {
        /// <summary>
        /// Gets or sets the content title.
        /// </summary>
        /// <value>The content title.</value>
        public string ContentTitle { get; set; }

        /// <summary>
        /// Gets or sets the CSS style.
        /// </summary>
        /// <value>The CSS style.</value>
        public string CssStyle { get; set; }

        /// <summary>
        /// Gets or sets the HTML attribute.
        /// </summary>
        /// <value>The HTML attribute.</value>
        public object HtmlAttribute { get; set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public string Tag { get; set; }

        /// <summary>
        /// Atributes the specified HTML attribute.
        /// </summary>
        /// <param name="htmlAttribute">The HTML attribute.</param>
        /// <returns>BaseTemplate.</returns>
        public BaseTemplate Atribute(object htmlAttribute)
        {
            HtmlAttribute = htmlAttribute;
            return this;
        }

        /// <summary>
        /// Contents the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>BaseTemplate.</returns>
        public BaseTemplate Content(string content)
        {
            ContentTitle = content;
            return this;
        }

        /// <summary>
        /// CSSs the class.
        /// </summary>
        /// <param name="cssClass">The CSS class.</param>
        /// <returns>BaseTemplate.</returns>
        public BaseTemplate CssClass(string cssClass)
        {
            CssStyle = cssClass;
            return this;
        }

        /// <summary>
        /// HTMLs the tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>BaseTemplate.</returns>
        public BaseTemplate HtmlTag(string tag)
        {
            Tag = tag;
            return this;
        }

        #region Implementation of ITemplate

        /// <summary>
        /// Renders this instance.
        /// </summary>
        /// <returns>System.String.</returns>
        public virtual string Render()
        {
            if (string.IsNullOrEmpty(Tag))
                return "<button " + Helper.Helper.ConvertToAttribut(HtmlAttribute) + " class=\"" + CssStyle + "\"> " + ContentTitle + "</button>";
            return Tag;
        }

        #endregion Implementation of ITemplate
    }
}