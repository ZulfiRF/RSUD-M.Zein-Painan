using System;
using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [ComVisible(true), ComImport,
     Guid("211A8766-03AC-11d1-8D13-00AA00BD8339"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMTVTuner
    {
        #region "IAMTuner Methods"

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

        #endregion

        [PreserveSig]
        int get_AvailableTVFormats(out AnalogVideoStandard lAnalogVideoStandard);

        [PreserveSig]
        int get_TVFormat(out AnalogVideoStandard lAnalogVideoStandard);

        [PreserveSig]
        int AutoTune(int lChannel, out int plFoundSignal);

        [PreserveSig]
        int StoreAutoTune();

        [PreserveSig]
        int get_NumInputConnections(out int plNumInputConnections);

        [PreserveSig]
        int put_InputType(int lIndex, TunerInputType inputType);

        [PreserveSig]
        int get_InputType(int lIndex, out TunerInputType inputType);

        [PreserveSig]
        int put_ConnectInput(int lIndex);

        [PreserveSig]
        int get_ConnectInput(out int lIndex);

        [PreserveSig]
        int get_VideoFrequency(out int lFreq);

        [PreserveSig]
        int get_AudioFrequency(out int lFreq);
    }
}