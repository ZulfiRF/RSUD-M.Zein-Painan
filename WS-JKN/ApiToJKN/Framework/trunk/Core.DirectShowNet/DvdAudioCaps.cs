using System;

namespace Core.DirectShowNET
{
    [Flags]
    public enum DvdAudioCaps // DVD_AUDIO_CAPS_xx
    {
        Ac3 = 0x00000001, // DVD_AUDIO_CAPS_AC3
        Mpeg2 = 0x00000002, // DVD_AUDIO_CAPS_MPEG2
        Lpcm = 0x00000004, // DVD_AUDIO_CAPS_LPCM
        Dts = 0x00000008, // DVD_AUDIO_CAPS_DTS
        Sdds = 0x00000010 // DVD_AUDIO_CAPS_SDDS
    }
}