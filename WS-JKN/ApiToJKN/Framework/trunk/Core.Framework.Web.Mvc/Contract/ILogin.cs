namespace Core.Framework.Web.Mvc.Contract
{
    public interface ILogin
    {
        bool Login(string username, string password);
    }
}