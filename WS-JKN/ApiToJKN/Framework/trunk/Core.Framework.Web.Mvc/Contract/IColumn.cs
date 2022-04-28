
namespace Core.Framework.Web.Mvc.Contract
{
    /// <summary>
    /// Interface IColumn digunakan untuk membuat sebuah custom control baru berupa colom pada grid
    /// </summary>
    public interface IColumn : ITemplate
    {
        /// <summary>
        /// Gets or sets the column template property. digunakan untuk menentukan Column template yang akan di pakai
        /// </summary>
        /// <value>The column template property.</value>
        ITemplate ColumnTemplateProperty { get; set; }

        /// <summary>
        /// Gets or sets the header template property.digunakan untuk menentukan Header template yang akan di pakai
        /// </summary>
        /// <value>The header template property.</value>
        ITemplate HeaderTemplateProperty { get; set; }

        /// <summary>
        /// Columns the specified column. digunakan untuk menset template colum yang digunakan
        /// </summary>
        /// <param name="column">berisikan template column</param>
        /// <returns>IColumn.</returns>
        IColumn Column(ITemplate column);

        /// <summary>
        /// Headers the specified header.digunakan untuk menset template header yang digunakan
        /// </summary>
        /// <param name="header">The header.</param>
        /// <returns>IColumn.</returns>
        IColumn Header(ITemplate header);
    }
}