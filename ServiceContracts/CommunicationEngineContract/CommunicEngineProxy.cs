using OMSSCADACommon;
using System;
using System.ServiceModel;

namespace CommunicationEngineContract
{
    public class CommunicEngineProxy : ChannelFactory<ICommunicationEngineContract>, ICommunicationEngineContract
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
        public CommunicEngineProxy() { }

        public CommunicEngineProxy(NetTcpBinding binding, string address)
            : base(binding, address)
        {
            this.Factory = this.CreateChannel();
        }

        public bool ReceiveValue()
        {
            bool result;
            try
            {
                result = factory.ReceiveValue();
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

       
    }
}
