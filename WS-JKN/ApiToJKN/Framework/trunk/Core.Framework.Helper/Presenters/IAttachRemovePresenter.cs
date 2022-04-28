namespace Core.Framework.Helper.Presenters
{
    public interface IAttachRemovePresenter<out TPresenter> : IAttachPresenter
    {
        void RemovePresenter();

    }
}