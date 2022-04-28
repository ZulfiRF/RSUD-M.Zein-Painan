using System.Collections.Generic;

namespace Core.Framework.Model
{
    public class FilterQuery
    {
        public object Source { get; set; }
        public List<string> Query { get; set; }
    }
}