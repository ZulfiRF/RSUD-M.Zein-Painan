using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [ComVisible(true), ComImport,
     Guid("211A8760-03AC-11d1-8D13-00AA00BD8339"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMTunerNotification
    {
        [PreserveSig]
        int OnEvent(AMTunerEventType Event);
    }
}