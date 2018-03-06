using DMSCommon;
using DMSCommon.Model;
using DMSContract;
using FTN.Common;
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
using System.Threading;
using System.Threading.Tasks;

namespace DMSService
{
    public class DMSServiceForSCADA : IDMSToSCADAContract
    {
        private IMServiceFabricClient _imServiceFabricClient;
        private IMServiceFabricClient _IMServiceFabricClient
        {
            get
            {
                if (_imServiceFabricClient == null)
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
                    _imServiceFabricClient = new IMServiceFabricClient(
                                    wcfClientFactory,
                                    new Uri("fabric:/ServiceFabricOMS/IMStatelessService"),
                                    ServicePartitionKey.Singleton);

                }
                return _imServiceFabricClient;
            }
            set { _imServiceFabricClient = value; }
        }

        public void ChangeOnSCADADigital(string mrID, OMSSCADACommon.States state)
        {
            ModelGdaDMS gda = new ModelGdaDMS();

            List<ResourceDescription> discreteMeasurements = gda.GetExtentValuesExtended(ModelCode.DISCRETE);
            ResourceDescription rdDMeasurement = discreteMeasurements.Where(r => r.GetProperty(ModelCode.IDOBJ_MRID).AsString() == mrID).FirstOrDefault();

            // if measurement exists here! if result is null it exists only on scada, but not in .data
            if (rdDMeasurement != null)
            {
                // find PSR element associated with measurement
                long rdAssociatedPSR = rdDMeasurement.GetProperty(ModelCode.MEASUREMENT_PSR).AsLong();

                List<UIUpdateModel> networkChange = new List<UIUpdateModel>();

                Element DMSElementWithMeas;
                Console.WriteLine("Change on scada Digital Instance.Tree");
                DMSService.Instance.Tree.Data.TryGetValue(rdAssociatedPSR, out DMSElementWithMeas);
                Switch sw = DMSElementWithMeas as Switch;

                if (sw != null)
                {
                    bool isIncident = false;
                    IncidentReport incident = new IncidentReport() { MrID = sw.MRID };
                    incident.Crewtype = CrewType.Investigation;

                    ElementStateReport elementStateReport = new ElementStateReport() { MrID = sw.MRID, Time = DateTime.UtcNow, State = (int)state };

                    if (state == OMSSCADACommon.States.OPENED)
                    {
                        isIncident = true;

                        sw.Incident = true;
                        sw.State = SwitchState.Open;
                        sw.Marker = false;
                        networkChange.Add(new UIUpdateModel(sw.ElementGID, false, OMSSCADACommon.States.OPENED));

                        // treba mi objasnjenje sta se ovde radi? ne kotnam ove ScadaupdateModele sta se kad gde dodaje, sta je sta
                        // uopste, summary iznad tih propertija u dms modelu
                        Node n = (Node)DMSService.Instance.Tree.Data[sw.End2];
                        n.Marker = false;
                        networkChange.Add(new UIUpdateModel(n.ElementGID, false));
                        // pojasnjenje mi treba, komentari u ovom algoritmu i slicno, da ne debagujem sve redom, nemam vremena sad za to xD 
                        networkChange = EnergizationAlgorithm.TraceDown(n, networkChange, false, false, DMSService.Instance.Tree);
                    }
                    else if (state == OMSSCADACommon.States.CLOSED)
                    {
                        sw.State = SwitchState.Closed;

                        // i ovde takodje pojasnjenje
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
                    }

                    //do
                    //{
                    //    try
                    //    {
                    //        if (IMSClient.State == CommunicationState.Created)
                    //        {
                    //            IMSClient.Open();
                    //        }

                    //        if (IMSClient.Ping())
                    //            break;
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        //Console.WriteLine(e);
                    //        Console.WriteLine("ProcessCrew() -> IMS is not available yet.");
                    //        if (IMSClient.State == CommunicationState.Faulted)
                    //        {
                    //            NetTcpBinding binding = new NetTcpBinding();
                    //            binding.CloseTimeout = TimeSpan.FromMinutes(10);
                    //            binding.OpenTimeout = TimeSpan.FromMinutes(10);
                    //            binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
                    //            binding.SendTimeout = TimeSpan.FromMinutes(10);
                    //            binding.MaxReceivedMessageSize = Int32.MaxValue;
                    //            IMSClient = new IMSClient(new EndpointAddress("net.tcp://localhost:6090/IncidentManagementSystemService"), binding);
                    //        }
                    //    }

                    //    Thread.Sleep(1000);
                    //} while (true);

                    // report changed state of the element
                    _IMServiceFabricClient.InvokeWithRetry(c => c.Channel.AddElementStateReport(elementStateReport));

                    // ni ovo ne kontam, tj. nemam vremena da kontam previse xD
                    Source s = (Source)DMSService.Instance.Tree.Data[DMSService.Instance.Tree.Roots[0]];
                    networkChange.Add(new UIUpdateModel(s.ElementGID, true));

                    Publisher publisher = new Publisher();
                    if (networkChange.Count > 0)
                    {
                        publisher.PublishUpdate(networkChange);
                    }
                    if (isIncident)
                    {
                        List<long> gids = new List<long>();
                        networkChange.ForEach(x => gids.Add(x.Gid));
                        List<long> listOfConsumersWithoutPower = gids.Where(x => (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(x) == DMSType.ENERGCONSUMER).ToList();
                        foreach (long gid in listOfConsumersWithoutPower)
                        {
                            ResourceDescription resDes = DMSService.Instance.Gda.GetValues(gid);
                            try { incident.LostPower += resDes.GetProperty(ModelCode.ENERGCONSUMER_PFIXED).AsFloat(); } catch { }
                        }
                        _IMServiceFabricClient.InvokeWithRetry(c => c.Channel.AddReport(incident));
                        publisher.PublishIncident(incident);
                    }
                }
            }
            else
            {
                Console.WriteLine("ChangeOnScada()-> element with mrid={0} do not exist in OMS.", mrID);
            }
        }

        public void ChangeOnSCADAAnalog(string mrID, float value)
        {
            ModelGdaDMS gda = new ModelGdaDMS();

            List<ResourceDescription> analogMeasurements = gda.GetExtentValuesExtended(ModelCode.ANALOG);
            ResourceDescription rdDMeasurement = analogMeasurements.Where(r => r.GetProperty(ModelCode.IDOBJ_MRID).AsString() == mrID).FirstOrDefault();

            // if measurement exists here! if result is null it exists only on scada, but not in .data
            if (rdDMeasurement != null)
            {
                long measGid = rdDMeasurement.GetProperty(ModelCode.IDOBJ_GID).AsLong();


                // to do: cuvanje u bazi promene za analogne, bla bla. Inicijalno uopste nije bilo planirano da se propagiraju promene za analogne,
                // receno je da te vrednosti samo zakucamo :D, zato tu implementaciju ostavljam za svetlu buducnost! 

                // ovde sad mogu neke kalkulacije opasne da se racunaju, kao ako je ta neka vrednost to se npr. ne uklapa sa 
                // izracunatom vrednoscu za taj customer..ma bla bla...to nama ne treba xD

                List<UIUpdateModel> networkChange = new List<UIUpdateModel>();
                networkChange.Add(new UIUpdateModel() { Gid = measGid, AnValue = value });

                Publisher publisher = new Publisher();
                if (networkChange.Count > 0)
                {
                    publisher.PublishUpdateAnalog(networkChange);
                }
            }
            else
            {
                Console.WriteLine("ChangeOnScada()-> element with mrid={0} do not exist in OMS.", mrID);
            }
        }
    }
}
