namespace Core.Framework.Windows.Contracts
{
    using System;

    public interface IDoneLoad
    {
        #region Public Events

        event EventHandler DoneLoad;

        #endregion

        #region Public Properties

        int CountCallDoneLoad { get; set; }
        int HasCountCallDoneLoad { get; set; }

        #endregion

        #region Public Methods and Operators

        void CallDoneLoad();

        #endregion
    }
}