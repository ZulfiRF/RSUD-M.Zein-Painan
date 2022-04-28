using System;

namespace Core.Framework.Model.Attr
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ModelAttribute : Attribute
    {
        public Type Type { get; set; }
    }
}