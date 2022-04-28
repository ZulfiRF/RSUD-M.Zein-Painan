using System.Windows.Controls;
using System.Windows;

namespace Core.Framework.Windows.Contracts
{
    public interface IGenericForm
    {
        Panel HeaderContent { get; }

        Panel MainContent { get; }

        Panel FooterContent { get; }
    }
}