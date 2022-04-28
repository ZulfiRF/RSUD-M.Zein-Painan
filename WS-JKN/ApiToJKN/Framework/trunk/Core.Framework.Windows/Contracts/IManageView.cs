namespace Core.Framework.Windows.Contracts
{
    using System.Windows.Controls;

    public interface IManageView
    {
        #region Public Methods and Operators

        void InitializeControl(ContentControl control);

        #endregion
    }
}