﻿using System;
using System.Collections.Generic;
using System.Fabric;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using PubSubContract;
using PubSubscribeService.Services;

namespace PubSubStatelessService
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class PubSubStatelessService : StatelessService
    {
        public PubSubStatelessService(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        //protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        //{
        //    return new ServiceInstanceListener[0];
        //}
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {            //yield return new ServiceInstanceListener(context => this.CreateWcfCommunicationListener(context));   
            return new List<ServiceInstanceListener>()
            {
                // Name parametar ServiceInstanceListener konstruktora moze biti bilo sta (ne mora biti isti kao name za endpoint)
                // samo ne sme biti prazan string - verovatno je to default 
                // tako da ako se koristi jedan listener moze kao prethodno sa yield ... ili kao lista koja vraca jedan ServiceInstanceListener
                // ako se koristi vise Listenera onda se mora setovati Name parametar
                new ServiceInstanceListener(context => this.CreateWcfCommunicationListener(context), "PublishingService"),
                new ServiceInstanceListener(context => this.CreateSubscriptionServiceCommunicationListener(context), "SubscriptionService")
            };
        }
        private ICommunicationListener CreateWcfCommunicationListener(StatelessServiceContext context)
        {
            string host = context.NodeContext.IPAddressOrFQDN;
            // ServiceManifest fajl
            var endpointConfig = context.CodePackageActivationContext.GetEndpoint("PublishingServiceEndpoint");
            int port = endpointConfig.Port;
            var scheme = endpointConfig.Protocol.ToString();
            var pathSufix = endpointConfig.PathSuffix.ToString();

            var binding = new NetTcpBinding();
            //var binding = WcfUtility.CreateTcpListenerBinding();
            string uri = string.Format(CultureInfo.InvariantCulture, "net.{0}://{1}:{2}/PubSubServiceEndpoint", scheme, host, port);

            var listener = new WcfCommunicationListener<IPublishing>(
                serviceContext: context,
                wcfServiceObject: new PublishingService(),
                listenerBinding: binding,
                address: new EndpointAddress(uri)
            );


            return listener;
        }

        private ICommunicationListener CreateSubscriptionServiceCommunicationListener(StatelessServiceContext context)
        {
            string host = context.NodeContext.IPAddressOrFQDN;
            // ServiceManifest fajl
            var endpointConfig = context.CodePackageActivationContext.GetEndpoint("SubscriptionServiceEndpoint");
            int port = endpointConfig.Port;
            var scheme = endpointConfig.Protocol.ToString();
            var pathSufix = endpointConfig.PathSuffix.ToString();

            var binding = new NetTcpBinding();
            //var binding = WcfUtility.CreateTcpListenerBinding();
            string uri = string.Format(CultureInfo.InvariantCulture, "net.{0}://{1}:{2}/SubscriptionService", scheme, host, port);

            var listener = new WcfCommunicationListener<ISubscription>(
                serviceContext: context,
                wcfServiceObject: new SubscriptionService(),
                listenerBinding: binding,
                address: new EndpointAddress(uri)
            );


            return listener;
        }
        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}