using System;
using System.Threading;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Core.Framework.Messaging
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class SubscribeAttribute : ActionFilterAttribute
    {
        public Type Type { get; set; }

        public string GroupName { get; set; }

        public SubscribeAttribute(Type type)
        {
            Type = type;
        }

        public SubscribeAttribute(string groupName)
        {
            GroupName = groupName;
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            if (filterContext.HttpContext.Session != null)
            {
                var data = filterContext.HttpContext.Session["Login"];
                if (string.IsNullOrEmpty(GroupName))
                {
                    var impl = Activator.CreateInstance(Type);
                    if (impl is ISubscribeValue)
                    {
                        var jsonResult = filterContext.Result as JsonResult;
                        if (jsonResult != null)
                            ThreadPool.QueueUserWorkItem(SubsribteMethod, new[] { this, jsonResult.Data, (impl as ISubscribeValue).GetValue() });
                    }
                    else if (impl is ISubscribeResultData)
                    {
                        var jsonResult = filterContext.Result as JsonResult;

                        if (jsonResult != null)
                        {
                            (impl as ISubscribeResultData).Data = jsonResult.Data;
                            ThreadPool.QueueUserWorkItem(SubsribteMethod, new[] { this, jsonResult.Data, (impl as ISubscribeResultData).GetValue() });
                        }
                    }
                }
            }
            if (filterContext.Result is JsonResult)
                ThreadPool.QueueUserWorkItem(SubsribteMethod, new[] { this, (filterContext.Result as JsonResult).Data });
        }

        private void SubsribteMethod(object sender)
        {
            var method = sender as object[];
            if (method != null)
            {
                var subscribeAttribute = method[0] as SubscribeAttribute;
                if (subscribeAttribute != null)
                {
                    try
                    {
                        ServiceMessaging.SendMessage(JsonConvert.SerializeObject(method[1]), method.Length == 3 ? method[2].ToString() : subscribeAttribute.GroupName);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            ;
        }
    }
}