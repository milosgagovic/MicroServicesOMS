using DMSCommon.Model;
using FTN.Common;
using IMSContract;
using PubSubContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PubSubscribe
{
    public delegate void PublishUpdateEvent(List<SCADAUpdateModel> update);
    public delegate void PublishCrewEvent(SCADAUpdateModel update);
    public delegate void PublishReportIncident(IncidentReport report);
    public delegate void PublishCallIncident(SCADAUpdateModel call);
    public delegate void PublishUIBreakers(bool IsIncident,long incidentBreaker);

    /// <summary>
    /// Client for Subscribing service
    /// </summary>
    public class Subscriber : IPublishing
    {
        ISubscription subscriptionProxy = null;

        public event PublishUpdateEvent publishUpdateEvent;
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
                NetTcpBinding netTcpbinding = new NetTcpBinding();
                EndpointAddress endpointAddress = new EndpointAddress("net.tcp://localhost:7002/Sub");
                InstanceContext callback = new InstanceContext(this);
                DuplexChannelFactory<ISubscription> channelFactory = new DuplexChannelFactory<ISubscription>(callback, netTcpbinding, endpointAddress);
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

        public void Publish(List<SCADAUpdateModel> update)
        {
            publishUpdateEvent?.Invoke(update);
        }

        public void PublishCrewUpdate(SCADAUpdateModel update)
        {
            publishCrewEvent?.Invoke(update);
        }

        public void PublishIncident(IncidentReport report)
        {
            publishIncident?.Invoke(report);
        }
        public void PublishCallIncident(SCADAUpdateModel call)
        {
            publishCall?.Invoke(call);
        }

        public void PublishUIBreakers(bool IsIncident, long incidentBreaker)
        {
            publiesBreakers?.Invoke(IsIncident, incidentBreaker);
        }
    }
}
