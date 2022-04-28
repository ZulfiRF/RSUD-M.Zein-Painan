using System;

namespace Core.Framework.Model.Attr
{
    /// <summary>
    ///     Class TableItemArgs
    /// </summary>
    public class ConnectionArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TableItemArgs" /> class.
        /// </summary>
        /// <param name="result">The model.</param>
        /// <param name="error"> </param>
        public ConnectionArgs(string result, Exception error = null)
        {
            Result = result;
            Error = error;
        }

        public string Result { get; set; }
        public Exception Error { get; set; }
    }
}