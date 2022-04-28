using ApiToJKN.Models.Responses;
using Core.Framework.Helper.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiToJKN.Utilities
{
    public static class Extention
    {
        public static string EncrypteForVb6(this string value)
        {
            try
            {                
                var code = "1234567890";
                string result = "";
                for (int i = 0; i < value.Length; i++)
                {
                    var lokasi = i % code.Length + 1;
                    result += Convert.ToChar((((int)(value.Substring(i, 1).ToCharArray()[0]))) ^ (int)code.Substring(lokasi, 1).ToCharArray()[0]);
                }
                return result;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }

    public class Helper
    {
        public static string CreateToken(string userName, string password)
        {
            var token = new TokenResponse()
            {
                UserName = userName,
                password = password,
                
            };
            var jsonToken = JsonConvert.SerializeObject(token);
            var encryptJsonToken = Cryptography.FuncAesEncrypt(jsonToken);
            return encryptJsonToken;
        }

        public static TokenResponse ValidateToken(string token)
        {
            var decrypt = Cryptography.FuncAesDecrypt(token);
            var jsonToken = JsonConvert.DeserializeObject<TokenResponse>(decrypt);
            return jsonToken;
        }
    }
}