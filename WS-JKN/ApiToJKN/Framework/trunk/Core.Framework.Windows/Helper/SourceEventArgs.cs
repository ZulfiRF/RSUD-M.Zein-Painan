namespace Core.Framework.Windows.Helper
{
    using System;
    using System.Collections.Generic;

    public class SourceEventArgs<T> : EventArgs
    {
        #region Constructors and Destructors

        public SourceEventArgs(IEnumerable<T> listSource)
        {
            this.ListSource = listSource;
        }

        #endregion

        #region Public Properties

        public IEnumerable<T> ListSource { get; set; }

        #endregion
    }
}