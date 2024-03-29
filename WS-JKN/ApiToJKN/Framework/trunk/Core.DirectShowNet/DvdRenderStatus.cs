/******************************************************
                  DirectShow .NET
		      netmaster@swissonline.ch
*******************************************************/
//					DsDVD
// DVD interfaces, ported from dvdif.idl

using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
// =================================================================================================
//											DVD GRAPH
// =================================================================================================


    [StructLayout(LayoutKind.Sequential, Pack = 1), ComVisible(false)]
    public struct DvdRenderStatus //  AM_DVD_RENDERSTATUS
    {
        public int vpeStatus;
        public bool volInvalid;
        public bool volUnknown;
        public bool noLine21In;
        public bool noLine21Out;
        public int numStreams;
        public int numStreamsFailed;
        public DvdStreamFlags failedStreams;
    }


// ---------------------------------------------------------------------------------------


// =================================================================================================
//											DVD CONTROL
// =================================================================================================


// ---------------------------------------------------------------------------------------


// ---------------------------------------------------------------------------------------


// ---------------------------------------------------------------------------------------


// =================================================================================================
//											DVD INFO
// =================================================================================================


// ---------------------------------------------------------------------------------------


// ---------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------


// ---------------------------------------------------------------------------------------


// ---------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------


// ---------------------------------------------------------------------------------------


// ---------------------------------------------------------------------------------------


// ---------------------------------------------------------------------------------------


// ---------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------


// ---------------------------------------------------------------------------------------


// ---------------------------------------------------------------------------------------


// ---------------------------------------------------------------------------------------
}

// namespace DShowNET.Dvd