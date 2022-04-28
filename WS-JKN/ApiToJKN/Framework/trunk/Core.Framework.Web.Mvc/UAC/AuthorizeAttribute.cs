using System;
using System.Web;
using System.Web.Mvc;
using Core.Framework.Model;
using Newtonsoft.Json;

namespace Core.Framework.Web.Mvc.UAC
{
    /// <summary>
    /// Class UacAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class UacAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Gets or sets the type of the uac redirect.
        /// </summary>
        /// <value>The type of the uac redirect.</value>
        public UacRedirectType UacRedirectType { get; set; }

        /// <summary>
        /// Gets or sets the module code.
        /// </summary>
        /// <value>The module code.</value>
        public string ModuleCode { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UacAttribute"/> class.
        /// </summary>
        public UacAttribute()
            : this(UacRedirectType.RedirectUrl, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UacAttribute"/> class.
        /// </summary>
        /// <param name="uacRedirectType">Type of the uac redirect.</param>
        public UacAttribute(UacRedirectType uacRedirectType)
            : this(uacRedirectType, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UacAttribute"/> class.
        /// </summary>
        /// <param name="uacRedirectType">Type of the uac redirect.</param>
        /// <param name="moduleCode">The module code.</param>
        public UacAttribute(UacRedirectType uacRedirectType, string moduleCode)
        {
            UacRedirectType = uacRedirectType;
            ModuleCode = moduleCode;
        }

        /// <summary>
        /// Processes HTTP requests that fail authorization.
        /// </summary>
        /// <param name="filterContext">Encapsulates the information for using <see cref="T:System.Web.Mvc.AuthorizeAttribute" />. The <paramref name="filterContext" /> object contains the controller, HTTP context, request context, action result, and route data.</param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            try
            {


                var result = new JsonResult { Data = new { status = "Not Authorize" } };
                result.ContentType = "application/json";
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                //filterContext.Result = result;
                if (UacRedirectType == UacRedirectType.Json)
                {
                    filterContext.HttpContext.Response.ContentType = result.ContentType;
                    filterContext.HttpContext.Response.Write(JsonConvert.SerializeObject(result.Data));
                }
                else
                {
                    var redirectUrl = UacBaseMethod.GetSectionMembership(HttpContext.Current, "redirectUrl");

                    if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.HttpContext.Response.ContentType = result.ContentType;
                        filterContext.HttpContext.Response.Write(JsonConvert.SerializeObject(result.Data));
                        filterContext.HttpContext.Response.End();
                        filterContext.HttpContext.Response.Flush();
                        //if (filterContext.Controller is BaseUserAccessControl)
                        //{
                        //    filterContext.Result = (filterContext.Controller as BaseUserAccessControl).PartialViewPublic(redirectUrl);
                        //}
                    }
                    else
                    {
                        filterContext.HttpContext.Response.Redirect(redirectUrl + "?returnUrl=" + HttpUtility.UrlEncode(filterContext.HttpContext.Request.Url.AbsoluteUri));
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// When overridden, provides an entry point for custom authorization checks.
        /// </summary>
        /// <param name="httpContext">The HTTP context, which encapsulates all HTTP-specific information about an individual HTTP request.</param>
        /// <returns>true if the user is authorized; otherwise, false.</returns>
        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            if (UserAccessControl.IsLogin)
            {
                if (!string.IsNullOrEmpty(ModuleCode))
                {
                    if (UacBaseMethod.AccessControl == null)
                        using (var context = new ContextManager(UacBaseMethod.ConnectionString))
                        {
                            //var user = context.GetFirstRow<Authorize>(n => n.ModuleCode.Equals(ModuleCode) && n.Username.Equals(UserAccessControl.CurrentUser));
                            //if (user == null)
                            return false;
                        }
                    else
                    {
                        return UacBaseMethod.AccessControl.Validate(ModuleCode, UserAccessControl.CurrentUser);
                    }
                }
            }
            return UserAccessControl.IsLogin;
        }
    }
}