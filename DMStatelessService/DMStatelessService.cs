﻿using System;
using System.Collections.Generic;
using System.Fabric;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using DMSContract;
using DMSService;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using TransactionManagerContract;

namespace DMStatelessService
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class DMStatelessService : StatelessService
    {
        public DMStatelessService(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            //yield return new ServiceInstanceListener(context => this.CreateWcfCommunicationListener(context));   
            return new List<ServiceInstanceListener>()
            {
                // Name parametar ServiceInstanceListener konstruktora moze biti bilo sta (ne mora biti isti kao name za endpoint)
                // samo ne sme biti prazan string - verovatno je to default 
                // tako da ako se koristi jedan listener moze kao prethodno sa yield ... ili kao lista koja vraca jedan ServiceInstanceListener
                // ako se koristi vise Listenera onda se mora setovati Name parametar
                
                // Name Parameter is optional if the Stateless Service has only one communication listener (otherwise it is requested). 
                // If it is not given, the Name is set to DefaultName. 

                new ServiceInstanceListener(context => this.CreateWcfCommunicationListenerDmsDispatch(context), "DMSDispatcherService"),
                new ServiceInstanceListener(context => this.CreateWcfCommunicationListenerTransaction(context), "DMSTransactionService"),
                new ServiceInstanceListener(context => this.CreateWcfCommunicationListenerSCADA(context), "DMSServiceForSCADA")
            };
        }

        private ICommunicationListener CreateWcfCommunicationListenerDmsDispatch(StatelessServiceContext context)
        {
            string host = context.NodeContext.IPAddressOrFQDN;

            // An Endpoint is specified in ServiceManifest.xml file.
            var endpointConfig = context.CodePackageActivationContext.GetEndpoint("DMSDispatcherService");
            int port = endpointConfig.Port;
            var scheme = endpointConfig.Protocol.ToString();
            var pathSufix = endpointConfig.PathSuffix.ToString();
            string uri = string.Format(CultureInfo.InvariantCulture, "net.{0}://{1}:{2}/IDMSContract", scheme, host, port);

            var binding = CreateListenBinding();
            
            // to do: address parameter, vs endpointResourceName?
            var listener = new WcfCommunicationListener<IDMSContract>(
                serviceContext: context,
                wcfServiceObject: new DMSDispatcherService(),
                listenerBinding: binding,
                address: new EndpointAddress(uri)
            );

            return listener;
        }

        private ICommunicationListener CreateWcfCommunicationListenerTransaction(StatelessServiceContext context)
        {
            string host = context.NodeContext.IPAddressOrFQDN;

            // An Endpoint is specified in ServiceManifest.xml file.
            var endpointConfig = context.CodePackageActivationContext.GetEndpoint("DMSTransactionService");
            int port = endpointConfig.Port;
            var scheme = endpointConfig.Protocol.ToString();
            var pathSufix = endpointConfig.PathSuffix.ToString();
            string uri = string.Format(CultureInfo.InvariantCulture, "net.{0}://{1}:{2}/ITransaction", scheme, host, port);

            var binding = CreateListenBinding();
           
            var listener = new WcfCommunicationListener<ITransaction>(
                serviceContext: context,
                wcfServiceObject: new DMSTransactionService(),
                listenerBinding: binding,
                address: new EndpointAddress(uri)
            );

            return listener;
        }

        private ICommunicationListener CreateWcfCommunicationListenerSCADA(StatelessServiceContext context)
        {
            string host = context.NodeContext.IPAddressOrFQDN;

            // An Endpoint is specified in ServiceManifest.xml file.
            var endpointConfig = context.CodePackageActivationContext.GetEndpoint("DMSServiceForSCADA");
            int port = endpointConfig.Port;
            var scheme = endpointConfig.Protocol.ToString();
            var pathSufix = endpointConfig.PathSuffix.ToString();
            string uri = string.Format(CultureInfo.InvariantCulture, "net.{0}://{1}:{2}/IDMSToSCADAContract", scheme, host, port);

            var binding = CreateListenBinding();
                      
            var listener = new WcfCommunicationListener<IDMSToSCADAContract>(
                serviceContext: context,
                wcfServiceObject: new DMSServiceForSCADA(),
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

            //long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                //ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
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
