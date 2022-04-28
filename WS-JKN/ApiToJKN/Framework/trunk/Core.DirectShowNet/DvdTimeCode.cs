using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [StructLayout(LayoutKind.Sequential, Pack = 1), ComVisible(false)]
    public struct DvdTimeCode //  DVD_HMSF_TIMECODE
    {
        public byte bHours;
        public byte bMinutes;
        public byte bSeconds;
        public byte bFrames;
    }
}