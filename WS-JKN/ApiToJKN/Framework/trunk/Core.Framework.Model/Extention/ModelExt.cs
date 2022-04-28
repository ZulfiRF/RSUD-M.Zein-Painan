using System.Collections.Generic;
using System.Data;

namespace Core.Framework.Model.Extention
{
    public static class ModelExt
    {
        public static Dictionary<string, object> ToCoreDictionary(this IDataReader model)
        {
            var table = new TableItem();
            table.OnInit(model, ContextManager.Current);
            table.IsLoad = true;
            return table.GetDictionary();
        }
    }
}