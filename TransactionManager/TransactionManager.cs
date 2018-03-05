﻿using DMSCommon.Model;
using DMSContract;
using FTN.Common;
using FTN.ServiceContracts;
using IMSContract;
using IncidentManagementSystem.Service;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using OMSSCADACommon;
using OMSSCADACommon.Commands;
using OMSSCADACommon.Responses;
using PubSubContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using TransactionManager.ServicecFabricClients;
using TransactionManagerContract;

namespace TransactionManager
{
    public class TransactionManager : IOMSClient
    {
        // properties for providing communication infrastructure for 2PC protocol
        List<WCFDMSTransactionClient> transactionProxys;
        List<TransactionCallback> transactionCallbacks;
        ITransaction proxyTransactionNMS;
        ITransaction proxyTransactionDMS;
        ITransactionSCADA proxyTransactionSCADA;
        TransactionCallback callBackTransactionNMS;
        TransactionCallback callBackTransactionDMS;
        TransactionCallback callBackTransactionSCADA;
        NetworkModelGDAProxy ProxyToNMSService;
        NetworkGDAServiceFabric proxyToNMServiceFabric;
        IDMSContract proxyToDispatcherDMS;
        IMServiceFabricClient IMSCommunicationClient;
        ServiceFabricDMSClient proxyToDMS;
        SubscriberServiceCloud proxyToSubscriberServiceCloud;
        WCFDMSTransactionClient _WCFDMSTransactionClient;
        WCFDMSTransactionClient _WCFNMSTransactionClient;
        ModelGDATMS gdaTMS;
        long instanceID = 0;

        private SCADAClient scadaClient;
        private SCADAClient ScadaClient
        {
            get
            {
                if (scadaClient == null)
                {
                    scadaClient = new SCADAClient(new EndpointAddress("net.tcp://localhost:4000/SCADAService"));
                }
                return scadaClient;
            }
            set { scadaClient = value; }
        }

        public List<WCFDMSTransactionClient> TransactionProxys { get => transactionProxys; set => transactionProxys = value; }
        public List<TransactionCallback> TransactionCallbacks { get => transactionCallbacks; set => transactionCallbacks = value; }
        public ITransaction ProxyTransactionNMS { get => proxyTransactionNMS; set => proxyTransactionNMS = value; }
        public ITransaction ProxyTransactionDMS { get => proxyTransactionDMS; set => proxyTransactionDMS = value; }
        public ITransactionSCADA ProxyTransactionSCADA { get => proxyTransactionSCADA; set => proxyTransactionSCADA = value; }
        public TransactionCallback CallBackTransactionNMS { get => callBackTransactionNMS; set => callBackTransactionNMS = value; }
        public TransactionCallback CallBackTransactionDMS { get => callBackTransactionDMS; set => callBackTransactionDMS = value; }
        public TransactionCallback CallBackTransactionSCADA { get => callBackTransactionSCADA; set => callBackTransactionSCADA = value; }

        public TransactionManager()
        {
            TransactionProxys = new List<WCFDMSTransactionClient>();
            TransactionCallbacks = new List<TransactionCallback>();

            InitializeChanels();

            gdaTMS = new ModelGDATMS();
        }

        public TransactionManager(long instanceID)
        {
            TransactionProxys = new List<WCFDMSTransactionClient>();
            TransactionCallbacks = new List<TransactionCallback>();

            InitializeChanels();
            this.instanceID = instanceID;
            gdaTMS = new ModelGDATMS();
        }

