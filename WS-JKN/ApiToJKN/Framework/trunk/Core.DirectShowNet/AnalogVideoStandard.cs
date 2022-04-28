using System;
using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [Flags, ComVisible(false)]
    public enum AnalogVideoStandard
    {
        None = 0x00000000, // This is a digital sensor
        NTSC_M = 0x00000001, //        75 IRE Setup
        NTSC_M_J = 0x00000002, // Japan,  0 IRE Setup
        NTSC_433 = 0x00000004,
        PAL_B = 0x00000010,
        PAL_D = 0x00000020,
        PAL_G = 0x00000040,
        PAL_H = 0x00000080,
        PAL_I = 0x00000100,
        PAL_M = 0x00000200,
        PAL_N = 0x00000400,
        PAL_60 = 0x00000800,
        SECAM_B = 0x00001000,
        SECAM_D = 0x00002000,
        SECAM_G = 0x00004000,
        SECAM_H = 0x00008000,
        SECAM_K = 0x00010000,
        SECAM_K1 = 0x00020000,
        SECAM_L = 0x00040000,
        SECAM_L1 = 0x00080000,
        PAL_N_COMBO = 0x00100000 // Argentina
    }
}