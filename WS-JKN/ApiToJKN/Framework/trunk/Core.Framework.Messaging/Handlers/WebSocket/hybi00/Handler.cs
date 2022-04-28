namespace Core.Framework.Messaging.Handlers.WebSocket.hybi00
{
    /// <summary>
    /// A threadsafe singleton that contains functions which are used to handle incoming connections for the WebSocket Protocol
    /// </summary>
    internal sealed class Handler : WebSocketHandler
    {
        private static Handler instance;

        private Handler()
        {
            Authentication = new Authentication();
        }

        public new static Handler Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (CreateLock)
                    {
                        if (instance == null)
                        {
                            instance = new Handler();
                        }
                    }
                }

                return instance;
            }
        }
    }
}