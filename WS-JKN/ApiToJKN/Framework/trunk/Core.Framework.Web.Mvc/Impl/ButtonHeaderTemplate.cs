namespace Core.Framework.Web.Mvc.Impl
{
    /// <summary>
    /// Class ButtonHeaderTemplate
    /// </summary>
    public class ButtonHeaderTemplate : BaseContentTemplate
    {

        #region Implementation of ITemplate

        /// <summary>
        /// Renders this instance.
        /// </summary>
        /// <returns>System.String.</returns>
        public override string Render()
        {
            return "<button " + Helper.Helper.ConvertToAttribut(Attribute) + ">" + ContentString + "</button>";
        }

        #endregion Implementation of ITemplate

        
    }
}