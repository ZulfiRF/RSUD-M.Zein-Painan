using System.IO;
using System.Web;

namespace Jasamedika.Medifirst.Web.API
{
    public class FileUpload : IHttpHandler
    {
        public string temp;

        public void ProcessRequest(HttpContext context)
        {
            string filename = context.Request.QueryString["filename"];

            if (context.Request.QueryString["folder"] != null)
                temp = context.Server.MapPath("~/Images/" + context.Request.QueryString["folder"] + "/" + filename);

            using (FileStream fs = File.Create(temp))
            {
                SaveFile(context.Request.InputStream, fs);
            }
        }

        private void SaveFile(Stream stream, FileStream fs)
        {
            var buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                fs.Write(buffer, 0, bytesRead);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}