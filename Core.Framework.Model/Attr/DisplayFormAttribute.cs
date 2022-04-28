using System;

namespace Core.Framework.Model.Attr
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DisplayFormAttribute : Attribute
    {
        public string Title { get; set; }
    }
}