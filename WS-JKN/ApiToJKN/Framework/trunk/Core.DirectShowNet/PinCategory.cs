using System;
using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [ComVisible(false)]
    public class PinCategory // PIN_CATEGORY_*
    {
        /// <summary> PIN_CATEGORY_CAPTURE </summary>
        public static readonly Guid Capture = new Guid(0xfb6c4281, 0x0353, 0x11d1, 0x90, 0x5f, 0x00, 0x00, 0xc0, 0xcc,
                                                       0x16, 0xba);

        /// <summary> PIN_CATEGORY_PREVIEW </summary>
        public static readonly Guid Preview = new Guid(0xfb6c4282, 0x0353, 0x11d1, 0x90, 0x5f, 0x00, 0x00, 0xc0, 0xcc,
                                                       0x16, 0xba);
    }
}