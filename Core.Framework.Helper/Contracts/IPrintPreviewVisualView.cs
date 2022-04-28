using System.Printing;
using System.Windows;

namespace Core.Framework.Helper.Contracts
{
    public interface IPrintPreviewVisualView
    {

        void Preview(FrameworkElement element, string namaPrinter,
            PageOrientation pageOrientation = PageOrientation.Portrait,
            PageMediaSizeName pageMediaSize = PageMediaSizeName.ISOA4, bool isPriview = false);
    }
}
