using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Xml.Linq;

namespace Core.Framework.Web.Mvc.UAC
{
    /// <summary>
    /// Class UacBaseMethod
    /// </summary>
    public class UacBaseMethod
    {
        /// <summary>
        /// Gets or sets the access control.
        /// </summary>
        /// <value>The access control.</value>
        public static IUserAccessControl AccessControl { get; set; }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public static string ConnectionString
        {
            get
            {
                var connectionStringAttributeName = GetSectionMembership(HttpContext.Current, "connectionStringName");
                var connectionString = (string.IsNullOrEmpty(connectionStringAttributeName)) ? ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString : ConfigurationManager.ConnectionStrings[connectionStringAttributeName].ConnectionString;
                return connectionString;
            }
        }

        /// <summary>
        /// The load xdoc
        /// </summary>
        private static XDocument loadXdoc;

        /// <summary>
        /// Gets the section membership.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// User Access Control;Section Providers in Web Config is Null
        /// or
        /// User Access Control;Section Providers in Web Config is Null
        /// or
        /// User Access Control;Section Membership in Web Config is Null
        /// or
        /// User Access Control;Section system.web in Web Config is Null
        /// </exception>
        internal static string GetSectionMembership(HttpContext context, string attributeName)
        {
            var config = WebConfigurationManager.OpenWebConfiguration(context.Request.ApplicationPath);
            if (File.Exists(config.FilePath))
            {
                if (loadXdoc == null)
                {
                    XDocument doc = XDocument.Load(config.FilePath);
                    loadXdoc = doc;
                }
                var data = loadXdoc.Descendants("system.web");
                var sysWebSection = loadXdoc.Descendants("system.web").FirstOrDefault();
                if (sysWebSection != null)
                {
                    var memberSection = sysWebSection.Element("membership");
                    if (memberSection != null)
                    {
                        var providers = memberSection.Element("providers");
                        if (providers != null)
                        {
                            var dataElement = providers.Elements("add").FirstOrDefault(n => n.Attribute("type") != null && n.Attribute("type").Value.Equals("Core.Framework.Web.Mvc.UAC.UserAccessControl"));
                            if (dataElement != null)
                            {
                                var redirectUrl = dataElement.Attribute(attributeName);
                                if (redirectUrl != null)
                                {
                                    return redirectUrl.Value;
                                }
                            }
                            else throw new ArgumentNullException("User Access Control", "Section Providers in Web Config is Null");
                        }
                        else throw new ArgumentNullException("User Access Control", "Section Providers in Web Config is Null");
                    }
                    else throw new ArgumentNullException("User Access Control", "Section Membership in Web Config is Null");
                }
                else
                    throw new ArgumentNullException("User Access Control", "Section system.web in Web Config is Null");
            }
            return "";
        }

        /// <summary>
        /// Gets the section membership.
        /// </summary>
        /// <param name="httpContextBase">The HTTP context base.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// User Access Control;Section Providers in Web Config is Null
        /// or
        /// User Access Control;Section Providers in Web Config is Null
        /// or
        /// User Access Control;Section Membership in Web Config is Null
        /// or
        /// User Access Control;Section system.web in Web Config is Null
        /// </exception>
        internal static string GetSectionMembership(HttpContextBase httpContextBase, string attributeName)
        {
            var config = WebConfigurationManager.OpenWebConfiguration(httpContextBase.Request.ApplicationPath);
            if (File.Exists(config.FilePath))
            {
                if (loadXdoc == null)
                {
                    XDocument doc = XDocument.Load(config.FilePath);
                    loadXdoc = doc;
                }
                var data = loadXdoc.Descendants("system.web");
                var sysWebSection = loadXdoc.Descendants("system.web").FirstOrDefault();
                if (sysWebSection != null)
                {
                    var memberSection = sysWebSection.Element("membership");
                    if (memberSection != null)
                    {
                        var providers = memberSection.Element("providers");
                        if (providers != null)
                        {
                            var dataElement = providers.Elements("add").FirstOrDefault(n => n.Attribute("type") != null && n.Attribute("type").Value.Equals("Core.Framework.Web.Mvc.UAC.UserAccessControl"));
                            if (dataElement != null)
                            {
                                var redirectUrl = dataElement.Attribute(attributeName);
                                if (redirectUrl != null)
                                {
                                    return redirectUrl.Value;
                                }
                            }
                            else throw new ArgumentNullException("User Access Control", "Section Providers in Web Config is Null");
                        }
                        else throw new ArgumentNullException("User Access Control", "Section Providers in Web Config is Null");
                    }
                    else throw new ArgumentNullException("User Access Control", "Section Membership in Web Config is Null");
                }
                else
                    throw new ArgumentNullException("User Access Control", "Section system.web in Web Config is Null");
            }
            return "";
        }
    }
}