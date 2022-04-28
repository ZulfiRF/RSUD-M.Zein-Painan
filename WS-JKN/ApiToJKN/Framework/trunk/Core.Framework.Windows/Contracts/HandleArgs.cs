namespace Core.Framework.Windows.Contracts
{
    using System;

    public class HandleArgs : EventArgs
    {
        #region Public Properties

        public bool Handled { get; set; }
        public string Message { get; set; }

        #endregion
    }
}