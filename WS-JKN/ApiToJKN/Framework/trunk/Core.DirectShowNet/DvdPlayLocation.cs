using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [StructLayout(LayoutKind.Sequential, Pack = 1), ComVisible(false)]
    public struct DvdPlayLocation // DVD_PLAYBACK_LOCATION2
    {
        public int TitleNum;
        public int ChapterNum;
        public DvdTimeCode timeCode;
        public int TimeCodeFlags;
    }
}