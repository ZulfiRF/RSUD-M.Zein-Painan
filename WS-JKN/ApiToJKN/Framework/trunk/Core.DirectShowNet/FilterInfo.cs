using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode), ComVisible(false)]
    public class FilterInfo //  FILTER_INFO
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string achName;
        [MarshalAs(UnmanagedType.IUnknown)] public object pUnk;
    }
}