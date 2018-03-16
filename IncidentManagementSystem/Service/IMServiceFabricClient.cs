using IMSContract;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using System;

namespace IncidentManagementSystem.Service
{
    public class IMServiceFabricClient : ServicePartitionClient<WcfCommunicationClient<IIMSContract>>
    {
        public IMServiceFabricClient(ICommunicationClientFactory<WcfCommunicationClient<IIMSContract>> communicationClientFactory, Uri serviceUri, ServicePartitionKey partitionKey = null, TargetReplicaSelector targetReplicaSelector = TargetReplicaSelector.Default, string listenerName = null, OperationRetrySettings retrySettings = null)
            : base(communicationClientFactory, serviceUri, partitionKey, targetReplicaSelector, listenerName, retrySettings)
        {
        }
    }
}
