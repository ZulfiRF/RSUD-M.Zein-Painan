using System.Text;
using Core.Framework.Web.Mvc.Contract;

namespace Core.Framework.Web.Mvc.Impl
{
    /// <summary>
    /// Class PhotoTemplate
    /// </summary>
    public class PhotoTemplate : ITemplate
    {
        /// <summary>
        /// Gets or sets the CSS style.
        /// </summary>
        /// <value>The CSS style.</value>
        protected string CssStyle { get; set; }

        /// <summary>
        /// Gets or sets the HTML attribute.
        /// </summary>
        /// <value>The HTML attribute.</value>
        protected object HtmlAttribute { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        protected string Source { get; set; }

        /// <summary>
        /// Attributes the specified HTML attribute. digunakan untuk memberikan atribute pada tag img
        /// </summary>
        /// <param name="htmlAttribute">The HTML attribute.</param>
        /// <returns>PhotoTemplate.</returns>
        public PhotoTemplate Attribute(object htmlAttribute)
        {
            HtmlAttribute = htmlAttribute;
            return this;
        }

        /// <summary>
        /// CSSs the specified CSS class.
        /// </summary>
        /// <param name="cssClass">The CSS class.</param>
        /// <returns>PhotoTemplate.</returns>
        public PhotoTemplate Css(string cssClass)
        {
            CssStyle = cssClass;
            return this;
        }

        /// <summary>
        /// Sizes the specified width.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>PhotoTemplate.</returns>
        public PhotoTemplate Size(int width, int height)
        {
            Width = width;
            Height = height;
            return this;
        }

        /// <summary>
        /// Sources the image.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>PhotoTemplate.</returns>
        public PhotoTemplate SourceImage(string source)
        {
            Source = source;
            return this;
        }

        #region Implementation of ITemplate

        /// <summary>
        /// Renders this instance.
        /// </summary>
        /// <returns>System.String.</returns>
        public string Render()
        {
            var str = new StringBuilder("<img ");
            if (!string.IsNullOrEmpty(CssStyle))
                str.Append(" class=\"" + CssStyle + "\" ");
            if (!string.IsNullOrEmpty(Source))
                str.Append(" src=\"" + Source + "\" ");
            if (Width != 0)
                str.Append(" width=\"" + Width + "\" ");
            if (Height != 0)
                str.Append(" height=\"" + Height + "\" ");
            str.Append(Helper.Helper.ConvertToAttribut(HtmlAttribute));
            str.Append("/>");
            return str.ToString();
        }

        #endregion Implementation of ITemplate

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        protected int Height { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        protected int Width { get; set; }
    }
}