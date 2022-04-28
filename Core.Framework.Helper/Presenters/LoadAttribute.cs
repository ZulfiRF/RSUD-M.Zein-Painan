using System;

namespace Core.Framework.Helper.Presenters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class LoadAttribute : Attribute
    {
        public string Name { get; set; }
    }
}