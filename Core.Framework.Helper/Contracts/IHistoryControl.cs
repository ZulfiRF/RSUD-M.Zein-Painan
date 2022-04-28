namespace Core.Framework.Helper.Contracts
{
    public interface IHistoryControl<in T> : IBackHistoryControl
    {
        #region Public Methods and Operators

        void Next(T control);

        #endregion
    }
}