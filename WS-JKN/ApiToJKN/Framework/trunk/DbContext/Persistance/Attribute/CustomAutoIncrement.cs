using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbContext.Persistance
{
    public class CustomAutoIncrementAttribute : CustomAttribute
    {
        public CustomAutoIncrementAttribute(int value)
        {
            this.Value = value;
        }
        public CustomAutoIncrementAttribute(int value, params string[] relationProperty)
        {
            this.Value = value;
            this.RelationProperty = relationProperty;
        }
        public int Value { get; set; }
        public string[] RelationProperty { get; set; }
    }
}
