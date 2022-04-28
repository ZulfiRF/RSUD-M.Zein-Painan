namespace Core.Framework.Helper.Security
{
    using System;

    public abstract class EncryptCoreAttribute : Attribute
    {
        #region Public Methods and Operators

        public abstract object Decrypt(string plainText);

        public abstract object Encrypt(string plainText);

        #endregion
    }
}