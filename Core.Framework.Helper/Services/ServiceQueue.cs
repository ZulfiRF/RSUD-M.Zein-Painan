//using Core.Framework.Helper.Contracts;using Core.Framework.Helper.Configuration;

namespace Core.Framework.Helper.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Threading;
    using System.Xml.Linq;

    using Core.Framework.Helper.Configuration;
    using Core.Framework.Helper.Contracts;

    using Newtonsoft.Json;
    using Core.Framework.Helper.Logging;

    // NOTE: If you change the class name "Service1" here, you must also update the reference to "Service1" in App.config.
    public class ServiceQueue : IServiceQueue
    {
        #region Static Fields

        public static Dictionary<Guid, Message> ListAllData = new Dictionary<Guid, Message>();

        private static Dictionary<int, DateTime> listHasCall = new Dictionary<int, DateTime>();

        #endregion

        #region Public Methods and Operators

        public string GetListSubscribe()
        {
            return JsonConvert.SerializeObject(ListAllData);
        }

        public string GetLog()
        {
            try
            {
                return
                    XDocument.Load(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Log.xml")
                        .ToString(SaveOptions.None);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public void Publish(Message message)
        {
            MessageProperties messageProperties = OperationContext.Current.IncomingMessageProperties;

            var endpointProperty =
                messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            ThreadPool.QueueUserWorkItem(OnPublish, new object[] { ListAllData, message, endpointProperty });
        }

        public bool Send(Message message)
        {
            try
            {
                Publish(message);
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e);
                //Setting.Save("Error", e.ToString());

                return false;
            }
        }

        public bool Subscribe(Guid guid, Message.TypeMessage type)
        {
            try
            {
                lock (ListAllData)
                {
                    try
                    {
                        //var temp = OperationContext.Current.GetCallbackChannel<>();
                        if (ListAllData.Count(x => x.Key.ToString().Equals(guid.ToString())) != 0)
                        {
                            ListAllData.Remove(guid);
                        }
                        //  if (ListAllData.Where(x => x.Key == guid).Count() == 0)
                        {
                            MessageProperties messageProperties = OperationContext.Current.IncomingMessageProperties;

                            var endpointProperty =
                                messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                            var dt = new Message
                                     {
                                         CallBack =
                                             OperationContext.Current.GetCallbackChannel<ICallBackService>(),
                                         Type = type,
                                         Address = endpointProperty.Address,
                                         Port = endpointProperty.Port,
                                         Date = DateTime.Now.Date
                                     };
                            ListAllData.Add(guid, dt);
                            ThreadPool.QueueUserWorkItem(
                                delegate
                                {
                                    try
                                    {
                                        ISetting xml =
                                            new SettingXml(
                                                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                                                + "\\Log.xml",
                                                SettingXml.TypeFile.PathFile);
                                        xml.Log(
                                            "Info",
                                            "Subscribe With ID : " + guid + ", From IP :" + endpointProperty.Address);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                });
                            //  Console.WriteLine(guid.ToString() + " >> " + dt.Type +" << "+dt.CallBack.ToString());
                            //ISetting xml = new SettingXml(@"D:\a.xml",SettingXml.TypeFile.PathFile);
                            //xml.Save("hasil", guid.ToString() + " >> " + dt.Type + " << " + dt.CallBack.ToString());                            
                        }
                    }
                    catch (Exception e)
                    {
                        ThreadPool.QueueUserWorkItem(
                            delegate
                            {
                                try
                                {
                                    ISetting xml =
                                        new SettingXml(
                                            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                                            + "\\Log.xml",
                                            SettingXml.TypeFile.PathFile);
                                    xml.Log("Error", e.ToString());
                                }
                                catch (Exception)
                                {
                                }
                            });
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                try
                {
                    ISetting xml =
                        new SettingXml(
                            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Log.xml",
                            SettingXml.TypeFile.PathFile);
                    xml.Log("Error", e.ToString());
                }
                catch (Exception)
                {
                }

                return false;
            }
        }

        public bool UnSubscribe(Guid guid)
        {
            try
            {
                lock (ListAllData)
                {
                    if (ListAllData.Count(x => x.Key.ToString().Equals(guid.ToString())) != 0)
                    {
                        ListAllData.Remove(guid);
                    }
                    try
                    {
                        MessageProperties messageProperties = OperationContext.Current.IncomingMessageProperties;

                        var endpointProperty =
                            messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                        ISetting xml =
                            new SettingXml(
                                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Log.xml",
                                SettingXml.TypeFile.PathFile);
                        xml.Log("Info", "UnSubscribe With ID : " + guid + ", From IP :" + endpointProperty.Address);
                    }
                    catch (Exception)
                    {
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                try
                {
                    ISetting xml =
                        new SettingXml(
                            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Log.xml",
                            SettingXml.TypeFile.PathFile);
                    xml.Log("Error", e.ToString());
                }
                catch (Exception)
                {
                }

                return false;
            }
        }

        #endregion

        #region Methods

        private void OnPublish(object sender)
        {
            var objects = sender as object[];
            if (objects == null)
            {
                return;
            }
            var listAllData = objects[0] as Dictionary<Guid, Message>;
            var message = objects[1] as Message;
            var endPoint = objects[2] as RemoteEndpointMessageProperty;
            bool flag = false;

            if (listAllData != null && !flag)
            {
                listAllData.ToList().ForEach(
                    n =>
                    {
                        try
                        {
                            n.Value.CallBack.ReceiveMessaged(message);
                            try
                            {
                                ISetting xml =
                                    new SettingXml(
                                        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Log.xml",
                                        SettingXml.TypeFile.PathFile);
                                xml.Log(
                                    "Info",
                                    n.Value.Address + " Success Recive Message \n Content : "
                                    + JsonConvert.SerializeObject(message));
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                            }
                        }
                        catch (CommunicationObjectAbortedException e)
                        {
                            try
                            {
                                Log.Error(e);
                                listAllData.Remove(n.Key);
                                var call = OperationContext.Current.GetCallbackChannel<ICallBackService>();
                                listAllData.Add(n.Key, new Message { CallBack = call, Type = n.Value.Type });
                                call.ReceiveMessaged(message);
                            }
                            catch (Exception ex)
                            {
                                try
                                {
                                    ISetting xml =
                                        new SettingXml(
                                            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                                            + "\\Log.xml",
                                            SettingXml.TypeFile.PathFile);
                                    MessageProperties messageProperties =
                                        OperationContext.Current.IncomingMessageProperties;

                                    var endpointProperty =
                                        messageProperties[RemoteEndpointMessageProperty.Name] as
                                            RemoteEndpointMessageProperty;
                                    xml.Log(
                                        "Error",
                                        "From IP :" + endpointProperty.Address + "\n  Content : " + ex.Message);
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            try
                            {
                                ISetting xml =
                                    new SettingXml(
                                        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Log.xml",
                                        SettingXml.TypeFile.PathFile);
                                xml.Log("Error", e.ToString());
                            }
                            catch (Exception)
                            {
                            }
                            listAllData.Remove(n.Key);
                        }
                    });
            }
        }

        #endregion
    }
}