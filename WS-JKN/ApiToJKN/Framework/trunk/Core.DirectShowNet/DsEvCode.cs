namespace Core.DirectShowNET
{
    public enum DsEvCode
    {
        None,
        Complete = 0x01, // EC_COMPLETE
        UserAbort = 0x02, // EC_USERABORT
        ErrorAbort = 0x03, // EC_ERRORABORT
        Time = 0x04, // EC_TIME
        Repaint = 0x05, // EC_REPAINT
        StErrStopped = 0x06, // EC_STREAM_ERROR_STOPPED
        StErrStPlaying = 0x07, // EC_STREAM_ERROR_STILLPLAYING
        ErrorStPlaying = 0x08, // EC_ERROR_STILLPLAYING
        PaletteChanged = 0x09, // EC_PALETTE_CHANGED
        VideoSizeChanged = 0x0a, // EC_VIDEO_SIZE_CHANGED
        QualityChange = 0x0b, // EC_QUALITY_CHANGE
        ShuttingDown = 0x0c, // EC_SHUTTING_DOWN
        ClockChanged = 0x0d, // EC_CLOCK_CHANGED
        Paused = 0x0e, // EC_PAUSED
        OpeningFile = 0x10, // EC_OPENING_FILE
        BufferingData = 0x11, // EC_BUFFERING_DATA
        FullScreenLost = 0x12, // EC_FULLSCREEN_LOST
        Activate = 0x13, // EC_ACTIVATE
        NeedRestart = 0x14, // EC_NEED_RESTART
        WindowDestroyed = 0x15, // EC_WINDOW_DESTROYED
        DisplayChanged = 0x16, // EC_DISPLAY_CHANGED
        Starvation = 0x17, // EC_STARVATION
        OleEvent = 0x18, // EC_OLE_EVENT
        NotifyWindow = 0x19, // EC_NOTIFY_WINDOW
        // EC_ ....

        // DVDevCod.h
        DvdDomChange = 0x101, // EC_DVD_DOMAIN_CHANGE
        DvdTitleChange = 0x102, // EC_DVD_TITLE_CHANGE
        DvdChaptStart = 0x103, // EC_DVD_CHAPTER_START
        DvdAudioStChange = 0x104, // EC_DVD_AUDIO_STREAM_CHANGE

        DvdSubPicStChange = 0x105, // EC_DVD_SUBPICTURE_STREAM_CHANGE
        DvdAngleChange = 0x106, // EC_DVD_ANGLE_CHANGE
        DvdButtonChange = 0x107, // EC_DVD_BUTTON_CHANGE
        DvdValidUopsChange = 0x108, // EC_DVD_VALID_UOPS_CHANGE
        DvdStillOn = 0x109, // EC_DVD_STILL_ON
        DvdStillOff = 0x10a, // EC_DVD_STILL_OFF
        DvdCurrentTime = 0x10b, // EC_DVD_CURRENT_TIME
        DvdError = 0x10c, // EC_DVD_ERROR
        DvdWarning = 0x10d, // EC_DVD_WARNING
        DvdChaptAutoStop = 0x10e, // EC_DVD_CHAPTER_AUTOSTOP
        DvdNoFpPgc = 0x10f, // EC_DVD_NO_FP_PGC
        DvdPlaybRateChange = 0x110, // EC_DVD_PLAYBACK_RATE_CHANGE
        DvdParentalLChange = 0x111, // EC_DVD_PARENTAL_LEVEL_CHANGE
        DvdPlaybStopped = 0x112, // EC_DVD_PLAYBACK_STOPPED
        DvdAnglesAvail = 0x113, // EC_DVD_ANGLES_AVAILABLE
        DvdPeriodAStop = 0x114, // EC_DVD_PLAYPERIOD_AUTOSTOP
        DvdButtonAActivated = 0x115, // EC_DVD_BUTTON_AUTO_ACTIVATED
        DvdCmdStart = 0x116, // EC_DVD_CMD_START
        DvdCmdEnd = 0x117, // EC_DVD_CMD_END
        DvdDiscEjected = 0x118, // EC_DVD_DISC_EJECTED
        DvdDiscInserted = 0x119, // EC_DVD_DISC_INSERTED
        DvdCurrentHmsfTime = 0x11a, // EC_DVD_CURRENT_HMSF_TIME
        DvdKaraokeMode = 0x11b // EC_DVD_KARAOKE_MODE
    }
}