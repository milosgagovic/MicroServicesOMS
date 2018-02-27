using DMSCommon.Model;
using DMSContract;
using FTN.Common;
using IMSContract;
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
        private IMSClient imsClient;
        private IMSClient IMSClient
        {
            get
            {
                if (imsClient == null)
                {
                    imsClient = new IMSClient(new EndpointAddress("net.tcp://localhost:6090/IncidentManagementSystemService"));
                }
                return imsClient;
            }
            set { imsClient = value; }
        }


        public void ChangeOnSCADA(string mrID, OMSSCADACommon.States state)
        {
            ModelGdaDMS gda = new ModelGdaDMS();

            List<ResourceDescription> discreteMeasurements = gda.GetExtentValuesExtended(ModelCode.DISCRETE);
            ResourceDescription rdDMeasurement = discreteMeasurements.Where(r => r.GetProperty(ModelCode.IDOBJ_MRID).AsString() == mrID).FirstOrDefault();

            // if measurement exists here! if result is null it exists only on scada, but not in .data
            if (rdDMeasurement != null)
            {
                // find PSR element associated with measurement
                long rdAssociatedPSR = rdDMeasurement.GetProperty(ModelCode.MEASUREMENT_PSR).AsLong();

                List<SCADAUpdateModel> networkChange = new List<SCADAUpdateModel>();

                Element DMSElementWithMeas;
                Console.WriteLine("Change on scada Instance.Tree");
                DMSService.Instance.Tree.Data.TryGetValue(rdAssociatedPSR, out DMSElementWithMeas);
                Switch sw = (Switch)DMSElementWithMeas;

                bool isIncident = false;
                IncidentReport incident = new IncidentReport() { MrID = sw.MRID };
               
                Random rand = new Random();
                Array crews = Enum.GetValues(typeof(CrewType));
                incident.Crewtype = (CrewType)crews.GetValue(rand.Next(0, crews.Length));

                ElementStateReport elementStateReport = new ElementStateReport() { MrID = sw.MRID, Time = DateTime.UtcNow, State = (int)state };

                bool isImsAvailable = false;
                do
                {
                    try
                    {
                        if (IMSClient.State == CommunicationState.Created)
                        {
                            IMSClient.Open();
                        }

                        isImsAvailable = IMSClient.Ping();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ChangeOnScada() -> IMS is not available yet.");
                        if (IMSClient.State == CommunicationState.Faulted)
                            IMSClient = new IMSClient(new EndpointAddress("net.tcp://localhost:6090/IncidentManagementSystemService"));
                    }
                    Thread.Sleep(1000);
                } while (!isImsAvailable);


                if (state == OMSSCADACommon.States.OPENED)
                {
                    IMSClient.AddReport(incident);
                    isIncident = true;

                    sw.Marker = false;
                    sw.State = SwitchState.Open;
                    networkChange.Add(new SCADAUpdateModel(sw.ElementGID, false, OMSSCADACommon.States.OPENED));
                    Node n = (Node)DMSService.Instance.Tree.Data[sw.End2];
                    n.Marker = false;
                    networkChange.Add(new SCADAUpdateModel(n.ElementGID, false));
                    networkChange = EnergizationAlgorithm.TraceDown(n, networkChange, false, false, DMSService.Instance.Tree);
                }
                else if (state == OMSSCADACommon.States.CLOSED)
                {
                    sw.State = SwitchState.Closed;
                    if (EnergizationAlgorithm.TraceUp((Node)DMSService.Instance.Tree.Data[sw.End1], DMSService.Instance.Tree))
                    {
                        networkChange.Add(new SCADAUpdateModel(sw.ElementGID, true, OMSSCADACommon.States.CLOSED));
                        sw.Marker = true;
                        Node n = (Node)DMSService.Instance.Tree.Data[sw.End2];
                        n.Marker = true;
                        networkChange.Add(new SCADAUpdateModel(n.ElementGID, true));
                        networkChange = EnergizationAlgorithm.TraceDown(n, networkChange, true, false, DMSService.Instance.Tree);
                    }
                    else
                    {
                        networkChange.Add(new SCADAUpdateModel(sw.ElementGID, false, OMSSCADACommon.States.CLOSED));
                    }
                }

                // report changed state of the element
                IMSClient.AddElementStateReport(elementStateReport);

                Source s = (Source)DMSService.Instance.Tree.Data[DMSService.Instance.Tree.Roots[0]];
                networkChange.Add(new SCADAUpdateModel(s.ElementGID, true));

                Publisher publisher = new Publisher();
                if (networkChange.Count > 0)
                {
                    publisher.PublishUpdate(networkChange);
                }
                if (isIncident)
                {
                    //Thread.Sleep(1000);
                    List<long> gids = new List<long>();
                    networkChange.ForEach(x => gids.Add(x.Gid));
                    List<long> listOfConsumersWithoutPower = gids.Where(x => (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(x) == DMSType.ENERGCONSUMER).ToList();
                    foreach (long gid in listOfConsumersWithoutPower)
                    {
                        ResourceDescription resDes = DMSService.Instance.Gda.GetValues(gid);
                        incident.LostPower += resDes.GetProperty(ModelCode.ENERGCONSUMER_PFIXED).AsFloat();
                    }
                    publisher.PublishIncident(incident);
                }
            }
            else
            {
                Console.WriteLine("ChangeOnScada()-> element with mrid={0} do not exist in OMS.", mrID);
            }
        }
    }
}
