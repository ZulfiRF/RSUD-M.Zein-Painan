using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Core.Framework.Messaging
{
    public abstract class TcpServer : IDisposable
    {
        /// <summary>
        /// This Semaphore protects our clients variable on increment/decrement when a user connects/disconnects.
        /// </summary>
        private readonly SemaphoreSlim clientLock = new SemaphoreSlim(1);

        /// <summary>
        /// Limits how many active connect events we have.
        /// </summary>
        private readonly SemaphoreSlim connectReady = new SemaphoreSlim(10);

        protected int BufferSize = 512;

        /// <summary>
        /// The number of connected clients.
        /// </summary>
        ///
        private int clients;

        private IPAddress listenAddress = IPAddress.Any;

        private TcpListener listener;

        private int port = 80;

        protected TcpServer(int listenPort, IPAddress listenAddress)
        {
            if (listenPort > 0)
            {
                port = listenPort;
            }
            if (listenAddress != null)
            {
                this.listenAddress = listenAddress;
            }
        }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        /// <summary>
        /// Gets the client count.
        /// </summary>
        public int Clients
        {
            get { return clients; }
        }

        /// <summary>
        /// Gets or sets the listener address.
        /// </summary>
        /// <value>
        /// The listener address.
        /// </value>
        public IPAddress ListenAddress
        {
            get { return listenAddress; }
            set { listenAddress = value; }
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public virtual void Start()
        {
            if (listener == null)
            {
                listener = new TcpListener(listenAddress, port);
                ThreadPool.QueueUserWorkItem(Listen, null);
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public virtual void Stop()
        {
            if (listener != null)
            {
                listener.Stop();
            }
            listener = null;
        }

        /// <summary>
        /// Restarts this instance.
        /// </summary>
        public virtual void Restart()
        {
            Stop();
            Start();
        }

        /// <summary>
        /// Listens on the ip and port specified.
        /// </summary>
        /// <param name="state">The state.</param>
        private void Listen(object state)
        {
            listener.Start();
            while (listener != null)
            {
                try
                {
                    listener.BeginAcceptTcpClient(RunClient, null);
                }
                catch (SocketException)
                {
                    /* Ignore */
                }
                connectReady.Wait();
            }
        }

        /// <summary>
        /// Runs the client.
        /// Sets up the UserContext.
        /// Executes in it's own thread.
        /// Utilizes a semaphore(ReceiveReady) to limit the number of receive events active for this client to 1 at a time.
        /// </summary>
        /// <param name="result">The A result.</param>
        private void RunClient(IAsyncResult result)
        {
            TcpClient connection = null;
            if (listener != null)
            {
                try
                {
                    connection = listener.EndAcceptTcpClient(result);
                }
                catch (Exception)
                {
                    connection = null;
                }
            }
            connectReady.Release();
            if (connection != null)
            {
                clientLock.Wait();
                clients++;
                clientLock.Release();

                ThreadPool.QueueUserWorkItem(OnRunClient, connection);

                clientLock.Wait();
                clients--;
                clientLock.Release();
            }
        }

        protected abstract void OnRunClient(object connection);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Stop();
        }
    }
}