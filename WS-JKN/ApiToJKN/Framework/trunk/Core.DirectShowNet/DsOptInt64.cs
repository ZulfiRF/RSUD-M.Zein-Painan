using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [StructLayout(LayoutKind.Sequential), ComVisible(false)]
    public class DsOptInt64
    {
        public DsOptInt64(long Value)
        {
            this.Value = Value;
        }

        public long Value;
    }
}