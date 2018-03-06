using DMSCommon;
using IMSContract;
using System.Collections.Generic;
using System.ServiceModel;

namespace PubSubContract
{
    [ServiceContract]
    public interface IPublishing
    {
        [OperationContract(IsOneWay = true)]
        void PublishDigitalUpdate(List<UIUpdateModel> update);

        [OperationContract(IsOneWay = true)]
        void PublishAnalogUpdate(List<UIUpdateModel> update);

        [OperationContract(IsOneWay = true)]
        void PublishCrewUpdate(UIUpdateModel update);

        [OperationContract(IsOneWay = true)]
        void PublishIncident(IncidentReport report);

        [OperationContract(IsOneWay = true)]
        void PublishCallIncident(UIUpdateModel call);

        [OperationContract(IsOneWay = true)]
        void PublishUIBreakers(bool isIncident, long incidentBreaker);
    }
}
