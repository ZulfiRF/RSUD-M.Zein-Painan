using Core.Framework.Windows.Implementations;
using System.Collections.Generic;

namespace Core.Framework.Windows.Helper
{
    class DependencyValue : KeyValue
    {
        
        public List<KeyValue> Conditions { get; set; }
    }
}