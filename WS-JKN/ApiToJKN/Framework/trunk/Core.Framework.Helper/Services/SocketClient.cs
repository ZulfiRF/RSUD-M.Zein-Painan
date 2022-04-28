using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;

namespace Core.Framework.Helper.Services
{
    using System;
    using System.Net.Sockets;
    using System.Text;

    using Core.Framework.Helper.Logging;

    public class SocketClient
    {
        #region Public Methods and Operators

        public static string SendMessage(string ipAddress, int port, string message)
        {
            try
            {
                var sending = new TcpClient(ipAddress, port);
                byte[] data = Encoding.ASCII.GetBytes(message);
                NetworkStream datastream = sending.GetStream();
                datastream.Write(data, 0, data.Length);
                data = new byte[3];
                var hasil = datastream.ReadContent();
                return hasil;
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                return "Terjadi Kesalahan";
            }
        }

        public class DataInformation
        {
            public byte[] Data { get; set; }
            public string Header { get; set; }
            public string Footer { get; set; }
        }
        public byte[] SendMessage(string ipAddress, int port, string message, Action<DataInformation> progress = null, Action Complate = null)
        {
            try
            {
                var sending = new TcpClient(ipAddress, port);
                byte[] data = Encoding.ASCII.GetBytes(message);
                NetworkStream datastream = sending.GetStream();
                datastream.Write(data, 0, data.Length);
                data = new byte[3];
                var bytes = new List<byte>();
                var indexBytes = 0;
                while ((bytes.Count == 6 && Encoding.ASCII.GetString(bytes.ToArray(), 0, bytes.Count).Equals("Finish")) || bytes.Count == 0 || bytes.Count == indexBytes)
                {
                    bytes = new List<byte>();
                    var temp = new byte[532 * 100];
                    temp = datastream.ReadByteContent();
                    //datastream.Read(temp, 0, temp.Length);
                    bytes.AddRange(temp);
                    //while (datastream.DataAvailable)
                    //{
                    //    bytes.Add(Convert.ToByte(datastream.ReadByte()));
                    //}
                    var lenghtFile = bytes.Take(10).ToArray().ConvertToString().Trim().Replace("\0", "");
                    var pageFile = bytes.Skip(10).Take(5).ToArray().ConvertToString().Trim().Replace("\0", "");
                    var fileread = bytes.Skip(15).Take(5).ToArray().ConvertToString().Trim().Replace("\0", "");
                    indexBytes = temp.Length;
                    if (string.IsNullOrEmpty(pageFile))
                        break;
                    Log.Info("Has Recive " + (Convert.ToInt16(pageFile) * Convert.ToInt16(fileread) + bytes.Count - 20) + " dari " + lenghtFile);
                    if (progress != null)
                        progress.Invoke(new DataInformation()
                        {
                            Data = bytes.Skip(20).Take(Convert.ToInt16(fileread)).ToArray(),
                            Header = lenghtFile,

                        });
                    data = "Success".ConvertToByteArray();
                    datastream.Write(data, 0, data.Length);
                }
                if (Complate != null)
                    Complate.Invoke();
                return null;
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                return null;
            }
        }

        #endregion
    }
}