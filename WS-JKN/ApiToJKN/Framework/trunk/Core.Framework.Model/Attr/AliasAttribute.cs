using System;

namespace Core.Framework.Model.Attr
{
    /// <summary>
    ///     Digunakan untuk mereference menu F12 di CoreComboBox
    ///     Alias Harus Sama Dengan Nama Menu Location
    /// </summary>
    public class AliasAttribute : Attribute
    {
        /// <summary>
        ///     Nama Alias
        /// </summary>
        /// <param name="aliasName"></param>
        public AliasAttribute(string aliasName)
        {
            AliasName = aliasName;
        }

        /// <summary>
        ///     Nama Alias
        /// </summary>
        public string AliasName { get; set; }
    }
}