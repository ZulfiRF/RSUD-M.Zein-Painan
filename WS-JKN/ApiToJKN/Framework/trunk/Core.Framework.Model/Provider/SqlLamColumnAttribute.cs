using System;

namespace Core.Framework.Model.Provider
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SqlLamColumnAttribute : Attribute
    {
        public string Name { get; set; }
    }
}