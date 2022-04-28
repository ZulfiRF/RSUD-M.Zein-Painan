using System;
using System.Diagnostics;

namespace Core.Framework.Windows.Annotations
{
    /// <summary>
    /// This attribute is intended to mark publicly available API
    /// which should not be removed and so is treated as used
    /// </summary>
    [MeansImplicitUse]
    [Conditional("JETBRAINS_ANNOTATIONS")]
    public sealed class PublicAPIAttribute : Attribute
    {
        public PublicAPIAttribute() { }
        public PublicAPIAttribute([NotNull] string comment)
        {
            Comment = comment;
        }

        public string Comment { get; private set; }
    }
}