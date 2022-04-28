namespace Core.Framework.Helper.Presenters
{
    public interface IAttachPresenter<out TPresenter> : IAttachPresenter
    {
        #region Public Properties
        TPresenter Presenter { get;  }

        #endregion
    }

    public interface IAttachPresenter
    {
        #region Public Properties

        MessageItem Message { set; }

        #endregion

        #region Public Methods and Operators

        void AttachPresenter(object presenter);


        #endregion
    }
}