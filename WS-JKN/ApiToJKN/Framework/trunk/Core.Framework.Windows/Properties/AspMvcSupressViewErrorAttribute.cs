using System;
using System.Diagnostics;

namespace Core.Framework.Windows.Annotations
{
    /// <summary>
    /// ASP.NET MVC attribute. Allows disabling inspections for MVC views within a class or a method
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    [Conditional("JETBRAINS_ANNOTATIONS")]
    public sealed class AspMvcSupressViewErrorAttribute : Attribute { }
}