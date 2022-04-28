using Core.Framework.Helper.ServiceQueue;

namespace Core.Framework.Helper.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;


    [CallbackBehavior(UseSynchronizationContext = true, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class Handler : IServiceQueueCallback
    {
        #region Fields

        private readonly List<string> subscribeList;

        #endregion

        #region Constructors and Destructors

        public Handler()
        {
            subscribeList = new List<string>();
        }

        #endregion

        #region Delegates

        public delegate void OtherHandler(object myObject, ChooseArgs myArgs);

        #endregion

        #region Public Events

        public event OtherHandler OtherCommand;

        #endregion

        #region Public Methods and Operators

        public IAsyncResult BeginReceiveMessaged(
            Message message,
            AsyncCallback callback,
            object asyncState)
        {
            return null;
        }

        public void EndReceiveMessaged(IAsyncResult result)
        {
        }

        public void OnOtherCommand(object myobject, ChooseArgs myargs)
        {
            OtherHandler handler = OtherCommand;
            if (handler != null)
            {
                handler(myobject, myargs);
            }
        }

        public void ReceiveMessaged(Message message)
        {
            var args = new ChooseArgs(message);
            var temp = new List<string>(subscribeList);
            if (temp.Any(n => n.Equals(message.Module)))
            {
                OnOtherCommand(this, args);
            }
        }

        public void Subscribe(string subscribe)
        {
            subscribeList.Add(subscribe);
        }

        #endregion

        public void ReceiveMessaged(Helper.ServiceQueue.Message message)
        {
            throw new NotImplementedException();
        }
    }
}