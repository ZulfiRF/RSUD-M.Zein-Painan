using System;
using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [Flags, ComVisible(false)]
    public enum SeekingFlags // AM_SEEKING_SeekingFlags AM_SEEKING_SEEKING_FLAGS
    {
        NoPositioning = 0x00, // No change
        AbsolutePositioning = 0x01, // Position is supplied and is absolute
        RelativePositioning = 0x02, // Position is supplied and is relative
        IncrementalPositioning = 0x03, // (Stop) position relative to current, useful for seeking when paused (use +1)
        PositioningBitsMask = 0x03, // Useful mask
        SeekToKeyFrame = 0x04, // Just seek to key frame (performance gain)
        ReturnTime = 0x08, // Plug the media time equivalents back into the supplied LONGLONGs
        Segment = 0x10, // At end just do EC_ENDOFSEGMENT, don't do EndOfStream
        NoFlush = 0x20 // Don't flush
    }
}