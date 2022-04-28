using System;

namespace Core.Framework.Windows.Implementations
{
    public class RowCountEventArgs : EventArgs
    {
        #region Constructors and Destructors

        public RowCountEventArgs(int rowCount)
        {
            this.RowCount = rowCount;
        }

        #endregion

        #region Public Properties

        public int RowCount { get; set; }

        #endregion
    }
}