        private void InitializeChanels()
        {

            CallBackTransactionNMS = new TransactionCallback();
            CallBackTransactionDMS = new TransactionCallback();


            ///
            NetTcpBinding binding = new NetTcpBinding();
            // Create a partition resolver
            IServicePartitionResolver partitionResolver = ServicePartitionResolver.GetDefault();
            // create a  WcfCommunicationClientFactory object.
            var wcfClientFactory = new WcfCommunicationClientFactory<IIMSContract>
                (clientBinding: binding, servicePartitionResolver: partitionResolver);

            //
            // Create a client for communicating with the ICalculator service that has been created with the
            // Singleton partition scheme.
            //
            IMSCommunicationClient = new IMServiceFabricClient(
                            wcfClientFactory,
                            new Uri("fabric:/ServiceFabricOMS/IMStatelessService"),
                            ServicePartitionKey.Singleton);



            ///
            NetTcpBinding binding2 = new NetTcpBinding();
            // Create a partition resolver
            IServicePartitionResolver partitionResolver2 = ServicePartitionResolver.GetDefault();

            //create calllback
            // duplex channel for DMS transaction
            //  TransactionCallback CallBackTransactionDMS2 = new TransactionCallback();
            // create a  WcfCommunicationClientFactory object.
            var wcfClientFactory2 = new WcfCommunicationClientFactory<ITransaction>
                (clientBinding: binding2, servicePartitionResolver: partitionResolver2, callback: CallBackTransactionDMS);

            //
            // Create a client for communicating with the ICalculator service that has been created with the
            // Singleton partition scheme.
            //
            _WCFDMSTransactionClient = new WCFDMSTransactionClient(
                            wcfClientFactory2,
                            new Uri("fabric:/ServiceFabricOMS/DMStatelessService"),
                            ServicePartitionKey.Singleton,
                            listenerName: "DMSTransactionService");

            TransactionProxys.Add(_WCFDMSTransactionClient);
            TransactionCallbacks.Add(CallBackTransactionDMS);


            NetTcpBinding binding3 = new NetTcpBinding();
            // Create a partition resolver
            IServicePartitionResolver partitionResolver3 = ServicePartitionResolver.GetDefault();

            TransactionCallbacks.Add(CallBackTransactionNMS);
            // create a  WcfCommunicationClientFactory object.
            var wcfClientFactory3 = new WcfCommunicationClientFactory<ITransaction>
                (clientBinding: binding3, servicePartitionResolver: partitionResolver3, callback: CallBackTransactionNMS);

            //
            // Create a client for communicating with the ICalculator service that has been created with the
            // Singleton partition scheme.
            //
            _WCFNMSTransactionClient = new WCFDMSTransactionClient(
                            wcfClientFactory3,
                            new Uri("fabric:/ServiceFabricOMS/NMStatelessService"),
                            ServicePartitionKey.Singleton,
                            listenerName: "NMTransactionServiceEndpoint");

            TransactionProxys.Add(_WCFNMSTransactionClient);

            IServicePartitionResolver partitionResolverToDMS = ServicePartitionResolver.GetDefault();
            var wcfClientFactoryToDMS = new WcfCommunicationClientFactory<IDMSContract>
                (clientBinding: new NetTcpBinding(), servicePartitionResolver: partitionResolverToDMS);
            proxyToDMS = new ServiceFabricDMSClient(
                            wcfClientFactoryToDMS,
                            new Uri("fabric:/ServiceFabricOMS/DMStatelessService"),
                            ServicePartitionKey.Singleton,
                            listenerName: "DMSDispatcherService");



            NetTcpBinding binding4 = new NetTcpBinding();
            // Create a partition resolver
            IServicePartitionResolver partitionResolver4 = ServicePartitionResolver.GetDefault();

            //create calllback
            // duplex channel for DMS transaction
            //  TransactionCallback CallBackTransactionDMS2 = new TransactionCallback();
            // create a  WcfCommunicationClientFactory object.
            var wcfClientFactory4 = new WcfCommunicationClientFactory<ISubscription>
                (clientBinding: binding4, servicePartitionResolver: partitionResolver4);

            //
            // Create a client for communicating with the ICalculator service that has been created with the
            // Singleton partition scheme.
            //
            proxyToSubscriberServiceCloud = new SubscriberServiceCloud(
                            wcfClientFactory4,
                            new Uri("fabric:/ServiceFabricOMS/PubSubStatelessService"),
                            ServicePartitionKey.Singleton,
                            listenerName: "SubscriptionService");


        }

        #region 2PC methods

