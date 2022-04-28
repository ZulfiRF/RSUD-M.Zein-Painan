using System;
using System.Diagnostics;

namespace Core.Framework.Windows.Annotations
{
    /// <summary>
    /// ASP.NET MVC attribute. Indicates that a parameter is an MVC area.
    /// Use this attribute for custom wrappers similar to
    /// <c>System.Web.Mvc.Html.ChildActionExtensions.RenderAction(HtmlHelper, String)</c>
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    [Conditional("JETBRAINS_ANNOTATIONS")]
    public sealed class AspMvcAreaAttribute : PathReferenceAttribute
    {
        public AspMvcAreaAttribute() { }
        public AspMvcAreaAttribute(string anonymousProperty)
        {
            AnonymousProperty = anonymousProperty;
        }

        public string AnonymousProperty { get; private set; }
    }
}