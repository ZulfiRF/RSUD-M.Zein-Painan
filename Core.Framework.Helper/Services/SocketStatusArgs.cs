namespace Core.Framework.Helper.Services
{
    using System;

    public class SocketStatusArgs : EventArgs
    {
        #region Constructors and Destructors

        public SocketStatusArgs(StatusType status, Exception error = null)
        {
            Status = status;
            Error = error;
        }

        #endregion

        #region Enums

        public enum StatusType
        {
            Connected,

            Failed,

            Stoped
        }

        #endregion

        #region Public Properties

        public Exception Error { get; private set; }

        public StatusType Status { get; private set; }

        #endregion
    }
}