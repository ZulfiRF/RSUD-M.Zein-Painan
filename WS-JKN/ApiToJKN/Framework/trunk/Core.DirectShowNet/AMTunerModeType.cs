using System;
using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [Flags, ComVisible(false)]
    public enum AMTunerModeType
    {
        Default = 0x0000, // AMTUNER_MODE_DEFAULT : default tuner mode
        TV = 0x0001, // AMTUNER_MODE_TV : tv
        FMRadio = 0x0002, // AMTUNER_MODE_FM_RADIO : fm radio
        AMRadio = 0x0004, // AMTUNER_MODE_AM_RADIO : am radio
        Dss = 0x0008 // AMTUNER_MODE_DSS : dss
    }
}