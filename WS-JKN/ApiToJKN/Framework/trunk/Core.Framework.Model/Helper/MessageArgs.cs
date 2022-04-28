using System;

namespace Core.Framework.Model.Helper
{
    public class MessageArgs : EventArgs
    {
        public MessageArgs(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}