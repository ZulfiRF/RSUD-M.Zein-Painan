using System;
using System.Collections.Generic;
using System.Reflection;

namespace Core.Framework.Helper.Contracts
{
    public class RegisterDomain
    {
        #region Static Fields

        private static readonly Dictionary<string, object> ListDomains = new Dictionary<string, object>();

        #endregion

        #region Public Methods and Operators

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
            Assembly asm = assembly;
            foreach (Type t in asm.GetExportedTypes())
            {
                try
                {
                    object typeModel = Activator.CreateInstance(t);
                    string key = t.FullName;
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

        #endregion
    }
}