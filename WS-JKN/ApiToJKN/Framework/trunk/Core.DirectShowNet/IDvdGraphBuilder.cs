using System;
using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [ComVisible(true), ComImport,
     Guid("FCC152B6-F372-11d0-8E00-00C04FD7C08B"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvdGraphBuilder
    {
        [PreserveSig]
        int GetFiltergraph(
            [Out] out IGraphBuilder ppGB);

        [PreserveSig]
        int GetDvdInterface(
            [In] ref Guid riid,
            [Out, MarshalAs(UnmanagedType.IUnknown)] out object ppvIF);

        [PreserveSig]
        int RenderDvdVideoVolume(
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpcwszPathName,
            DvdGraphFlags dwFlags,
            [Out] out DvdRenderStatus pStatus);
    }
}