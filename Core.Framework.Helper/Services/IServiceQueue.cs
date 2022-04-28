namespace Core.Framework.Helper.Services
{
    using System;
    using System.ServiceModel;

    [ServiceContract(CallbackContract = typeof(ICallBackService), SessionMode = SessionMode.Required)]
    public interface IServiceQueue
    {
        #region Public Methods and Operators

        [OperationContract]
        string GetListSubscribe();

        [OperationContract]
        bool Send(Message message);

        [OperationContract]
        bool Subscribe(Guid guid, Message.TypeMessage type);

        [OperationContract]
        bool UnSubscribe(Guid guid);

        #endregion
    }
}