        public void Enlist(Delta d)
        {
            Console.WriteLine("Transaction Manager calling enlist");

            foreach (WCFDMSTransactionClient svc in TransactionProxys)
            {
                svc.InvokeWithRetry(client => client.Channel.Enlist());
            }

            //            ProxyTransactionSCADA.Enlist();

            while (true)
            {
                if (TransactionCallbacks.Where(k => k.AnswerForEnlist == TransactionAnswer.Unanswered).Count() > 0)
                {
                    Thread.Sleep(1000);
                    continue;
                }
                else
                {
                    Prepare(d);
                    break;
                }
            }
        }

        public void Prepare(Delta delta)
        {
            Console.WriteLine("Transaction Manager calling prepare");
            //foreach (WCFDMSTransactionClient c in TransactionProxys)
            //{
            //    c.InvokeWithRetry(x => x.Channel.Prepare(delta));
            //}
            Delta fixedGuidDelta = gdaTMS.GetFixedDelta(delta);

            _WCFNMSTransactionClient.InvokeWithRetry(client => client.Channel.Prepare(delta));

            _WCFDMSTransactionClient.InvokeWithRetry(client => client.Channel.Prepare(fixedGuidDelta));

            //  proxyTransactionNMS.Prepare(delta);
            // ScadaDelta scadaDelta = GetDeltaForSCADA(delta);
            do
            {
                Thread.Sleep(50);
            } while (CallBackTransactionNMS.AnswerForPrepare.Equals(TransactionAnswer.Unanswered));

            if (CallBackTransactionNMS.AnswerForPrepare.Equals(TransactionAnswer.Unprepared))
            {
                Rollback();
            }
            else
            {
                // tek nakon sto prodje prepere na nms i dms onda se pise u bazu
               

                while (true)
                {
                    if (TransactionCallbacks.Where(k => k.AnswerForPrepare == TransactionAnswer.Unanswered).Count() > 0)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    else if (TransactionCallbacks.Where(u => u.AnswerForPrepare == TransactionAnswer.Unprepared).Count() > 0)
                    {
                        Rollback();
                        break;
                    }
                    PushDataToDatabase(fixedGuidDelta);
                    Commit();
                    break;
                }
            }
        }

        private void PushDataToDatabase(Delta delta)
        {
            using (NMSAdoNet ctx = new NMSAdoNet())
            {
                foreach (ResourceDescription rd in delta.InsertOperations)
                {
                    ctx.ResourceDescription.Add(rd);
                    foreach (Property item in rd.Properties)
                    {
                        ctx.PropertyValue.Add(item.PropertyValue);
                        ctx.Property.Add(item);
                    }
                    ctx.SaveChanges();
                }
                foreach (ResourceDescription rd in delta.UpdateOperations)
                {
                    List<Property> propForRD = ctx.Property.Where(x => x.ResourceDescription_Id == rd.IdDb).ToList();
                    propForRD.ForEach(x => x.PropertyValue = ctx.PropertyValue.Where(y => y.Id == x.IdDB).FirstOrDefault());
                    ctx.SaveChanges();
                }

                foreach (ResourceDescription rd in delta.DeleteOperations)
                {
                }
            }
        }

        private void Commit()
        {
            Console.WriteLine("Transaction Manager calling commit");
            foreach (WCFDMSTransactionClient c in TransactionProxys)
            {
                c.InvokeWithRetry(x => x.Channel.Commit());
            }

            //   ProxyTransactionSCADA.Commit();
        }

        public void Rollback()
        {
            Console.WriteLine("Transaction Manager calling rollback");
            foreach (WCFDMSTransactionClient c in TransactionProxys)
            {
                c.InvokeWithRetry(x => x.Channel.Rollback());
            }
            //foreach (ITransaction svc in TransactionProxys)
            //{
            //    svc.Rollback();
            //}
            //ProxyTransactionSCADA.Rollback();
        }

        #endregion

        #region IOMSClient CIMAdapter Methods

        // so, in order for network to be initialized, UpdateSystem must be called first

        /// <summary>
        /// Called by ModelLabs(CIMAdapter) when Static data changes
        /// </summary>
        /// <param name="d">Delta</param>
        /// <returns></returns>
        public bool UpdateSystem(Delta d)
        {
            Console.WriteLine("Update System started." + d.Id);
            Enlist(d);
            //  Prepare(d);
            return true;
        }

