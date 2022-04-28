namespace Core.Framework.Helper.Contracts
{
    using System.Collections.Generic;

    public class FilterQueryArgs : ItemEventArgs<IEnumerable<object>>
    {
        #region Constructors and Destructors

        public FilterQueryArgs(IEnumerable<object> items, string domains, object parameters = null)
            : base(items)
        {
            Parameters = parameters;
            Domains = domains;
        }

        protected FilterQueryArgs(IEnumerable<object> item)
            : base(item)
        {
        }

        #endregion

        #region Public Properties

        public string Domains { get; set; }
        public object Parameters { get; set; }

        #endregion
    }
}