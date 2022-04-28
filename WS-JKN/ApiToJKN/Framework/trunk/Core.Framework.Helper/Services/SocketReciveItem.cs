namespace Core.Framework.Helper.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;

    public class SocketReciveItem
    {
        public static bool errorSocket;
        public virtual void Dispose()
        {
            if (SocketServer.ListItem != null)
                SocketServer.ListItem.Remove(this);
        }
        #region Fields

        protected internal readonly List<string> ListSubscribe = new List<string>();

        private List<EventHandler<ItemEventArgs<string>>> listReciveMessage =
            new List<EventHandler<ItemEventArgs<string>>>();

        #endregion

        #region Constructors and Destructors

        public SocketReciveItem()
        {
            MyGuid = Guid.NewGuid();
        }

        public SocketReciveItem(string subscribe)
        {
            MyGuid = Guid.NewGuid();
            Subscribe(subscribe);
            //if (SocketServer.ListItem != null)
            //{
            //    //SocketServer.ListItem.Clear();
            //    SocketServer.ListItem.Add(this);
            //}
        }

        #endregion

        #region Public Events

        public event EventHandler<ItemEventArgs<string>> ReciveMessage;

        public event EventHandler<ItemEventArgs<string>> SubscribeChanged;

        public event EventHandler<ItemEventArgs<string>> UnubscribeChanged;

        #endregion

        #region Properties

        public Guid MyGuid { get; set; }

        #endregion

        #region Public Methods and Operators

        public override bool Equals(object obj)
        {
            if (obj is SocketReciveItem)
            {
                return MyGuid.Equals((obj as SocketReciveItem).MyGuid);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual void OnReciveMessage(ItemEventArgs<string> e, object sender = null)
        {
            EventHandler<ItemEventArgs<string>> handler = ReciveMessage;
            if (handler != null)
            {
                if (sender == null)
                    handler(this, e);
                else
                    handler(sender, e);
            }
        }

        public void Subscribe(string value)
        {
            ListSubscribe.Add(value);
            if (SocketServer.ListItem != null)
            {
                if (!SocketServer.ListItem.Any(n => n.Equals(this)))
                {
                    SocketServer.ListItem.Add(this);
                }
            }
            OnSubscribeChanged(new ItemEventArgs<string>(value));
        }

        public void Unsubscribe(string value)
        {
            ListSubscribe.Remove(value);
            OnUnubscribeChanged(new ItemEventArgs<string>(value));
        }

        #endregion

        #region Methods

        protected virtual void OnSubscribeChanged(ItemEventArgs<string> e)
        {
            EventHandler<ItemEventArgs<string>> handler = SubscribeChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnUnubscribeChanged(ItemEventArgs<string> e)
        {
            EventHandler<ItemEventArgs<string>> handler = UnubscribeChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        public System.Net.Sockets.NetworkStream NetworkStream { get; set; }
    }
}