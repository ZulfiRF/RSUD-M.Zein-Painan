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

namespace Core.Framework.Windows.Barcodes.Client.Result
{
   /// <author>dswitkin@google.com (Daniel Switkin)</author>
   public sealed class ProductParsedResult : ParsedResult
   {
      internal ProductParsedResult(String productID)
         : this(productID, productID)
      {
      }

      internal ProductParsedResult(String productID, String normalizedProductID)
         : base(ParsedResultType.PRODUCT)
      {
         ProductID = productID;
         NormalizedProductID = normalizedProductID;
         displayResultValue = productID;
      }

      public String ProductID { get; private set; }

      public String NormalizedProductID { get; private set; }
   }
}