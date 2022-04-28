using System;
using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [Flags, ComVisible(false)]
    public enum SeekingCapabilities // AM_SEEKING_SeekingCapabilities AM_SEEKING_SEEKING_CAPABILITIES
    {
        CanSeekAbsolute = 0x001,
        CanSeekForwards = 0x002,
        CanSeekBackwards = 0x004,
        CanGetCurrentPos = 0x008,
        CanGetStopPos = 0x010,
        CanGetDuration = 0x020,
        CanPlayBackwards = 0x040,
        CanDoSegments = 0x080,
        Source = 0x100 // Doesn't pass thru used to count segment ends
    }
}