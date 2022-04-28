namespace Core.Framework.Helper.Contracts
{
    using System;
    using System.Collections.Generic;

    public interface IControlCommonRepository
    {
        #region Public Events

        event EventHandler<FilterQueryArgs> FilterQuery;

        #endregion

        #region Public Methods and Operators

        IEnumerable<object> GetBaseData(string keyDomain, string field, object value, object parameters = null);

        IEnumerable<object> GetBaseData(
            string keyDomain,
            string field,
            object value,
            SearchType type,
            object parameters = null);

        IEnumerable<object> GetBaseData(
            Type keyDomain,
            string field,
            object value,
            SearchType type,
            object parameters = null);
        #endregion
    }
}