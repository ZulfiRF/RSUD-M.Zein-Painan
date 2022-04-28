using System;

namespace Core.Framework.Model.Error
{
    /// <summary>
    ///     Class ErrorArgs
    /// </summary>
    public class ErrorArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ErrorArgs" /> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public ErrorArgs(Exception exception)
        {
            Exception = exception;
        }

        /// <summary>
        ///     Gets or sets the exception.
        /// </summary>
        /// <value>The exception.</value>
        public Exception Exception { get; set; }
    }
}