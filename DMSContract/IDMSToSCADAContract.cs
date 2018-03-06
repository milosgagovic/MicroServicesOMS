using OMSSCADACommon;
using System.ServiceModel;

namespace DMSContract
{
    [ServiceContract]
    public interface IDMSToSCADAContract
    {
        [OperationContract]
        void ChangeOnSCADADigital(string mrID, States state);

        [OperationContract]
        void ChangeOnSCADAAnalog(string mrID, float value);
    }
}
