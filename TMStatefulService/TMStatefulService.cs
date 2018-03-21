using System;
using System.Collections.Generic;
using System.Fabric;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using FTN.Common;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using TransactionManager;
using TransactionManagerContract;

namespace TMStatefulService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class TMStatefulService : StatefulService
    {
        public TMStatefulService(StatefulServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        //protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        //{
        //    return new ServiceReplicaListener[0];
        //}

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {            //yield return new ServiceInstanceListener(context => this.CreateWcfCommunicationListener(context));   
            return new List<ServiceReplicaListener>()
            {
                // Name parametar ServiceInstanceListener konstruktora moze biti bilo sta (ne mora biti isti kao name za endpoint)
                // samo ne sme biti prazan string - verovatno je to default 
                // tako da ako se koristi jedan listener moze kao prethodno sa yield ... ili kao lista koja vraca jedan ServiceInstanceListener
                // ako se koristi vise Listenera onda se mora setovati Name parametar
                new ServiceReplicaListener(context => this.CreateWcfCommunicationListener(context), "TMServiceEndpoint"),
              //  new ServiceInstanceListener(context => this.CreateWcfHiCommunicationListener(context), "WCFHiServiceEndpoint")
            };
        }

        private ICommunicationListener CreateWcfCommunicationListener(StatefulServiceContext context)
        {
            string host = context.NodeContext.IPAddressOrFQDN;
            // ServiceManifest fajl
            var endpointConfig = context.CodePackageActivationContext.GetEndpoint("TMServiceEndpoint");
            int port = endpointConfig.Port;
            var scheme = endpointConfig.Protocol.ToString();
            var pathSufix = endpointConfig.PathSuffix.ToString();

            var binding = new NetTcpBinding();
            //var binding = WcfUtility.CreateTcpListenerBinding();
            string uri = string.Format(CultureInfo.InvariantCulture, "net.{0}://{1}:{2}/TMServiceEndpoint", scheme, host, port);

            var listener = new WcfCommunicationListener<IOMSClient>(
                serviceContext: context,
                wcfServiceObject: new TransactionManager(this.Context),
                listenerBinding: BindingForTCP.CreateCustomNetTcp(),
                address: new EndpointAddress(uri)
               // endpointResourceName: "TMServiceEndpoint"
            );


            return listener;
        }


        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await myDictionary.TryGetValueAsync(tx, "Counter");

                    ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is saved to the secondary replicas.
                    await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
            }
        }
    }
}
