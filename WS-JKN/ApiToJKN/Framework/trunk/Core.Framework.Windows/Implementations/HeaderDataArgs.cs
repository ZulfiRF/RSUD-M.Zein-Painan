namespace Core.Framework.Windows.Implementations
{
    using System;
    using System.Collections.Generic;

    public class HeaderDataArgs : EventArgs
    {
        #region Constructors and Destructors

        public HeaderDataArgs()
        {
            this.Headers = new Dictionary<string, object>();
        }

        #endregion

        #region Public Properties

        public Dictionary<string, object> Headers { get; set; }

        #endregion
    }
}