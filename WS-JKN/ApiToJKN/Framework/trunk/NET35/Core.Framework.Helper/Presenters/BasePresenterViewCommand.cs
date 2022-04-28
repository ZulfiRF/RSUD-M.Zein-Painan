namespace Core.Framework.Helper.Presenters
{
    public class BasePresenterViewCommand<TView, TCommand> : BasePresenter
        where TCommand : class
    {
        protected TView View { get; set; }
        protected TCommand Command { get; set; }
        public BasePresenterViewCommand()
        {

        }
        public BasePresenterViewCommand(TView view, TCommand command)
        {
            View = view;
            Command = command;
            if (View is IAttachPresenter)
                (View as IAttachPresenter).AttachPresenter(this);
        }
    }
}