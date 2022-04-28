namespace Core.Framework.Helper.Contracts
{
    public interface IUploadFile
    {
        #region Public Methods and Operators

        void Upload(byte[] binary, string title);

        #endregion
    }
}