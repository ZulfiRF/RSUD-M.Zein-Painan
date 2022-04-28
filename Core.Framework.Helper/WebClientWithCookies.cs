using Microsoft.Win32;
using System;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Core.Framework.Helper
{
    using System.Reflection;

    public class WebClientWithCookies : WebClient
    {
        #region Fields

        private readonly CookieContainer _container = new CookieContainer();

        #endregion

        #region Methods


        public object Tag { get; set; }

        public WebClient CreateWebClient(DateTime? DefaultTime, int Tahun, int Bulan, int Hari, int Jam, int Menit, int Detik, string ConsumerID, string PasswordKey)
        {
            if (Registry.CurrentUser.OpenSubKey(@"Software\Medifirst2000") == null) throw new ArgumentException("Url Tidak Di temukan");
            UTF8Encoding encoding = new UTF8Encoding();
            string str = "";
            WebClient client = new WebClient();
            DateTime time;
            if (DefaultTime.HasValue)
                time = DefaultTime.Value;
            else
                time = new DateTime(Tahun, Bulan, Hari, Jam, Menit, Detik, 0);
            TimeSpan span = (TimeSpan)(DateTime.Now - time.ToLocalTime());
            str = Convert.ToInt64(span.TotalSeconds).ToString(CultureInfo.InvariantCulture);
            string s = ConsumerID + "&" + str;
            byte[] bytes = encoding.GetBytes(s);
            HMACSHA256 hmacsha = new HMACSHA256(encoding.GetBytes(PasswordKey));
            string str3 = Convert.ToBase64String(hmacsha.ComputeHash(bytes));
            client.Headers.Add("Accept", "application/json");
            client.Headers.Add("X-cons-id", ConsumerID);
            client.Headers.Add("X-timestamp", str);
            client.Headers.Add("X-signature", str3);
            return client;
        }
        protected override WebRequest GetWebRequest(Uri address)
        {
            if (Default)
                return base.GetWebRequest(address);
            HttpWebRequest request = base.GetWebRequest(address) as HttpWebRequest;
            ((HttpWebRequest)request).KeepAlive = true;



            var sp = ((HttpWebRequest)request).ServicePoint;
            var prop = sp.GetType().GetProperty("HttpBehaviour",
                                    BindingFlags.Instance | BindingFlags.NonPublic);
            prop.SetValue(sp, (byte)0, null);
            

            if (request != null)
            {
                request.Method = "Post";
                request.CookieContainer = _container;
                if (CookieId != null)
                    request.CookieContainer.SetCookies(address, CookieId);
            }

            return request;
        }

        public bool Default { get; set; }
        #endregion

        public string Url { get; set; }

        public string CookieId { get; set; }
    }
}