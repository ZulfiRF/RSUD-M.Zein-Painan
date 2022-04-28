using System.ServiceModel;

namespace Core.Framework.Helper.Services
{
    public interface ICallBackService
    {
        #region Public Methods and Operators

        [OperationContract(IsOneWay = true)]
        void ReceiveMessaged(Message message);

        #endregion
    }
}