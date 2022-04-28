using System;
using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [ComVisible(true), ComImport,
     Guid("211A8761-03AC-11d1-8D13-00AA00BD8339"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMTuner
    {
        [PreserveSig]
        int put_Channel(int lChannel, AMTunerSubChannel lVideoSubChannel, AMTunerSubChannel lAudioSubChannel);

        [PreserveSig]
        int get_Channel(out int plChannel, out int plVideoSubChannel, out int plAudioSubChannel);

        [PreserveSig]
        int ChannelMinMax(out int lChannelMin, out int lChannelMax);

        [PreserveSig]
        int put_CountryCode(int lCountryCode);

        [PreserveSig]
        int get_CountryCode(out int plCountryCode);

        [PreserveSig]
        int put_TuningSpace(int lTuningSpace);

        [PreserveSig]
        int get_TuningSpace(out int plTuningSpace);

        [PreserveSig]
        int Logon(IntPtr hCurrentUser);

        [PreserveSig]
        int Logout();

        [PreserveSig]
        int SignalPresent(out AMTunerSignalStrength plSignalStrength);

        [PreserveSig]
        int put_Mode(AMTunerModeType lMode);

        [PreserveSig]
        int get_Mode(out AMTunerModeType plMode);

        [PreserveSig]
        int GetAvailableModes(out AMTunerModeType plModes);

        [PreserveSig]
        int RegisterNotificationCallBack(IAMTunerNotification pNotify, AMTunerEventType lEvents);

        [PreserveSig]
        int UnRegisterNotificationCallBack(IAMTunerNotification pNotify);
    }
}