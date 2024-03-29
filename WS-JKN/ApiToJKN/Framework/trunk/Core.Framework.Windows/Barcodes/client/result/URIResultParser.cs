/*
* Copyright 2007 ZXing authors
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Text.RegularExpressions;
using Core.Framework.Windows.Barcodes.client.result;

namespace Core.Framework.Windows.Barcodes.Client.Result
{
   /// <summary> Tries to parse results that are a URI of some kind.
   /// 
   /// </summary>
   /// <author>  Sean Owen
   /// </author>
   /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
   /// </author>
   sealed class URIResultParser : ResultParser
   {
      private static readonly Regex URL_WITH_PROTOCOL_PATTERN = new Regex("[a-zA-Z0-9]{2,}:"
#if !(SILVERLIGHT4 || SILVERLIGHT5 || NETFX_CORE || PORTABLE)
, RegexOptions.Compiled);
#else
);
#endif
      private static readonly Regex URL_WITHOUT_PROTOCOL_PATTERN = new Regex(
           "([a-zA-Z0-9\\-]+\\.)+[a-zA-Z]{2,}" + // host name elements
           "(:\\d{1,5})?" + // maybe port
           "(/|\\?|$)" // query, path or nothing
#if !(SILVERLIGHT4 || SILVERLIGHT5 || NETFX_CORE || PORTABLE)
              , RegexOptions.Compiled);
#else
);
#endif

      override public ParsedResult parse(Barcodes.Result result)
      {
         String rawText = result.Text;
         // We specifically handle the odd "URL" scheme here for simplicity and add "URI" for fun
         // Assume anything starting this way really means to be a URI
         if (rawText.StartsWith("URL:") || rawText.StartsWith("URI:"))
         {
            return new URIParsedResult(rawText.Substring(4).Trim(), null);
         }
         rawText = rawText.Trim();
         return isBasicallyValidURI(rawText) ? new URIParsedResult(rawText, null) : null;
      }

      internal static bool isBasicallyValidURI(String uri)
      {
         if (uri.IndexOf(" ") >= 0)
         {
            // Quick hack check for a common case
            return false;
         }
         var m = URL_WITH_PROTOCOL_PATTERN.Match(uri);
         if (m.Success && m.Index == 0)
         {
            // match at start only
            return true;
         }
         m = URL_WITHOUT_PROTOCOL_PATTERN.Match(uri);
         return m.Success && m.Index == 0;
      }
   }
}