using System.Diagnostics;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.Framework.Helper.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    public class SocketServer : IDisposable
    {
        #region Static Fields

        internal static List<SocketReciveItem> ListItem;
        public static List<TcpClient> ListTcpListener;
        #endregion

        #region Fields

        //private TcpListener listener;

        #endregion

        #region Constructors and Destructors

        public SocketServer(int port)
        {
            Port = port;
            if (ListTcpListener == null) ListTcpListener = new List<TcpClient>();
        }

        #endregion

        #region Public Events

        public event EventHandler<SocketStatusArgs> StatusConnected;

        #endregion

        #region Public Properties

        public int Port { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Connected()
        {
            ThreadPool.QueueUserWorkItem(InitializeWithPort, Port);
        }

        public void Dispose()
        {
        }

        public void Stop()
        {
            try
            {
                //listener.Stop();
                OnStatusConnected(new SocketStatusArgs(SocketStatusArgs.StatusType.Stoped));
            }
            catch (Exception exception)
            {
                OnStatusConnected(new SocketStatusArgs(SocketStatusArgs.StatusType.Failed, exception));
            }
        }

        #endregion

        #region Methods

        private void CallBack(object state)
        {
            var listenerItem = state as TcpListener;
            if (listenerItem != null)
            {
                while (true)
                {
                    try
                    {
                        TcpClient client = listenerItem.AcceptTcpClient();
                        //client.ReceiveTimeout = 1000;
                        //client.SendTimeout = 1000;
                        //    listenerItem.Server.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.KeepAlive, 1);
                        ThreadPool.QueueUserWorkItem(CallBackListenIten, client);
                    }
                    catch (Exception exception)
                    {
                        if (exception.GetBaseException().ToString().Contains("Not listening."))
                        {
                            break;
                        }
                    }
                }
            }
        }

        public event EventHandler<ItemEventArgs<SendItem>> ReciveData;
        public class SendItem
        {
            public NetworkStream NetworkStream { get; set; }
            public string KeyWord { get; set; }

            public System.Net.EndPoint RemoteIp { get; set; }
        }
        public void OnReciveData(ItemEventArgs<SendItem> e)
        {
            EventHandler<ItemEventArgs<SendItem>> handler = ReciveData;
            if (handler != null) handler(this, e);
        }

        private void CallBackListenIten(object state)
        {
            var client = state as TcpClient;
            try
            {
                while (true)
                {

                    NetworkStream clientStream = client.GetStream();
                    var message = new byte[4096];
                    if (clientStream != null)
                    {
                        var clientIp = client.Client.RemoteEndPoint.ToString();
                        if (!ListTcpListener.Exists(n => n.Client.RemoteEndPoint.ToString().Equals(clientIp)))
                            ListTcpListener.Add(client);
                    }

                    clientStream.Read(message, 0, message.Length);

                    string src = Encoding.UTF8.GetString(message, 0, message.Length).TrimEnd(new[] { '\0' });

                    Debug.WriteLine("Retrive :" + client.Client.RemoteEndPoint + src);
                    if (src.ToLower().Equals("echo"))
                    {
                        src = "success";
                        clientStream.Write(src.ConvertToByteArray(), 0, src.Length);
                        return;
                    }
                    OnReciveData(new ItemEventArgs<SendItem>(new SendItem()
                                                                 {
                                                                     NetworkStream = clientStream,
                                                                     KeyWord = src,
                                                                     RemoteIp = client.Client.RemoteEndPoint
                                                                 }));

                    var listRemove = new List<SocketReciveItem>();
                    try
                    {
                        int index = src.IndexOf("<>", StringComparison.Ordinal);
                        if (index == -1)
                        {
                            if (string.IsNullOrEmpty(src)) return;
                            foreach (SocketReciveItem socketReciveItem in ListItem)
                            {
                                try
                                {
                                    string jsonLoket = null;
                                    if (src.Contains("?"))
                                    {
                                        var arrSrc = JsonConvert.DeserializeObject(src.Replace("?", ""));
                                        foreach (var json in arrSrc as JObject)
                                        {
                                            if (json.Key.Contains("Loket"))
                                            {
                                                jsonLoket = json.Value.ToString().Replace("{", "").Replace("}", "");
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var arrSrc = JsonConvert.DeserializeObject(src);
                                        foreach (var json in arrSrc as JObject)
                                        {
                                            if (json.Key.Contains("Content"))
                                            {
                                                jsonLoket = json.Value.ToString().Replace("{", "").Replace("}", "").Split('-').Last();
                                                break;
                                            }
                                        }
                                    }

                                    if (socketReciveItem.ListSubscribe.Any(n => n.ToLower().Equals(jsonLoket.ToLower())))
                                    {
                                        Logging.Log.Info("Panggil => dari " + client.Client.RemoteEndPoint + " JSON " + src);
                                        socketReciveItem.NetworkStream = clientStream;
                                        socketReciveItem.OnReciveMessage(new ItemEventArgs<string>(src), socketReciveItem);
                                    }
                                }
                                catch (Exception)
                                {
                                    listRemove.Add(socketReciveItem);
                                }
                            }
                            //src = "success";
                            //clientStream.Write(src.ConvertToByteArray(), 0, src.Length);
                            ThreadPool.QueueUserWorkItem(delegate(object o)
                            {
                                var netword = o as NetworkStream;
                                while (true)
                                {
                                    try
                                    {
                                        var temp = new List<byte>();
                                        Thread.Sleep(100);
                                        while (netword.DataAvailable)
                                        {
                                            var a = netword.ReadByte();
                                            temp.Add(Convert.ToByte(a));
                                        }
                                        src = Encoding.ASCII.GetString(temp.ToArray(), 0, temp.Count);
                                        if (string.IsNullOrEmpty(src)) continue;
                                        foreach (SocketReciveItem socketReciveItem in ListItem)
                                        {
                                            try
                                            {
                                                string jsonLoket = null;
                                                var arrSrc = JsonConvert.DeserializeObject(src.Replace("?", ""));
                                                foreach (var json in arrSrc as JObject)
                                                {
                                                    if (json.Key.Contains("Loket"))
                                                    {
                                                        jsonLoket = json.Value.ToString().Replace("{", "").Replace("}", "");
                                                        break;
                                                    }
                                                }

                                                if (socketReciveItem.ListSubscribe.Any(n => n.Equals(jsonLoket)))
                                                {
                                                    Logging.Log.Info("Panggil => dari " + client.Client.RemoteEndPoint + " JSON " + src);
                                                    socketReciveItem.NetworkStream = clientStream;
                                                    socketReciveItem.OnReciveMessage(new ItemEventArgs<string>(src), socketReciveItem);
                                                }

                                            }
                                            catch (Exception)
                                            {
                                                listRemove.Add(socketReciveItem);
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        break;

                                    }

                                }
                            }, clientStream);
                            return;
                        }

                        string subscribe = src.Substring(0, index);
                        string content = src.Substring(index + 2);
                        foreach (SocketReciveItem socketReciveItem in ListItem)
                        {
                            try
                            {
                                if (socketReciveItem.ListSubscribe.Any(n => n.ToLower().Equals(subscribe.ToLower())))
                                {
                                    socketReciveItem.OnReciveMessage(new ItemEventArgs<string>(content));
                                }
                            }
                            catch (Exception)
                            {
                                listRemove.Add(socketReciveItem);
                            }
                        }
                        src = "success";
                        NetworkHelper.TimeOut = 1000000;
                        clientStream.Write(src.ConvertToByteArray(), 0, src.Length);
                        ThreadPool.QueueUserWorkItem(delegate(object o)
                                                         {
                                                             var netword = o as NetworkStream;
                                                             while (true)
                                                             {
                                                                 try
                                                                 {
                                                                     Console.WriteLine(netword.ReadContent());
                                                                 }
                                                                 catch (Exception)
                                                                 {
                                                                     break;

                                                                 }

                                                             }
                                                         }, clientStream);
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

        private void InitializeWithPort(object sender)
        {
            try
            {
                short port = Convert.ToInt16(sender);
                string IPHost = Dns.GetHostName();
                //string ip = Dns.GetHostEntry(IPHost).AddressList[0].ToString();
                var listConnected = new List<string>();
                foreach (var ip in Dns.GetHostEntry(IPHost).AddressList)
                {
                    var tcpListener = new TcpListener(ip, port);
                    listConnected.Add(ip + " " + port);
                    tcpListener.Start();
                    if (ListItem == null)
                    {
                        ListItem = new List<SocketReciveItem>();
                    }
                    OnStatusConnected(new SocketStatusArgs(SocketStatusArgs.StatusType.Connected));
                    ThreadPool.QueueUserWorkItem(CallBack, tcpListener);
                    tcpListener.Start();
                    if (ListItem == null)
                    {
                        ListItem = new List<SocketReciveItem>();
                    }
                    OnStatusConnected(new SocketStatusArgs(SocketStatusArgs.StatusType.Connected));
                    ThreadPool.QueueUserWorkItem(CallBack, tcpListener);
                }
                //File.WriteAllLines("socket", listConnected.ToArray());
            }
            catch (Exception exception)
            {
                OnStatusConnected(new SocketStatusArgs(SocketStatusArgs.StatusType.Failed, exception));
            }
        }

        private void OnStatusConnected(SocketStatusArgs e)
        {
            EventHandler<SocketStatusArgs> handler = StatusConnected;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion
    }
}
