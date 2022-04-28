using System;

namespace Core.DirectShowNET
{
    [Flags]
    internal enum CLSCTX
    {
        Inproc = 0x03,
        Server = 0x15,
        All = 0x17,
    }
}