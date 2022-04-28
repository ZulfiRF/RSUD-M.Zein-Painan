using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Core.Framework.Helper.Security
{
    public class Cryptography
    {
        private static byte[] AESBLOCK = Encoding.ASCII.GetBytes("jasamedikajasmed");
        private static byte[] AESKEY = Encoding.ASCII.GetBytes("jasamedikasaranatamabymysteriouz");

        public static string FuncAesDecrypt(string ciphertext)
        {
            try
            {
                var provider = new AesCryptoServiceProvider();
                var stream = new MemoryStream(Convert.FromBase64String(ciphertext));
                var stream2 = new CryptoStream(stream, provider.CreateDecryptor(AESKEY, AESBLOCK), CryptoStreamMode.Read);
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
                var stream2 = new CryptoStream(stream, provider.CreateEncryptor(AESKEY, AESBLOCK), CryptoStreamMode.Write);
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
    }
}
