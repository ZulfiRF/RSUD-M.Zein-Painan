using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jasamedika.Sdk.Vclaim.Attr
{    

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class PropertyLabelAttribute : Attribute
    {
        public string Kode { get; set; }
        public string Name { get; set; }
        public string DefaultValue { get; set; }
        public Type TypeData { get; set; }
        public PropertyLabelAttribute(string name, string kode, string defValue, Type typeData)
        {
            Name = name;
            Kode = kode;
            DefaultValue = defValue;
            TypeData = typeData;
        }
    }
}
