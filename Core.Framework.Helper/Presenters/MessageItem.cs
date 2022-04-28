using System;

namespace Core.Framework.Helper.Presenters
{
    public class MessageItem
    {
        #region Public Properties

        public object Content { get; set; }
        public MessageType MessageType { get; set; }

        public Exception Exceptions { get; set; }

        #endregion
    }
}