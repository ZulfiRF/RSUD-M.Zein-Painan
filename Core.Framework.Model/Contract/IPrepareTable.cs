using System.Collections.Generic;

namespace Core.Framework.Model.Contract
{
    /// <summary>
    ///     Interface IPrepareTable
    /// </summary>
    public interface IPrepareTable
    {
        /// <summary>
        ///     Initializes the data. digunakan untuk menginsert data saat pembuatan table
        /// </summary>
        /// <returns>List{System.Object}.</returns>
        List<object> InitializeData();
    }
}