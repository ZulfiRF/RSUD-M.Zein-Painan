using System.Text;
using System.Text.RegularExpressions;

namespace Core.Framework.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;


    public static class TextHelper
    {

        public static byte[] ConvertToByteArray(this string keyword)
        {
            byte[] data = Encoding.UTF8.GetBytes(keyword);
            return data;
        }
        public static string ConvertToString(this byte[] keyword)
        {
            var result = Encoding.ASCII.GetString(keyword, 0, keyword.Length).Trim().Replace("\0", "");
            return result;
        }
        #region Static Fields

        public static string[] belasan = new string[10]
                                         {
                                             "sepuluh", "sebelas", "dua belas", "tiga belas", "empat belas", "lima belas",
                                             "enam belas", "tujuh belas", "delapan belas", "sembilan belas"
                                         };

        public static string[] puluhan = new string[10]
                                         {
                                             "", "", "dua puluh", "tiga puluh", "empat puluh", "lima puluh", "enam puluh",
                                             "tujuh puluh", "delapan puluh", "sembilan puluh"
                                         };

        public static string[] ribuan = new string[5] { "", "ribu", "juta", "milyar", "triliyun" };

        public static string[] satuan = new string[10]
                                        {
                                            "nol", "satu", "dua", "tiga", "empat", "lima", "enam", "tujuh", "delapan",
                                            "sembilan"
                                        };

        #endregion

        #region Public Methods and Operators

        public static String GetTextBetween(String source, String leftWord, String rightWord)
        {
            return
                Regex.Match(source, String.Format(@"{0}\s(?<words>[\w\s]+)\s{1}", leftWord, rightWord),
                            RegexOptions.IgnoreCase).Groups["words"].Value;
        }
        public static string ConvertTextBetween(this string words, char param1, char param2, string replaceWord, string toReplaceWord)
        {
            var temp = "";
            var index = words.IndexOf(param1);

            while (index != -1)
            {

                //if (temp == string.Empty)
                //{
                //    temp = words.Substring(0, index);
                //}
                var indexRight = words.IndexOf(param2);
                if (indexRight == -1)
                {
                    temp += words;
                    return temp;
                }
                var localTemp = words.Substring(index, indexRight - index + 1);

                if (temp.Length != 0)
                    temp += " ";
                words = words.Replace(localTemp, localTemp.Replace(replaceWord, toReplaceWord));
                localTemp = words.Substring(0, indexRight + 1);
                temp += localTemp;
                words = words.Replace(localTemp, "");
                index = words.IndexOf(param1);
            }
            temp = temp + words;
            return temp;
        }

        public static string ConvertToStatementText(this string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return name;
                }
                string tempText = name[0].ToString(CultureInfo.InvariantCulture).ToUpper();
                string nameTemp = name;
                name = "";
                foreach (string s in nameTemp.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    name += s + " ";
                }

                char[] arr = name.ToCharArray();
                for (int i = 1; i < arr.Length; i++)
                {
                    if (i + 1 < arr.Length && char.IsUpper(arr[i + 1]))
                    {
                        tempText += arr[i] + " ";
                    }
                    else
                    {
                        tempText += arr[i];
                    }
                }
                return tempText.Trim();
                //if (string.IsNullOrEmpty(name))
                //{
                //    return name;
                //}

                //string tempText = name[0].ToString(CultureInfo.InvariantCulture).ToUpper();
                //string nameTemp = name;
                //name = "";
                //foreach (string s in nameTemp.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                //{
                //    var indexWord = 0;
                //    for (int i = 0; i < s.Length; i++)
                //    {
                //        if ((s[i] >= 'A' && s[i] <= 'Z') || (s[i] >= 'a' && s[i] <= 'z'))
                //        {
                //            indexWord = i;
                //            break;
                //        }
                //    }
                //    if (indexWord != 0)
                //        name += s.Substring(0, indexWord - 1) + s[indexWord].ToString().ToUpper() + s.Substring(indexWord + 1).ToLower() + " ";
                //    else
                //        name += s[indexWord].ToString().ToUpper() + s.Substring(indexWord + 1).ToLower() + " ";
                //}
                //char[] arr = name.ToCharArray();
                //for (int i = 1; i < arr.Length; i++)
                //{
                //    if (i + 1 < arr.Length && char.IsUpper(arr[i + 1]))
                //    {
                //        tempText += arr[i];
                //    }
                //    else
                //    {
                //        tempText += arr[i];
                //    }
                //}
                //return tempText.Trim();
            }
            catch (Exception)
            {
                return name;
            }

        }


        public static int[] FindMultipleText(this String text, string keyWord)
        {
            string temp = text;
            int index = temp.IndexOf(keyWord, StringComparison.InvariantCulture);
            if (index != -1)
            {
                var listInt = new List<int>();
                listInt.Add(index);
                temp = temp.Substring(index + keyWord.Length);
                int tempInt = index;
                index = temp.IndexOf(keyWord, StringComparison.InvariantCulture);

                while (index != -1)
                {
                    tempInt += index;
                    listInt.Add(tempInt);
                    if (temp.Length >= index + keyWord.Length)
                    {
                        temp = temp.Substring(index + keyWord.Length);
                        index = temp.IndexOf(keyWord, StringComparison.InvariantCulture);
                    }
                }
                return listInt.ToArray();
            }

            return null;
        }

        //public static string ToStatement(this string value)
        //{
        //    return "";
        //}
        public static bool SmartSearchText(this string value, IEnumerable<string> keyWords)
        {
            int countMatch =
                value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Sum(word => keyWords.Count(keyWord => keyWord.ToLower().Equals(word.ToLower())));
            return countMatch != 0;
        }

        public static bool SmartSearchText(this string value, string keyWord)
        {
            string[] keyWords = value.Split(new[] { ' ' });
            if (string.IsNullOrEmpty(keyWord))
            {
                return true;
            }
            int countMatch = 0;
            if (keyWord.StartsWith("\"") && keyWord.EndsWith("\""))
            {
                countMatch = value.ToLower().Contains(keyWord.Replace("\"", "").ToLower()) ? 1 : 0;
            }
            else
            {
                foreach (string word in keyWords)
                {
                    foreach (string key in keyWord.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (word.ToLower().Contains(key.ToLower()))
                        {
                            countMatch++;
                        }
                    }
                }
            }
            return countMatch != 0;
        }

        public static string Terbilang(Decimal d)
        {
            string strHasil = "";

            Decimal frac = d - Decimal.Truncate(d);
            string strTemp = decimal.Truncate(d).ToString(CultureInfo.InvariantCulture);
            for (int i = strTemp.Length; i > 0; i--)
            {
                var nDigit = Convert.ToInt32(strTemp.Substring(i - 1, 1));
                var nPosisi = strTemp.Length - i + 1;
                switch (nPosisi % 3)
                {
                    case 1:
                        bool bAllZeros = false;
                        var tmpBuff = "";
                        if (i == 1)
                        {
                            tmpBuff = satuan[nDigit] + " ";
                        }
                        else if (strTemp.Substring(i - 2, 1) == "1")
                        {
                            tmpBuff = belasan[nDigit] + " ";
                        }
                        else if (nDigit > 0)
                        {
                            tmpBuff = satuan[nDigit] + " ";
                        }
                        else
                        {
                            bAllZeros = true;
                            if (i > 1)
                            {
                                if (strTemp.Substring(i - 2, 1) != "0")
                                {
                                    bAllZeros = false;
                                }
                            }
                            if (i > 2)
                            {
                                if (strTemp.Substring(i - 3, 1) != "0")
                                {
                                    bAllZeros = false;
                                }
                            }
                            tmpBuff = "";
                        }

                        if (!bAllZeros && (nPosisi > 1))
                        {
                            if ((strTemp.Length == 4) && (strTemp.Substring(0, 1) == "1"))
                            {
                                tmpBuff = "se" + ribuan[(int)Decimal.Round(nPosisi / 3m)] + " ";
                            }
                            else
                            {
                                tmpBuff = tmpBuff + ribuan[(int)Decimal.Round(nPosisi / 3)] + " ";
                            }
                        }
                        strHasil = tmpBuff + strHasil;
                        break;

                    case 2:
                        if (nDigit > 0)
                        {
                            strHasil = puluhan[nDigit] + " " + strHasil;
                        }
                        break;

                    case 0:
                        if (nDigit > 0)
                        {
                            if (nDigit == 1)
                            {
                                strHasil = "seratus " + strHasil;
                            }
                            else
                            {
                                strHasil = satuan[nDigit] + " ratus " + strHasil;
                            }
                        }
                        break;
                }
            }
            strHasil = strHasil.Trim().ToLower();
            if (strHasil.Length > 0)
            {
                strHasil = strHasil.Substring(0, 1).ToUpper() + strHasil.Substring(1, strHasil.Length - 1);
            }

            return strHasil;
        }

        #endregion
    }
}