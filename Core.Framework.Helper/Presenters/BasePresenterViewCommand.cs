using System;
using System.Security.AccessControl;
using System.ServiceModel.Configuration;

namespace Core.Framework.Helper.Presenters
{
    public class BasePresenterViewCommand<TView, TCommand> : BasePresenter
        where TCommand : class
    {
        public void Clear()
        {
            OnDestroy();
            View = default(TView);
            Command = default(TCommand);
            GC.Collect();
        }
        public event EventHandler Destroy;
        ~BasePresenterViewCommand()
        {
            Clear();

        }
        protected virtual void OnDestroy()
        {
            var handler = Destroy;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #region Constructors and Destructors

        public BasePresenterViewCommand()
        {
        }

        public BasePresenterViewCommand(TView view, TCommand command)
        {
            View = view;
            Command = command;
            if (View is IAttachPresenter)
            {
                (View as IAttachPresenter).AttachPresenter(this);
            }
        }

        #endregion

        #region Properties
        
        public TView View { get; set; }
        public TCommand Command { get; protected set; }
        #endregion
    }
}