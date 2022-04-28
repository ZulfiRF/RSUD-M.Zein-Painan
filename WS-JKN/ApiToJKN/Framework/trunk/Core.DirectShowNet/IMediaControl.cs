/******************************************************
                  DirectShow .NET
		      netmaster@swissonline.ch
*******************************************************/
//					DsControl
// basic Quartz control interfaces, ported from control.odl

using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
// ---------------------------------------------------------------------------------------

    [ComVisible(true), ComImport,
     Guid("56a868b1-0ad4-11ce-b03a-0020af0ba770"),
     InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IMediaControl
    {
        [PreserveSig]
        int Run();

        [PreserveSig]
        int Pause();

        [PreserveSig]
        int Stop();

        [PreserveSig]
        int GetState(int msTimeout, out int pfs);

        [PreserveSig]
        int RenderFile(string strFilename);

        [PreserveSig]
        int AddSourceFilter(
            [In] string strFilename,
            [Out, MarshalAs(UnmanagedType.IDispatch)] out object ppUnk);

        [PreserveSig]
        int get_FilterCollection(
            [Out, MarshalAs(UnmanagedType.IDispatch)] out object ppUnk);

        [PreserveSig]
        int get_RegFilterCollection(
            [Out, MarshalAs(UnmanagedType.IDispatch)] out object ppUnk);

        [PreserveSig]
        int StopWhenReady();
    }


// ---------------------------------------------------------------------------------------


// ---------------------------------------------------------------------------------------


// ---------------------------------------------------------------------------------------


// ---------------------------------------------------------------------------------------


// ---------------------------------------------------------------------------------------


// ---------------------------------------------------------------------------------------
}

// namespace DShowNET