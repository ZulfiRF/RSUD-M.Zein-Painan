namespace Core.Framework.Web.Mvc.Contract
{
    /// <summary>
    /// Interface ITemplate digunakan untuk membut template 
    /// </summary>
    public interface ITemplate
    {
        /// <summary>
        /// Renders this instance.
        /// </summary>
        /// <returns>System.String.</returns>
        string Render();
    }
}