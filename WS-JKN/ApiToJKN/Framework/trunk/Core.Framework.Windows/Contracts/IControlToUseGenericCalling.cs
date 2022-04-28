namespace Core.Framework.Windows.Contracts
{
    public interface IControlToUseGenericCalling
    {
        #region Public Properties

        string ControlNameSpace { get; set; }

        #endregion

        #region Public Methods and Operators

        void ExecuteControl();

        #endregion
    }
}