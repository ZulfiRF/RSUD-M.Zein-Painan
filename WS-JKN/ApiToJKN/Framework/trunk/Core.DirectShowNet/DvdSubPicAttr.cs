using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [StructLayout(LayoutKind.Sequential, Pack = 1), ComVisible(false)]
    public struct DvdSubPicAttr // DVD_SubpictureAttributes
    {
        public DvdSubPicType type;
        public DvdSubPicCoding coding;
        public int language;
        public DvdSubPicLangExt languageExt;
    }
}