using System;
using System.Configuration;
using System.Web;
using Core.Framework.Model;
using Core.Framework.Web.Mvc.Contract;

//using Core.Framework.Web.Mvc.Domain;

namespace Core.Framework.Web.Mvc.UAC
{
    /// <summary>
    /// Class UserAccessControl
    /// </summary>
    public class UserAccessControl
    {
        /// <summary>
        /// Gets the current user. digunakan untuk mengambiil user ID ketika user sudah Login
        /// </summary>
        /// <value>berisikan user ID</value>
        public static string CurrentUser
        {
            get
            {
                var user = HttpContext.Current.Session["Login"];
                return (user == null) ? "1" : HttpContext.Current.Session["Login"].ToString();
            }
        }

        /// <summary>
        /// Gets the name of the current. digunkan untuk mengambil nama user login
        /// </summary>
        /// <value>berisikan nama user </value>
        public static string CurrentName
        {
            get
            {
                var user = HttpContext.Current.Session["Name"];
                return (user == null) ? "1" : HttpContext.Current.Session["Name"].ToString();
            }
        }

        /// <summary>
        /// Gets the current id. digunakan untuk mengambiil user ID ketika user sudah Login
        /// </summary>
        /// <value>The current id.</value>
        public static string CurrentId
        {
            get
            {
                var user = HttpContext.Current.Session["IdLogin"];
                return (user == null) ? "1" : HttpContext.Current.Session["IdLogin"].ToString();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.  digunakan untuk menyimpan settingan pada session
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>System.Object.</returns>
        public object this[string key]
        {
            get { return HttpContext.Current.Session[key]; }
            set { HttpContext.Current.Session[key] = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is login.
        /// </summary>
        /// <value><c>true</c> if this instance is login; otherwise, <c>false</c>.</value>
        public static bool IsLogin
        {
            get
            {
                var user = HttpContext.Current.Session["Login"];
                return user != null;
            }
        }

        /// <summary>
        /// Gets the current role. digunakan untuk  mengambil role dari user yang login
        /// </summary>
        /// <value>The current role.</value>
        public static string CurrentRole
        {
            get
            {
                var user = HttpContext.Current.Session["Role"];
                return (user == null) ? "" : user.ToString();
            }
        }

        /// <summary>
        /// Logins the specified username. digunakan untuk authoentication user saat login
        /// </summary>
        /// <param name="username">berisikan user name dari user login</param>
        /// <param name="password">berisikan password dari user login.</param>
        /// <param name="persistCookie">if set to <c>true</c> [persist cookie].</param>
        /// <param name="login">bersikan class override dari login</param>
        /// <returns>System.Object.</returns>
        public static object Login(string username, string password, bool persistCookie, ILogin login = null)
        {
            using (var context = new ContextManager(Current.ConnectionString, Helper.Helper.Manager))
            {
                if (login == null)
                {
                    //var userDb =
                    //    context.GetFirstRow<AuthorizeUser>(
                    //        n => n.Username.Equals(username) && n.Password.Equals(password));
                    //if (userDb != null)
                    //{
                    //    HttpContext.Current.Session["Login"] = username;
                    //    HttpContext.Current.Session["Role"] = userDb.IsRole;
                    //    userDb.LastLogin = DateTime.Now;
                    //    context.Commit();
                    //    HttpContext.Current.Session["IdLogin"] = Guid.NewGuid().ToString();
                    //    return true;
                    //}
                }
                else
                {
                    var result = login.Login(username, password);
                    if (result)
                        HttpContext.Current.Session["IdLogin"] = Guid.NewGuid().ToString();
                    return result;
                }
            }
            return false;
        }

        /// <summary>
        /// Logs the out.
        /// </summary>
        /// <returns><c>true</c> jika tidak ada kesalahan ketika clear session, <c>false</c> jika terjadi kesalahan</returns>
        public static bool LogOut()
        {
            try
            {
                HttpContext.Current.Session.RemoveAll();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// The connection string
        /// </summary>
        private string connectionString;

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get
            {
                var conncetion = UacBaseMethod.GetSectionMembership(HttpContext.Current, "connectionStringName");
                ProviderName = UacBaseMethod.GetSectionMembership(HttpContext.Current, "name"); ;
                connectionString = ConfigurationManager.ConnectionStrings[conncetion].ConnectionString;
                return connectionString;
            }
        }

        /// <summary>
        /// Gets or sets the name of the provider.
        /// </summary>
        /// <value>The name of the provider.</value>
        public string ProviderName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAccessControl"/> class.
        /// </summary>
        public UserAccessControl()
        {
            Current = this;
        }

        /// <summary>
        /// The current
        /// </summary>
        private static UserAccessControl current;

        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <value>The current.</value>
        protected static UserAccessControl Current
        {
            get
            {
                if (current == null)
                    current = new UserAccessControl();
                return current;
            }

            private set { current = value; }
        }
    }
}