using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Core.Framework.Web.Mvc.Contract
{

    /// <summary>
    /// Interface ICustomControl digunakan untuk membuat custom control
    /// </summary>
    public interface ICustomControl
    {
        /// <summary>
        /// Renders this instance. digunakan untuk menggenerate control
        /// </summary>
        /// <returns>MvcHtmlString.</returns>
        MvcHtmlString Render();

    }
}
