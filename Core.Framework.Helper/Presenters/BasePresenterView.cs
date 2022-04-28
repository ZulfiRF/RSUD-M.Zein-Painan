namespace Core.Framework.Helper.Presenters
{
    public class BasePresenterView<TView> : BasePresenter
        where TView : IAttachPresenter
    {
        #region Constructors and Destructors

        public BasePresenterView(TView view)
        {
            View = view;
        }

        #endregion

        #region Public Properties

        public TView View { get; set; }

        #endregion
    }
}