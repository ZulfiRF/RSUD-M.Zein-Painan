using System;
using System.Collections.Generic;
using System.Reflection;

namespace Core.Framework.Helper.Contracts
{
    public enum SearchType
    {
        Equal,
        Contains,
        GratherThan,
        LessThan,

    }
    public interface IControlCommonRepository
    {
        event EventHandler<FilterQueryArgs> FilterQuery;
        IEnumerable<object> GetBaseData(string keyDomain, string field, object value, object parameters );
        IEnumerable<object> GetBaseData(string keyDomain, string field, object value, SearchType type, object parameters );
    }

    public class MappingValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Display { get; set; }
    }

    public class RegisterDomain
    {
        private static readonly Dictionary<string, object> ListDomains = new Dictionary<string, object>();
        public static void Add(string key, object typeModel)
        {
            object domain;
            if (ListDomains.TryGetValue(key, out domain))
            {
                ListDomains[key] = domain;
            }
            else
            {
                ListDomains.Add(key, typeModel);
            }
        }

        public static void Add(Assembly assembly)
        {
            var asm = assembly;
            foreach (var t in asm.GetExportedTypes())
            {
                try
                {
                    var typeModel = Activator.CreateInstance(t);
                    var key = t.FullName;
                    object domain;
                    if (ListDomains.TryGetValue(key, out domain))
                    {
                        ListDomains[key] = domain;
                    }
                    else
                    {
                        ListDomains.Add(key, typeModel);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public static object Get(string key)
        {
            object obj;
            if (ListDomains.TryGetValue(key, out obj))
            {
                return obj;
            }
            return null;
        }

    }
}
