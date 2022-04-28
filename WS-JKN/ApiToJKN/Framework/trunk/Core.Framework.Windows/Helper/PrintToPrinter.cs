using System;
using System.ComponentModel;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using Core.Framework.Helper;
using Core.Framework.Helper.Contracts;
using Core.Framework.Helper.Logging;
using Core.Framework.Windows.Contracts;

namespace Core.Framework.Windows.Helper
{
    public class PrintToPrinter
    {
        #region PrintVisualToOwnPrinter

        /// <summary>
        /// Untuk mencetak ke printer local
        /// </summary>
        /// <param name="userControl">parameter FrameworkElement view</param>
        /// <param name="printerName">parameter string nama printer</param>
        /// <param name="titlePrint">parameter string title</param>
        /// <param name="width">parameter int width</param>
        /// <param name="height">parameter int height</param>
        /// <param name="copyCount">parameter int number of copies</param>
        /// <param name="lanscape"> </param>
        public static void PrintVisualToPrinterLocal(FrameworkElement userControl, string printerName, string titlePrint = null, int width = 0, int height = 0, int copyCount = 1, bool lanscape = false)
        {
            try
            {
                var dialog = new PrintDialog { PageRangeSelection = PageRangeSelection.AllPages, UserPageRangeEnabled = true };
                var printQueue = new LocalPrintServer().GetPrintQueue(printerName);
                dialog.PrintQueue = printQueue;
                dialog.PrintTicket.CopyCount = copyCount;
                if (lanscape)
                    dialog.PrintTicket.PageOrientation = PageOrientation.Landscape;
                Mouse.OverrideCursor = Cursors.Wait;
                userControl.UpdateLayout();

                var sz = new Size(width, height);
                if (width == 0 && height == 0)
                    sz = new Size(userControl.Margin.Left + userControl.ActualWidth, userControl.Margin.Top + userControl.ActualHeight);
                userControl.Measure(sz);
                userControl.Arrange(new Rect(new Point(0, 0), sz));

                dialog.PrintVisual(userControl, titlePrint);
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            catch (Exception e)
            {
                Log.Error(e);
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }
        #endregion

        #region PrintVisualToNetworkPrinter

        /// <summary>
        /// Untuk mencetak ke printer ke IP Address Lain
        /// </summary>
        /// <param name="userControl">parameter FrameworkElement view</param>
        /// <param name="printerAddress">parameter string IP Address, Contoh : \\192.168.0.20</param>
        /// <param name="printerName">parameter string nama printer</param>
        /// <param name="titlePrint">parameter string title</param>
        /// <param name="width">parameter int width</param>
        /// <param name="height">parameter int height</param>
        /// <param name="copyCount">parameter int number of copies</param>
        /// <param name="lanscape"> </param>
        public static void PrintVisualToPrinterNetwork(FrameworkElement userControl, string printerAddress, string printerName, string titlePrint = null, int width = 0, int height = 0, int copyCount = 1, bool lanscape = false)
        {
            try
            {
                var dialog = new PrintDialog { PageRangeSelection = PageRangeSelection.AllPages, UserPageRangeEnabled = true };
                var printServer = new PrintServer(printerAddress);
                var printQueue = printServer.GetPrintQueue(printerName);
                dialog.PrintQueue = printQueue;
                dialog.PrintTicket.CopyCount = copyCount;
                if (lanscape)
                    dialog.PrintTicket.PageOrientation = PageOrientation.Landscape;
                Mouse.OverrideCursor = Cursors.Wait;
                userControl.UpdateLayout();

                var sz = new Size(width, height);
                userControl.Measure(sz);
                userControl.Arrange(new Rect(new Point(0, 0), sz));

                dialog.PrintVisual(userControl, titlePrint);
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            catch (Exception e)
            {
                Log.Error(e);
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }
        #endregion

        #region PrintVisualToAnyPrinter
        /// <summary>
        /// Untuk mencetak printer dengan mendetect IP Networknya
        /// </summary>
        /// <param name="userControl">parameter FrameworkElement</param>
        /// <param name="printerName">parameter string nama printer</param>
        /// <param name="titlePrint">parameter string title</param>
        /// <param name="width">parameter int panjang kertas</param>
        /// <param name="height">parameter int lebar kertas</param>
        /// <param name="copyCount">parameter int number of copy</param>
        /// <param name="lanscape">parameter bool lanscape / potret</param>
        public static void PrintVisualToAnyPrinter(FrameworkElement userControl, string printerName, string titlePrint = null, int width = 0, int height = 0, int copyCount = 1, bool lanscape = false)
        {
            try
            {
                var pName = printerName;
                string pAddress = null;
                var isSplits = printerName.Contains("\\");
                if (isSplits)
                {
                    var splits = printerName.Split('\\');
                    var addressAndName = splits.Where(n => !string.IsNullOrEmpty(n)).ToArray();
                    pAddress = addressAndName[0];
                    pName = addressAndName[1];
                }
                if (width == 0 && height == 0)
                {
                    if (pAddress != null)
                        PrintVisualToPrinterNetwork(userControl, pAddress, pName, titlePrint, width, height, copyCount, lanscape);
                    else
                        PrintVisualToPrinterLocal(userControl, pName, titlePrint, width, height, copyCount, lanscape);
                }
                else
                {
                    if (pAddress != null)
                        PrintVisualToPrinterNetwork(userControl, pAddress, pName, titlePrint, width, height, copyCount, lanscape);
                    else
                        PrintVisualToPrinterLocal(userControl, pName, titlePrint, width, height, copyCount, lanscape);
                }

            }
            catch (Exception e)
            {
                Log.Error(e);
                Mouse.OverrideCursor = Cursors.Arrow;
            }

        }
        #endregion

        /// <summary>
        /// Untuk mencetak lebih dari satu lembar kertas
        /// </summary>
        /// <param name="element">FrameworkElement</param>
        /// <param name="namaPrinter">Nama printer</param>
        /// <param name="pageOrientation">PageOrientation</param>
        /// <param name="pageMediaSize">Jenis Kertas</param>
        /// <param name="isPriview"></param>
        /// <param name="form"></param>
        public static void PrintContinuesPage(FrameworkElement element, string namaPrinter, PageOrientation pageOrientation = PageOrientation.Portrait, PageMediaSizeName pageMediaSize = PageMediaSizeName.ISOA4, bool isPriview = false, object form = null)
        {
            if (isPriview)
            {
                var common = BaseDependency.Get<ICommonModulePrintPreviewVisual>();                
                common.InitView(form as FrameworkElement, new object[]
                {
                    element, namaPrinter, pageOrientation, pageMediaSize
                });
            }
            else
            {
                var dialog = new PrintDialog { PageRangeSelection = PageRangeSelection.AllPages, UserPageRangeEnabled = true };
                var printQueue = new LocalPrintServer().GetPrintQueue(namaPrinter);
                dialog.PrintQueue = printQueue;
                dialog.PrintTicket.PageOrientation = pageOrientation;
                dialog.PrintTicket.PageMediaSize = new PageMediaSize(pageMediaSize);

                var fixedDoc = GetFixedDocument(element, dialog, new Thickness(0, 0, 0, 0));
                dialog.PrintDocument(fixedDoc.DocumentPaginator, "");                
            }
        }

        private static FixedDocument GetFixedDocument(FrameworkElement toPrint, PrintDialog printDialog, Thickness margin)
        {
            var capabilities = printDialog.PrintQueue.GetPrintCapabilities(printDialog.PrintTicket);
            var pageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);
            var visibleSize = new Size(capabilities.PageImageableArea.ExtentWidth - margin.Left - margin.Right, capabilities.PageImageableArea.ExtentHeight - margin.Top - margin.Bottom);
            var fixedDoc = new FixedDocument();
            toPrint.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            toPrint.Arrange(new Rect(new Point(0, 0), toPrint.DesiredSize));
            var size = toPrint.DesiredSize;
            double yOffset = 0;
            while (yOffset < size.Height)
            {
                var vb = new VisualBrush(toPrint);
                vb.Stretch = Stretch.None;
                vb.AlignmentX = AlignmentX.Left;
                vb.AlignmentY = AlignmentY.Top;
                vb.ViewboxUnits = BrushMappingMode.Absolute;
                vb.TileMode = TileMode.None;
                vb.Viewbox = new Rect(0, yOffset, visibleSize.Width, visibleSize.Height);

                var pageContent = new PageContent();
                var page = new FixedPage();
                ((IAddChild)pageContent).AddChild(page);
                fixedDoc.Pages.Add(pageContent);
                page.Width = pageSize.Width;
                page.Height = pageSize.Height;
                var canvas = new Canvas();
                FixedPage.SetLeft(canvas, capabilities.PageImageableArea.OriginWidth);
                FixedPage.SetTop(canvas, capabilities.PageImageableArea.OriginHeight);
                canvas.Width = visibleSize.Width;
                canvas.Height = visibleSize.Height;
                canvas.Background = vb;

                page.Children.Add(canvas);
                yOffset += visibleSize.Height;
            }
            return fixedDoc;
        }
    }
}
