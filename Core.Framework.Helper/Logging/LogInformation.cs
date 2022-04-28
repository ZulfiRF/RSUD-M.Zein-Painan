using System.Net;

namespace Core.Framework.Helper.Logging
{
    public class LogInformation
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public object Detail { get; set; }
    }
}