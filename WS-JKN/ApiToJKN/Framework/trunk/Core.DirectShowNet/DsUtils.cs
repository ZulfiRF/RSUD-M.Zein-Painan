/******************************************************
                  DirectShow .NET
		      netmaster@swissonline.ch
*******************************************************/
//					DsUtils
// DirectShow utility classes, partial from the SDK Common sources

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [ComVisible(false)]
    public class DsUtils
    {
        public static bool IsCorrectDirectXVersion()
        {
            return File.Exists(Path.Combine(Environment.SystemDirectory, @"dpnhpast.dll"));
        }


        public static bool ShowCapPinDialog(ICaptureGraphBuilder2 bld, IBaseFilter flt, IntPtr hwnd)
        {
            int hr;
            object comObj = null;
            ISpecifyPropertyPages spec = null;
            var cauuid = new DsCAUUID();

            try
            {
                Guid cat = PinCategory.Capture;
                Guid type = MediaType.Interleaved;
                Guid iid = typeof (IAMStreamConfig).GUID;
                hr = bld.FindInterface(ref cat, ref type, flt, ref iid, out comObj);
                if (hr != 0)
                {
                    type = MediaType.Video;
                    hr = bld.FindInterface(ref cat, ref type, flt, ref iid, out comObj);
                    if (hr != 0)
                        return false;
                }
                spec = comObj as ISpecifyPropertyPages;
                if (spec == null)
                    return false;

                hr = spec.GetPages(out cauuid);
                hr = OleCreatePropertyFrame(hwnd, 30, 30, null, 1,
                                            ref comObj, cauuid.cElems, cauuid.pElems, 0, 0, IntPtr.Zero);
                return true;
            }
            catch (Exception ee)
            {
                Trace.WriteLine("!Ds.NET: ShowCapPinDialog " + ee.Message);
                return false;
            }
            finally
            {
                if (cauuid.pElems != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(cauuid.pElems);

                spec = null;
                if (comObj != null)
                    Marshal.ReleaseComObject(comObj);
                comObj = null;
            }
        }

        public static bool ShowTunerPinDialog(ICaptureGraphBuilder2 bld, IBaseFilter flt, IntPtr hwnd)
        {
            int hr;
            object comObj = null;
            ISpecifyPropertyPages spec = null;
            var cauuid = new DsCAUUID();

            try
            {
                Guid cat = PinCategory.Capture;
                Guid type = MediaType.Interleaved;
                Guid iid = typeof (IAMTVTuner).GUID;
                hr = bld.FindInterface(ref cat, ref type, flt, ref iid, out comObj);
                if (hr != 0)
                {
                    type = MediaType.Video;
                    hr = bld.FindInterface(ref cat, ref type, flt, ref iid, out comObj);
                    if (hr != 0)
                        return false;
                }
                spec = comObj as ISpecifyPropertyPages;
                if (spec == null)
                    return false;

                hr = spec.GetPages(out cauuid);
                hr = OleCreatePropertyFrame(hwnd, 30, 30, null, 1,
                                            ref comObj, cauuid.cElems, cauuid.pElems, 0, 0, IntPtr.Zero);
                return true;
            }
            catch (Exception ee)
            {
                Trace.WriteLine("!Ds.NET: ShowCapPinDialog " + ee.Message);
                return false;
            }
            finally
            {
                if (cauuid.pElems != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(cauuid.pElems);

                spec = null;
                if (comObj != null)
                    Marshal.ReleaseComObject(comObj);
                comObj = null;
            }
        }


        // from 'DShowUtil.cpp'
        public int GetPin(IBaseFilter filter, PinDirection dirrequired, int num, out IPin ppPin)
        {
            ppPin = null;
            int hr;
            IEnumPins pinEnum;
            hr = filter.EnumPins(out pinEnum);
            if ((hr < 0) || (pinEnum == null))
                return hr;

            var pins = new IPin[1];
            int f;
            PinDirection dir;
            do
            {
                hr = pinEnum.Next(1, out pins, out f);
                if ((hr != 0) || (pins[0] == null))
                    break;
                dir = (PinDirection) 3;
                hr = pins[0].QueryDirection(out dir);
                if ((hr == 0) && (dir == dirrequired))
                {
                    if (num == 0)
                    {
                        ppPin = pins[0];
                        pins[0] = null;
                        break;
                    }
                    num--;
                }
                Marshal.ReleaseComObject(pins[0]);
                pins[0] = null;
            } while (hr == 0);

            Marshal.ReleaseComObject(pinEnum);
            pinEnum = null;
            return hr;
        }

        [DllImport("olepro32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int OleCreatePropertyFrame(IntPtr hwndOwner, int x, int y,
                                                         string lpszCaption, int cObjects,
                                                         [In, MarshalAs(UnmanagedType.Interface)] ref object ppUnk,
                                                         int cPages, IntPtr pPageClsID, int lcid, int dwReserved,
                                                         IntPtr pvReserved);
    }


// ---------------------------------------------------------------------------------------


// ---------------------------------------------------------------------------------------


// ---------------------------------------------------------------------------------------


// ---------------------------------------------------------------------------------------


// ---------------------------------- ocidl.idl ------------------------------------------------

// ---------------------------------------------------------------------------------------
}

// namespace DShowNET