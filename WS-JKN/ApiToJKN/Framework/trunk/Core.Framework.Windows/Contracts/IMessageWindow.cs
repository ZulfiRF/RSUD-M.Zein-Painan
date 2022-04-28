namespace Core.Framework.Windows.Contracts
{
    public interface IMessageWindow
    {
        #region Public Methods and Operators

        void Error(string title, string content);

        void Info(string title, string content);

        #endregion
    }
}