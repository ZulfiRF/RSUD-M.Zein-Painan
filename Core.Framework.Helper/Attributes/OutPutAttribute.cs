using System;

namespace Core.Framework.Helper.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class OutPutAttribute : Attribute
    {
        public string Name { get; set; }
    }
}