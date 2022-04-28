using System;
using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [ComVisible(false)]
    public class MediaSubType // MEDIASUBTYPE_*
    {
        /// <summary> MEDIASUBTYPE_YUYV 'YUYV' </summary>
        public static readonly Guid YUYV = new Guid(0x56595559, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b,
                                                    0x71);

        /// <summary> MEDIASUBTYPE_IYUV 'IYUV' </summary>
        public static readonly Guid IYUV = new Guid(0x56555949, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b,
                                                    0x71);

        /// <summary> MEDIASUBTYPE_DVSD 'DVSD' </summary>
        public static readonly Guid DVSD = new Guid(0x44535644, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b,
                                                    0x71);

        /// <summary> MEDIASUBTYPE_RGB1 'RGB1' </summary>
        public static readonly Guid RGB1 = new Guid(0xe436eb78, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7,
                                                    0x70);

        /// <summary> MEDIASUBTYPE_RGB4 'RGB4' </summary>
        public static readonly Guid RGB4 = new Guid(0xe436eb79, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7,
                                                    0x70);

        /// <summary> MEDIASUBTYPE_RGB8 'RGB8' </summary>
        public static readonly Guid RGB8 = new Guid(0xe436eb7a, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7,
                                                    0x70);

        /// <summary> MEDIASUBTYPE_RGB565 'RGB565' </summary>
        public static readonly Guid RGB565 = new Guid(0xe436eb7b, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b,
                                                      0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_RGB555 'RGB555' </summary>
        public static readonly Guid RGB555 = new Guid(0xe436eb7c, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b,
                                                      0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_RGB24 'RGB24' </summary>
        public static readonly Guid RGB24 = new Guid(0xe436eb7d, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b,
                                                     0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_RGB32 'RGB32' </summary>
        public static readonly Guid RGB32 = new Guid(0xe436eb7e, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b,
                                                     0xa7, 0x70);


        /// <summary> MEDIASUBTYPE_Avi </summary>
        public static readonly Guid Avi = new Guid(0xe436eb88, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7,
                                                   0x70);

        /// <summary> MEDIASUBTYPE_Asf </summary>
        public static readonly Guid Asf = new Guid(0x3db80f90, 0x9412, 0x11d1, 0xad, 0xed, 0x0, 0x0, 0xf8, 0x75, 0x4b,
                                                   0x99);
    }
}