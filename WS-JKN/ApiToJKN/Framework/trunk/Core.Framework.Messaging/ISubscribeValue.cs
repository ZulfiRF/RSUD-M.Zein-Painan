namespace Core.Framework.Messaging
{
    public interface ISubscribeValue
    {
        string GetValue();
    }

    public interface ISubscribeResultData
    {
        string GetValue();

        object Data { set; }
    }
}