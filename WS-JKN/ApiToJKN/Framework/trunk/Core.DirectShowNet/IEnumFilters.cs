using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [ComVisible(true), ComImport,
     Guid("56a86893-0ad4-11ce-b03a-0020af0ba770"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumFilters
    {
        [PreserveSig]
        int Next(
            [In] int cFilters,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] out IBaseFilter[] ppFilter,
            [Out] out int pcFetched);

        [PreserveSig]
        int Skip([In] int cFilters);

        void Reset();
        void Clone([Out] out IEnumFilters ppEnum);
    }
}