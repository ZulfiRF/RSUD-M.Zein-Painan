using System;
using Core.Framework.Model.Provider.ValueObjects;

namespace Core.Framework.Model.Provider.Adapter
{
    internal interface ISqlAdapter
    {
        string Field(string tableName, string fieldName, string type = "");
        string Parameter(string parameterId);

        string QueryString(string selection, string source, string conditions, string order, string grouping,
            string having, bool isExist = false, int top = int.MaxValue);

        string QueryUpdate(string updates, string source, string conditions);

        string QueryDelete(string source, string conditions);

        string QueryStringPage(string selection, string source, string conditions, string order, int pageSize);

        string QueryStringPage(string selection, string source, string conditions, string order, int pageSize,
            int pageNumber);

        string Table(string tableName);
        string ConvertDateTime(object dateTime);
        string ResolveValue(LikeMethod method, object expressionValue);
        string QueryInsertInto(string source);
        string Function(string fieldName, string value);
    }
}