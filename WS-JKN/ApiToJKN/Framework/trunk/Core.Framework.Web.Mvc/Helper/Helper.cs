using System.Text;
using Core.Framework.Model.Impl;

namespace Core.Framework.Web.Mvc.Helper
{
    /// <summary>
    /// Class Helper 
    /// </summary>
    public class Helper
    {
        /// <summary>
        /// Converts to attribut. digunakan unttuk merubah dari object ke dalam bentuk attribute html
        /// </summary>
        /// <param name="htmlAttribute">berisikan object yang akan dirubah</param>
        /// <returns>System.String.</returns>
        public static string ConvertToAttribut(object htmlAttribute)
        {
            var builder = new StringBuilder();
            if (htmlAttribute == null)
                return "";
            foreach (var propertyInfo in htmlAttribute.GetType().GetProperties())
            {
                builder.Append(" " + propertyInfo.Name.Replace("_", "-") + "=\"" + propertyInfo.GetValue(htmlAttribute, null) +
                               "\"");
            }
            return builder.ToString();
        }

        /// <summary>
        /// Gets or sets the manager.
        /// </summary>
        /// <value>The manager.</value>
        public static BaseConnectionManager Manager { get; set; }
    }
}