using System;

namespace Core.Framework.Messaging
{
    public class MessageArgs : EventArgs
    {
        public string From { get; set; }

        public object Content { get; set; }

        public string To { get; set; }

        public MessageType MessageType { get; set; }
    }
}