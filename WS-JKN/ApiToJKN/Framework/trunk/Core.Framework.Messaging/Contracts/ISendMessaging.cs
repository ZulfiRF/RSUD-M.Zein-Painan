namespace Core.Framework.Messaging.Contracts
{
    public interface ISendMessaging
    {

        bool SendMessage(string flag, string content, string hostName = null, string username = null, string password = null, string vhost = null);

        string KdRuanganAsal { get; set; }
        void Close();
    }
}
