using System;
using System.Text;
using Core.Framework.Helper.Security;

namespace Core.Framework.Model.Attr
{
    /// <summary>
    ///     Class EncrypteAttribute
    /// </summary>
    public class EncrypteAttribute : SkipAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EncrypteAttribute" /> class.
        /// </summary>
        /// <param name="key">Berisikan kata kunci untuk enkripsi</param>
        public EncrypteAttribute(string key)
        {
            Key = key;
        }

        public EncrypteAttribute(string aeskey, string aesblock)
        {
            Aeskey = Encoding.ASCII.GetBytes(aeskey);
            Aesblock = Encoding.ASCII.GetBytes(aesblock);
        }

        public EncrypteAttribute(EncryptCoreAttribute context)
        {
            Context = context;
        }

        public EncrypteAttribute(byte[] aeskey, byte[] aesblock)
        {
            Aeskey = aeskey;
            Aesblock = aesblock;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EncrypteAttribute" /> class.
        /// </summary>
        /// <param name="key">Berisikan kata kunci untuk enkripsi</param>
        /// <param name="relationProperty">berisikan property yang digunakan untuk enkripsi</param>
        public EncrypteAttribute(string key, params string[] relationProperty)
        {
            Key = key;
            RelationProperty = relationProperty;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EncrypteAttribute" /> class.
        /// </summary>
        /// <param name="typeEncrypte">The type encrypte.</param>
        /// <param name="relationProperty">berisikan property yang digunakan untuk enkripsi</param>
        public EncrypteAttribute(Type typeEncrypte, params string[] relationProperty)
        {
            TypeEncrypte = typeEncrypte;
            if (TypeEncrypte != null)
            {
                var objEncrypte = Activator.CreateInstance(typeEncrypte);
                Key = objEncrypte is IEncrypteAttribute ? (objEncrypte as IEncrypteAttribute).CertificateKey : "";
            }
            RelationProperty = relationProperty;
        }

        public byte[] Aeskey { get; set; }
        public byte[] Aesblock { get; set; }
        public EncryptCoreAttribute Context { get; set; }

        /// <summary>
        ///     Gets or sets the key.
        /// </summary>
        /// <value>Berisikan kata kunci untuk enkripsi.</value>
        public string Key { get; set; }

        /// <summary>
        ///     Gets or sets the type encrypte.
        /// </summary>
        /// <value>The type encrypte.</value>
        public Type TypeEncrypte { get; set; }

        /// <summary>
        ///     Gets or sets the relation property.
        /// </summary>
        /// <value>berisikan property yang digunakan untuk enkripsi</value>
        public string[] RelationProperty { get; set; }
    }
}