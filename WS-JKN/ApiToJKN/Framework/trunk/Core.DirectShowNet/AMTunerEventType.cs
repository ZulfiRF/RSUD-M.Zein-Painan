using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [ComVisible(false)]
    public enum AMTunerEventType
    {
        Changed = 0x0001, // AMTUNER_EVENT_CHANGED : status changed
    }
}