using DMSCommon;
using FTN.Common;
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
                NetTcpBinding binding = new NetTcpBinding();
                binding.CloseTimeout = TimeSpan.FromDays(10);
                binding.OpenTimeout = TimeSpan.FromDays(10);
                binding.ReceiveTimeout = TimeSpan.FromDays(10);
                binding.SendTimeout = TimeSpan.FromDays(10);
                binding.MaxReceivedMessageSize = Int32.MaxValue;
                EndpointAddress endpointAddress = new EndpointAddress("net.tcp://23.99.82.238:4080/SubscriptionService");
                InstanceContext callback = new InstanceContext(this);
                DuplexChannelFactory<ISubscription> channelFactory = new DuplexChannelFactory<ISubscription>(callback, BindingForTCP.CreateCustomNetTcp(), endpointAddress);
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
    }
}
