using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace Core.Framework.Helper.Xml
{
    public class XmlHelper
    {
        /// <summary>
        /// method untuk mendapatkan path file image lalu merubahnya ke byte array
        /// </summary>
        /// <param name="pathFile">parameter string path file</param>
        /// <param name="nameTag">parameter string name tag xml</param>
        /// <returns>byte array</returns>
        public static byte[] PathByte(string pathFile, string nameTag)
        {
            try
            {
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
                var xElement = XElement.Load(path + pathFile);
                var element = xElement.Element(nameTag);
                if (element != null)
                {
                    var imageString = element.Value;
                    if (File.Exists(path + imageString))
                        return File.ReadAllBytes(path + imageString);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// method untuk mendapatkan path string di dalam xml
        /// </summary>
        /// <param name="pathFile">parameter string file</param>
        /// <param name="nameTag">parameter string nama tag xml</param>
        /// <returns>string</returns>
        public static string PathString(string pathFile, string nameTag)
        {
            try
            {
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
                var xElemenet = XElement.Load(path + pathFile);
                var element = xElemenet.Element(nameTag);
                if (element != null)
                {
                    var elementValue = element.Value;
                    return elementValue;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
