namespace Core.Framework.Windows.Implementations.Drag
{
    public interface IManualInterTabClient : IInterTabClient
    {
        void Add(object item);
        void Remove(object item);
    }
}