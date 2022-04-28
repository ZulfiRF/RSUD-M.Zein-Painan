namespace Core.Framework.Windows.Contracts
{
    public interface IRefreshData
    {
        /// <summary>
        /// Digunakan Untuk control yang akan di load kembali ke tika Back
        /// </summary>
        void InitLoad();

        
    }

    public interface IRefreshReceiveMessage
    {
        void ReceiveMessage(string message);
    }
}