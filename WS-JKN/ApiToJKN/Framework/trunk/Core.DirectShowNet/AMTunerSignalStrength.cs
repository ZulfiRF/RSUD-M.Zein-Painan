using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [ComVisible(false)]
    public enum AMTunerSignalStrength
    {
        NA = -1, // AMTUNER_HASNOSIGNALSTRENGTH : cannot indicate signal strength
        NoSignal = 0, // AMTUNER_NOSIGNAL : no signal available
        SignalPresent = 1 // AMTUNER_SIGNALPRESENT : signal present
    }
}