using System;

namespace Core.Framework.Helper.Contracts
{
    public interface IClientMessaging:IDisposable
    {
        #region Public Events

        event EventHandler<ItemEventArgs<string>> ReciveMessage;

        #endregion

        #region Public Methods and Operators

        void SendData(string module, object item);

        void SetEndPoint(string endPoint);

        void SubscribeModule(string module);

        void UnsubscribeModule(string module);

        #endregion
    }
}