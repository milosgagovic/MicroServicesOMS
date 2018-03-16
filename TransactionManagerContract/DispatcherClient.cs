using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using System;

namespace TransactionManagerContract
{
    // unable to use this because Dispatcher app as a WPF app type,
    // so it has to connect to the service as a typical WCF client (ChannelFactory or ClientBase)
    public class DispatcherClient : ServicePartitionClient<WcfCommunicationClient<IOMSClient>>
    {
        public DispatcherClient(ICommunicationClientFactory<WcfCommunicationClient<IOMSClient>> communicationClientFactory, Uri serviceUri, ServicePartitionKey partitionKey = null, TargetReplicaSelector targetReplicaSelector = TargetReplicaSelector.Default, string listenerName = null, OperationRetrySettings retrySettings = null)
            : base(communicationClientFactory, serviceUri, partitionKey, targetReplicaSelector, listenerName, retrySettings)
        {
        }
    }
}
