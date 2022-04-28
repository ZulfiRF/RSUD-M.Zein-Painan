using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Core.Framework.Helper
{
    public enum WebClientType
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public class CoreWebClient
    {
        /// <summary>
        /// untuk membuat webclient manual
        /// </summary>
        /// <param name="headers">masukan header yg dibutuhkan</param>
        /// <returns></returns>
        private static WebClient CreateWebClient(List<KeyValuePair<string, string>> headers = null)
        {
            var client = new WebClient();
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    client.Headers.Add(header.Key, header.Value);
                }
            }
            return client;
        }

        /// <summary>
        /// untuk mengupload data ke webclient, webclient di buat otomatis
        /// </summary>
        /// <param name="url">alamat host</param>
        /// <param name="type">type rest</param>
        /// <param name="data">data yg di upload dalam bentuk string</param>
        /// <param name="headers">header yg dibutuhkan</param>
        /// <returns></returns>
        public static string Upload(string url, WebClientType type, string data, List<KeyValuePair<string, string>> headers = null)
        {
            var client = CreateWebClient(headers);
            var result = client.UploadString(url, type.ToString(), data);
            return result;
        }

        /// <summary>
        /// untuk mendownload webclient
        /// </summary>
        /// <param name="url">alamat host</param>
        /// <param name="headers">header yg dibutuhkan</param>
        /// <returns></returns>
        public static string Download(string url, List<KeyValuePair<string, string>> headers = null)
        {
            var client = CreateWebClient(headers);
            var result = client.DownloadString(url);
            return result;
        }
    }    
}
