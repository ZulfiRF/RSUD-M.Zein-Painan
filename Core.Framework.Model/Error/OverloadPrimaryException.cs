using System;

namespace Core.Framework.Model.Error
{
    /// <summary>
    ///     Class OverloadPrimaryException
    /// </summary>
    public class OverloadPrimaryException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OverloadPrimaryException" /> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public OverloadPrimaryException(object key)
            : base(key.ToString())
        {
        }
    }
}