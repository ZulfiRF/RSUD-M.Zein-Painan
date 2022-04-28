using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Framework.Messaging
{
    public enum MessageType
    {
        Send,
        Broadcast,
        Join,
        PrivateMessage,
        AddToGroup,
        RemoveFromGroup,
        SendToGroup,
        None
    }

    public class Message
    {
        public MessageType Type { get; set; }

        public string Value { get; set; }
    }

    public interface IMessage : IDisposable
    {
        Guid SubscribeId { get; }

        void SubscribeName(string name);

        void SubscribeGroup(string group);

        void Recive(Message message, MessageType type);

        void Send(object content, MessageType type);

        void Send(object content, string group);

        event EventHandler<MessageArgs> OnReciveMessage;
    }

    public class ServiceMessaging : IMessage, IDisposable
    {
        private static readonly List<ServiceMessaging> listServiceActive = new List<ServiceMessaging>();
        private static readonly List<IMessage> listService = new List<IMessage>();
        private static readonly List<string> listMessageAccept = new List<string>();

        public ServiceMessaging()
        {
            listServiceActive.Add(this);
        }

        public static void Add(IMessage messageImpl)
        {
            if (listService.FirstOrDefault(n => n == messageImpl) == null)
            {
                listService.Add(messageImpl);
                messageImpl.OnReciveMessage += (sender, evt) =>
                                                          {
                                                              foreach (var message in listServiceActive)
                                                              {
                                                                  if (message.OnReciveMessage != null)
                                                                      message.OnReciveMessage(sender, evt);
                                                              }
                                                          };
            }
        }

        public static void Remove(IMessage messageImpl)
        {
            if (listService.FirstOrDefault(n => n == messageImpl) != null)
                listService.Remove(messageImpl);
        }

        public static void SendMessage(object content, MessageType type)
        {
            foreach (var message in listService)
            {
                message.Send(content, type);
            }
        }

        public static void SendMessage(object content, string groupName)
        {
            foreach (var message in listService)
            {
                message.Send(content, groupName);
            }
        }

        public static void SubscribeNameMessage(string name)
        {
            foreach (var message in listService)
            {
                message.SubscribeName(name);
            }
        }

        public static void SubscribeGroupMessage(string name)
        {
            foreach (var message in listService)
            {
                message.SubscribeGroup(name);
            }
        }

        #region Implementation of IMessage

        public Guid SubscribeId { get; private set; }

        public void SubscribeName(string name)
        {
            foreach (var message in listService)
            {
                message.SubscribeName(name);
            }
        }

        public void SubscribeGroup(string name)
        {
            foreach (var message in listService)
            {
                message.SubscribeGroup(name);
            }
        }

        public void Recive(Message message, MessageType type)
        {
        }

        public void Send(object content, MessageType type)
        {
            foreach (var message in listService)
            {
                message.Send(content, type);
            }
        }

        public void Send(object content, string @group)
        {
        }

        public event EventHandler<MessageArgs> OnReciveMessage;

        #endregion Implementation of IMessage

        public void Dispose()
        {
            listServiceActive.Remove(this);
        }
    }
}