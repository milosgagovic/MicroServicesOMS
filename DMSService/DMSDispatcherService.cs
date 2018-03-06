using DMSCommon;
using DMSCommon.Model;
using DMSContract;
using IMSContract;
using IncidentManagementSystem.Service;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using OMSSCADACommon;
using PubSubscribe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DMSService
{
    public class DMSDispatcherService : IDMSContract
    {
        private IMServiceFabricClient imsCommunicationClient;

        private IMServiceFabricClient IMSCommunicationClient
        {
            get
            {
                if (imsCommunicationClient == null)
                {
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
                    imsCommunicationClient = new IMServiceFabricClient(
                                    wcfClientFactory,
                                    new Uri("fabric:/ServiceFabricOMS/IMStatelessService"),
                                    ServicePartitionKey.Singleton);
                }
                return imsCommunicationClient;
            }
            set { imsCommunicationClient = value; }
        }

        public DMSDispatcherService()
        {
            Console.WriteLine("Dispatcher instantiated");
            //Pokretanje inicijalizacije mreze
            DMSService dmsService = DMSService.Instance;
            DMSCallService call = new DMSCallService();
        }

        public List<Element> GetAllElements()
        {
            List<Element> retVal = new List<Element>();
            try
            {
                foreach (Element e in DMSService.Instance.Tree.Data.Values)
                {
                    retVal.Add(e);
                }
                return retVal;
            }
            catch (Exception)
            {

                return new List<Element>();
            }
        }

        public int GetNetworkDepth()
        {
            try
            {
                return DMSService.Instance.Tree.Links.Max(x => x.Value.Depth) + 1;

            }
            catch (Exception)
            {
                return 1;
            }
        }

        public List<ACLine> GetAllACLines()
        {
            List<ACLine> pom = new List<ACLine>();
            try
            {
                foreach (var item in DMSService.Instance.Tree.Data.Values)
                {
                    if (item is ACLine)
                    {
                        pom.Add((ACLine)item);
                    }
                }
                return pom;
            }
            catch (Exception)
            {

                return new List<ACLine>();
            }

        }

        public List<Consumer> GetAllConsumers()
        {
            List<Consumer> pom = new List<Consumer>();
            try
            {
                foreach (var item in DMSService.Instance.Tree.Data.Values)
                {
                    if (item is Consumer)
                    {
                        pom.Add((Consumer)item);
                    }
                }
                return pom;
            }
            catch (Exception)
            {

                return new List<Consumer>();
            }

        }

        public List<Node> GetAllNodes()
        {
            List<Node> pom = new List<Node>();
            try
            {
                foreach (var item in DMSService.Instance.Tree.Data.Values)
                {
                    if (item is Node)
                    {
                        pom.Add((Node)item);
                    }
                }
                return pom;
            }
            catch (Exception)
            {

                return new List<Node>();
            }

        }

        public List<Source> GetAllSource()
        {
            List<Source> pom = new List<Source>();
            try
            {
                foreach (var item in DMSService.Instance.Tree.Data.Values)
                {
                    if (item is Source)
                    {
                        pom.Add((Source)item);
                    }
                }
                return pom;
            }
            catch (Exception)
            {

                return new List<Source>();
            }

        }

        public List<Switch> GetAllSwitches()
        {
            List<Switch> pom = new List<Switch>();
            try
            {
                foreach (var item in DMSService.Instance.Tree.Data.Values)
                {
                    if (item is Switch)
                    {
                        pom.Add((Switch)item);
                    }
                }
                return pom;
            }
            catch (Exception)
            {

                return new List<Switch>();
            }

        }

        public Source GetTreeRoot()
        {
            try
            {
                Source s = (Source)DMSService.Instance.Tree.Data[DMSService.Instance.Tree.Roots[0]];
                return s;
            }
            catch (Exception)
            {

                return new Source();
            }
        }

        public Dictionary<long, Element> InitNetwork()
        {
            try
            {
                return DMSService.Instance.Tree.Data;
            }
            catch (Exception)
            {
                return new Dictionary<long, Element>();
            }
        }

        public void SendCrewToDms(IncidentReport report)
        {
            /*Logic dms*/
            Thread crewprocess = new Thread(() => ProcessCrew(report));
            crewprocess.Start();
            return;

        }

        private void ProcessCrew(IncidentReport report)
        {
            report.Id = IMSCommunicationClient.InvokeWithRetry(client => client.Channel.GetReport(report.Time)).Id;

            if (report != null)
            {
                if (report.Crewtype == CrewType.Investigation)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(4));

                    var rnd = new Random(DateTime.Now.Second);
                    int repairtime = rnd.Next(30, 180);

                    Array values = Enum.GetValues(typeof(ReasonForIncident));
                    Random rand = new Random();
                    ReasonForIncident res = (ReasonForIncident)values.GetValue(rand.Next(1, values.Length));

                    report.Reason = res;
                    report.RepairTime = TimeSpan.FromMinutes(repairtime);
                    report.CrewSent = true;

                    report.IncidentState = IncidentState.READY_FOR_REPAIR;
                    report.Crewtype = CrewType.Repair;
                }
                else if (report.Crewtype == CrewType.Repair)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(report.RepairTime.TotalMinutes / 10));

                    report.IncidentState = IncidentState.REPAIRED;

                    Switch sw = null;
                    foreach (var item in DMSService.Instance.Tree.Data.Values)
                    {
                        if (item.MRID == report.MrID)
                        {
                            sw = (Switch)item;

                            if (item.UnderSCADA)
                            {
                                sw.CanCommand = true;
                                break;
                            }
                            else
                            {
                                sw.State = SwitchState.Closed;
                                ElementStateReport elementStateReport = new ElementStateReport() { MrID = sw.MRID, Time = DateTime.UtcNow, State = 0 };
                                IMSCommunicationClient.InvokeWithRetry(client => client.Channel.AddElementStateReport(elementStateReport));

                                List<UIUpdateModel> networkChange = new List<UIUpdateModel>();
                                if (EnergizationAlgorithm.TraceUp((Node)DMSService.Instance.Tree.Data[sw.End1], DMSService.Instance.Tree))
                                {
                                    networkChange.Add(new UIUpdateModel(sw.ElementGID, true, OMSSCADACommon.States.CLOSED));
                                    sw.Marker = true;
                                    Node n = (Node)DMSService.Instance.Tree.Data[sw.End2];
                                    n.Marker = true;
                                    networkChange.Add(new UIUpdateModel(n.ElementGID, true));
                                    networkChange = EnergizationAlgorithm.TraceDown(n, networkChange, true, false, DMSService.Instance.Tree);
                                }
                                else
                                {
                                    networkChange.Add(new UIUpdateModel(sw.ElementGID, false, OMSSCADACommon.States.CLOSED));
                                }

                                Publisher publisher1 = new Publisher();
                                publisher1.PublishUpdate(networkChange);
                                break;
                            }
                        }
                    }
                }

                IMSCommunicationClient.InvokeWithRetry(client => client.Channel.UpdateReport(report));

                Publisher publisher = new Publisher();
                publisher.PublishIncident(report);
            }
        }

        public bool IsNetworkAvailable()
        {
            return DMSService.isNetworkInitialized;
        }
    }
}