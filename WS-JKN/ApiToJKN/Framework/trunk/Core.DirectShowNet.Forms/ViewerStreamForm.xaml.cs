using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Core.Framework.Helper;
using Core.Framework.Helper.Contracts;

namespace Core.DirectShowNet.Forms
{
    /// <summary>
    /// Interaction logic for ViewerStreamForm.xaml
    /// </summary>
    public partial class ViewerStreamForm : UserControl
    {
        public ViewerStreamForm()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Size = new Size(Width, Height);
        }

        public Size Size { get; set; }

        private bool isStream;

        public bool IsStream
        {
            get { return isStream; }
            set
            {
                if (isStream == value) return;
                isStream = value;
                if (isStream)
                    ThreadPool.QueueUserWorkItem(ListenDataImage);
                else
                {
                    newsock.Close();
                }
            }
        }

        public int Port { get; set; }
        private UdpClient newsock;
        private void ListenDataImage(object state)
        {
            try
            {


                if (Port == 0)
                {
                    var contextSetting = BaseDependency.Get<ISetting>();
                    if (contextSetting == null)
                        return;
                    Port = Convert.ToInt16(contextSetting.GetValue("Server-Address").Split(':')[1]);
                }
                byte[] data = new byte[1024];
                IPEndPoint ipep = new IPEndPoint(IPAddress.Any, Port);
                newsock = new UdpClient(ipep);

                Console.WriteLine("Waiting for a client...");

                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                while (IsStream)
                {
                    try
                    {
                        Console.WriteLine("X-Plane Data Read: \n\n");
                        data = newsock.Receive(ref sender);
                        //Bitmap b = new Bitmap();

                        imageDestinatioin.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
                        {
                            BitmapImage image = new BitmapImage();
                            image.BeginInit();
                            image.StreamSource = new System.IO.MemoryStream(data);
                            image.EndInit();
                            imageDestinatioin.Source = image;
                        }));
                    }
                    catch (Exception)
                    {
                    }


                }
            }
            catch (Exception)
            {
            }
        }
    }
}
