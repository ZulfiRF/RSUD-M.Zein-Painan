using System;
using System.Windows.Controls;
using Core.Framework.Windows.Contracts;

namespace Core.Framework.Windows.Implementations
{
    public class CoreBorderClose : Border, ICloseControl
    {
        public void CloseControl()
        {
            Child = null;
            GC.Collect();
        }
    }
}