using System;

namespace DbContext.Persistance
{
    public class TabelNameAttribute : Attribute
    {
        public string Name { get; set; }
        public TabelNameAttribute(string name)
        {
            this.Name = name;
        }
    }
}
