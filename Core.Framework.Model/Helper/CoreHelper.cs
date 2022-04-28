using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Core.Framework.Model.Provider.ValueObjects;

namespace Core.Framework.Model.Helper
{
    public static class SqlFunctionHelper
    {
        public static SelectFunction Sum(this int source)
        {
            return SelectFunction.SUM;
        }
        public static SelectFunction Sum(this Int16 source)
        {
            return SelectFunction.SUM;
        }
        public static SelectFunction Sum(this Int64 source)
        {
            return SelectFunction.SUM;
        }
        public static SelectFunction Sum(this double source)
        {
            return SelectFunction.SUM;
        }
        public static SelectFunction Sum(this Single source)
        {
            return SelectFunction.SUM;
        }
        public static SelectFunction Sum(this Decimal source)
        {
            return SelectFunction.SUM;
        }


        public static SelectFunction AVG(this int source)
        {
            return SelectFunction.AVG;
        }
        public static SelectFunction AVG(this Int16 source)
        {
            return SelectFunction.AVG;
        }
        public static SelectFunction AVG(this Int64 source)
        {
            return SelectFunction.AVG;
        }
        public static SelectFunction AVG(this double source)
        {
            return SelectFunction.AVG;
        }
        public static SelectFunction AVG(this Single source)
        {
            return SelectFunction.AVG;
        }
        public static SelectFunction AVG(this Decimal source)
        {
            return SelectFunction.AVG;
        }


        public static SelectFunction COUNT(this object source)
        {
            return SelectFunction.COUNT;
        }

        public static SelectFunction MAX(this object source)
        {
            return SelectFunction.MAX;
        }

        public static SelectFunction MIN(this object source)
        {
            return SelectFunction.MIN;
        }
    }

    public class CoreHelper
    {
        public static T FillObject<T>(T obj, IDataReader reader)
        {
            var type = obj.GetType();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var property = type.GetProperty(reader.GetName(i));
                if (property != null)
                {
                    if (property.PropertyType == typeof(string))
                        property.SetValue(obj, reader[i].ToString(), null);
                    property.SetValue(obj, Convert.ChangeType(reader[i], property.PropertyType), null);
                }
            }
            return obj;
        }
        public static TResult GetEnum<TResult>(Type type, string value) where TResult : struct
        {
            try
            {
                value.ToCharArray().Max(n => n);
                var valueEnum = value[0];
                var values = Enum.GetValues(type);
                
                var count = 0;
                foreach (var val in values)
                {
                    if (char.IsDigit(valueEnum))
                    {
                        if (count.ToString(CultureInfo.InvariantCulture).Equals(value))
                            return (TResult)Enum.Parse(type, val.ToString());
                    }
                    else
                    {
                        if (val.ToString().Equals(value))
                            return (TResult)Enum.Parse(type, val.ToString());
                    }
                    count++;
                }
            }
            catch (Exception)
            {
            }
            return default(TResult);
        }

        public static string ToString(object obj)
        {
            if (obj == null)
                return string.Empty;
            return obj.ToString();
        }
    }
}