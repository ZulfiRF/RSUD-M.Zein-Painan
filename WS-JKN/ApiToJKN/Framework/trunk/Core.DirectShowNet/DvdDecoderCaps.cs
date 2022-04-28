using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [StructLayout(LayoutKind.Sequential, Pack = 1), ComVisible(false)]
    public struct DvdDecoderCaps // DVD_DECODER_CAPS
    {
        public int size; // size of this struct
        public DvdAudioCaps audioCaps;
        public double fwdMaxRateVideo;
        public double fwdMaxRateAudio;
        public double fwdMaxRateSP;
        public double bwdMaxRateVideo;
        public double bwdMaxRateAudio;
        public double bwdMaxRateSP;
        public int res1;
        public int res2;
        public int res3;
        public int res4;
    }
}