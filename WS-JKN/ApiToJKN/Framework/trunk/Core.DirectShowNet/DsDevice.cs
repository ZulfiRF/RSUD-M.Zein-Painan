/******************************************************
                  DirectShow .NET
		      netmaster@swissonline.ch
*******************************************************/
//					DsDevice
// enumerate directshow devices and moniker handling


using System;
using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [ComVisible(false)]
    public class DsDevice : IDisposable
    {
        public UCOMIMoniker Mon;
        public string Name;

        #region IDisposable Members

        public void Dispose()
        {
            if (Mon != null)
                Marshal.ReleaseComObject(Mon);
            Mon = null;
        }

        #endregion
    }
}

// namespace DShowNET.Device