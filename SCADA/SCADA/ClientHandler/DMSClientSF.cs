﻿using DMSContract;
using IMSContract;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.ClientHandler
{
    public class DmsClientSF : ServicePartitionClient<WcfCommunicationClient<IDMSToSCADAContract>>
    {
        public DmsClientSF(ICommunicationClientFactory<WcfCommunicationClient<IDMSToSCADAContract>> communicationClientFactory, Uri serviceUri, ServicePartitionKey partitionKey = null, TargetReplicaSelector targetReplicaSelector = TargetReplicaSelector.Default, string listenerName = null, OperationRetrySettings retrySettings = null)
            : base(communicationClientFactory, serviceUri, partitionKey, targetReplicaSelector, listenerName, retrySettings)
        {
        }
    }
}
