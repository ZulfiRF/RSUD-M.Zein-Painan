using System;

namespace Core.Framework.Helper.Security
{
    public class AesCryptographyAttribute : EncryptCoreAttribute
    {
        #region Overrides of EncryptCoreAttribute

        public override object Decrypt(string plainText)
        {
            throw new NotImplementedException();
        }

        public override object Encrypt(string plainText)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}