using System;
using System.Diagnostics;

namespace Core.Framework.Windows.Annotations
{
    [AttributeUsage(
        AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field)]
    [Conditional("JETBRAINS_ANNOTATIONS")]
    public sealed class HtmlElementAttributesAttribute : Attribute
    {
        public HtmlElementAttributesAttribute() { }
        public HtmlElementAttributesAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}