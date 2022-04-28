/******************************************************
                  DirectShow .NET
		      netmaster@swissonline.ch
*******************************************************/
//				UUIDs from uuids.h

using System;
using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [ComVisible(false)]
    public class FilterCategory // uuids.h  :  CLSID_*
    {
        /// <summary> CLSID_AudioInputDeviceCategory, audio capture category </summary>
        public static readonly Guid AudioInputDevice = new Guid(0x33d9a762, 0x90c8, 0x11d0, 0xbd, 0x43, 0x00, 0xa0, 0xc9,
                                                                0x11, 0xce, 0x86);

        /// <summary> CLSID_VideoInputDeviceCategory, video capture category </summary>
        public static readonly Guid VideoInputDevice = new Guid(0x860BB310, 0x5D01, 0x11d0, 0xBD, 0x3B, 0x00, 0xA0, 0xC9,
                                                                0x11, 0xCE, 0x86);
    }
}

// namespace DShowNET