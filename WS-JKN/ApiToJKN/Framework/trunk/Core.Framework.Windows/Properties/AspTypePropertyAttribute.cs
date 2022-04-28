using System;
using System.Diagnostics;

namespace Core.Framework.Windows.Annotations
{
    [AttributeUsage(AttributeTargets.Property)]
    [Conditional("JETBRAINS_ANNOTATIONS")]
    public sealed class AspTypePropertyAttribute : Attribute
    {
        public bool CreateConstructorReferences { get; private set; }

        public AspTypePropertyAttribute(bool createConstructorReferences)
        {
            CreateConstructorReferences = createConstructorReferences;
        }
    }
}