        public void ClearNMSDB()
        {
            using (NMSAdoNet ctx = new NMSAdoNet())
            {
                var tableNames = ctx.Database.SqlQuery<string>("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME NOT LIKE '%Migration%'").ToList();
                foreach (var tableName in tableNames)
                {
                    ctx.Database.ExecuteSqlCommand(string.Format("DELETE FROM {0}", tableName));
                }

                ctx.SaveChanges();
            }
        }

        #endregion

        #region  IOMSClient DispatcherApp Methods

        public TMSAnswerToClient GetNetwork()
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=storageomsmsc;AccountKey=Hu9kN3vtydaxcJoqHLdScZghIDqQqxVoGxZf5yi1pE/W0NY5nC5np68CeE2b72sFGTtB160zl3DBD5XY1EQQUQ==;EndpointSuffix=core.windows.net");
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("logs");
            var bla = cloudBlobContainer.CreateIfNotExists();

            CloudAppendBlob appendBlob = cloudBlobContainer.GetAppendBlobReference("logfile");

            if(!appendBlob.Exists())
            {
                appendBlob.CreateOrReplace();
            }
           
            appendBlob.AppendText(string.Format("TransactionManager, GetNetwork(), instanceID: {0}, Time: {1}{2}", this.instanceID, DateTime.Now, Environment.NewLine));
            cloudBlobContainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            List<Element> listOfDMSElement = proxyToDMS.InvokeWithRetry(client => client.Channel.GetAllElements());
            // ako se ne podignu svi servisi na DMSu, ovde pada
            //   List<Element> listOfDMSElement = proxyToDispatcherDMS.GetAllElements();

            List<ResourceDescription> resourceDescriptionFromNMS = new List<ResourceDescription>();
            List<ResourceDescription> descMeas = new List<ResourceDescription>();
           
            gdaTMS.GetExtentValues(ModelCode.BREAKER).ForEach(u => resourceDescriptionFromNMS.Add(u));
            gdaTMS.GetExtentValues(ModelCode.CONNECTNODE).ForEach(u => resourceDescriptionFromNMS.Add(u));
            gdaTMS.GetExtentValues(ModelCode.ENERGCONSUMER).ForEach(u => resourceDescriptionFromNMS.Add(u));
            gdaTMS.GetExtentValues(ModelCode.ENERGSOURCE).ForEach(u => resourceDescriptionFromNMS.Add(u));
            gdaTMS.GetExtentValues(ModelCode.ACLINESEGMENT).ForEach(u => resourceDescriptionFromNMS.Add(u));
            gdaTMS.GetExtentValues(ModelCode.DISCRETE).ForEach(u => resourceDescriptionFromNMS.Add(u));
            gdaTMS.GetExtentValues(ModelCode.ANALOG).ForEach(u => resourceDescriptionFromNMS.Add(u));

            int GraphDeep = proxyToDMS.InvokeWithRetry(client => client.Channel.GetNetworkDepth());

          
            var crews = IMSCommunicationClient.InvokeWithRetry(client => client.Channel.GetCrews());
            var incidentReports = IMSCommunicationClient.InvokeWithRetry(client => client.Channel.GetAllReports());

            TMSAnswerToClient answer = new TMSAnswerToClient(resourceDescriptionFromNMS, listOfDMSElement, GraphDeep, descMeas, crews, incidentReports);
            return answer;
        }

        public void SendCommandToSCADA(TypeOfSCADACommand command, string mrid, CommandTypes commandtype, float value)
        {
            try
            {
                Command c = MappingEngineTransactionManager.Instance.MappCommand(command, mrid, commandtype, value);

                // to do: ping
                Response r = scadaClient.ExecuteCommand(c);
                //Response r = SCADAClientInstance.ExecuteCommand(c);

            }
            catch (Exception e)
            { }
        }

        public void SendCrew(IncidentReport report)
        {
            proxyToDMS.InvokeWithRetry(client => client.Channel.SendCrewToDms(report));
            //proxyToDispatcherDMS.SendCrewToDms(report);
            return;
        }

