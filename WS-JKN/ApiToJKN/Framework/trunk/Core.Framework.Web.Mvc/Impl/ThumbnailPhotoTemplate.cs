using System.Text;
using Core.Framework.Web.Mvc.Contract;

namespace Core.Framework.Web.Mvc.Impl
{
    /// <summary>
    /// Class ThumbnailPhotoTemplate
    /// </summary>
    public class ThumbnailPhotoTemplate : ITemplate
    {
        /// <summary>
        /// Gets or sets the big photo source.
        /// </summary>
        /// <value>The big photo source.</value>
        protected string BigPhotoSource { get; set; }

        /// <summary>
        /// Gets or sets the small photo source.
        /// </summary>
        /// <value>The small photo source.</value>
        protected string SmallPhotoSource { get; set; }

        /// <summary>
        /// Gets or sets the CSS style.
        /// </summary>
        /// <value>The CSS style.</value>
        protected string CssStyle { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        protected int Height { get; set; }

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
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        protected int Width { get; set; }

        /// <summary>
        /// Attributes the specified HTML attribute.
        /// </summary>
        /// <param name="htmlAttribute">The HTML attribute.</param>
        /// <returns>ThumbnailPhotoTemplate.</returns>
        public ThumbnailPhotoTemplate Attribute(object htmlAttribute)
        {
            HtmlAttribute = htmlAttribute;
            return this;
        }

        /// <summary>
        /// Sizes the specified width.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>ThumbnailPhotoTemplate.</returns>
        public ThumbnailPhotoTemplate Size(int width, int height)
        {
            Width = width;
            Height = height;
            return this;
        }

        /// <summary>
        /// Sources the big image.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>ThumbnailPhotoTemplate.</returns>
        public ThumbnailPhotoTemplate SourceBigImage(string source)
        {
            BigPhotoSource = source;
            return this;
        }

        /// <summary>
        /// Alternates the text.
        /// </summary>
        /// <param name="alternate">The alternate.</param>
        /// <returns>ThumbnailPhotoTemplate.</returns>
        public ThumbnailPhotoTemplate AlternateText(string alternate)
        {
            Alternate = alternate;
            return this;
        }

        /// <summary>
        /// Sources the small image.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>ThumbnailPhotoTemplate.</returns>
        public ThumbnailPhotoTemplate SourceSmallImage(string source)
        {
            SmallPhotoSource = source;
            return this;
        }

        /// <summary>
        /// Bigs the attribute.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns>ThumbnailPhotoTemplate.</returns>
        public ThumbnailPhotoTemplate BigAttribute(object attribute)
        {
            BigSourceAttribute = attribute;
            return this;
        }

        /// <summary>
        /// Smalls the attribute.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns>ThumbnailPhotoTemplate.</returns>
        public ThumbnailPhotoTemplate SmallAttribute(object attribute)
        {
            SmallSourceAttribute = attribute;
            return this;
        }

        /// <summary>
        /// Uses the title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns>ThumbnailPhotoTemplate.</returns>
        public ThumbnailPhotoTemplate UseTitle(string title)
        {
            Title = title;
            return this;
        }

        #region Implementation of ITemplate

        /// <summary>
        /// Renders this instance.
        /// </summary>
        /// <returns>System.String.</returns>
        public string Render()
        {
            var str = new StringBuilder(" ");
            str.Append("<div class=\"thumbnail-item\">");
            str.Append("<a href=\"#\">");
            if (string.IsNullOrEmpty(Title))
                str.Append("<img " + Helper.Helper.ConvertToAttribut(SmallSourceAttribute) + " src=\"" + SmallPhotoSource + "\" width=\"" + Width + "\"  height=\"" + Height + "\" class=\"thumbnail\"/>");
            else
                str.Append("<span class=\"title-thumbnail\" " + Helper.Helper.ConvertToAttribut(SmallSourceAttribute) + " >" + Title + "</span>");
            str.Append("</a>");
            str.Append("<div class=\"tooltip\">");
            str.Append("<img " + Helper.Helper.ConvertToAttribut(BigSourceAttribute) + " src=\"" + BigPhotoSource + "\" alt=\"" + Alternate + "\"  />");
            str.Append("<span class=\"overlay\"></span>");
            str.Append("</div> ");
            str.Append("</div> ");

            return str.ToString();
        }

        #endregion Implementation of ITemplate

        /// <summary>
        /// Gets or sets the alternate.
        /// </summary>
        /// <value>The alternate.</value>
        protected string Alternate { get; set; }

        /// <summary>
        /// Gets or sets the small source attribute.
        /// </summary>
        /// <value>The small source attribute.</value>
        protected object SmallSourceAttribute { get; set; }

        /// <summary>
        /// Gets or sets the big source attribute.
        /// </summary>
        /// <value>The big source attribute.</value>
        protected object BigSourceAttribute { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        protected string Title { get; set; }
    }
}