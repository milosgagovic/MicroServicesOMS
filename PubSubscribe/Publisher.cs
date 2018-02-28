using DMSCommon.Model;
using FTN.Common;
using IMSContract;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
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
        PublishServiceCloud proxyToCloud = null;

        public Publisher()
        {
            CreateProxy();
        }

        public void PublishUpdate(List<SCADAUpdateModel> update)
        {
            try
            {
                proxyToCloud.InvokeWithRetry(client => client.Channel.Publish(update));
            }
            catch { }
        }

        // not used
        public void PublishCrew(SCADAUpdateModel update)
        {
            try
            {
                proxyToCloud.InvokeWithRetry(client => client.Channel.PublishCrewUpdate(update));
             //   proxy.PublishCrewUpdate(update);
            }
            catch { }
        }

        public void PublishIncident(IncidentReport report)
        {
            try
            {
                proxyToCloud.InvokeWithRetry(client => client.Channel.PublishIncident(report));
               // proxy.PublishIncident(report);
            }
            catch { }
        }

        public void PublishCallIncident(SCADAUpdateModel call)
        {
            try
            {
                proxyToCloud.InvokeWithRetry(c => c.Channel.PublishCallIncident(call));
            }
            catch(FaultException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void PublishUIBreaker(bool isIncident,long incidentBreaker)
        {
            try
            {
                proxyToCloud.InvokeWithRetry(c => c.Channel.PublishUIBreakers(isIncident, incidentBreaker));
            }
            catch { }
        }

        private void CreateProxy()
        {
            NetTcpBinding binding = new NetTcpBinding();
            // Create a partition resolver
            IServicePartitionResolver partitionResolver = ServicePartitionResolver.GetDefault();

            //create calllback
            // duplex channel for DMS transaction
            //  TransactionCallback CallBackTransactionDMS2 = new TransactionCallback();
            // create a  WcfCommunicationClientFactory object.
            var wcfClientFactory = new WcfCommunicationClientFactory<IPublishing>
                (clientBinding: binding, servicePartitionResolver: partitionResolver);

            //
            // Create a client for communicating with the ICalculator service that has been created with the
            // Singleton partition scheme.
            //
            proxyToCloud = new PublishServiceCloud(
                            wcfClientFactory,
                            new Uri("fabric:/ServiceFabricOMS/PubSubStatelessService"),
                            ServicePartitionKey.Singleton,
                            listenerName: "PublishingService");


            //string address = "";
            //try
            //{
            //    address = "net.tcp://localhost:7001/Pub";
            //    EndpointAddress endpointAddress = new EndpointAddress(address);
            //    NetTcpBinding netTcpBinding = new NetTcpBinding();
            //    proxy = ChannelFactory<IPublishing>.CreateChannel(netTcpBinding, endpointAddress);
            //}
            //catch (Exception e)
            //{
            //    throw e;
            //    //TODO log error;
            //}

        }
    }
}
