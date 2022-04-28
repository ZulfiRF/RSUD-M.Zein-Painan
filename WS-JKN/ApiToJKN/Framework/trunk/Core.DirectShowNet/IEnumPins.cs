using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [ComVisible(true), ComImport,
     Guid("56a86892-0ad4-11ce-b03a-0020af0ba770"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumPins
    {
        [PreserveSig]
        int Next(
            [In] int cPins,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] out IPin[] ppPins,
            [Out] out int pcFetched);

        [PreserveSig]
        int Skip([In] int cPins);

        void Reset();
        void Clone([Out] out IEnumPins ppEnum);
    }
}