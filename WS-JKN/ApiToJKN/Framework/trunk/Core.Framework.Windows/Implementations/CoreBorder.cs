using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Core.Framework.Windows.Controls;
using Core.Framework.Windows.Extensions;

namespace Core.Framework.Windows.Implementations
{
   
    using System.Windows.Controls;

    public class CoreBorder : Border
    {
        public void CloseControl()
        {
            Child = null;
            GC.Collect();            
        }
    }
}