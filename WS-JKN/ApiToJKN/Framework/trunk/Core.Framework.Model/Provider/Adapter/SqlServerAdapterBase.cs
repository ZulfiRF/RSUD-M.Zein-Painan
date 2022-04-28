using System;
using System.Collections;
using System.Linq;
using System.Text;
using Core.Framework.Model.Provider.ValueObjects;

namespace Core.Framework.Model.Provider.Adapter
{
    internal class SqlServerAdapterBase : SqlAdapterBase
    {
        public string Function(string fieldName, string value)
        {
            if (value.StartsWith("+="))
                return (fieldName + "=" + fieldName + "+" + value.Replace("+=", "")).Replace("--", "+").Replace("+-", "-").Replace("-+", "-") + "";
            if (value.StartsWith("-="))
                return (fieldName + "=" + fieldName + "-" + value.Replace("-=", "")).Replace("--", "+").Replace("+-", "-").Replace("-+", "-") + "";
            return fieldName + "='" + value + "'";
        }
        public string ConvertDateTime(object dateTime)
        {
            return ((DateTime)dateTime).ToString("yyyy-MM-dd HH:mm:ss.000");
            //return "convert(datetime2, '" + ((DateTime)dateTime).ToString("yyyy-MMM-dd HH:mm:ss.000") + "', 121)";
        }

        public string Field(string tableName, string fieldName, string type)
        {
            return string.Format("[{0}].[{1}]", tableName, fieldName);
        }

        public string ResolveValue(LikeMethod method, object expressionValue)
        {
            var resultQuery = new StringBuilder();
            switch (method)
            {
                case LikeMethod.StartsWith:
                    break;
                case LikeMethod.EndsWith:
                    break;
                case LikeMethod.Contains:
                    break;
                case LikeMethod.Equals:
                    break;
                case LikeMethod.Between:
                    if (expressionValue is IEnumerable)
                    {
                        var values = expressionValue as IEnumerable;
                        var arrObject = values.OfType<object>().ToArray();

                        resultQuery.Append(" '" + arrObject[1] + "' and '" + arrObject[2] + "'");
                        return resultQuery.ToString();
                    }
                    else
                    {
                        return expressionValue.ToString();
                    }
                case LikeMethod.In:
                    if (expressionValue is IEnumerable)
                    {
                        var values = expressionValue as IEnumerable;
                        resultQuery.Append("(");
                        var firstLoad = true;
                        foreach (var value in values)
                        {
                            if (firstLoad)
                                resultQuery.Append("'" + value + "'");
                            else resultQuery.Append(",'" + value + "'");
                            firstLoad = false;
                        }
                        resultQuery.Append(")");
                        return resultQuery.ToString();
                    }
                    else
                    {
                        return expressionValue.ToString();
                    }
                case LikeMethod.NotIn:
                    if (expressionValue is IEnumerable)
                    {
                        var values = expressionValue as IEnumerable;
                        resultQuery.Append("(");
                        var firstLoad = true;
                        foreach (var value in values)
                        {
                            if (firstLoad)
                                resultQuery.Append("'" + value + "'");
                            else resultQuery.Append(",'" + value + "'");
                            firstLoad = false;
                        }
                        resultQuery.Append(")");
                        return resultQuery.ToString();
                    }
                    else
                    {
                        return expressionValue.ToString();
                    }
                default:
                    throw new ArgumentOutOfRangeException("method");
            }
            if (expressionValue is DateTime)
            {
                return ((DateTime)expressionValue).ToString("yyyy-MM-dd HH:mm:ss");
            }
            if (expressionValue is DateTime?)
            {
                if (((DateTime?)expressionValue).HasValue)
                    return ((DateTime?)expressionValue).Value.ToString("yyyy-MM-dd HH:mm:ss");
                return null;
            }
            if (expressionValue == null)
                return "";
            return expressionValue.ToString();
        }

        public string Parameter(string parameterId)
        {
            return ("@" + parameterId);
        }

        public string QueryStringPage(string source, string selection, string conditions, string order, int pageSize)
        {
            return string.Format("SELECT TOP({4}) {0} FROM {1} {2} {3}", selection, source, conditions, order, pageSize);
        }

        public string QueryUpdate(string updates, string source, string conditions)
        {
            return string.Format("UPDATE {1} SET  {0} {2} ", updates, source, conditions);
        }
        public string QueryInsertInto(string source)
        {
            return string.Format("INSERT INTO " + source + " ");
        }


        public string QueryDelete(string source, string conditions)
        {
            return string.Format("DELETE FROM  {0} {1} ", source, conditions);
        }

        public string Table(string tableName)
        {
            return string.Format("[{0}]", tableName);
        }
    }
}