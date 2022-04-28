using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;
using Core.Framework.Windows.Barcodes;
using Core.Framework.Windows.Barcodes.Aztec;
using Core.Framework.Windows.Barcodes.Common;
using Core.Framework.Windows.Barcodes.Datamatrix;
using Core.Framework.Windows.Barcodes.OneD;
using Core.Framework.Windows.Barcodes.PDF417;
using Core.Framework.Windows.Barcodes.QrCode;
using Core.Framework.Helper.Logging;

namespace Core.Framework.Windows.Helper
{
    public class BarcodeGenerator
    {

        #region GenerateBarcode
        /// <summary>
        /// method untuk menghasilkan barcode
        /// </summary>
        /// <param name="values">parameter string nilai yang akan dijadikan barcode</param>
        /// <param name="typeBarcode">parameter enum TypeBarcode</param>
        /// <param name="widht">parameter int panjang barcode</param>
        /// <param name="height">parameter int lebar barcode</param>
        /// <returns>bitmap</returns>
        public static Image CodeGenerate(string values, TypeBarcode typeBarcode, int widht = 0, int height = 0)
        {
            try
            {
                if (widht == 0) widht = 100;
                if (height == 0) height = 100;
                OneDimensionalCodeWriter oneDimensionalCodeWriter;
                BitMatrix result = null;
                switch (typeBarcode)
                {
                    case TypeBarcode.CODE_128:
                        oneDimensionalCodeWriter = new Code128Writer();
                        result = oneDimensionalCodeWriter.encode(values, BarcodeFormat.CODE_128, widht, height);
                        break;
                    case TypeBarcode.CODE_39:
                        oneDimensionalCodeWriter = new Code39Writer();
                        result = oneDimensionalCodeWriter.encode(values, BarcodeFormat.CODE_39, widht, height);
                        break;
                    case TypeBarcode.EAN_13:
                        oneDimensionalCodeWriter = new EAN13Writer();
                        result = oneDimensionalCodeWriter.encode(values, BarcodeFormat.EAN_13, widht, height);
                        break;
                    case TypeBarcode.EAN_8:
                        oneDimensionalCodeWriter = new EAN8Writer();
                        result = oneDimensionalCodeWriter.encode(values, BarcodeFormat.EAN_8, widht, height);
                        break;
                    case TypeBarcode.CODABAR:
                        oneDimensionalCodeWriter = new CodaBarWriter();
                        result = oneDimensionalCodeWriter.encode(values, BarcodeFormat.CODABAR, widht, height);
                        break;

                    case TypeBarcode.ITF:
                        oneDimensionalCodeWriter = new ITFWriter();
                        result = oneDimensionalCodeWriter.encode(values, BarcodeFormat.ITF, widht, height);
                        break;
                    case TypeBarcode.MSI:
                        oneDimensionalCodeWriter = new MSIWriter();
                        result = oneDimensionalCodeWriter.encode(values, BarcodeFormat.MSI, widht, height);
                        break;
                    case TypeBarcode.PLESSEY:
                        oneDimensionalCodeWriter = new PlesseyWriter();
                        result = oneDimensionalCodeWriter.encode(values, BarcodeFormat.PLESSEY, widht, height);
                        break;
                    case TypeBarcode.AZTEC:
                        var aztecWriter = new AztecWriter();
                        result = aztecWriter.encode(values, BarcodeFormat.AZTEC, widht, height);
                        break;
                    case TypeBarcode.DATA_MATRIX:
                        var dataMatrixWriter = new DataMatrixWriter();
                        result = dataMatrixWriter.encode(values, BarcodeFormat.DATA_MATRIX, widht, height);
                        break;
                    case TypeBarcode.PDF_417:
                        var pdf417Writer = new PDF417Writer();
                        result = pdf417Writer.encode(values, BarcodeFormat.PDF_417, widht, height);
                        break;
                    case TypeBarcode.QR_CODE:
                        var qrCodeWriter = new QRCodeWriter();
                        result = qrCodeWriter.encode(values, BarcodeFormat.QR_CODE, widht, height);
                        break;
                }
                var barcodeWriter = new BarcodeWriter();
                return barcodeWriter.Write(result);
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
             

        }
        #endregion
        
        #region EnumTypeOfBarcode
        public enum TypeBarcode
        {
            AZTEC,
            CODABAR,
            CODE_39,
            CODE_128,
            DATA_MATRIX,
            EAN_8,
            EAN_13,
            ITF,
            PDF_417,
            QR_CODE,
            MSI,
            PLESSEY
        }
        #endregion

        #region GetImageStream
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
        #endregion        
    }
}
