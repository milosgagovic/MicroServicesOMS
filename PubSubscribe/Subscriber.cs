using DMSCommon;
using IMSContract;
using PubSubContract;
using System;
using System.Collections.Generic;
using System.ServiceModel;


namespace PubSubscribe
{
    public delegate void PublishDigitalUpdateEvent(List<UIUpdateModel> update);
    public delegate void PublishAnalogUpdateEvent(List<UIUpdateModel> update);
    public delegate void PublishCrewEvent(UIUpdateModel update);
    public delegate void PublishReportIncident(IncidentReport report);
    public delegate void PublishCallIncident(UIUpdateModel call);
    public delegate void PublishUIBreakers(bool IsIncident, long incidentBreaker);

    /// <summary>
    /// Client for Subscribing service
    /// </summary>
    public class Subscriber : IPublishing
    {
        ISubscription subscriptionProxy = null;

        public event PublishDigitalUpdateEvent publishDigitalUpdateEvent;
        public event PublishAnalogUpdateEvent publishAnalogUpdateEvent;
        public event PublishCrewEvent publishCrewEvent;
        public event PublishReportIncident publishIncident;
        public event PublishCallIncident publishCall;
        public event PublishUIBreakers publiesBreakers;

        public Subscriber()
        {
            CreateProxy();
        }

        private void CreateProxy()
        {
            try
            {  //***git
                NetTcpBinding binding = CreateListenBinding();
                //EndpointAddress endpointAddress = new EndpointAddress("net.tcp://omsmsc.westeurope.cloudapp.azure.com::4080/SubscriptionService");
                EndpointAddress endpointAddress = new EndpointAddress("net.tcp://localhost:4080/SubscriptionService");
                InstanceContext callback = new InstanceContext(this);
                DuplexChannelFactory<ISubscription> channelFactory = new DuplexChannelFactory<ISubscription>(callback, binding, endpointAddress);
                subscriptionProxy = channelFactory.CreateChannel();
            }
            catch (Exception e)
            {
                throw e;
                //TODO  Log error : PubSub not started
            }
        }

        public void Subscribe()
        {
            try
            {
                subscriptionProxy.Subscribe();
            }
            catch (Exception e)
            {
                //throw e;
                //TODO  Log error 
            }

        }

        public void UnSubscribe()
        {
            try
            {
                subscriptionProxy.UnSubscribe();
            }
            catch (Exception e)
            {
                throw e;
                //TODO  Log error 
            }
        }

        public void PublishDigitalUpdate(List<UIUpdateModel> update)
        {
            publishDigitalUpdateEvent?.Invoke(update);
        }

        public void PublishAnalogUpdate(List<UIUpdateModel> update)
        {
            publishAnalogUpdateEvent?.Invoke(update);
        }

        public void PublishCrewUpdate(UIUpdateModel update)
        {
            publishCrewEvent?.Invoke(update);
        }

        public void PublishIncident(IncidentReport report)
        {
            publishIncident?.Invoke(report);
        }

        public void PublishCallIncident(UIUpdateModel call)
        {
            publishCall?.Invoke(call);
        }

        public void PublishUIBreakers(bool IsIncident, long incidentBreaker)
        {
            publiesBreakers?.Invoke(IsIncident, incidentBreaker);
        }

        private NetTcpBinding CreateListenBinding()
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None)
            {
                SendTimeout = TimeSpan.MaxValue,
                ReceiveTimeout = TimeSpan.MaxValue,
               // OpenTimeout = TimeSpan.FromSeconds(5),
               // CloseTimeout = TimeSpan.FromSeconds(5),
                //OpenTimeout = TimeSpan.MaxValue,
                //CloseTimeout = TimeSpan.MaxValue,
                OpenTimeout = TimeSpan.FromMinutes(5),
                CloseTimeout = TimeSpan.FromMinutes(5),
                MaxConnections = int.MaxValue,
                MaxReceivedMessageSize = 1024 * 1024
            };
            binding.MaxBufferSize = (int)binding.MaxReceivedMessageSize;
            binding.MaxBufferPoolSize = Environment.ProcessorCount * binding.MaxReceivedMessageSize;
            return binding;
        }
    }
}
