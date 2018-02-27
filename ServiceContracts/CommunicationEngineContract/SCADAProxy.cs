using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationEngineContract
{
    public class SCADAProxy : DuplexChannelFactory<ICommunicationEngineContract>, ICommunicationEngineContract
    {
        private ICommunicationEngineContract factory;

        public ICommunicationEngineContract Factory
        {
            get
            {
                return factory;
            }

            set
            {
                factory = value;
            }
        }
        public SCADAProxy(InstanceContext context,string adress)
            : base(context, adress)
        {
            Factory = this.CreateChannel();
        }

        public SCADAProxy(InstanceContext context,NetTcpBinding binding, EndpointAddress adress)
           : base(context,binding, adress)
        {
            Factory = this.CreateChannel();
        }


        public bool ReceiveValue()
        {
            bool result = false;
            try
            {
                result = factory.ReceiveValue();
                return result;
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
                return result;
            }
        }
    }
}
