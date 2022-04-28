namespace Core.Framework.Web.Mvc.Impl
{
    /// <summary>
    /// Class TextHeaderTemplate
    /// </summary>
    public class TextHeaderTemplate : BaseContentTemplate
    {
        #region Implementation of ITemplate

        /// <summary>
        /// Renders this instance.
        /// </summary>
        /// <returns>System.String.</returns>
        public override string Render()
        {
            return "<span " + Helper.Helper.ConvertToAttribut(Attribute) + ">" + ContentString + "</span>";
        }

        #endregion Implementation of ITemplate
    }
}