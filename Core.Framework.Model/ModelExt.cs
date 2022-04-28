using System;
using System.Collections.Generic;
using System.Linq;
using Core.Framework.Helper.Contracts;

namespace Core.Framework.Model
{
    public static class ModelExt
    {

        public static IEnumerable<T> Convert<T>(this ICoreQueryable<TableItem> model) where T : TableItem
        {
            return model.Select(n => Convert<T>((TableItem)n));
        }
        public static IEnumerable<T> Convert<T>(this IEnumerable<TableItem> model) where T : TableItem
        {
            return model.Select(n => Convert<T>((TableItem)n));
        }
        public static T Convert<T>(this TableItem model) where T : TableItem
        {
            var table = Activator.CreateInstance<T>() as TableItem;
            if (model != null) table.OnInitByPass(model.GetDictionary());
            table.IsLoad = true;
            return (T)table;
        }
    }
}