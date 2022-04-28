using System;
using Core.Framework.Helper.Services;

namespace Core.Framework.Helper.Contracts
{
    public interface IServerMessaging
    {
        #region Public Events

        event EventHandler<SocketStatusArgs> StatusConnected;

        #endregion

        #region Public Methods and Operators

        void Connected();

        void Stop();

        #endregion
    }
}