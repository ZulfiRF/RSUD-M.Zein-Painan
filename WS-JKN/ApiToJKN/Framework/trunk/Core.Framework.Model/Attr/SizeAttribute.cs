using System;

namespace Core.Framework.Model.Attr
{
    /// <summary>
    ///     Class TableAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SizeAttribute : Attribute
    {
        public Int16 Width { get; set; }
        public Int16 Height { get; set; }
        public Int16 CountColumn { get; set; }
    }
}