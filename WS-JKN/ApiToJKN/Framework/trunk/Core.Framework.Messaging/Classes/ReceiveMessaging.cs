using System;
using System.Diagnostics;
using System.Text;
using Core.Framework.Helper;
using Core.Framework.Helper.Logging;
using Core.Framework.Messaging.Contracts;
using Medifirst.Domain.Common.Default;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Core.Framework.Messaging.Classes
{
    public class ReceiveMessaging : IReceiveMessaging
    {
        public static string KdRuangan { get; set; }
        public IConnection Connection { get; set; }
        public IModel Model { get; set; }

        public void ReceiveMessage(string flag, string hostName = null, string username = null, string password = null, string vhost = null)
        {
            try
            {
                KdRuangan = flag;

                if (string.IsNullOrEmpty(hostName))
                    hostName = DefaultLoadData.DefaultOnLoad("RabbitMQHostName");
                if (string.IsNullOrEmpty(username))
                    username = DefaultLoadData.DefaultOnLoad("RabbitMQUserName");
                if (string.IsNullOrEmpty(password))
                    password = DefaultLoadData.DefaultOnLoad("RabbitMQPassword");
                if (string.IsNullOrEmpty(vhost))
                    vhost = DefaultLoadData.DefaultOnLoad("RabbitMQVirtualHost");

                var rabbit = new ConnectionFactory()
                {
                    HostName = hostName,
                    UserName = username,
                    Password = password,
                    VirtualHost = vhost
                };

                //var connection = rabbit.CreateConnection();
                Connection = rabbit.CreateConnection();

                var connection = Connection;
                connection.CallbackException += ConnectionOnCallbackException;

                //var channel = connection.CreateModel();
                Model = connection.CreateModel();
                var channel = Model;

                channel.CallbackException += ChannelOnCallbackException;
                channel.ModelShutdown += ChannelOnModelShutdown;
                channel.ExchangeDeclare(exchange: flag, type: "fanout", durable: true);

                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName,
                                  exchange: flag,
                                  routingKey: "");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received -= ConsumerOnReceived;
                consumer.Received += ConsumerOnReceived;
                consumer.Shutdown += ConsumerOnShutdown;
                channel.BasicConsume(queue: queueName,
                                     noAck: true,
                                     consumer: consumer);                                


            }
            catch (Exception ex)
            {
                //Debug.Print("ERROR " + ex.Message);
                //Helper.Logging.Log.Error(ex);
                //Log.ThrowError(ex, "500");
                Log.Error(ex);
            }
        }

        public void Close()
        {
            try
            {
                Model.Close(200, "Disconnected");
                Connection.Close();
            }
            catch (Exception e)
            {
                //Log.ThrowError(e,"500");
            }            
        }

        private void ChannelOnModelShutdown(object sender, ShutdownEventArgs e)
        {
            //Log.ThrowError(new Exception(e.ReplyText), "500");
        }

        private void ChannelOnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            //Log.ThrowError(e.Exception, "500");
        }

        private void ConnectionOnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            //Log.ThrowError(e.Exception, "500");
        }

        private void ConsumerOnShutdown(object sender, ShutdownEventArgs e)
        {
            //Log.ThrowError(new Exception(e.ReplyText), "500");
        }


        private void ConsumerOnReceived(object sender, BasicDeliverEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Body);
            var msg = "Message Receive > " + message;
            Debug.Print(msg);
            Helper.Logging.Log.Info(msg);
            OnMessageReceive(new ItemEventArgs<string>(message));
        }

        public event EventHandler<ItemEventArgs<string>> MessageReceive;
       

        public void OnMessageReceive(ItemEventArgs<string> e)
        {
            var handler = MessageReceive;
            if (handler != null) handler(this, e);
        }
    }
}