using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [ComVisible(false)]
    public enum AMTunerSubChannel
    {
        NoTune = -2, // AMTUNER_SUBCHAN_NO_TUNE : don't tune
        Default = -1 // AMTUNER_SUBCHAN_DEFAULT : use default sub chan
    }
}