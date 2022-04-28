using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Framework.Windows.Helper.DragDropFrameworkData
{
    public interface IMoveObject
    {
        void WhenDrop(double x , double y);
    }
}
