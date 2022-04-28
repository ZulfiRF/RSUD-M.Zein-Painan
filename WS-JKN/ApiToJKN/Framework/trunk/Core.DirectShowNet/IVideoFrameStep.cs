using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [ComVisible(true), ComImport,
     Guid("e46a9787-2b71-444d-a4b5-1fab7b708d6a"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVideoFrameStep
    {
        [PreserveSig]
        int Step(int dwFrames,
                 [In, MarshalAs(UnmanagedType.IUnknown)] object pStepObject);

        [PreserveSig]
        int CanStep(int bMultiple,
                    [In, MarshalAs(UnmanagedType.IUnknown)] object pStepObject);

        [PreserveSig]
        int CancelStep();
    }
}