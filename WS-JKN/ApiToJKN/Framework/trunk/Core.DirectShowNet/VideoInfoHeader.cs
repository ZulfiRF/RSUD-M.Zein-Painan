/******************************************************
                  DirectShow .NET
		      netmaster@swissonline.ch
*******************************************************/
//					QEdit
// Extended streaming interfaces, ported from qedit.idl

using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [StructLayout(LayoutKind.Sequential), ComVisible(false)]
    public class VideoInfoHeader // VIDEOINFOHEADER
    {
        public DsRECT SrcRect;
        public DsRECT TagRect;
        public int BitRate;
        public int BitErrorRate;
        public long AvgTimePerFrame;
        public DsBITMAPINFOHEADER BmiHeader;
    }
}

// namespace DShowNET