namespace Core.Framework.Windows.Contracts
{
    public interface IGenericCalling
    {
        #region Public Properties

        string Title { get; }

        #endregion

        #region Public Methods and Operators

        void InitView();

        #endregion
    }
}