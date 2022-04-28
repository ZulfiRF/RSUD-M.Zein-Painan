using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Framework.Helper.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ModuleAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
