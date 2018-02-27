using OMSSCADACommon;
using OMSSCADACommon.Commands;
using OMSSCADACommon.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SCADAContracts
{
    public class SCADAProxy : ChannelFactory<ISCADAContract>, ISCADAContract, IDisposable
    {
        private ISCADAContract factory;

        public SCADAProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public SCADAProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }
        public Response ExecuteCommand(Command command)
        {
            try
            {
                var r = factory.ExecuteCommand(command);
                return r;
                //return factory.ExecuteCommand(command);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new Response();
            }
        }

        public bool Ping()
        {
            throw new NotImplementedException();
        }
    }
}
