using System;
using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [StructLayout(LayoutKind.Sequential), ComVisible(false)]
    public class DsOptIntPtr
    {
        public IntPtr Pointer;
    }
}