        // currently unused
        public bool IsNetworkAvailable()
        {
            bool retVal = false;
            try
            {
                retVal = proxyToDispatcherDMS.IsNetworkAvailable();
            }
            catch (System.ServiceModel.EndpointNotFoundException e)
            {
                //Console.WriteLine("DMSDispatcher is not available yet.");
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {

            }

            return retVal;
        }

        private ScadaDelta GetDeltaForSCADA(Delta d)
        {
            // zasto je ovo bitno, da ima measurement direction?? 
            // po tome odvajas measuremente od ostatka?
            List<ResourceDescription> rescDesc = d.InsertOperations.Where(u => u.ContainsProperty(ModelCode.MEASUREMENT_DIRECTION)).ToList();
            ScadaDelta scadaDelta = new ScadaDelta();

            foreach (ResourceDescription rd in rescDesc)
            {
                ScadaElement element = new ScadaElement();
                if (rd.ContainsProperty(ModelCode.MEASUREMENT_TYPE))
                {
                    string type = rd.GetProperty(ModelCode.MEASUREMENT_TYPE).ToString();
                    if (type == "Analog")
                    {
                        element.Type = DeviceTypes.ANALOG;                       
                        element.UnitSymbol = ((UnitSymbol)rd.GetProperty(ModelCode.MEASUREMENT_UNITSYMB).AsEnum()).ToString();
                        element.WorkPoint = rd.GetProperty(ModelCode.ANALOG_NORMVAL).AsFloat();
                    }
                    else if (type == "Discrete")
                    {
                        element.Type = DeviceTypes.DIGITAL;
                    }
                }

                element.ValidCommands = new List<CommandTypes>() { CommandTypes.CLOSE, CommandTypes.OPEN };
                element.ValidStates = new List<OMSSCADACommon.States>() { OMSSCADACommon.States.CLOSED, OMSSCADACommon.States.OPENED };

                if (rd.ContainsProperty(ModelCode.IDOBJ_MRID))
                {
                    //element.Name = rd.GetProperty(ModelCode.IDOBJ_NAME).ToString();
                    element.Name = rd.GetProperty(ModelCode.IDOBJ_MRID).ToString();
                }
                scadaDelta.InsertOps.Add(element);
            }
            return scadaDelta;
        }

        #endregion

        // da li se ove metode ikada pozivaju?  Onaj console1 ne koristimo?

        // SVUDA PRVO PROVERITI DA LI JE IMS DOSTUPAN? 
        // tj naprviti metodu koja to radi
        #region Unused? check this!!!

        public void GetNetworkWithOutParam(out List<Element> DMSElements, out List<ResourceDescription> resourceDescriptions, out int GraphDeep)
        {
            //List<Element> listOfDMSElement = proxyToDMS.InvokeWithRetry(client => client.Channel.GetAllElements());
            List<Element> listOfDMSElement = new List<Element>();//proxyToDMS.GetAllElements();
            List<ResourceDescription> resourceDescriptionFromNMS = new List<ResourceDescription>();
            List<ACLine> acList = proxyToDispatcherDMS.GetAllACLines();
            List<Node> nodeList = proxyToDispatcherDMS.GetAllNodes();
            List<Source> sourceList = proxyToDispatcherDMS.GetAllSource();
            List<Switch> switchList = proxyToDispatcherDMS.GetAllSwitches();
            List<Consumer> consumerList = proxyToDispatcherDMS.GetAllConsumers();

            acList.ForEach(u => listOfDMSElement.Add(u));
            nodeList.ForEach(u => listOfDMSElement.Add(u));
            sourceList.ForEach(u => listOfDMSElement.Add(u));
            switchList.ForEach(u => listOfDMSElement.Add(u));
            consumerList.ForEach(u => listOfDMSElement.Add(u));

            gdaTMS.GetExtentValues(ModelCode.BREAKER).ForEach(u => resourceDescriptionFromNMS.Add(u));
            gdaTMS.GetExtentValues(ModelCode.CONNECTNODE).ForEach(u => resourceDescriptionFromNMS.Add(u));
            gdaTMS.GetExtentValues(ModelCode.ENERGCONSUMER).ForEach(u => resourceDescriptionFromNMS.Add(u));
            gdaTMS.GetExtentValues(ModelCode.ENERGSOURCE).ForEach(u => resourceDescriptionFromNMS.Add(u));
            gdaTMS.GetExtentValues(ModelCode.ACLINESEGMENT).ForEach(u => resourceDescriptionFromNMS.Add(u));
            GraphDeep = proxyToDispatcherDMS.GetNetworkDepth();
            TMSAnswerToClient answer = new TMSAnswerToClient(resourceDescriptionFromNMS, null, GraphDeep, null, null, null);
            resourceDescriptions = resourceDescriptionFromNMS;
            DMSElements = listOfDMSElement;
            GraphDeep = proxyToDispatcherDMS.GetNetworkDepth();

            // return resourceDescriptionFromNMS;
        }

        public List<List<ElementStateReport>> GetElementStateReportsForMrID(string mrID)
        {
            return IMSCommunicationClient.InvokeWithRetry(client => client.Channel.GetElementStateReportsForMrID(mrID));
        }

        public List<ElementStateReport> GetElementStateReportsForSpecificTimeInterval(DateTime startTime, DateTime endTime)
        {
            return IMSCommunicationClient.InvokeWithRetry(client => client.Channel.GetElementStateReportsForSpecificTimeInterval(startTime, endTime));
        }

        public List<ElementStateReport> GetElementStateReportsForSpecificMrIDAndSpecificTimeInterval(string mrID, DateTime startTime, DateTime endTime)
        {
            return IMSCommunicationClient.InvokeWithRetry(client => client.Channel.GetElementStateReportsForSpecificMrIDAndSpecificTimeInterval(mrID, startTime, endTime));
        }

        public void SendCrew(string mrid)
        {
            throw new NotImplementedException();
        }

        public List<Crew> GetCrews()
        {
            return IMSCommunicationClient.InvokeWithRetry(client => client.Channel.GetCrews());
        }

        public bool AddCrew(Crew crew)
        {
            return IMSCommunicationClient.InvokeWithRetry(client => client.Channel.AddCrew(crew));
        }

        public void AddReport(IncidentReport report)
        {
            IMSCommunicationClient.InvokeWithRetry(client => client.Channel.AddReport(report));
        }

        public List<IncidentReport> GetAllReports()
        {
            return IMSCommunicationClient.InvokeWithRetry(client => client.Channel.GetAllReports());
        }

        public List<List<IncidentReport>> GetReportsForMrID(string mrID)
        {
            return IMSCommunicationClient.InvokeWithRetry(client => client.Channel.GetReportsForMrID(mrID));
        }

        public List<IncidentReport> GetReportsForSpecificTimeInterval(DateTime startTime, DateTime endTime)
        {
            return IMSCommunicationClient.InvokeWithRetry(client => client.Channel.GetReportsForSpecificTimeInterval(startTime, endTime));
        }

        public List<IncidentReport> GetReportsForSpecificMrIDAndSpecificTimeInterval(string mrID, DateTime startTime, DateTime endTime)
        {
            return IMSCommunicationClient.InvokeWithRetry(client => client.Channel.GetReportsForSpecificMrIDAndSpecificTimeInterval(mrID, startTime, endTime));
        }

        public List<ElementStateReport> GetAllElementStateReports()
        {
            return IMSCommunicationClient.InvokeWithRetry(client => client.Channel.GetAllElementStateReports());
        }

        public List<List<IncidentReport>> GetReportsForSpecificDateSortByBreaker(List<string> mrids, DateTime date)
        {
            return IMSCommunicationClient.InvokeWithRetry(client => client.Channel.GetReportsForSpecificDateSortByBreaker(mrids, date));
        }

        public List<List<IncidentReport>> GetAllReportsSortByBreaker(List<string> mrids)
        {
            return IMSCommunicationClient.InvokeWithRetry(client => client.Channel.GetAllReportsSortByBreaker(mrids));
        }

        public void Subscribe()
        {
            proxyToSubscriberServiceCloud.InvokeWithRetry(client => client.Channel.Subscribe());
        }

        public void UnSubscribe()
        {
            proxyToSubscriberServiceCloud.InvokeWithRetry(client => client.Channel.UnSubscribe());
        }

        #endregion
    }
}