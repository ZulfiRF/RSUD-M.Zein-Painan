using System.Windows;

namespace Core.Framework.Windows.Contracts
{
    public interface IPassingView
    {
        FrameworkElement PassingView { get; set; }
        string UserHead { get; set; }  
    }
}