using System;
using System.Diagnostics;

namespace Core.Framework.Windows.Annotations
{
    /// <summary>
    /// For a parameter that is expected to be one of the limited set of values.
    /// Specify fields of which type should be used as values for this parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field)]
    [Conditional("JETBRAINS_ANNOTATIONS")]
    public sealed class ValueProviderAttribute : Attribute
    {
        public ValueProviderAttribute(string name)
        {
            Name = name;
        }

        [NotNull] public string Name { get; private set; }
    }
}