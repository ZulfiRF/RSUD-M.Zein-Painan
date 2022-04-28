using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [StructLayout(LayoutKind.Sequential, Pack = 1), ComVisible(false)]
    public struct DvdTitleAttr // DVD_TitleAttributes
    {
        public DvdTitleAppMode appMode; // DVD_TITLE_APPMODE
        public DvdVideoAttr videoAt; // DVD_VideoAttributes
        public int numberOfAudioStreams;
        // WARNING: incomplete
    }
}