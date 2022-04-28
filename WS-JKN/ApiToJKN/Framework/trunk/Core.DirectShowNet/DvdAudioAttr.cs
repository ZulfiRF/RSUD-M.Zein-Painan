using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [StructLayout(LayoutKind.Sequential, Pack = 1), ComVisible(false)]
    public struct DvdAudioAttr // DVD_AudioAttributes
    {
        public DvdAudioAppMode appMode;
        public int appModeData;
        public DvdAudioFormat audioFormat;
        public int language;
        public DvdAudioLangExt languageExtension;
        public bool hasMultichannelInfo;
        public int frequency;
        public byte quantization;
        public byte numberOfChannels;
        public short dummy;
        public int res1;
        public int res2;
    }
}