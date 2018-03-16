using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading;
using DMSContract;
using OpenPop.Mime;
using OpenPop.Pop3;
using DMSCommon.Model;
using FTN.Common;
using System.Text.RegularExpressions;
using PubSubscribe;
using DMSCommon.TreeGraph;
using System.Linq;
using DMSCommon.TreeGraph.Tree;
using IMSContract;
using System.ServiceModel;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using IncidentManagementSystem.Service;
using System.Threading.Tasks;
using DMSCommon;

namespace DMSService
{
    public class DMSCallService : IDMSCallContract, IDisposable
    {
        public Dictionary<string, Message> messagesFormClents = new Dictionary<string, Message>();
        public List<List<long>> possibleBreakers = new List<List<long>>();
        public Pop3Client client;
        public List<long> clientsCall = new List<long>();
        public List<long> allBrekersUp = new List<long>();
        public object sync = new object();
        bool waitForMoreCalls = true;

        private IMServiceFabricClient _imServiceFabricClient;
        private IMServiceFabricClient _IMServiceFabricClient
        {
            get
            {
                if (_imServiceFabricClient == null)
                {
                    NetTcpBinding binding = new NetTcpBinding();
                    binding.SendTimeout = TimeSpan.MaxValue;
                    binding.ReceiveTimeout = TimeSpan.MaxValue;
                    binding.OpenTimeout = TimeSpan.MaxValue;
                    binding.CloseTimeout = TimeSpan.MaxValue;
                    //binding.OpenTimeout = TimeSpan.FromMinutes(5);
                    //binding.CloseTimeout = TimeSpan.FromMinutes(5);
                    //MaxConnections = int.MaxValue,
                    binding.MaxReceivedMessageSize = 1024 * 1024;
                    // Create a partition resolver
                    IServicePartitionResolver partitionResolver = ServicePartitionResolver.GetDefault();
                    // create a  WcfCommunicationClientFactory object.
                    var wcfClientFactory = new WcfCommunicationClientFactory<IIMSContract>
                        (clientBinding: binding, servicePartitionResolver: partitionResolver);

                    //
                    // Create a client for communicating with the ICalculator service that has been created with the
                    // Singleton partition scheme.
                    //
                    _imServiceFabricClient = new IMServiceFabricClient(
                                    wcfClientFactory,
                                    new Uri("fabric:/ServiceFabricOMS/IMStatelessService"),
                                    ServicePartitionKey.Singleton);

                }
                return _imServiceFabricClient;
            }
            set { _imServiceFabricClient = value; }
        }
        public DMSCallService()
        {
            Thread t = new Thread(new ThreadStart(Process));
            t.Start();
        }
        public void Process()
        {
            while (true)
            {
                LogIn();
                Message message;
                try
                {
                    var fg = client.GetMessageUids();
                    var count = client.GetMessageCount();
                    List<int> indexforDelete = new List<int>();
                    if (count > 0)
                    {
                        for (int i = 1; i <= count; i++)
                        {
                            message = client.GetMessage(i);
                            MessagePart mp = message.FindFirstPlainTextVersion();
                            messagesFormClents.Add(message.Headers.MessageId, message);
                            /*
                            Console.WriteLine(mp.GetBodyAsText());
                            Pronaci ec na osnovu MRID_ja
                            */
                            UIUpdateModel call = TryGetConsumer(mp.GetBodyAsText());
                            if (call.Gid > 0)
                            {
                                SendMailMessageToClient(message, true);
                                lock (sync)
                                {
                                    if (DMSService.Instance.Tree.Data[call.Gid].Marker == true)
                                    {
                                        clientsCall.Add(call.Gid);
                                        DMSService.Instance.Tree.Data[call.Gid].Marker = false;
                                    }
                                }
                                if (clientsCall.Count == 3)
                                {
                                    Thread t = new Thread(new ThreadStart(TraceUpAlgorithm));
                                    t.Start();
                                }
                                Publisher publisher = new Publisher();
                                publisher.PublishCallIncident(call);
                            }
                            else
                                SendMailMessageToClient(message, false);

                            indexforDelete.Add(i);
                        }
                        foreach (int item in indexforDelete)
                        {
                            client.DeleteMessage(item);
                        }
                        client.Disconnect();
                        LogIn();
                    }
                }
                catch (Exception)
                {
                    client.Disconnect();
                    LogIn();
                }
                //Console.WriteLine(message.Headers.Subject);
                Thread.Sleep(3000);
            }
        }
        /// <summary>
        /// call je gid EC
        /// </summary>
        public void TraceUpAlgorithm()
        {
            List<NodeLink> maxDepthCheck = new List<NodeLink>();
            possibleBreakers = new List<List<long>>();

            Task.Factory.StartNew(() => Timer());

            int callsNum = 0;
            Publisher pub = new Publisher();
            long? incidentBreaker = null;

            while (true)
            {
                //zakljucavamo pozive klijenata i pronalazimo zajednicke prekidace
                if (callsNum != clientsCall.Count)
                {
                    callsNum = clientsCall.Count;

                    lock (sync)
                    {
                        foreach (long call in clientsCall)
                        {
                            Consumer c = (Consumer)DMSService.Instance.Tree.Data[call];
                            Node upNode = (Node)DMSService.Instance.Tree.Data[c.End1];
                            UpToSource(upNode, DMSService.Instance.Tree);
                            if (allBrekersUp.Count > 0)
                            {
                                List<long> pom = new List<long>();
                                foreach (var item in allBrekersUp)
                                {
                                    Switch s = (Switch)DMSService.Instance.Tree.Data[item];
                                    if (s.UnderSCADA == false)
                                    {
                                        pom.Add(s.ElementGID);
                                    }
                                }
                                possibleBreakers.Add(pom);
                                allBrekersUp.Clear();
                            }
                        }
                    }
                    //ako lista ima vise clanova onda se bira prekidac koji je dublje u mrezi
                    int numOfConsumers = possibleBreakers.Count;
                    List<long> intesection = possibleBreakers.Aggregate((previousList, nextList) => previousList.Intersect(nextList).ToList());
                    maxDepthCheck = new List<NodeLink>();
                    foreach (long breaker in intesection)
                    {
                        maxDepthCheck.Add(DMSService.Instance.Tree.Links.Values.FirstOrDefault(x => x.Parent == breaker));
                    }
                    NodeLink possibileIncident = maxDepthCheck.FirstOrDefault(x => x.Depth == maxDepthCheck.Max(y => y.Depth));
                    incidentBreaker = possibileIncident.Parent;

                    if (waitForMoreCalls)
                    {
                        pub.PublishUIBreaker(false, (long)incidentBreaker);

                        //Thread.Sleep(37000);
                    }
                }

                if (!waitForMoreCalls)
                {
                    lock (sync)
                    {
                        if (/*callsNum == clientsCall.Count || */waitForMoreCalls == false)
                        {
                            //publishujes incident
                            string mrid = DMSService.Instance.Tree.Data[(long)incidentBreaker].MRID;
                            IncidentReport incident = new IncidentReport() { MrID = mrid };
                            incident.Crewtype = CrewType.Investigation;

                            // to do: BUG -> ovo state opened srediti
                            ElementStateReport elementStateReport = new ElementStateReport() { MrID = mrid, Time = DateTime.UtcNow, State = 1 };
                            //ElementStateReport elementStateReport = new ElementStateReport() { MrID = mrid, Time = DateTime.UtcNow, State = "OPENED" };                        

                            _IMServiceFabricClient.InvokeWithRetry(client => client.Channel.AddElementStateReport(elementStateReport));
                            pub.PublishUIBreaker(true, (long)incidentBreaker);

                            List<UIUpdateModel> networkChange = new List<UIUpdateModel>();
                            Switch sw;
                            try
                            {
                                sw = (Switch)DMSService.Instance.Tree.Data[(long)incidentBreaker];
                            }
                            catch (Exception)
                            {
                                return;
                            }
                            sw.Marker = false;
                            sw.State = SwitchState.Open;
                            sw.Incident = true;
                            networkChange.Add(new UIUpdateModel(sw.ElementGID, false, OMSSCADACommon.States.OPENED));
                            Node n = (Node)DMSService.Instance.Tree.Data[sw.End2];
                            n.Marker = false;
                            networkChange.Add(new UIUpdateModel(n.ElementGID, false));
                            networkChange = EnergizationAlgorithm.TraceDown(n, networkChange, false, false, DMSService.Instance.Tree);

                            Source s = (Source)DMSService.Instance.Tree.Data[DMSService.Instance.Tree.Roots[0]];
                            networkChange.Add(new UIUpdateModel(s.ElementGID, true));


                            List<long> gids = new List<long>();
                            networkChange.ForEach(x => gids.Add(x.Gid));
                            List<long> listOfConsumersWithoutPower = gids.Where(x => (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(x) == DMSType.ENERGCONSUMER).ToList();

                            foreach (long gid in listOfConsumersWithoutPower)
                            {
                                ResourceDescription resDes = DMSService.Instance.Gda.GetValues(gid);
                                try { incident.LostPower += resDes.GetProperty(ModelCode.ENERGCONSUMER_PFIXED).AsFloat(); } catch { }
                            }

                            _IMServiceFabricClient.InvokeWithRetry(client => client.Channel.AddReport(incident));

                            Thread.Sleep(3000);

                            pub.PublishUpdate(networkChange);
                            pub.PublishIncident(incident);

                            clientsCall.Clear();
                            return;
                        }
                    }
                }
            }
        }

        private async Task Timer()
        {
            await Task.Delay(37000);
            this.waitForMoreCalls = false;
        }

        private void UpToSource(Node no, Tree<Element> tree)
        {

            //Element el = tree.Data[no.Parent];

            //if (tree.Data[el.ElementGID] is Source)
            //{
            //    return;
            //}
            //else if (tree.Data[el.ElementGID] is Switch)
            //{
            //    Switch s = (Switch)tree.Data[el.ElementGID];
            //    allBrekersUp.Add(s.ElementGID);
            //    Node n = (Node)tree.Data[s.End1];
            //    UpToSource(n, tree);
            //}
            //else if (tree.Data[el.ElementGID] is ACLine)
            //{
            //    ACLine acl = (ACLine)tree.Data[el.ElementGID];
            //    Node n = (Node)tree.Data[acl.End1];
            //    UpToSource(n, tree);
            //}

            while (true)
            {
                Element el = tree.Data[no.Parent];

                if (tree.Data[el.ElementGID] is Source)
                {
                    return;
                }
                else if (tree.Data[el.ElementGID] is Switch)
                {
                    Switch s = (Switch)tree.Data[el.ElementGID];
                    allBrekersUp.Add(s.ElementGID);
                    Node n = (Node)tree.Data[s.End1];
                    no = n;
                }
                else if (tree.Data[el.ElementGID] is ACLine)
                {
                    ACLine acl = (ACLine)tree.Data[el.ElementGID];
                    Node n = (Node)tree.Data[acl.End1];
                    no = n;
                }
            }
        }

        private UIUpdateModel TryGetConsumer(string mrid)
        {
            UIUpdateModel consumer = new UIUpdateModel();
            string s = Regex.Match(mrid.ToUpper().Trim(), @"\d+").Value;
            if (mrid.Contains("ec_") || mrid.Contains("EC_"))
            {
                foreach (ResourceDescription rd in DMSService.Instance.EnergyConsumersRD)
                {
                    if (rd.GetProperty(ModelCode.IDOBJ_MRID).AsString() == "EC_" + s)
                    {
                        consumer = new UIUpdateModel(rd.Id, false);
                        return consumer;
                    }
                }
            }
            else if (mrid.Contains("ec") || mrid.Contains("EC"))
            {
                foreach (ResourceDescription rd in DMSService.Instance.EnergyConsumersRD)
                {
                    if (rd.GetProperty(ModelCode.IDOBJ_MRID).AsString() == "EC_" + s)
                    {
                        consumer = new UIUpdateModel(rd.Id, false);
                        return consumer;
                    }
                }
            }
            return consumer;
        }

        public void LogIn()
        {
            client = new Pop3Client();
            bool canConnectToGmailServer = false;
            do
            {
                try
                {
                    client.Connect("pop.gmail.com", 995, true);
                    client.Authenticate("omscallreport@gmail.com", "omsreport");
                    canConnectToGmailServer = true;
                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
                }
            } while (!canConnectToGmailServer);
        }
        public void SendMailMessageToClient(Message message, bool canFind)
        {
            bool canConnectToGmailServer = false;
            do
            {
                try
                {
                    var mess = new MailMessage("omscallreport@gmail.com", message.Headers.From.ToString());
                    mess.Subject = "no replay?";
                    SmtpClient mailer = new SmtpClient("smtp.gmail.com", 587);
                    mailer.Credentials = new NetworkCredential("omscallreport@gmail.com", "omsreport");
                    mailer.EnableSsl = true;

                    if (canFind)
                        mess.Body = "Your outage request is in progress!";
                    else
                        mess.Body = "We canot identyfy you as a consumer. \n Please write your id in format EC_number or ec_number. ";

                    mailer.Send(mess);
                    canConnectToGmailServer = true;

                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
                }
            } while (!canConnectToGmailServer);
        }

        public void SendCall(string mrid)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
