using CommunicationEngineContract;
using FTN.Common;
using OMSSCADACommon.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationEngine
{
    public class ClientCommEngine : ICommunicationEngineContractUpdate
    {
        public bool ReceiveAllMeasValue(TypeOfSCADACommand typeOfCommand)
        {
            Command c = MappingEngine.Instance.MappCommand(typeOfCommand);
            if(c!=null)
            {
                ReadAll ra = (ReadAll)c;
                SCADAClient client = new SCADAClient();
                client.ExecuteCommand(ra);
                return true;
            }
            else
            {
                ///logovati
                return false;
            }
        }

        public bool ReceiveValue()
        {
            throw new NotImplementedException();
        }
    }
}
