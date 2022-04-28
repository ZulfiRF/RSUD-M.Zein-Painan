using System.Web;
using Medifirst.Security;

namespace Jasamedika.Medifirst.Web.API
{
    public class Crypto : IHttpHandler
    {
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
            context.Response.Clear();
            if (context.Request.QueryString["encrypt"] != null)
            {
                context.Response.Write(Cryptography.funcAESEncrypt(context.Request.QueryString["encrypt"]));
            }
            else
                if (context.Request.QueryString["decrypt"] != null)
                {
                    string dec = context.Request.QueryString["decrypt"].Replace(" ", "+");
                    string temp =Cryptography.funcAESDecrypt(dec);
                    context.Response.Write(temp);
                }
        }

        #endregion
    }
}
