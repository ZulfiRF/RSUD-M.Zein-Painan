using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Core.DirectShowNET;
using Core.Framework.Helper;
using Core.Framework.Helper.Contracts;
using Core.Framework.Windows.Helper;
using Button = System.Windows.Forms.Button;
using MessageBox = System.Windows.MessageBox;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Rectangle = System.Drawing.Rectangle;
using UserControl = System.Windows.Controls.UserControl;

namespace Core.DirectShowNet.Forms
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class StreamVideoForm : UserControl, ISampleGrabberCB
    {
        /// <summary> list of installed video devices. </summary>
        private const int WM_GRAPHNOTIFY = 0x00008001; // message from graph

        private const int WS_CHILD = 0x40000000; // attributes for video window
        private const int WS_CLIPCHILDREN = 0x02000000;
        private const int WS_CLIPSIBLINGS = 0x04000000;
        private readonly object m_cslock = new object();
        private readonly AutoResetEvent m_evCapture = new AutoResetEvent(false);
        private readonly AutoResetEvent m_evReady = new AutoResetEvent(false);

        /// <summary> grabber filter interface. </summary>
        private IBaseFilter baseGrabFlt;

        private int bufferedSize;

        private Button button;

        private ArrayList capDevices;

        /// <summary> base filter of the actually used video devices. </summary>
        private IBaseFilter capFilter;

        /// <summary> capture graph builder interface. </summary>
        private ICaptureGraphBuilder2 capGraph;

        private bool captured = true;
        private DsDevice dev;
        private bool firstActive;

        /// <summary> graph builder interface. </summary>
        private IGraphBuilder graphBuilder;

        private int m_nBitCount = 32;

        /// <summary> control interface. </summary>
        private IMediaControl mediaCtrl;

        /// <summary> event interface. </summary>
        private IMediaEventEx mediaEvt;

        private ISampleGrabber sampGrabber;

        /// <summary> buffer for bitmap data. </summary>
        private byte[] savedArray;

        /// <summary> structure describing the bitmap to grab. </summary>
        private VideoInfoHeader videoInfoHeader;

        /// <summary> video window interface. </summary>
        private IVideoWindow videoWin;
        public StreamVideoForm()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        #region ISampleGrabberCB Members

        public int SampleCB(double SampleTime, IMediaSample pSample)
        {
            return 0;
        }

        public int BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
        {
            try
            {
                if (captured || (savedArray == null))
                {
                    Trace.WriteLine("!!CB: ISampleGrabberCB.BufferCB");
                    return 0;
                }

                captured = true;
                bufferedSize = BufferLen;
                Trace.WriteLine("!!CB: ISampleGrabberCB.BufferCB  !GRAB! size = " + BufferLen.ToString());
                if ((pBuffer != IntPtr.Zero) && (BufferLen > 1000) && (BufferLen <= savedArray.Length))
                    Marshal.Copy(pBuffer, savedArray, 0, BufferLen);
                else
                    Trace.WriteLine("    !!!GRAB! failed ");
                OnCaptureDone();
            }
            catch (Exception)
            {
            }


            return 0;
        }
        public class StreamItem
        {
            public byte[] Bytes { get; set; }
            public Bitmap Bitmap { get; set; }
        }
        public event EventHandler<ItemEventArgs<StreamItem>> Buffered;

        public void OnBuffered(object e)
        {
            var evt = e as ItemEventArgs<StreamItem>;
            EventHandler<ItemEventArgs<StreamItem>> handler = Buffered;
            if (handler != null) handler(this, evt);
        }

        private int loopRead = 0;
        private void OnCaptureDone()
        {
            Trace.WriteLine("!!DLG: OnCaptureDone");
            try
            {
                int hr;
                if (sampGrabber == null)
                    return;
                //Manager.Timeout(Dispatcher, () =>
                //{


                //        hr = sampGrabber.SetCallback(null, 0);

                int w = videoInfoHeader.BmiHeader.Width;
                int h = videoInfoHeader.BmiHeader.Height;
                if (((w & 0x03) != 0) || (w < 32) || (w > 4096) || (h < 32) || (h > 4096))
                    return;
                int stride = w * 3;

                GCHandle handle = GCHandle.Alloc(savedArray, GCHandleType.Pinned);
                var scan0 = (int)handle.AddrOfPinnedObject();
                scan0 += (h - 1) * stride;
                var b = new Bitmap(w, h, -stride, PixelFormat.Format24bppRgb, (IntPtr)scan0);
                currentBitmap = (Bitmap)b.Clone();
                var args = new ItemEventArgs<StreamItem>(new StreamItem()
                                                             {
                                                                 Bytes = savedArray,
                                                                 Bitmap = b
                                                             });

                CallBack(args);


                handle.Free();
                savedArray = null;
                //                });
            }
            catch (Exception exception)
            {
                Manager.HandleException(exception);
            }
        }

        private Bitmap currentBitmap;
        public Bitmap CurrentImage { get { return currentBitmap; } }
        private void CallBack(object state)
        {
            try
            {


                var args = state as ItemEventArgs<StreamItem>;
                if (IpAddress == null)
                {
                    var contextSetting = BaseDependency.Get<ISetting>();
                    if (contextSetting == null)
                        return;
                    IpAddress = new List<IPEndPoint>();
                    foreach (var value in contextSetting.GetValues("Client-Address"))
                    {
                        var arr = value.Split(':');
                        IpAddress.Add(new IPEndPoint(IPAddress.Parse(arr[0]), Convert.ToInt16(arr[1])));
                    }
                }

                if (IpAddress != null)
                {
                    var data = new List<IPEndPoint>(IpAddress);
                    var jgpEncoder = GetEncoder(ImageFormat.Jpeg);

                    System.Drawing.Imaging.Encoder myEncoder =
                        System.Drawing.Imaging.Encoder.Quality;
                    var myEncoderParameters = new EncoderParameters(1);
                    var contras = 1L * QualityImage;
                    var myEncoderParameter = new EncoderParameter(myEncoder, contras);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    var stream = new MemoryStream();
                    if (args != null)
                        if (jgpEncoder != null) args.Item.Bitmap.Save(stream, jgpEncoder, myEncoderParameters);
                    byte[] byteArray = stream.ToArray();
                    stream.Dispose();
                    stream = null;
                    if (args != null)
                    {
                        args.Item.Bytes = byteArray;
                        OnBuffered(args);
                    }
                    foreach (var ipAddres in data)
                    {
                        // Thread.Sleep(1);
                        SendData(new object[] { byteArray, ipAddres });

                    }
                    args.Item.Bitmap = null;
                    args.Item.Bytes = null;

                }
            }
            catch (Exception)
            {
            }
        }
        private static ManualResetEvent connectDone =
       new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private void SendData(object state)
        {
            try
            {

                var arr = state as object[];
                if (arr != null)
                {
                    byte[] byteArray = arr[0] as byte[];
                    var ipAddres = arr[1] as IPEndPoint;
                    var remoteEndPoint = ipAddres;
                    var server = new Socket(AddressFamily.InterNetwork,
                                            SocketType.Dgram, ProtocolType.Udp);
                    server.BeginConnect(ipAddres,
               new AsyncCallback(ConnectCallback), server);
                    connectDone.WaitOne();
                    if (byteArray != null)
                    {


                        if (remoteEndPoint != null)
                        {

                            // Begin sending the data to the remote device.
                            server.BeginSend(byteArray, 0, byteArray.Length, 0,
                                new AsyncCallback(SendCallback), server);
                            //server.SendTo(byteArray, byteArray.Length, SocketFlags.None, remoteEndPoint);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

        }
        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        protected int Port { get; set; }

        protected List<IPEndPoint> IpAddress { get; set; }

        #endregion
        public static List<string> GetDevice
        {
            get
            {
                var result = new List<string>();
                try
                {
                    ArrayList data;
                    if (!DsDev.GetDevicesOfCat(FilterCategory.VideoInputDevice, out data))
                    {
                        Manager.HandleException(new Exception("No video capture devices found!"), "DirectShow.NET");
                    }
                    else
                        foreach (dynamic item in data)
                        {
                            result.Add(item.Name);
                        }
                }
                catch (Exception)
                {
                }

                return result;
            }
        }
        private DispatcherTimer timer;
        public string DeviceName { get; set; }
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {


                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(100);
                timer.Tick += TimerOnTick;
                currentWindow.SizeChanged += WindowsFormsHost1OnSizeChanged;
                button = new Button();
                button.Text = "poerasdas";
                button.Margin = new Padding(0);
                currentWindow.Child = button;
                if (!DsUtils.IsCorrectDirectXVersion())
                {
                    Manager.HandleException(new Exception("DirectX 8.1 NOT installed!"), "DirectShow.NET");
                    return;
                }

                if (!DsDev.GetDevicesOfCat(FilterCategory.VideoInputDevice, out capDevices))
                {
                    Manager.HandleException(new Exception("No video capture devices found!"), "DirectShow.NET");
                    return;
                }

                if (string.IsNullOrEmpty(DeviceName))
                    dev = capDevices[0] as DsDevice;
                else
                {
                    foreach (dynamic device in capDevices)
                    {
                        if (device.Name.ToString().Equals(DeviceName))
                        {
                            dev = device as DsDevice;
                        }
                    }
                }


                if (dev == null)
                {

                    return;
                }

                if (!StartupVideo(dev.Mon))
                    return;
            }
            catch (Exception)
            {
            }
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            try
            {
                Grab();
            }
            catch (Exception)
            {
            }
        }

        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (value)
                {
                    if (mediaCtrl != null)
                        mediaCtrl.Run();
                    timer.Start();
                }
                else
                {
                    if (mediaCtrl != null)
                        mediaCtrl.Stop();
                    timer.Stop();
                }

            }
        }
        private void WindowsFormsHost1OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            try
            {
                currentWindow.Child = null;
                button.Width = Convert.ToInt16(sizeChangedEventArgs.NewSize.Width);
                button.Height = Convert.ToInt16(sizeChangedEventArgs.NewSize.Height);
                currentWindow.Child = button;
                ResizeVideoWindow();
            }
            catch (Exception)
            {
            }
        }

        private bool CreateCaptureDevice(UCOMIMoniker mon)
        {
            object capObj = null;
            try
            {
                Guid gbf = typeof(IBaseFilter).GUID;
                mon.BindToObject(null, null, ref gbf, out capObj);
                capFilter = (IBaseFilter)capObj;
                capObj = null;
                return true;
            }
            catch (Exception exception)
            {
                Manager.HandleException(exception, "Create Capture Device");
                return false;
            }
            finally
            {
                if (capObj != null)
                    Marshal.ReleaseComObject(capObj);
                capObj = null;
            }
        }

        /// <summary> create the used COM components and get the interfaces. </summary>
        private bool GetInterfaces()
        {
            Type comType = null;
            object comObj = null;
            try
            {
                comType = Type.GetTypeFromCLSID(Clsid.FilterGraph);
                if (comType == null)
                    throw new NotImplementedException(@"DirectShow FilterGraph not installed/registered!");
                comObj = Activator.CreateInstance(comType);
                graphBuilder = (IGraphBuilder)comObj;
                comObj = null;

                Guid clsid = Clsid.CaptureGraphBuilder2;
                Guid riid = typeof(ICaptureGraphBuilder2).GUID;
                comObj = DsBugWO.CreateDsInstance(ref clsid, ref riid);
                capGraph = (ICaptureGraphBuilder2)comObj;
                comObj = null;

                comType = Type.GetTypeFromCLSID(Clsid.SampleGrabber);
                if (comType == null)
                    throw new NotImplementedException(@"DirectShow SampleGrabber not installed/registered!");
                comObj = Activator.CreateInstance(comType);
                sampGrabber = (ISampleGrabber)comObj;
                comObj = null;

                mediaCtrl = (IMediaControl)graphBuilder;
                videoWin = (IVideoWindow)graphBuilder;
                mediaEvt = (IMediaEventEx)graphBuilder;
                baseGrabFlt = (IBaseFilter)sampGrabber;
                return true;
            }
            catch (Exception exception)
            {
                Manager.HandleException(exception, "Get Interface");
                return false;
            }
            finally
            {
                if (comObj != null)
                    Marshal.ReleaseComObject(comObj);
                comObj = null;
            }
        }


        /// <summary> build the capture graph for grabber. </summary>
        private bool SetupGraph()
        {
            int hr;
            try
            {
                hr = capGraph.SetFiltergraph(graphBuilder);
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                hr = graphBuilder.AddFilter(capFilter, "Ds.NET Video Capture Device");
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                //   DsUtils.ShowCapPinDialog(capGraph, capFilter, Handle);

                var media = new AMMediaType();
                media.majorType = MediaType.Video;
                media.subType = MediaSubType.RGB24;
                media.formatType = FormatType.VideoInfo; // ???
                hr = sampGrabber.SetMediaType(media);
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                hr = graphBuilder.AddFilter(baseGrabFlt, "Ds.NET Grabber");
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                Guid cat = PinCategory.Preview;
                Guid med = MediaType.Video;
                hr = capGraph.RenderStream(ref cat, ref med, capFilter, null, null); // baseGrabFlt 
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                cat = PinCategory.Capture;
                med = MediaType.Video;
                hr = capGraph.RenderStream(ref cat, ref med, capFilter, null, baseGrabFlt); // baseGrabFlt 
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                media = new AMMediaType();
                hr = sampGrabber.GetConnectedMediaType(media);
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);
                if ((media.formatType != FormatType.VideoInfo) || (media.formatPtr == IntPtr.Zero))
                    throw new NotSupportedException("Unknown Grabber Media Format");

                videoInfoHeader = (VideoInfoHeader)Marshal.PtrToStructure(media.formatPtr, typeof(VideoInfoHeader));
                Marshal.FreeCoTaskMem(media.formatPtr);
                media.formatPtr = IntPtr.Zero;

                hr = sampGrabber.SetBufferSamples(false);
                if (hr == 0)
                    hr = sampGrabber.SetOneShot(false);
                if (hr == 0)
                    hr = sampGrabber.SetCallback(null, 0);
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                return true;
            }
            catch (Exception exception)
            {
                Manager.HandleException(exception, "Start Up Graph");
                return false;
            }
        }

        /// <summary> make the video preview window to show in videoPanel. </summary>
        private bool SetupVideoWindow()
        {
            int hr;
            try
            {
                // Set the video window to be a child of the main window
                hr = videoWin.put_Owner(button.Handle);
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                // Set video window style
                hr = videoWin.put_WindowStyle(WS_CHILD | WS_CLIPCHILDREN);
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                // Use helper function to position video window in client rect of owner window
                ResizeVideoWindow();

                // Make the video window visible, now that it is properly positioned
                hr = videoWin.put_Visible(DsHlp.OATRUE);
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                hr = mediaEvt.SetNotifyWindow(button.Handle, WM_GRAPHNOTIFY, IntPtr.Zero);
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);
                return true;
            }
            catch (Exception exception)
            {
                Manager.HandleException(exception, "Setup Up Video Window");
                return false;
            }
        }

        /// <summary> resize preview video window to fill client area. </summary>
        private void ResizeVideoWindow()
        {
            if (videoWin != null)
            {
                Rectangle rc = button.ClientRectangle;
                videoWin.SetWindowPosition(0, 0, rc.Right, rc.Bottom);
            }
        }

        /// <summary> start all the interfaces, graphs and preview window. </summary>
        private bool StartupVideo(UCOMIMoniker mon)
        {
            int hr;
            try
            {
                if (!CreateCaptureDevice(mon))
                    return false;

                if (!GetInterfaces())
                    return false;

                if (!SetupGraph())
                    return false;

                if (!SetupVideoWindow())
                    return false;

                //#if DEBUG
                //                DsROT.AddGraphToRot(graphBuilder, out rotCookie); // graphBuilder capGraph
                //#endif


                //if (hr < 0)
                //    Marshal.ThrowExceptionForHR(hr);

                //bool hasTuner = DsUtils.ShowTunerPinDialog(capGraph, capFilter, button.Handle);
                //    toolBarBtnTune.Enabled = hasTuner;

                return true;
            }
            catch (Exception exception)
            {
                Manager.HandleException(exception, "Start Up Video");
                return false;
            }
        }




        #region Nested type: CaptureDone

        private delegate void CaptureDone();

        #endregion

        private void Grab()
        {
            if (savedArray == null)
            {
                int size = videoInfoHeader.BmiHeader.ImageSize;
                if ((size < 1000) || (size > 16000000))
                    return;
                savedArray = new byte[size + 64000];
            }

            //  toolBarBtnGrab.Enabled = false;
            captured = false;
            var hr = sampGrabber.SetCallback(this, 1);
        }

        public long QualityImage { get; set; }
    }
}
