using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [StructLayout(LayoutKind.Sequential, Pack = 1), ComVisible(false)]
    public struct DvdMenuAttr // DVD_MenuAttributes
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)] public bool[] compatibleRegion;
        public DvdVideoAttr videoAt; // DVD_VideoAttributes

        public bool audioPresent;
        public DvdAudioAttr audioAt; // DVD_AudioAttributes

        public bool subPicPresent;
        public DvdSubPicAttr subPicAt; // DVD_SubpictureAttributes
    }
}