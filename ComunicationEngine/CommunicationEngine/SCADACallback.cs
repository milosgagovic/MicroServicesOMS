using FTN.Common;
using OMSSCADACommon;
using OMSSCADACommon.Responses;
using SCADAContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationEngine
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class SCADACallback : ISCADAContract_Callback
    {
        public void DigitalStateChanged(string mRID, OMSSCADACommon.States newState)
        {
            // mapirati i prodlediti DMS-u
            //throw new NotImplementedException();
        }

        //ovde puca proveriti to
        //MappingEngine mapEngine = new MappingEngine();

        public void ReceiveResponse(Response response)
        {
            CommunicationEngine.Instance.SendResponseToClient(response);
        }
    }
}
