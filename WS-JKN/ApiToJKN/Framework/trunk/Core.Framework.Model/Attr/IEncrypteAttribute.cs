namespace Core.Framework.Model.Attr
{
    /// <summary>
    ///     Interface IEncrypteAttribute
    /// </summary>
    public interface IEncrypteAttribute
    {
        /// <summary>
        ///     Gets the certificate key.
        /// </summary>
        /// <value>The certificate key.</value>
        string CertificateKey { get; }

        /// <summary>
        ///     Gets the certificate value.
        /// </summary>
        /// <value>The certificate value.</value>
        object CertificateValue { get; }
    }
}