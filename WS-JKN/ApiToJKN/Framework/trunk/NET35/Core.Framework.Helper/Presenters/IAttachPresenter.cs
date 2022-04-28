namespace Core.Framework.Helper.Presenters
{
    public interface IAttachPresenter<TPresenter> : IAttachPresenter
    {
        TPresenter Presenter { get; }
    }

    public interface IAttachPresenter
    {
        void AttachPresenter(object presenter);
        MessageItem Message { set; }
    }
    public enum MessageType
    {
        Error, Success, Information

    }
    public class MessageItem
    {
        public MessageType MessageType { get; set; }
        public object Content { get; set; }
    }
}