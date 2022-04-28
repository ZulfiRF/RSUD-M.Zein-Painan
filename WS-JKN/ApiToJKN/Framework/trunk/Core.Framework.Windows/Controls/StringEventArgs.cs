using System;

namespace Core.Framework.Windows.Controls
{
    public class StringEventArgs : EventArgs
    {
        #region Constructors and Destructors

        public StringEventArgs(string value)
        {
            this.Value = value;
        }

        #endregion

        #region Public Properties

        public string Value { get; set; }

        #endregion
    }
}