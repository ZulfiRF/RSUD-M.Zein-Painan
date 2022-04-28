using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [ComVisible(true), ComImport,
     Guid("56a868b9-0ad4-11ce-b03a-0020af0ba770"),
     InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IAMCollection
    {
        [PreserveSig]
        int get_Count(out int plCount);

        [PreserveSig]
        int Item(int lItem,
                 [Out, MarshalAs(UnmanagedType.IUnknown)] out object ppUnk);

        [PreserveSig]
        int get_NewEnum(
            [Out, MarshalAs(UnmanagedType.IUnknown)] out object ppUnk);
    }
}