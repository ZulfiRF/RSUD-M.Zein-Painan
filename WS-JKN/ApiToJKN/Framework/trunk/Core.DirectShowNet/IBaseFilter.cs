using System;
using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [ComVisible(true), ComImport,
     Guid("56a86895-0ad4-11ce-b03a-0020af0ba770"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBaseFilter
    {
        #region "IPersist Methods"

        [PreserveSig]
        int GetClassID(
            [Out] out Guid pClassID);

        #endregion

        #region "IMediaFilter Methods"

        [PreserveSig]
        int Stop();

        [PreserveSig]
        int Pause();

        [PreserveSig]
        int Run(long tStart);

        [PreserveSig]
        int GetState(int dwMilliSecsTimeout, [Out] out int filtState);

        [PreserveSig]
        int SetSyncSource([In] IReferenceClock pClock);

        [PreserveSig]
        int GetSyncSource([Out] out IReferenceClock pClock);

        #endregion

        [PreserveSig]
        int EnumPins(
            [Out] out IEnumPins ppEnum);

        [PreserveSig]
        int FindPin(
            [In, MarshalAs(UnmanagedType.LPWStr)] string Id,
            [Out] out IPin ppPin);

        [PreserveSig]
        int QueryFilterInfo(
            [Out] FilterInfo pInfo);

        [PreserveSig]
        int JoinFilterGraph(
            [In] IFilterGraph pGraph,
            [In, MarshalAs(UnmanagedType.LPWStr)] string pName);

        [PreserveSig]
        int QueryVendorInfo(
            [Out, MarshalAs(UnmanagedType.LPWStr)] out string pVendorInfo);
    }
}