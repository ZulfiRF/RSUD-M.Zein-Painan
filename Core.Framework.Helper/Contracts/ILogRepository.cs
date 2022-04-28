using System;

namespace Core.Framework.Helper.Contracts
{
    public interface ILogRepository
    {
        #region Public Methods and Operators

        void Error(string message);
        void Error(Exception message);

        void Info(string message);

        void Warning(string message);

        #endregion
    }
}