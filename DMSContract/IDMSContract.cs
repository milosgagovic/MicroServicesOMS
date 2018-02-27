using DMSCommon.Model;
using IMSContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DMSContract
{
    [ServiceContract]
    public interface IDMSContract
    {
        [OperationContract]
        bool IsNetworkAvailable();

        [OperationContract]
        List<Source> GetAllSource();

        [OperationContract]
        List<Consumer> GetAllConsumers();

        [OperationContract]
        List<Switch> GetAllSwitches();

        [OperationContract]
        List<ACLine> GetAllACLines();

        [OperationContract]
        List<Node> GetAllNodes();

        [OperationContract]
        Dictionary<long, Element> InitNetwork();

        [OperationContract]

        Source GetTreeRoot();

        [OperationContract]
        int GetNetworkDepth();
        [OperationContract]
        List<Element> GetAllElements();

        //[OperationContract]
        //void SendCrewToDms(string mrid);

        [OperationContract]
        void SendCrewToDms(IncidentReport report);
    }
}
