using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Framework.Helper.Presenters;

namespace Core.Framework.Helper.Views
{
    public interface ICreateUpdateDeleteView :IAttachPresenter
    {
    }
    public interface ICreateUpdateDeleteViews<TDomain> : ICreateUpdateDeleteView where TDomain : class
    {
        /// <summary>
        /// digunakan untuk menyimpan model yang akan di simpan atau di update
        /// </summary>
        TDomain CurrentDomain { get; }
        /// <summary>
        /// digunakan untuk melakukan penghapusan data
        /// </summary>
        TDomain CurrentSelectedDomains { get; }

        /// <summary>
        /// digunakan untuk meyimpan domains yang ada di dalam repository
        /// </summary>
        IEnumerable<TDomain> ListDomains { set; }
        
    }
}
