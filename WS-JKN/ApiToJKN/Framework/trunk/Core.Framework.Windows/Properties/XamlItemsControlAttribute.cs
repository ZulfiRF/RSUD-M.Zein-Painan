using System;
using System.Diagnostics;

namespace Core.Framework.Windows.Annotations
{
    /// <summary>
    /// XAML attribute. Indicates the type that has <c>ItemsSource</c> property and should be
    /// treated as <c>ItemsControl</c>-derived type, to enable inner  items <c>DataContext</c>
    /// type resolve.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [Conditional("JETBRAINS_ANNOTATIONS")]
    public sealed class XamlItemsControlAttribute : Attribute { }
}