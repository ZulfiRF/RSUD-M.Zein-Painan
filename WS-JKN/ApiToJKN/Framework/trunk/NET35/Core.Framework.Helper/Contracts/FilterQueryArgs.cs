using System.Collections.Generic;

namespace Core.Framework.Helper.Contracts
{
    public class FilterQueryArgs : ItemEventArgs<IEnumerable<object>>
    {
        public object Parameters { get; set; }

        protected FilterQueryArgs(IEnumerable<object> item)
            : base(item)
        {
        }

        public FilterQueryArgs(IEnumerable<object> items, string domains, object parameters)
            : base(items)
        {
            Parameters = parameters;
            Domains = domains;
        }

        public string Domains { get; set; }

    }
}