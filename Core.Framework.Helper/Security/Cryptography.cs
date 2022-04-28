namespace Core.Framework.Helper.Security
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public class Cryptography
    {
        #region Static Fields

        private static readonly byte[] AESBLOCK = Encoding.ASCII.GetBytes("jasamedikajasmed"); //jasamedikajasmed

        private static readonly byte[] AESKEY = Encoding.ASCII.GetBytes("jasamedikasaranatamabymysteriouz"); //jasamedikasaranatamabymysteriouz

        #endregion

        #region Public Methods and Operators

        public static string FuncAesDecrypt(string ciphertext)
        {
            try
            {
                var provider = new AesCryptoServiceProvider();
                var stream = new MemoryStream(Convert.FromBase64String(ciphertext));
                var stream2 = new CryptoStream(
                    stream,
                    provider.CreateDecryptor(AESKEY, AESBLOCK),
                    CryptoStreamMode.Read);
                var reader = new StreamReader(stream2);
                return reader.ReadToEnd();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string FuncAesEncrypt(string plaintext)
        {
            try
            {
                var provider = new AesCryptoServiceProvider();
                var stream = new MemoryStream();
                var stream2 = new CryptoStream(
                    stream,
                    provider.CreateEncryptor(AESKEY, AESBLOCK),
                    CryptoStreamMode.Write);
                var writer = new StreamWriter(stream2);
                writer.Write(plaintext);
                writer.Flush();
                stream2.FlushFinalBlock();
                writer.Flush();
                return Convert.ToBase64String(stream.GetBuffer(), 0, Convert.ToInt32(stream.Length));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string Crypt(string value)
        {
            var lokasi = 0;
            var code = "1234567890";
            string result = "";
            for (int i = 0; i < value.Length; i++)
            {
                lokasi = (i % code.Length) + 1;
                result += Convert.ToChar((((int)(value.Substring(i, 1).ToCharArray()[0]))) ^ (int)code.Substring(lokasi, 1).ToCharArray()[0]);
            }
            return result;
        }

        #endregion
    }
}