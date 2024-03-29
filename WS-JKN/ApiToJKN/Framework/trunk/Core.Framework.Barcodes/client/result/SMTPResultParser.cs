/*
 * Copyright 2010 ZXing authors
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

namespace Core.Framework.Barcodes.Client.Result
{
   /// <summary>
   /// <p>Parses an "smtp:" URI result, whose format is not standardized but appears to be like:
   /// <code>smtp[:subject[:body]]}</code>.</p>
   /// <p>See http://code.google.com/p/zxing/issues/detail?id=536</p>
   /// </summary>
   /// <author>Sean Owen</author>
   public class SMTPResultParser : ResultParser
   {
      override public ParsedResult parse(Core.Framework.Barcodes.Result result)
      {
         String rawText = result.Text;
         if (!(rawText.StartsWith("smtp:") || rawText.StartsWith("SMTP:")))
         {
            return null;
         }
         String emailAddress = rawText.Substring(5);
         String subject = null;
         String body = null;
         int colon = emailAddress.IndexOf(':');
         if (colon >= 0)
         {
            subject = emailAddress.Substring(colon + 1);
            emailAddress = emailAddress.Substring(0, colon);
            colon = subject.IndexOf(':');
            if (colon >= 0)
            {
               body = subject.Substring(colon + 1);
               subject = subject.Substring(0, colon);
            }
         }
         String mailtoURI = "mailto:" + emailAddress;
         return new EmailAddressParsedResult(emailAddress, subject, body, mailtoURI);
      }
   }
}
