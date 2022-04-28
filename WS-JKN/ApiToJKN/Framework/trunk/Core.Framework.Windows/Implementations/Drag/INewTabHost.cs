using System.Windows;

namespace Core.Framework.Windows.Implementations.Drag
{
    public interface INewTabHost<out TElement> where TElement : UIElement
    {
        TElement Container { get; }
        TabablzControl TabablzControl { get; }
    }
}