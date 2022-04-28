namespace Core.Framework.Helper.Contracts
{
    public interface IBackHistoryControl
    {
        #region Public Methods and Operators

        object Back(bool mustReload = false);

        bool IsPostBack { get; set; }

        #endregion
    }
}