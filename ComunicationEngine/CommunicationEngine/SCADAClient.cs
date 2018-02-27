using OMSSCADACommon;
using OMSSCADACommon.Commands;
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
    public class SCADAClient : ISCADAContract
    {
        SCADAProxy proxy;

        public SCADAClient()
        {
            proxy = new SCADAProxy(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:4000/SCADAService"), new SCADACallback());
        }

        public void CheckIn()
        {
            proxy.CheckIn();
        }

        public Response ExecuteCommand(Command command)
        {
            return proxy.ExecuteCommand(command);
        }
    }
}
