using System;

namespace Core.Framework.Model.Attr
{
    /// <summary>
    ///     Class SkipAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class SkipAttribute : CustomAttribute
    {
    }
}