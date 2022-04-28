using System;

namespace Core.Framework.Windows.Implementations.Drag
{
    internal sealed class DefaultDisposable : IDisposable
    {
        public static readonly DefaultDisposable Instance = new DefaultDisposable();

        static DefaultDisposable()
        {
        }

        private DefaultDisposable()
        {
        }

        public void Dispose()
        {
        }
    }
}