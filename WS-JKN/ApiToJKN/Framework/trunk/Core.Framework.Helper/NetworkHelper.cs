using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Core.Framework.Helper
{
    public static class NetworkHelper
    {
        public static int TimeOut { private get; set; }
        #region Public Methods and Operators

        public static string ReadContent(this NetworkStream networkStream)
        {
            if (TimeOut == 0)
                TimeOut = 5000;
            var temp = new List<byte>();
            var now = DateTime.Now;
            while (!networkStream.DataAvailable)
            {
                if ((DateTime.Now - now).TotalMilliseconds > TimeSpan.FromMilliseconds(TimeOut).TotalMilliseconds)
                    return "Time Out";
                Thread.Sleep(10);
            }
            while (networkStream.DataAvailable)
            {
                var a = networkStream.ReadByte();
                temp.Add(Convert.ToByte(a));
            }


            //data = new byte[4096];
            //datastream.Read(data, 0, data.Length);
            var result = Encoding.ASCII.GetString(temp.ToArray(), 0, temp.Count);
            return result;
        }
        public static byte[] ReadByteContent(this NetworkStream networkStream)
        {
            var temp = new List<byte>();
            while (!networkStream.DataAvailable)
            {
                Thread.Sleep(10);
            }
            while (networkStream.DataAvailable)
            {
                var a = networkStream.ReadByte();
                temp.Add(Convert.ToByte(a));
            }
            return temp.ToArray();
        }
        public static string LocalIPAddress()
        {
            string localIP = "";
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        #endregion
    }
}