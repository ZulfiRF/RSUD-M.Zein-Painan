using System;

namespace Core.Framework.Helper.Security
{
    public abstract class EncryptCoreAttribute : Attribute
    {
        public abstract object Decrypt(string plainText);
        public abstract object Encrypt(string plainText);
    }
}