using System;
using System.Threading;

namespace Core.Framework.Windows.Implementations.Drag.Referenceless
{
    internal sealed class AnonymousDisposable : ICancelable, IDisposable
    {
        private volatile Action _dispose;

        public bool IsDisposed
        {
            get
            {
                return this._dispose == null;
            }
        }

        public AnonymousDisposable(Action dispose)
        {
            this._dispose = dispose;
        }

        public void Dispose()
        {
            var location1 = _dispose;
            var action = Interlocked.Exchange<Action>(ref location1, (Action)null);
            if (action == null)
                return;
            action();
        }
    }
}
