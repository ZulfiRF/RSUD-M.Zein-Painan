using Core.Framework.Windows.Barcodes.Rendering;

namespace Core.Framework.Windows.Barcodes
{
    /// <summary>
    /// A smart class to encode some content to a svg barcode image
    /// </summary>
    public class BarcodeWriterSvg : BarcodeWriterGeneric<SvgRenderer.SvgImage>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeWriter"/> class.
        /// </summary>
        public BarcodeWriterSvg()
        {
            Renderer = new SvgRenderer();
        }
    }
}