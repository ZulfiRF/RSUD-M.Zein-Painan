using System;

namespace Core.Framework.Windows
{
    public class OnThemeChangedEventArgs : EventArgs
    {
        public Theme Theme { get; set; }
        public Accent Accent { get; set; }
    }
}