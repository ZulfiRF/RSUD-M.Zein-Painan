using System;
using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [ComVisible(true), ComImport,
     Guid("329bb360-f6ea-11d1-9038-00a0c9697298"),
     InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IBasicVideo2
    {
        [PreserveSig]
        int AvgTimePerFrame(out double pAvgTimePerFrame);

        [PreserveSig]
        int BitRate(out int pBitRate);

        [PreserveSig]
        int BitErrorRate(out int pBitRate);

        [PreserveSig]
        int VideoWidth(out int pVideoWidth);

        [PreserveSig]
        int VideoHeight(out int pVideoHeight);


        [PreserveSig]
        int put_SourceLeft(int SourceLeft);

        [PreserveSig]
        int get_SourceLeft(out int pSourceLeft);

        [PreserveSig]
        int put_SourceWidth(int SourceWidth);

        [PreserveSig]
        int get_SourceWidth(out int pSourceWidth);

        [PreserveSig]
        int put_SourceTop(int SourceTop);

        [PreserveSig]
        int get_SourceTop(out int pSourceTop);

        [PreserveSig]
        int put_SourceHeight(int SourceHeight);

        [PreserveSig]
        int get_SourceHeight(out int pSourceHeight);


        [PreserveSig]
        int put_DestinationLeft(int DestinationLeft);

        [PreserveSig]
        int get_DestinationLeft(out int pDestinationLeft);

        [PreserveSig]
        int put_DestinationWidth(int DestinationWidth);

        [PreserveSig]
        int get_DestinationWidth(out int pDestinationWidth);

        [PreserveSig]
        int put_DestinationTop(int DestinationTop);

        [PreserveSig]
        int get_DestinationTop(out int pDestinationTop);

        [PreserveSig]
        int put_DestinationHeight(int DestinationHeight);

        [PreserveSig]
        int get_DestinationHeight(out int pDestinationHeight);

        [PreserveSig]
        int SetSourcePosition(int left, int top, int width, int height);

        [PreserveSig]
        int GetSourcePosition(out int left, out int top, out int width, out int height);

        [PreserveSig]
        int SetDefaultSourcePosition();


        [PreserveSig]
        int SetDestinationPosition(int left, int top, int width, int height);

        [PreserveSig]
        int GetDestinationPosition(out int left, out int top, out int width, out int height);

        [PreserveSig]
        int SetDefaultDestinationPosition();


        [PreserveSig]
        int GetVideoSize(out int pWidth, out int pHeight);

        [PreserveSig]
        int GetVideoPaletteEntries(int StartIndex, int Entries, out int pRetrieved, IntPtr pPalette);

        [PreserveSig]
        int GetCurrentImage(ref int pBufferSize, IntPtr pDIBImage);

        [PreserveSig]
        int IsUsingDefaultSource();

        [PreserveSig]
        int IsUsingDefaultDestination();

        [PreserveSig]
        int GetPreferredAspectRatio(out int plAspectX, out int plAspectY);
    }
}