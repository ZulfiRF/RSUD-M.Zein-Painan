namespace Core.Framework.Windows.Controls
{
    using System;

    public class ClosingWindowEventHandlerArgs : EventArgs
    {
        #region Public Properties

        public bool Cancelled { get; set; }

        #endregion
    }
}