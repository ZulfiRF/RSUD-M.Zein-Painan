using System;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Core.Framework.Web.Mvc
{
    /// <summary>
    /// Class JsonResultExtend
    /// </summary>
    public class JsonResultExtend : JsonResult
    {
        /// <summary>
        /// Enables processing of the result of an action method by a custom type that inherits from the <see cref="T:System.Web.Mvc.ActionResult" /> class.
        /// </summary>
        /// <param name="context">The context within which the result is executed.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            try
            {
                context.HttpContext.Response.ContentType = "application/json";
                string str = "";
                try
                {
                    str = JsonConvert.SerializeObject(Data, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                }
                catch (Exception exception)
                {
                    str = "[Result : error , Message:" + exception.Message + "]";
                }
                context.HttpContext.Response.Write(str);
            }
            catch (Exception)
            {
            }
            

            //dat
        }
    }
}