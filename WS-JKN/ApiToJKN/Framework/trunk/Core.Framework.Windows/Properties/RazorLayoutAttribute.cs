using System;
using System.Diagnostics;

namespace Core.Framework.Windows.Annotations
{
    [AttributeUsage(AttributeTargets.Property)]
    [Conditional("JETBRAINS_ANNOTATIONS")]
    public sealed class RazorLayoutAttribute : Attribute { }
}