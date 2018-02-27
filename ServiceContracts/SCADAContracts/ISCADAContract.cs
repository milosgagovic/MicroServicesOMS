using OMSSCADACommon;
using OMSSCADACommon.Commands;
using OMSSCADACommon.Responses;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SCADAContracts
{
    [ServiceContract]
    public interface ISCADAContract
    {
        [OperationContract]
        bool Ping();

        [OperationContract]
        Response ExecuteCommand(Command command);
    }
}
