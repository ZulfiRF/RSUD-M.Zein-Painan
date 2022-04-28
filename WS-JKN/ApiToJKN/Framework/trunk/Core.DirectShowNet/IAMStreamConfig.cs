using System;
using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [ComVisible(true), ComImport,
     Guid("C6E13340-30AC-11d0-A18C-00A0C9118956"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMStreamConfig
    {
        [PreserveSig]
        int SetFormat(
            [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);

        [PreserveSig]
        int GetFormat(
            [Out, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);

        [PreserveSig]
        int GetNumberOfCapabilities(out int piCount, out int piSize);

        [PreserveSig]
        int GetStreamCaps(int iIndex,
                          [Out, MarshalAs(UnmanagedType.LPStruct)] out AMMediaType ppmt,
                          IntPtr pSCC);
    }
}