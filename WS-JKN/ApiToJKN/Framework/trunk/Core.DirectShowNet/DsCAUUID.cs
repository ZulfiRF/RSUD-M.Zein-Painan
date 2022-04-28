using System;
using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [StructLayout(LayoutKind.Sequential), ComVisible(false)]
    public struct DsCAUUID // CAUUID
    {
        public int cElems;
        public IntPtr pElems;
    }
}