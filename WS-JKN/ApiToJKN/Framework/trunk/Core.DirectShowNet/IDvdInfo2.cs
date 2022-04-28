using System;
using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [ComVisible(true), ComImport,
     Guid("34151510-EEC0-11D2-8201-00A0C9D74842"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvdInfo2
    {
        [PreserveSig]
        int GetCurrentDomain([Out] out DvdDomain pDomain);

        [PreserveSig]
        int GetCurrentLocation([Out] out DvdPlayLocation pLocation);

        [PreserveSig]
        int GetTotalTitleTime([Out] out DvdTimeCode pTotalTime, out int ulTimeCodeFlags);

        [PreserveSig]
        int GetCurrentButton(out int pulButtonsAvailable, out int pulCurrentButton);

        [PreserveSig]
        int GetCurrentAngle(out int pulAnglesAvailable, out int pulCurrentAngle);

        [PreserveSig]
        int GetCurrentAudio(out int pulStreamsAvailable, out int pulCurrentStream);

        [PreserveSig]
        int GetCurrentSubpicture(out int pulStreamsAvailable, out int pulCurrentStream,
                                 [Out, MarshalAs(UnmanagedType.Bool)] out bool pbIsDisabled);

        [PreserveSig]
        int GetCurrentUOPS(out int pulUOPs);

        [PreserveSig]
        int GetAllSPRMs(out IntPtr pRegisterArray);

        [PreserveSig]
        int GetAllGPRMs(out IntPtr pRegisterArray);

        [PreserveSig]
        int GetAudioLanguage(int ulStream, out int pLanguage);

        [PreserveSig]
        int GetSubpictureLanguage(int ulStream, out int pLanguage);

        [PreserveSig]
        int GetTitleAttributes(int ulTitle,
                               [Out] out DvdMenuAttr pMenu, IntPtr pTitle);

        // incomplete

        [PreserveSig]
        int GetVMGAttributes([Out] out DvdMenuAttr pATR);

        [PreserveSig]
        int GetCurrentVideoAttributes([Out] out DvdVideoAttr pATR);

        [PreserveSig]
        int GetAudioAttributes(int ulStream, [Out] out DvdAudioAttr pATR);

        [PreserveSig]
        int GetKaraokeAttributes(int ulStream, IntPtr pATR);

        [PreserveSig]
        int GetSubpictureAttributes(int ulStream, [Out] out DvdSubPicAttr pATR);

        [PreserveSig]
        int GetDVDVolumeInfo(out int pulNumOfVolumes, out int pulVolume,
                             out DvdDiscSide pSide, out int pulNumOfTitles);

        [PreserveSig]
        int GetDVDTextNumberOfLanguages(out int pulNumOfLangs);

        [PreserveSig]
        int GetDVDTextLanguageInfo(int ulLangIndex,
                                   out int pulNumOfStrings, out int pLangCode, out DvdCharSet pbCharacterSet);

        [PreserveSig]
        int GetDVDTextStringAsNative(int ulLangIndex, int ulStringIndex,
                                     IntPtr pbBuffer, int ulMaxBufferSize, out int pulActualSize, out int pType);

        [PreserveSig]
        int GetDVDTextStringAsUnicode(int ulLangIndex, int ulStringIndex,
                                      IntPtr pchwBuffer, int ulMaxBufferSize, out int pulActualSize, out int pType);

        [PreserveSig]
        int GetPlayerParentalLevel(out int pulParentalLevel, [Out] byte[] pbCountryCode);

        [PreserveSig]
        int GetNumberOfChapters(int ulTitle, out int pulNumOfChapters);

        [PreserveSig]
        int GetTitleParentalLevels(int ulTitle, out int pulParentalLevels);

        [PreserveSig]
        int GetDVDDirectory(IntPtr pszwPath, int ulMaxSize, out int pulActualSize);

        [PreserveSig]
        int IsAudioStreamEnabled(int ulStreamNum,
                                 [Out, MarshalAs(UnmanagedType.Bool)] out bool pbEnabled);

        [PreserveSig]
        int GetDiscID(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszwPath,
            out long pullDiscID);

        [PreserveSig]
        int GetState(
            [Out] out IDvdState pStateData);

        [PreserveSig]
        int GetMenuLanguages([Out] int[] pLanguages, int ulMaxLanguages, out int pulActualLanguages);

        [PreserveSig]
        int GetButtonAtPosition(DsPOINT point, out int pulButtonIndex);

        [PreserveSig]
        int GetCmdFromEvent(int lParam1,
                            [Out] out IDvdCmd pCmdObj);

        [PreserveSig]
        int GetDefaultMenuLanguage(out int pLanguage);

        [PreserveSig]
        int GetDefaultAudioLanguage(out int pLanguage, out DvdAudioLangExt pAudioExtension);

        [PreserveSig]
        int GetDefaultSubpictureLanguage(out int pLanguage, out DvdSubPicLangExt pSubpictureExtension);

        [PreserveSig]
        int GetDecoderCaps(ref DvdDecoderCaps pCaps);

        [PreserveSig]
        int GetButtonRect(int ulButton, out DsRECT pRect);

        [PreserveSig]
        int IsSubpictureStreamEnabled(int ulStreamNum,
                                      [Out, MarshalAs(UnmanagedType.Bool)] out bool pbEnabled);
    }
}