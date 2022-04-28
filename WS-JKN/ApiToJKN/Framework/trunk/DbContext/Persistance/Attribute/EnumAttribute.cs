using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbContext.Persistance
{
    public class EnumAttribute : System.Attribute
    {
        public EnumAttribute(string value)
        {
            this.Value = value;
        }
        public string Value { get; set; }
    }
}
