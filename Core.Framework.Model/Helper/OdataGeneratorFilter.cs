using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Core.Framework.Model.Helper
{
    /// <summary>
    ///     Class OdataGeneratorFilter
    /// </summary>
    public class OdataGeneratorFilter
    {
        /// <summary>
        ///     Generates the query from odata filter.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        private static string GenerateQueryFromOdataFilter(string input)
        {
            var indexCharacterOpen = 0;
            var hasUseIndex = new List<int>();
            var dictionary = new Dictionary<string, string>();
            foreach (var chareacter in input.ToCharArray().Reverse())
            {
                if (chareacter.Equals('('))
                {
                    for (var i = input.Length - indexCharacterOpen; i < input.Length; i++)
                    {
                        if (input[i].Equals(')'))
                        {
                            if (hasUseIndex.Count(n => n == i) == 0)
                            {
                                hasUseIndex.Add(i);
                                var temp = input.Substring(input.Length - indexCharacterOpen,
                                    i - (input.Length - indexCharacterOpen));
                                var result = temp;
                                var split = Regex.Split(result, "eq");
                                if (split.Length > 2)
                                {
                                    foreach (var dict in dictionary.OrderByDescending(n => n.Value.Length))
                                    {
                                        if (result.Contains(dict.Key))
                                            result = result.Replace(dict.Key, dict.Value);
                                    }
                                }
                                split = Regex.Split(result, "eq");
                                if (split.Length == 2)
                                {
                                    result = split[0] + " = " + split[1] + "";
                                }

                                dictionary.Add(temp.Trim(), result.Trim());
                                break;
                            }
                        }
                    }
                }
                indexCharacterOpen++;
            }
            return dictionary.OrderByDescending(n => n.Value.Length).FirstOrDefault().Value;
        }
    }
}