namespace Core.Framework.Helper.Presenters
{
    public class BasePresenterView<TView> : BasePresenter where TView : IAttachPresenter
    {
        public TView View { get; set; }

        public BasePresenterView(TView view)
        {
            View = view;
        }
    }
}
