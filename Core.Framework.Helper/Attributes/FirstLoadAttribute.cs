using System;

namespace Core.Framework.Helper.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FirstLoadAttribute : Attribute
    {
        public string Name { get; set; }
    }
}