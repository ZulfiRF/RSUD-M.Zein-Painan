using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [StructLayout(LayoutKind.Sequential), ComVisible(false)]
    public struct DsPOINT // POINT
    {
        public int X;
        public int Y;
    }
}