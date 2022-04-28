using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Core.Framework.Model;
using Core.Framework.Web.Mvc.UAC;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.Framework.Web.Mvc
{
    /// <summary>
    /// Class CoreController
    /// </summary>
    public abstract class CoreController : BaseController
    {
        /// <summary>
        /// Menus this instance.
        /// </summary>
        /// <returns>ActionResult.</returns>
        public abstract ActionResult Menu();
    }

    /// <summary>
    /// Class BaseController, digunakan untuk base controlller yang menggunakan authentikasi defaul core Framework
    /// </summary>
    [HandleError]
    public abstract class BaseController : Controller
    {


        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
        /// <summary>
        /// The has redirect to login
        /// </summary>
        private static bool hasRedirectToLogin;


        /// <summary>
        /// Initializes data that might not be available when the constructor is called.
        /// </summary>
        /// <param name="requestContext">The HTTP context and route data.</param>
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            ViewBag.Url = false;
            if (requestContext.HttpContext.Request.Url != null)
            {
                ViewBag.Url = requestContext.HttpContext.Request.Url.AbsolutePath.ToCharArray().Count(n => n.Equals('/')) == 2;
            }
            if (!UserAccessControl.IsLogin)
            {
                if (hasRedirectToLogin)
                    return;
                hasRedirectToLogin = true;
                var redirectUrl = UacBaseMethod.GetSectionMembership(System.Web.HttpContext.Current, "redirectUrl");
                var result = new JsonResult { Data = new { status = "Not Authorize" } };
                if (Request.IsAjaxRequest())
                {
                    Response.ContentType = "application/json";
                    Response.Write(JsonConvert.SerializeObject(result.Data));
                    Response.End();
                    Response.Flush();
                }
                else
                {
                    Response.Redirect(redirectUrl + "?returnUrl=" + HttpUtility.UrlEncode(Request.Url.AbsoluteUri));
                }
            }
        }

        /// <summary>
        /// Called when an unhandled exception occurs in the action.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action.</param>
        protected override void OnException(ExceptionContext filterContext)
        {
            try
            {
                Response.ContentType = "application/json";

                Response.Write(JsonConvert.SerializeObject(new { status = "failed", data = filterContext.Exception.Message }));
                Response.Flush();
                Response.End();
            }
            catch (Exception)
            {
            }

            //base.OnException(filterContext);
        }


        /// <summary>
        /// Results the exculde. untuk memfilter data yang akan di kembalikan ke client yang berupa json
        /// </summary>
        /// <param name="data">berisikan data yang akan di kembalikan ke client</param>
        /// <param name="property">berisikan propety apa saja yang tidak boleh di kembalikan ke client</param>
        /// <returns>JsonResultExtend.</returns>
        public JsonResultExtend ResultExculde(object data, params  string[] property)
        {
            object dt;
            int count = 0;
            if (property.Length != 0)
            {
                if (data is IEnumerable)
                {
                    var list = new List<object>();
                    foreach (var item in data as IEnumerable)
                    {
                        if (item is TableItem)
                        {
                            var value = (item as TableItem).GetType().GetProperty("Type",
                                                                                    BindingFlags.NonPublic |
                                                                                    BindingFlags.Instance |
                                                                                    BindingFlags.Public);
                            if (value != null)
                            {
                                value.SetValue(item, RowType.CannotLoad, null);
                            }
                            foreach (var propertyInfo in (item as TableItem).GetType().GetProperties())
                            {
                                if (propertyInfo.PropertyType.IsClass && !propertyInfo.ToString().Contains("String"))
                                {
                                    var objValue = propertyInfo.GetValue(item, null);
                                    if (objValue is TableItem)
                                    {
                                        value = (objValue as TableItem).GetType().GetProperty("Type",
                                                                                     BindingFlags.NonPublic |
                                                                                     BindingFlags.Instance |
                                                                                     BindingFlags.Public);
                                        if (value != null)
                                        {
                                            value.SetValue(objValue, RowType.CannotLoad, null);
                                        }
                                    }
                                }
                            }
                        }
                        list.Add(item);
                    }
                    data = list;
                }
                var result = JsonConvert.SerializeObject(data, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                var obj = JsonConvert.DeserializeObject(result) as JArray;
                var arr = new List<JObject>();
                if (obj != null)
                    foreach (var o in obj)
                    {
                        var e = (o as JObject);
                        var j = new JObject();
                        foreach (var prop in e.Properties())
                        {
                            JToken field;
                            if (!property.Any(n => n.Equals(prop.Name)))
                            {
                                if (e.TryGetValue(prop.Name, out field))
                                {
                                    j.Add(prop.Name, field);
                                }
                            }
                        }
                        count++;
                        arr.Add(j);
                    }
                dt = new JArray(arr);
            }
            else
            {
                dt = data;
                if (data is IEnumerable)
                {
                    count += (data as IEnumerable).Cast<object>().Count();
                }
            }

            var json = new JsonResultExtend();
            json.ContentType = "application/json";
            json.Data = new { status = "success", list = dt, count = count };
            return json;
        }

        /// <summary>
        /// Results the specified data.untuk memfilter data yang akan di kembalikan ke client yang berupa json
        /// </summary>
        /// <param name="data">berisikan data yang akan di kembalikan ke client</param>
        /// <param name="property">berisikan propety apa saja yang boleh di kembalikan ke client</param>
        /// <returns>JsonResultExtend.</returns>
        public JsonResultExtend Result(object data, params string[] property)
        {
            object dt;
            int count = 0;
            if (property.Length != 0)
            {
                var result = JsonConvert.SerializeObject(data, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Error });
                var obj = JsonConvert.DeserializeObject(result) as JArray;
                var arr = new List<JObject>();
                if (obj != null)
                    foreach (var o in obj)
                    {
                        var e = (o as JObject);
                        var j = new JObject();
                        foreach (var prop in property)
                        {
                            JToken field;
                            if (e.TryGetValue(prop, out field))
                            {
                                j.Add(prop, field);
                            }
                        }
                        count++;
                        arr.Add(j);
                    }
                dt = new JArray(arr);
            }
            else
            {
                dt = data;
                if (data is IEnumerable)
                {
                    count += (data as IEnumerable).Cast<object>().Count();
                }
            }

            var json = new JsonResultExtend();
            json.ContentType = "application/json";
            json.Data = new { status = "success", list = dt, count = count };
            return json;
        }
    }
}