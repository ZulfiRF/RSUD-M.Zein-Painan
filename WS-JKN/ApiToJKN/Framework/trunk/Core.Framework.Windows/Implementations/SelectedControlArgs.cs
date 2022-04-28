using System;
using System.Windows;

namespace Core.Framework.Windows.Implementations
{
    public class SelectedControlArgs : EventArgs
    {
        #region Constructors and Destructors

        public SelectedControlArgs(FrameworkElement control)
        {
            this.Control = control;
        }

        #endregion

        #region Public Properties

        public FrameworkElement Control { get; set; }

        #endregion
    }
}