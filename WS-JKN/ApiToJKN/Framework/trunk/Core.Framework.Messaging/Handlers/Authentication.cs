using Core.Framework.Messaging.Classes;

namespace Core.Framework.Messaging.Handlers
{
    internal abstract class Authentication : IAuthentication
    {
        private static string origin = string.Empty;
        private static string destination = string.Empty;

        public static string Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        public static string Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        #region IAuthentication Members

        /// <summary>
        /// Attempts to authenticates the specified user context.
        /// If authentication fails it kills the connection.
        /// </summary>
        /// <param name="context">The user context.</param>
        public void Authenticate(Context context)
        {
            if (CheckAuthentication(context))
            {
                context.UserContext.Protocol = context.Header.Protocol;
                context.UserContext.RequestPath = context.Header.RequestPath;
                context.Header = null;
                context.IsSetup = true;
                context.UserContext.OnConnected();
            }
            else
            {
                context.Disconnect();
            }
        }

        #endregion IAuthentication Members

        protected abstract bool CheckAuthentication(Context context);
    }
}