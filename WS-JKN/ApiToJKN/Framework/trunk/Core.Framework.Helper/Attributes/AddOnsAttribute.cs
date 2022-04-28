using System;

namespace Core.Framework.Helper.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AddOnsAttribute : Attribute
    {
        public string Header { get; set; }

        public AddonsType Type { get; set; }

        public string MethodName { get; set; }

        public string IconPath { get; set; }
    }
}