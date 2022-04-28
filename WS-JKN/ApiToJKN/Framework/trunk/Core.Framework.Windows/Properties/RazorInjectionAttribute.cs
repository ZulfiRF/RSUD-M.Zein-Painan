using System;
using System.Diagnostics;

namespace Core.Framework.Windows.Annotations
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    [Conditional("JETBRAINS_ANNOTATIONS")]
    public sealed class RazorInjectionAttribute : Attribute
    {
        public RazorInjectionAttribute(string type, string fieldName)
        {
            Type = type;
            FieldName = fieldName;
        }

        public string Type { get; private set; }
        public string FieldName { get; private set; }
    }
}