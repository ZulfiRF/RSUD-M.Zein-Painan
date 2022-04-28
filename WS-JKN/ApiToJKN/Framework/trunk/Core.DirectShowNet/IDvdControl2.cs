using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [ComVisible(true), ComImport,
     Guid("33BC7430-EEC0-11D2-8201-00A0C9D74842"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvdControl2
    {
        [PreserveSig]
        int PlayTitle(int ulTitle, DvdCmdFlags dwFlags,
                      [Out] OptIDvdCmd ppCmd);

        [PreserveSig]
        int PlayChapterInTitle(int ulTitle, int ulChapter, DvdCmdFlags dwFlags,
                               [Out] OptIDvdCmd ppCmd);

        [PreserveSig]
        int PlayAtTimeInTitle(int ulTitle, [In] ref DvdTimeCode pStartTime, DvdCmdFlags dwFlags,
                              [Out] OptIDvdCmd ppCmd);

        [PreserveSig]
        int Stop();

        [PreserveSig]
        int ReturnFromSubmenu(DvdCmdFlags dwFlags,
                              [Out] OptIDvdCmd ppCmd);

        [PreserveSig]
        int PlayAtTime([In] ref DvdTimeCode pTime, DvdCmdFlags dwFlags,
                       [Out] OptIDvdCmd ppCmd);

        [PreserveSig]
        int PlayChapter(int ulChapter, DvdCmdFlags dwFlags,
                        [Out] OptIDvdCmd ppCmd);

        [PreserveSig]
        int PlayPrevChapter(DvdCmdFlags dwFlags,
                            [Out] OptIDvdCmd ppCmd);

        [PreserveSig]
        int ReplayChapter(DvdCmdFlags dwFlags,
                          [Out] OptIDvdCmd ppCmd);

        [PreserveSig]
        int PlayNextChapter(DvdCmdFlags dwFlags,
                            [Out] OptIDvdCmd ppCmd);

        [PreserveSig]
        int PlayForwards(double dSpeed, DvdCmdFlags dwFlags,
                         [Out] OptIDvdCmd ppCmd);

        [PreserveSig]
        int PlayBackwards(double dSpeed, DvdCmdFlags dwFlags,
                          [Out] OptIDvdCmd ppCmd);

        [PreserveSig]
        int ShowMenu(DvdMenuID MenuID, DvdCmdFlags dwFlags,
                     [Out] OptIDvdCmd ppCmd);

        [PreserveSig]
        int Resume(DvdCmdFlags dwFlags,
                   [Out] OptIDvdCmd ppCmd);

        [PreserveSig]
        int SelectRelativeButton(DvdRelButton buttonDir);

        [PreserveSig]
        int ActivateButton();

        [PreserveSig]
        int SelectButton(int ulButton);

        [PreserveSig]
        int SelectAndActivateButton(int ulButton);

        [PreserveSig]
        int StillOff();

        [PreserveSig]
        int Pause(
            [In, MarshalAs(UnmanagedType.Bool)] bool bState);

        [PreserveSig]
        int SelectAudioStream(int ulAudio, DvdCmdFlags dwFlags,
                              [Out] OptIDvdCmd ppCmd);

        [PreserveSig]
        int SelectSubpictureStream(int ulSubPicture, DvdCmdFlags dwFlags,
                                   [Out] OptIDvdCmd ppCmd);

        [PreserveSig]
        int SetSubpictureState(
            [In, MarshalAs(UnmanagedType.Bool)] bool bState,
            DvdCmdFlags dwFlags,
            [Out] OptIDvdCmd ppCmd);

        [PreserveSig]
        int SelectAngle(int ulAngle, DvdCmdFlags dwFlags,
                        [Out] OptIDvdCmd ppCmd);

        [PreserveSig]
        int SelectParentalLevel(int ulParentalLevel);

        [PreserveSig]
        int SelectParentalCountry(byte[] bCountry);

        [PreserveSig]
        int SelectKaraokeAudioPresentationMode(int ulMode);

        [PreserveSig]
        int SelectVideoModePreference(int ulPreferredDisplayMode);

        [PreserveSig]
        int SetDVDDirectory(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszwPath);

        [PreserveSig]
        int ActivateAtPosition(DsPOINT point);

        [PreserveSig]
        int SelectAtPosition(DsPOINT point);

        [PreserveSig]
        int PlayChaptersAutoStop(int ulTitle, int ulChapter, int ulChaptersToPlay, DvdCmdFlags dwFlags,
                                 [Out] OptIDvdCmd ppCmd);

        [PreserveSig]
        int AcceptParentalLevelChange(
            [In, MarshalAs(UnmanagedType.Bool)] bool bAccept);

        [PreserveSig]
        int SetOption(DvdOptionFlag flag,
                      [In, MarshalAs(UnmanagedType.Bool)] bool fState);

        [PreserveSig]
        int SetState(IDvdState pState, DvdCmdFlags dwFlags,
                     [Out] OptIDvdCmd ppCmd);

        [PreserveSig]
        int PlayPeriodInTitleAutoStop(int ulTitle,
                                      [In] ref DvdTimeCode pStartTime,
                                      [In] ref DvdTimeCode pEndTime,
                                      DvdCmdFlags dwFlags,
                                      [Out] OptIDvdCmd ppCmd);

        [PreserveSig]
        int SetGPRM(int ulIndex, short wValue, DvdCmdFlags dwFlags,
                    [Out] OptIDvdCmd ppCmd);

        [PreserveSig]
        int SelectDefaultMenuLanguage(int Language);

        [PreserveSig]
        int SelectDefaultAudioLanguage(int Language, DvdAudioLangExt audioExtension);

        [PreserveSig]
        int SelectDefaultSubpictureLanguage(int Language, DvdSubPicLangExt subpictureExtension);
    }
}