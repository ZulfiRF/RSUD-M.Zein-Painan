using System;
using System.ServiceModel;
using System.Web;
using Medifirst.PointOfService.Impl;

namespace Jasamedika.Medifirst.Web.API
{
    public class Print : IHttpHandler, Jasamedika.Medifirst.Web.API.ServicesPrinting.IServicesCallback
    {
        public static Object obj = new object();
        #region IHttpHandler Members

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (
            context.Request.QueryString["report"] != null
            && context.Request.QueryString["preview"] != null
            )
            {
                string url = context.Request.Params["REMOTE_ADDR"];
                lock (obj)
                {

                    try
                    {
                        string post = context.Request.ContentEncoding.GetString(context.Request.BinaryRead(context.Request.ContentLength));
                        Jasamedika.Medifirst.Web.API.ServicesPrinting.ServicesClient svc = new Jasamedika.Medifirst.Web.API.ServicesPrinting.ServicesClient(new InstanceContext(this), PointService.SetNetTCP(), PointService.SetNetEndPoint("net.tcp://serveroltp:8731/ServicePrinting/"));
                        bool preview = (context.Request.QueryString["preview"].Equals("1")) ? true : false;
                        svc.SendMessage(url, post, context.Request.QueryString["report"], preview);
                    }
                    catch (Exception ex)
                    {
                        context.Response.Write(ex);
                    }
                }

            }
            else
            {
                string url = "net.tcp://" + context.Request.Params["REMOTE_ADDR"] + ":8731/ServicePrinting/ ";
                context.Response.Write(url);
            }
        }

        #endregion

        #region IServicesCallback Members

        public void RetriveMessage(string value, string Report, bool preview)
        {

        }

        #endregion
    }
}
