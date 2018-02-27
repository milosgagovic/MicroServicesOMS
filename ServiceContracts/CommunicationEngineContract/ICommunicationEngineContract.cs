using OMSSCADACommon;
using OMSSCADACommon.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationEngineContract
{
    [ServiceContract(CallbackContract = typeof(ICommuncEngineContract_CallBack))]
    public interface ICommunicationEngineContract
    {
        /// <summary>
        /// Initiall method for receiving value from SCADA
        /// </summary>
        [OperationContract]
        bool ReceiveValue();
    }
    [ServiceContract]
    public interface ICommuncEngineContract_CallBack
    {
        /// <summary>
        /// Initiall method for sending command to SCADA  
        /// </summary>
        [OperationContract]
        bool SendCommand(Command command);

        [OperationContract]
        bool InvokeMeasurements();
    }
   
}
