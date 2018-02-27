using OMSSCADACommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace DMSContract
{
    public class DMSToSCADAProxy : ChannelFactory<IDMSToSCADAContract>, IDMSToSCADAContract, IDisposable
    {
        IDMSToSCADAContract factory;

        public DMSToSCADAProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }
        public DMSToSCADAProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public void ChangeOnSCADA(string mrID, States state)
        {
            try
            {
                factory.ChangeOnSCADA(mrID, state);
            }
            catch (Exception e)
            {
                Console.WriteLine("DMSServiceForScada not available yet.");
                //Console.WriteLine(e.StackTrace);
                //Console.WriteLine(e.Message);
            }
        }
    }
}
