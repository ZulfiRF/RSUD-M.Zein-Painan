using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using Core.Framework.Helper.Logging;
using Core.Framework.Windows.Helper;

namespace Core.Framework.Windows.Converters
{
    public class ByteToImageConverter
    {
        /// <summary>
        /// byte to image converter
        /// </summary>
        /// <param name="byteArrayIn">parameter array byte</param>
        /// <returns>image</returns>
        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            try
            {
                if (byteArrayIn != null)
                {
                    var ms = new MemoryStream(byteArrayIn);
                    var returnImage = Image.FromStream(ms);
                    return returnImage;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }            
        }

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr value);
        public static BitmapSource GetImageStream(Image myImage)
        {
            try
            {
                var bitmap = new Bitmap(myImage);
                var bmpPt = bitmap.GetHbitmap();
                var bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                       bmpPt,
                       IntPtr.Zero,
                       Int32Rect.Empty,
                       BitmapSizeOptions.FromEmptyOptions());
                bitmapSource.Freeze();
                DeleteObject(bmpPt);
                return bitmapSource;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }
    }
}
