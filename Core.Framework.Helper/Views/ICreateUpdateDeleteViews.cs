using System.Collections.Generic;

namespace Core.Framework.Helper.Views
{
    public interface ICreateUpdateDeleteViews<TDomain> : ICreateUpdateDeleteView
        where TDomain : class
    {
        #region Public Properties

        /// <summary>
        ///     digunakan untuk menyimpan model yang akan di simpan atau di update
        /// </summary>
        TDomain CurrentDomain { get; }

        /// <summary>
        ///     digunakan untuk melakukan penghapusan data
        /// </summary>
        TDomain CurrentSelectedDomains { get; }

        /// <summary>
        ///     digunakan untuk meyimpan domains yang ada di dalam repository
        /// </summary>
        IEnumerable<TDomain> ListDomains { set; }

        #endregion
    }
}