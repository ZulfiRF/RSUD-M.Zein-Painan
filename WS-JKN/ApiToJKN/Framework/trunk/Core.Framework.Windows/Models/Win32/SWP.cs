﻿namespace Core.Framework.Windows.Models.Win32
{
    using System;

    [Flags]
    internal enum SWP
    {
        NOSIZE = 0x1,

        NOMOVE = 0x2,

        NOZORDER = 0x4,

        NOREDRAW = 0x8,

        NOACTIVATE = 0x10,

        FRAMECHANGED = 0x20,

        SHOWWINDOW = 0x0040,

        NOOWNERZORDER = 0x200,

        NOSENDCHANGING = 0x0400,

        ASYNCWINDOWPOS = 0x4000,
    }
}