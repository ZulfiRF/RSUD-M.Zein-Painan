namespace Core.Framework.Helper.Security
{
    using System;

    public class AesCryptographyAttribute : EncryptCoreAttribute
    {
        #region Public Methods and Operators

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