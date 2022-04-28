using System;

namespace Core.Framework.Helper.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class InformationModule : Attribute
    {
        public string Description { get; set; }
        public string FilterValue { get; set; }
        public string FilterControl { get; set; }
    }    
}