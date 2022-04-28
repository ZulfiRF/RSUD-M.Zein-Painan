using System.Diagnostics;
using Core.Framework.Helper.Logging;
using Core.Framework.Helper.ServiceQueue;

namespace Core.Framework.Helper.Services
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;

    using Core.Framework.Helper.Configuration;
    using Core.Framework.Helper.Contracts;


    using Newtonsoft.Json;

    public class QueueService : IDisposable
    {
        #region Static Fields

        private static string EndPointStatic;

        private static InstanceContext ic;

        private static List<Guid> listGuid;

        #endregion

        #region Fields

        private string _endPoint;

        private int noRepeat;

        private ISetting setting;

        private ServiceQueueClient svc;

        #endregion

        #region Constructors and Destructors

        public QueueService(InstanceContext _ic, string endPoint)
        {
            ic = _ic;
            setting = new SettingXml();
            if (listGuid == null)
            {
                listGuid = new List<Guid>();
            }

            //string s = new PointService().GetSetting("EndPointServiceQueue");

            EndPoint = endPoint;
            if (!string.IsNullOrEmpty(EndPoint))
            {
                svc = new ServiceQueueClient(
                    ic,
                    new NetTcpBinding(SecurityMode.None),
                    new EndpointAddress(EndPoint));
                    //ic, PointService.SetNetTCP(), PointService.SetNetEndPoint(new PointService().GetSetting("EndPointServiceQueue")));
                svc.InnerChannel.Faulted += InnerChannelFaulted;
                Service = svc;
            }
            else if (!string.IsNullOrEmpty(EndPoint))
            {
                svc = new ServiceQueueClient(ic, "NetTcpBinding_IServiceQueue", new EndpointAddress(EndPoint));
                    //ic, PointService.SetNetTCP(), PointService.SetNetEndPoint(new PointService().GetSetting("EndPointServiceQueue")));
                svc.InnerChannel.Faulted += InnerChannelFaulted;
                Service = svc;
            }
            else
            {
                return;
            }

            //svc.InnerDuplexChannel.Faulted += InnerDuplexChannel_Faulted;
        }

        public QueueService(InstanceContext _ic)
            : this(_ic, new SettingXml().GetValue("address"))
        {
        }

        #endregion

        #region Public Events

        public event EventHandler Faulted;

        #endregion

        #region Public Properties

        public string EndPoint
        {
            get
            {
                return _endPoint;
            }
            set
            {
                try
                {
                    _endPoint = value;
                    EndPointStatic = _endPoint;
                    svc = new ServiceQueueClient(ic, "NetTcpBinding_IServiceQueue", new EndpointAddress(value));
                        //ic, PointService.SetNetTCP(), PointService.SetNetEndPoint(new PointService().GetSetting("EndPointServiceQueue")));
                    if (listGuid == null)
                    {
                        listGuid = new List<Guid>();
                    }
                    svc.InnerChannel.Faulted += InnerChannelFaulted;
                    Service = svc;
                    string endpoint = setting.GetValue("address");
                    if (!string.IsNullOrEmpty(EndPoint))
                    {
                        svc.Endpoint.Address = new EndpointAddress(EndPoint);
                        Service.Endpoint.Address = new EndpointAddress(EndPoint);
                    }
                    else if (!string.IsNullOrEmpty(endpoint))
                    {
                        svc.Endpoint.Address = new EndpointAddress(endpoint);
                        Service.Endpoint.Address = new EndpointAddress(endpoint);
                    }
                    else
                    {
                        return;
                    }
                    if (svc != null)
                    {
                        svc.Endpoint.Address = new EndpointAddress(value);
                    }
                    if (Service != null)
                    {
                        Service.Endpoint.Address = new EndpointAddress(value);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public ServiceQueueClient Service { get; set; }

        #endregion

        #region Public Methods and Operators

        public static void Clear()
        {
            try
            {
                var setting = new SettingXml();
                if (listGuid == null)
                {
                    listGuid = new List<Guid>();
                }

                //string s = new PointService().GetSetting("EndPointServiceQueue");

                string endpoint = setting.GetValue("address");
                var svc = new ServiceQueueClient(ic);
                if (!string.IsNullOrEmpty(EndPointStatic))
                {
                    svc = new ServiceQueueClient(ic, "NetTcpBinding_IServiceQueue", new EndpointAddress(EndPointStatic));
                        //ic, PointService.SetNetTCP(), PointService.SetNetEndPoint(new PointService().GetSetting("EndPointServiceQueue")));
                }
                else if (!string.IsNullOrEmpty(endpoint))
                {
                    svc = new ServiceQueueClient(ic, "NetTcpBinding_IServiceQueue", new EndpointAddress(endpoint));
                        //ic, PointService.SetNetTCP(), PointService.SetNetEndPoint(new PointService().GetSetting("EndPointServiceQueue")));
                }
                foreach (Guid guid in listGuid)
                {
                    svc.UnSubscribe(guid);
                }
            }
            catch (Exception)
            {
            }
        }

        public void Dispose()
        {
            try
            {
                //foreach (var guid in listGuid)
                //{
                //    svc.UnSubscribe(guid);
                //}
            }
            catch (Exception)
            {
            }
            if (svc != null && svc.State != CommunicationState.Faulted)
            {
                svc.Close();
            }
        }

        public AntrianPasienRegistrasi_T RegistrasiPasein(AntrianPasienRegistrasi_T guid)
        {
            lock (svc)
            {
                try
                {
                    if (svc.State == CommunicationState.Faulted)
                    {
                        svc.Close();
                        svc.Open();
                    }
                    return svc.RegistrasiPasien(guid);
                }
                catch (CommunicationException exception)
                {
                    Log.Error(exception);
                    svc.Abort();
                    svc.Close();
                    svc = new ServiceQueueClient(ic, "NetTcpBinding_IServiceQueue");
                    svc.Open();
                    return RegistrasiPasein(guid);
                    
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public void Send(Message message)
        {
            lock (svc)
            {
                try
                {
                    if (svc.State == CommunicationState.Faulted)
                    {
                        svc.Close();
                        svc.Open();
                    }
                    //svc.Send(message);
                    Debug.WriteLine(JsonConvert.SerializeObject(message));
                }
                catch (CommunicationException exception)
                {
                    Log.Error(exception);
                }
                catch (Exception exception)
                {
                    Log.Error(exception);
                }
            }
        }

        public bool Subscribe(Guid guid, Message.TypeMessage type)
        {
            bool result = false;
            lock (svc)
            {
                try
                {
                    if (svc.State == CommunicationState.Faulted)
                    {
                        svc.Close();
                        svc.Open();
                    }
                   // result = svc.Subscribe(guid, type);
                }
                catch (CommunicationException)
                {
                    result = false; // Subscribe(guid, type);
                }
                catch (Exception)
                {
                    result = false;
                }
                finally
                {
                    if (result)
                    {
                        if (listGuid != null)
                        {
                            listGuid.Add(guid);
                        }
                    }
                }
                if (result)
                {
                    noRepeat = 0;
                }
                return result;
            }
        }

        public void UnSubscribe(Guid guid)
        {
            lock (svc)
            {
                try
                {
                    if (svc.State == CommunicationState.Faulted)
                    {
                        svc.Close();
                        svc.Open();
                    }
                    svc.UnSubscribe(guid);
                }
                catch (CommunicationException exception)
                {
                    Log.Error(exception);
                }
                catch (Exception exception)
                {
                    Log.Error(exception);
                }
            }
        }

        #endregion

        #region Methods

        private void InnerChannelFaulted(object sender, EventArgs e)
        {
            if (noRepeat < 100)
            {
                try
                {
                    noRepeat++;
                    setting = new SettingXml();
                    if (listGuid == null)
                    {
                        listGuid = new List<Guid>();
                    }

                    //string s = new PointService().GetSetting("EndPointServiceQueue");

                    string endpoint = EndPoint;
                    if (!string.IsNullOrEmpty(EndPoint))
                    {
                        svc = new ServiceQueueClient(
                            ic,
                            "NetTcpBinding_IServiceQueue",
                            new EndpointAddress(EndPoint));
                            //ic, PointService.SetNetTCP(), PointService.SetNetEndPoint(new PointService().GetSetting("EndPointServiceQueue")));
                        //svc.InnerChannel.Faulted += InnerChannelFaulted;
                        Service = svc;
                    }
                    else if (!string.IsNullOrEmpty(endpoint))
                    {
                        svc = new ServiceQueueClient(
                            ic,
                            "NetTcpBinding_IServiceQueue",
                            new EndpointAddress(endpoint));
                            //ic, PointService.SetNetTCP(), PointService.SetNetEndPoint(new PointService().GetSetting("EndPointServiceQueue")));
                        //svc.InnerChannel.Faulted += InnerChannelFaulted;
                        Service = svc;
                    }
                    else
                    {
                        return;
                    }
                    //ic, PointService.SetNetTCP(), PointService.SetNetEndPoint(new PointService().GetSetting("EndPointServiceQueue")));
                    svc.InnerChannel.Faulted -= InnerChannelFaulted;
                    //svc.InnerChannel.Faulted += InnerChannelFaulted;
                    Service = svc;
                    if (listGuid == null)
                    {
                        listGuid = new List<Guid>();
                    }
                    if (Faulted != null)
                    {
                        Faulted(this, null);
                    }
                }
                catch (StackOverflowException)
                {
                }
                catch (Exception exception)
                {
                    Log.Error(exception);
                }
            }
        }

        #endregion
    }
}