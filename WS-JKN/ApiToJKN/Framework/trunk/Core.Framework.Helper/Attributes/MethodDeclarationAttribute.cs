using System;

namespace Core.Framework.Helper.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MethodDeclarationAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Type TypeParameter { get; set; }
    }
}