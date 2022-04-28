using System;
using System.Diagnostics;

namespace Core.Framework.Windows.Annotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    [Conditional("JETBRAINS_ANNOTATIONS")]
    public sealed class AspChildControlTypeAttribute : Attribute
    {
        public AspChildControlTypeAttribute(string tagName, Type controlType)
        {
            TagName = tagName;
            ControlType = controlType;
        }

        public string TagName { get; private set; }
        public Type ControlType { get; private set; }
    }
}