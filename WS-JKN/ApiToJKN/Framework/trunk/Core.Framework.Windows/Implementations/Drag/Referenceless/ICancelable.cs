using System;

namespace Core.Framework.Windows.Implementations.Drag.Referenceless
{
    internal interface ICancelable : IDisposable
    {
        bool IsDisposed { get; }
    }
}
