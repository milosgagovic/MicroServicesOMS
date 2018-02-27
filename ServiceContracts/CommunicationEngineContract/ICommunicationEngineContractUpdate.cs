using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace CommunicationEngineContract
{
    [ServiceContract]
    public interface ICommunicationEngineContractUpdate
    {
        /// <summary>
        /// Initiall method for receiving value from SCADA
        /// </summary>
        [OperationContract]
        bool ReceiveValue();

        [OperationContract]
        bool ReceiveAllMeasValue(TypeOfSCADACommand typeOfCommand);
    }
}