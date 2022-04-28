using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Core.Framework.Model;
using Core.Framework.Model.Impl;
using Core.Framework.Model.Impl.SqlServer;
using Core.Framework.Web.Mvc.UAC;

namespace Core.Framework.Web.Mvc
{
    /// <summary>
    /// Class BaseApiController
    /// </summary>
    public class BaseApiController : Controller
    {
        /// <summary>
        /// The context
        /// </summary>
        protected BaseConnectionManager Context;

        /// <summary>
        /// Gets or sets the domain library.
        /// </summary>
        /// <value>The domain library.</value>
        protected string[] DomainLibrary { get; set; }

        /// <summary>
        /// Gets or sets the service.
        /// </summary>
        /// <value>The service.</value>
        protected static Dictionary<string, object> Service { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseApiController"/> class.
        /// </summary>
        /// <param name="nameLibrary">The name library.</param>
        /// <example> contoh yang digunakan untuk  memanggil  constructor 
        /// <code>
        ///   public WebApiController()
        ///    : base("CorporatePortal.Domain", "CorporatePortal.Presentation.Domain")
        /// {
        ///    context = Helper.Manager;
        /// }
        ///</code>
        ///</example>
        public BaseApiController(params string[] nameLibrary)
        {
            DomainLibrary = nameLibrary;
            Context = new SqlConnectionManager(UacBaseMethod.ConnectionString);
        }


        /// <summary>
        /// Called when a request matches this controller, but no method with the specified action name is found in the controller.
        /// </summary>
        /// <param name="actionName">The name of the attempted action.</param>
        protected override void HandleUnknownAction(string actionName)
        {
            if (Service == null)
            {
                Service = new Dictionary<string, object>();
                var directoryRoot = "";
                directoryRoot = HttpContext.Request.PhysicalApplicationPath + "\\bin";
                foreach (var name in DomainLibrary)
                {
                    string fileName = directoryRoot + "\\" + name + ".dll";
                    if (System.IO.File.Exists(fileName))
                    {
                        var dataAssembly = Assembly.LoadFile(fileName);
                        foreach (var typeAssembly in dataAssembly.GetTypes())
                        {
                            try
                            {
                                var obj = Activator.CreateInstance(typeAssembly);
                                if (obj is TableItem)
                                {
                                    object data = null;
                                    if (!Service.TryGetValue(typeAssembly.Name, out data))
                                        Service.Add(typeAssembly.Name, obj);
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
            }
            CreateJsonResult(Request.RawUrl);
        }

        /// <summary>
        /// Creates the json result. untuk menambkep hasil request dari client
        /// </summary>
        /// <param name="restFull">The rest full.</param>
        private void CreateJsonResult(string restFull)
        {
            try
            {
                //var breakToArray = restFull.Split(new char[] { '/' });
                var indexLast = restFull.LastIndexOf('/');
                var domainUrl = restFull.Substring(indexLast).Replace("/", "");

                var splitDomainUrl = domainUrl.Split(new[] { '?' });

                string select = "", filter = "", order = "";
                int top = -1, take = -1, skip = -1;
                var include = new string[2];
                var domainName = splitDomainUrl[0].Substring(0, splitDomainUrl[0].Length - 1);
                using (var contextManager = new ContextManager(UacBaseMethod.ConnectionString, Context))
                {
                    object domain = null;
                    if (Service.TryGetValue(domainName, out domain))
                    {
                        if (splitDomainUrl.Length == 2)
                        {
                            string filterUrl = HttpUtility.UrlDecode(splitDomainUrl[1]);
                            // mengambil query string yang di request oleh client
                            foreach (var resource in filterUrl.Split(new[] { '&' }))
                            {
                                if (resource.StartsWith("$"))
                                {
                                    var queryOptions = resource.Substring(1);
                                    if (queryOptions.ToLower().StartsWith("include"))
                                    {
                                        queryOptions = queryOptions.Trim();
                                        include = queryOptions.Substring(queryOptions.IndexOf('=') + 1).Split(new[] { ',' });
                                    }
                                    else if (queryOptions.ToLower().StartsWith("select"))
                                    {
                                        queryOptions = queryOptions.Trim();
                                        select = queryOptions.Substring(queryOptions.IndexOf('=') + 1);
                                    }
                                    else if (queryOptions.ToLower().StartsWith("top"))
                                    {
                                        queryOptions = queryOptions.Trim();
                                        top = Convert.ToInt16(queryOptions.Substring(queryOptions.IndexOf('=') + 1));
                                    }
                                    else if (queryOptions.ToLower().StartsWith("orderby"))
                                    {
                                        queryOptions = queryOptions.Trim();
                                        order = queryOptions.Substring(queryOptions.IndexOf('=') + 1);
                                    }
                                    else if (queryOptions.ToLower().StartsWith("take"))
                                    {
                                        queryOptions = queryOptions.Trim();
                                        take = Convert.ToInt16(queryOptions.Substring(queryOptions.IndexOf('=') + 1));
                                    }
                                    else if (queryOptions.ToLower().StartsWith("filter"))
                                    {
                                        queryOptions = queryOptions.Trim();
                                        filter = queryOptions.Substring(queryOptions.IndexOf('=') + 1);
                                        filter = HttpUtility.UrlDecode(filter);
                                    }
                                    else if (queryOptions.ToLower().StartsWith("skip"))
                                    {
                                        queryOptions = queryOptions.Trim();
                                        skip = Convert.ToInt16(queryOptions.Substring(queryOptions.IndexOf('=') + 1));
                                    }
                                }
                            }
                        }
                        Response.ContentType = "application/json";
                        //mengambil data json dari context manager sesuai dengan request client
                        var result = contextManager.GetJson(domain.GetType(), select, top, order, take, filter, skip, include);
                        var repeat = true;
                        while (repeat)
                        {
                            var split = result.IndexOf(":***", System.StringComparison.Ordinal);
                            if (split != -1)
                            {
                                var endSplit = result.IndexOf("\"***", System.StringComparison.Ordinal);
                                var splitText = result.Substring(split, endSplit - split + 4);
                                var textHtml = splitText.Replace(":***\"", "").Replace("\"***", "");
                                textHtml = textHtml.Replace("\"", "'");

                                //textHtml = HttpUtility.HtmlEncode(textHtml);
                                result = result.Replace(splitText, ":\"" + textHtml + "\"");
                            }
                            else
                            {
                                repeat = false;
                            }
                        }

                        Response.Write("{\"status\" : \"success\", \"list\" :" + result + "}");
                        Response.Flush();
                    }
                }
            }
            catch (Exception exception)
            {
                Response.ContentType = "application/json";
                Response.Write("[{\"status\" : \"failed\", \"message\" :\"" + exception.Message + "\"}]");
                Response.Flush();
            }
        }
    }
}