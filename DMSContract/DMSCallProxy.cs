using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DMSContract
{
    public class DMSCallProxy : ChannelFactory<IDMSCallContract>, IDMSCallContract, IDisposable
    {
        IDMSCallContract factory;

        public DMSCallProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }
        public DMSCallProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public void SendCall(string mrid)
        {
            try
            {
                factory.SendCall(mrid);
            }
            catch (Exception e )
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
