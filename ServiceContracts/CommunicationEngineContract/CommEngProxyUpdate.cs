using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace CommunicationEngineContract
{
    public class CommEngProxyUpdate : ChannelFactory<ICommunicationEngineContractUpdate>, ICommunicationEngineContractUpdate
    {

        private ICommunicationEngineContractUpdate factory;

        public ICommunicationEngineContractUpdate Factory
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
        public CommEngProxyUpdate() { }

        public CommEngProxyUpdate(NetTcpBinding binding, string address)
            : base(binding, address)
        {
            this.Factory = this.CreateChannel();
        }

        public CommEngProxyUpdate( string address)
           : base( address)
        {
            this.Factory = this.CreateChannel();
        }
        public bool ReceiveAllMeasValue(TypeOfSCADACommand typeOfCommand)
        {
            bool result;
            try
            {
                result = factory.ReceiveAllMeasValue(typeOfCommand);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
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
