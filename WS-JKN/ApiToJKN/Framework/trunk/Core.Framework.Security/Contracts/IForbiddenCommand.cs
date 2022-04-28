using System.Windows;

namespace Core.Framework.Security.Contracts
{
    public interface IForbiddenCommand
    {
        bool Authentication(string username, string passowrd, bool isNewLogin);
        string MsgException { get; set; }
    }
}
