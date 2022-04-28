using System;

namespace Core.Framework.Windows.Implementations.Drag
{
    internal interface ICancelable : IDisposable
    {
        bool IsDisposed { get; }
    }
}