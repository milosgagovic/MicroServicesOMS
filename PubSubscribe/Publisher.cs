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
    /// <summary>
    /// Client for Publishing service
    /// </summary>
    public class Publisher
    {
        IPublishing proxy;

        public Publisher()
        {
            CreateProxy();
        }

        public void PublishUpdate(List<SCADAUpdateModel> update)
        {
            try
            {
                proxy.Publish(update);
            }
            catch { }
        }

        // not used
        public void PublishCrew(SCADAUpdateModel update)
        {
            try
            {
                proxy.PublishCrewUpdate(update);
            }
            catch { }
        }

        public void PublishIncident(IncidentReport report)
        {
            try
            {
                proxy.PublishIncident(report);
            }
            catch { }
        }

        public void PublishCallIncident(SCADAUpdateModel call)
        {
            try
            {
                proxy.PublishCallIncident(call);
            }
            catch { }
        }

        public void PublishUIBreaker(bool isIncident,long incidentBreaker)
        {
            try
            {
                proxy.PublishUIBreakers(isIncident, incidentBreaker);
            }
            catch { }
        }

        private void CreateProxy()
        {
            string address = "";
            try
            {
                address = "net.tcp://localhost:7001/Pub";
                EndpointAddress endpointAddress = new EndpointAddress(address);
                NetTcpBinding netTcpBinding = new NetTcpBinding();
                proxy = ChannelFactory<IPublishing>.CreateChannel(netTcpBinding, endpointAddress);
            }
            catch (Exception e)
            {
                throw e;
                //TODO log error;
            }

        }
    }
}
