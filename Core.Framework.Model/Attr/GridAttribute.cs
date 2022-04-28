using System;

namespace Core.Framework.Model.Attr
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class GridAttribute : CoreInputAttribute
    {
    }
}