using Core.Framework.Web.Mvc.Contract;

namespace Core.Framework.Web.Mvc.Impl
{
    /// <summary>
    /// Class BaseContentTemplate digunakan untuk dasar tempalte saat akan melakukan pembuatan Custom control
    /// </summary>
    public class BaseContentTemplate : IColumn
    {
        /// <summary>
        /// Gets or sets the content string.
        /// </summary>
        /// <value>The content string.</value>
        public ITemplate ContentString { get; set; }

        /// <summary>
        /// Gets or sets the header title.
        /// </summary>
        /// <value>The header title.</value>
        public ITemplate HeaderTitle { get; set; }

        /// <summary>
        /// Gets or sets the attribute.
        /// </summary>
        /// <value>The attribute.</value>
        internal object Attribute { get; set; }

        /// <summary>
        /// Contents the template.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>BaseContentTemplate.</returns>
        public BaseContentTemplate ContentTemplate(ITemplate content)
        {
            ContentString = content;
            ColumnTemplateProperty = content;
            return this;
        }

        /// <summary>
        /// Headers the template.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <returns>BaseContentTemplate.</returns>
        public BaseContentTemplate HeaderTemplate(ITemplate header)
        {
            HeaderTitle = header;
            HeaderTemplateProperty = header;
            return this;
        }

        /// <summary>
        /// HTMLs the attribute.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns>BaseContentTemplate.</returns>
        public BaseContentTemplate HtmlAttribute(object attribute)
        {
            Attribute = attribute;
            return this;
        }

        #region Implementation of ITemplate

        /// <summary>
        /// Renders this instance.
        /// </summary>
        /// <returns>System.String.</returns>
        public virtual string Render()
        {
            return "<div " + Helper.Helper.ConvertToAttribut(Attribute) + ">" + ContentString + "</div>";
        }

        #endregion Implementation of ITemplate

        #region Implementation of IColumn

        /// <summary>
        /// Gets or sets the column template property. digunakan untuk menentukan Column template yang akan di pakai
        /// </summary>
        /// <value>The column template property.</value>
        public ITemplate ColumnTemplateProperty { get; set; }

        /// <summary>
        /// Gets or sets the header template property.digunakan untuk menentukan Header template yang akan di pakai
        /// </summary>
        /// <value>The header template property.</value>
        public ITemplate HeaderTemplateProperty { get; set; }

        /// <summary>
        /// Columns the specified column. digunakan untuk menset template colum yang digunakan
        /// </summary>
        /// <param name="column">berisikan template column</param>
        /// <returns>IColumn.</returns>
        public IColumn Column(ITemplate column)
        {
            ColumnTemplateProperty = column;
            return this;
        }

        /// <summary>
        /// Headers the specified header.digunakan untuk menset template header yang digunakan
        /// </summary>
        /// <param name="header">The header.</param>
        /// <returns>IColumn.</returns>
        public IColumn Header(ITemplate header)
        {
            HeaderTemplateProperty = header;
            return this;
        }

        #endregion Implementation of IColumn
    }
}