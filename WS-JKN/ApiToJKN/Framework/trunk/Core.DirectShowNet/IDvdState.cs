using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [ComVisible(true), ComImport,
     Guid("86303d6d-1c4a-4087-ab42-f711167048ef"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvdState
    {
        [PreserveSig]
        int GetDiscID([Out] out long pullUniqueID);

        [PreserveSig]
        int GetParentalLevel([Out] out int pulParentalLevel);
    }
}