using System;
using System.Diagnostics;

namespace Core.Framework.Windows.Annotations
{
    /// <summary>
    /// Indicates that a parameter is a path to a file or a folder within a web project.
    /// Path can be relative or absolute, starting from web root (~)
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    [Conditional("JETBRAINS_ANNOTATIONS")]
    public class PathReferenceAttribute : Attribute
    {
        public PathReferenceAttribute() { }
        public PathReferenceAttribute([PathReference] string basePath)
        {
            BasePath = basePath;
        }

        public string BasePath { get; private set; }
    }
}