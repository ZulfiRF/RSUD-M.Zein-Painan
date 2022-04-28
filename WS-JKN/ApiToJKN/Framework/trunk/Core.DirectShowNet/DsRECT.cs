using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [StructLayout(LayoutKind.Sequential), ComVisible(false)]
    public struct DsRECT // RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}