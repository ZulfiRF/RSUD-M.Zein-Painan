using System;

namespace Core.DirectShowNET
{
    [Flags]
    public enum DvdStreamFlags // AM_DVD_STREAM_FLAGS
    {
        None = 0x00000000,
        Video = 0x00000001, // AM_DVD_STREAM_VIDEO
        Audio = 0x00000002, // AM_DVD_STREAM_AUDIO
        SubPic = 0x00000004 // AM_DVD_STREAM_SUBPIC
    }
}