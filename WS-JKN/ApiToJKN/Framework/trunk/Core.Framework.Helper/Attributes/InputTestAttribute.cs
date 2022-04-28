using System;

namespace Core.Framework.Helper.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class InputTestAttribute : Attribute
    {
        public InputTestAttribute()
        {
            PropertyType = typeof(string);
        }
        public string Name { get; set; }
        public object Value { get; set; }
        public Type PropertyType { get; set; }

        public Type Contract { get; set; }

        public string MethodName { get; set; }
    }
}