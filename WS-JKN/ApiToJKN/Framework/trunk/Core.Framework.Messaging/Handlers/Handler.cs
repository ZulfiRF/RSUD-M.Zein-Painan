using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Core.Framework.Messaging.Classes;
using Core.Framework.Messaging.Handlers.WebSocket;

namespace Core.Framework.Messaging.Handlers
{
    /// <summary>
    /// When the protocol has not yet been determined the system defaults to this request handler.
    /// Singleton, just like the other handlers.
    /// </summary>
    public class Handler : IDisposable
    {
        private static Handler instance;

        protected static object CreateLock = new object();
        internal IAuthentication Authentication;

        private readonly Thread[] processSendThreads = new Thread[Environment.ProcessorCount];

        private ConcurrentQueue<HandlerMessage> MessageQueue { get; set; }

        /// <summary>
        /// Cancellation of threads if disposing
        /// </summary>
        private static readonly CancellationTokenSource Cancellation = new CancellationTokenSource();

        protected Handler()
        {
            MessageQueue = new ConcurrentQueue<HandlerMessage>();

            for (int i = 0; i < processSendThreads.Length; i++)
            {
                processSendThreads[i] = new Thread(ProcessSend) { Name = "Alchemy Send Handler Thread " + (i + 1) };
                processSendThreads[i].Start();
            }
        }

        public static Handler Instance
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

        /// <summary>
        /// Handles the initial request.
        /// Attempts to process the header that should have been sent.
        /// Otherwise, through magic and wizardry, the client gets disconnected.
        /// </summary>
        /// <param name="context">The user context.</param>
        public virtual void HandleRequest(Context context)
        {
            if (context.IsSetup)
            {
                context.Disconnect();
            }
            else
            {
                ProcessHeader(context);
            }
        }

        /// <summary>
        /// Processes the header.
        /// </summary>
        /// <param name="context">The user context.</param>
        public void ProcessHeader(Context context)
        {
            string data = Encoding.UTF8.GetString(context.Buffer, 0, context.ReceivedByteCount);
            //Check first to see if this is a flash socket XML request.
            if (data == "<policy-file-request/>\0")
            {
                //if it is, we access the Access Policy Server instance to send the appropriate response.
                context.Server.AccessPolicyServer.SendResponse(context.Connection);
                context.Disconnect();
            }
            else //If it isn't, process http/websocket header as normal.
            {
                context.Header = new Header(data);
                switch (context.Header.Protocol)
                {
                    case Protocol.WebSocketHybi00:
                        context.Handler.UnregisterContext(context);
                        context.Handler = WebSocket.hybi00.Handler.Instance;
                        context.UserContext.DataFrame = new WebSocket.hybi00.DataFrame();
                        context.Handler.RegisterContext(context);
                        break;

                    case Protocol.WebSocketRfc6455:
                        context.Handler.UnregisterContext(context);
                        context.Handler = WebSocket.rfc6455.Handler.Instance;
                        context.UserContext.DataFrame = new WebSocket.rfc6455.DataFrame();
                        context.Handler.RegisterContext(context);
                        break;
                    default:
                        context.Header.Protocol = Protocol.None;
                        break;
                }
                if (context.Header.Protocol != Protocol.None)
                {
                    context.Handler.HandleRequest(context);
                }
                else
                {
                    context.UserContext.Send(Response.NotImplemented, true, true);
                }
            }
        }

        private void ProcessSend()
        {
            while (!Cancellation.IsCancellationRequested)
            {
                while (MessageQueue.IsEmpty)
                {
                    Thread.Sleep(10);
                    if (Cancellation.IsCancellationRequested) return;
                }

                HandlerMessage message;

                if (!MessageQueue.TryDequeue(out message))
                {
                    continue;
                }

                Send(message);
            }
        }

        private void Send(HandlerMessage message)
        {
            message.Context.SendEventArgs.UserToken = message;

            try
            {
                message.Context.SendReady.Wait(Cancellation.Token);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            try
            {
                List<ArraySegment<byte>> data = message.IsRaw ? message.DataFrame.AsRaw() : message.DataFrame.AsFrame();
                message.Context.SendEventArgs.BufferList = data;
                message.Context.Connection.Client.SendAsync(message.Context.SendEventArgs);
            }
            catch
            {
                message.Context.Disconnect();
            }
        }

        /// <summary>
        /// Sends the specified data.
        /// </summary>
        /// <param name="dataFrame">The data.</param>
        /// <param name="context">The user context.</param>
        /// <param name="raw">whether or not to send raw data</param>
        /// <param name="close">if set to <c>true</c> [close].</param>
        public void Send(DataFrame dataFrame, Context context, bool raw = false, bool close = false)
        {
            if (!context.Connected) return;
            var message = new HandlerMessage { DataFrame = dataFrame, Context = context, IsRaw = raw, DoClose = close };
            MessageQueue.Enqueue(message);
        }

        private void SendEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            var message = (HandlerMessage)e.UserToken;

            if (e.SocketError != SocketError.Success)
            {
                message.Context.Disconnect();
                return;
            }

            message.Context.SendReady.Release();
            message.Context.UserContext.OnSend();

            if (message.DoClose)
            {
                message.Context.Disconnect();
            }
        }

        public void RegisterContext(Context context)
        {
            context.SendEventArgs.Completed += SendEventArgs_Completed;
        }

        public void UnregisterContext(Context context)
        {
            context.SendEventArgs.Completed -= SendEventArgs_Completed;
        }

        private class HandlerMessage
        {
            public DataFrame DataFrame { get; set; }

            public Context Context { get; set; }

            public Boolean IsRaw { get; set; }

            public Boolean DoClose { get; set; }
        }

        public void Dispose()
        {
            Cancellation.Cancel();
        }
    }
}