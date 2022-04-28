using System;

namespace Core.DirectShowNET
{
    [Flags]
    public enum DvdGraphFlags // AM_DVD_GRAPH_FLAGS
    {
        Default = 0x00000000,
        HwDecPrefer = 0x00000001, // AM_DVD_HWDEC_PREFER
        HwDecOnly = 0x00000002, // AM_DVD_HWDEC_ONLY
        SwDecPrefer = 0x00000004, // AM_DVD_SWDEC_PREFER
        SwDecOnly = 0x00000008, // AM_DVD_SWDEC_ONLY
        NoVpe = 0x00000100 // AM_DVD_NOVPE
    }
}