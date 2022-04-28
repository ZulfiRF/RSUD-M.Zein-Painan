using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Core.Framework.Helper.Logging;
using Core.Framework.Messaging.Contracts;
using Medifirst.Domain.Common.Default;
using RabbitMQ.Client;

namespace Core.Framework.Messaging.Classes
{
    public class SendMessaging : ISendMessaging
    {
        public string KdRuanganAsal { get; set; }
        public void Close()
        {
            try
            {
                Model.Close(200, "Disconnected");
                Connection.Close();
            }
            catch (Exception e)
            {
                Log.ThrowError(e, "500");
            }
           
        }

        public IConnection Connection { get; set; }
        public IModel Model { get; set; }

        public bool SendMessage(string flag, string content, string hostName = null, string username = null, string password = null, string vhost = null)
        {
            try
            {
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
                //var channel = connection.CreateModel();
                Model = connection.CreateModel();
                var channel = Model;
                channel.ExchangeDeclare(exchange: flag, type: "fanout", durable: true);

                var body = Encoding.UTF8.GetBytes(content + ";" + KdRuanganAsal);
                channel.BasicPublish(exchange: flag,
                                     routingKey: "",
                                     basicProperties: null,
                                     body: body);
                Debug.Print(content);
                Helper.Logging.Log.Info(content);
                return true;
            }
            catch (Exception e)
            {
                Log.ThrowError(e, "500");
                return false;
            }
        }
    }
}
