using Core.Framework.Messaging.Classes;

namespace Core.Framework.Messaging.Handlers
{
    internal interface IAuthentication
    {
        void Authenticate(Context context);
    }
}