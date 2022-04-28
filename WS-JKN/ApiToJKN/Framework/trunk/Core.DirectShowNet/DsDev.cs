using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace Core.DirectShowNET
{
    [ComVisible(false)]
    public class DsDev
    {
        public static bool GetDevicesOfCat(Guid cat, out ArrayList devs)
        {
            devs = null;
            int hr;
            object comObj = null;
            ICreateDevEnum enumDev = null;
            UCOMIEnumMoniker enumMon = null;
            var mon = new UCOMIMoniker[1];
            try
            {
                Type srvType = Type.GetTypeFromCLSID(Clsid.SystemDeviceEnum);
                if (srvType == null)
                    throw new NotImplementedException("System Device Enumerator");

                comObj = Activator.CreateInstance(srvType);
                enumDev = (ICreateDevEnum) comObj;
                hr = enumDev.CreateClassEnumerator(ref cat, out enumMon, 0);
                if (hr != 0)
                    throw new NotSupportedException("No devices of the category");

                int f, count = 0;
                do
                {
                    hr = enumMon.Next(1, mon, out f);
                    if ((hr != 0) || (mon[0] == null))
                        break;
                    var dev = new DsDevice();
                    dev.Name = GetFriendlyName(mon[0]);
                    if (devs == null)
                        devs = new ArrayList();
                    dev.Mon = mon[0];
                    mon[0] = null;
                    devs.Add(dev);
                    dev = null;
                    count++;
                } while (true);

                return count > 0;
            }
            catch (Exception)
            {
                if (devs != null)
                {
                    foreach (DsDevice d in devs)
                        d.Dispose();
                    devs = null;
                }
                return false;
            }
            finally
            {
                enumDev = null;
                if (mon[0] != null)
                    Marshal.ReleaseComObject(mon[0]);
                mon[0] = null;
                if (enumMon != null)
                    Marshal.ReleaseComObject(enumMon);
                enumMon = null;
                if (comObj != null)
                    Marshal.ReleaseComObject(comObj);
                comObj = null;
            }
        }

        private static string GetFriendlyName(UCOMIMoniker mon)
        {
            object bagObj = null;
            IPropertyBag bag = null;
            try
            {
                Guid bagId = typeof (IPropertyBag).GUID;
                mon.BindToStorage(null, null, ref bagId, out bagObj);
                bag = (IPropertyBag) bagObj;
                object val = "";
                int hr = bag.Read("FriendlyName", ref val, IntPtr.Zero);
                if (hr != 0)
                    Marshal.ThrowExceptionForHR(hr);
                var ret = val as string;
                if ((ret == null) || (ret.Length < 1))
                    throw new NotImplementedException("Device FriendlyName");
                return ret;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                bag = null;
                if (bagObj != null)
                    Marshal.ReleaseComObject(bagObj);
                bagObj = null;
            }
        }
    }
}