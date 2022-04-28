using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Core.Framework.Helper
{
    public static class TextHelper
    {
        //public static string ToStatement(this string value)
        //{
        //    return "";
        //}
        public static bool SmartSearchText(this string value, IEnumerable<string> keyWords)
        {
            var countMatch = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Sum(word => keyWords.Count(keyWord => keyWord.ToLower().Equals(word.ToLower())));
            return countMatch != 0;
        }
        public static bool SmartSearchText(this string value, string keyWord)
        {
            var keyWords = value.Split(new[] { ' ' });
            if (string.IsNullOrEmpty(keyWord))
                return true;
            var countMatch = 0;
            if (keyWord.StartsWith("\"") && keyWord.EndsWith("\""))
                countMatch = value.ToLower().Contains(keyWord.Replace("\"", "").ToLower()) ? 1 : 0;
            else
            {
                foreach (var word in keyWords)
                {
                    foreach (var key in keyWord.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (word.ToLower().Contains(key.ToLower()))
                            countMatch++;
                    }
                }
            }
            return countMatch != 0;
        }

        public static string ConvertToStatementText(this string name)
        {
            var tempText = name[0].ToString(CultureInfo.InvariantCulture).ToUpper();
            var arr = name.ToCharArray();
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
            return tempText;
        }
    }
}
