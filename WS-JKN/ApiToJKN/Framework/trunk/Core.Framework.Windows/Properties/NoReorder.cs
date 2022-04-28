using System;
using System.Diagnostics;

namespace Core.Framework.Windows.Annotations
{
    /// <summary>
    /// Prevents the Member Reordering feature from tossing members of the marked class.
    /// </summary>
    /// <remarks>
    /// The attribute must be mentioned in your member reordering patterns.
    /// </remarks>
    [AttributeUsage(AttributeTargets.All)]
    [Conditional("JETBRAINS_ANNOTATIONS")]
    public sealed class NoReorder : Attribute { }
}