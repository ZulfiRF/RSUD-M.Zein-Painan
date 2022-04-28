using System;
using System.Web;
using System.IO;

namespace Jasamedika.Medifirst.Web.API
{
    class FileDelete : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            string fileName = context.Request.QueryString["filename"];

            if (context.Request.QueryString["folder"] != null)
            {
                FileInfo file = new FileInfo(Path.Combine(context.Server.MapPath("~/Images/" + context.Request.QueryString["folder"] + "/"), fileName));
                if (file.Exists)
                    file.Delete();
            }
            context.Response.Clear();
            context.Response.End();
        }
    }
}
