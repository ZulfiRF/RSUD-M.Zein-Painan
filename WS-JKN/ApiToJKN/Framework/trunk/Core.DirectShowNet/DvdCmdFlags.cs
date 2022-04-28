using System;

namespace Core.DirectShowNET
{
    [Flags]
    public enum DvdCmdFlags // DVD_CMD_FLAGS
    {
        None = 0x00000000, // DVD_CMD_FLAG_None
        Flush = 0x00000001, // DVD_CMD_FLAG_Flush
        SendEvt = 0x00000002, // DVD_CMD_FLAG_SendEvents
        Block = 0x00000004, // DVD_CMD_FLAG_Block
        StartWRendered = 0x00000008, // DVD_CMD_FLAG_StartWhenRendered
        EndARendered = 0x00000010 // DVD_CMD_FLAG_EndAfterRendered
    }
}