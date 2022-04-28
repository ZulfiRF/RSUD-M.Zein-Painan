using System;
using Core.Framework.Helper;

namespace Core.Framework.Messaging.Contracts
{
    public interface IReceiveMessaging
    {
        void ReceiveMessage(string flag, string hostName = null, string username = null, string password = null, string vhost = null);

        event EventHandler<ItemEventArgs<string>> MessageReceive;

        void Close();
    }
}