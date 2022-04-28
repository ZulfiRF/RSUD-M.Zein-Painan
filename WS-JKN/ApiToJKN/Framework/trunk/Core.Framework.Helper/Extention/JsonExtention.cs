using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Core.Framework.Helper.Extention
{
    public static class JsonExtention
    {
        public static string SerelizeJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
