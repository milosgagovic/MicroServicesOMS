using OMSSCADACommon;
using System;
using System.ServiceModel;

namespace DMSContract
{
    public class ScadaToDMSProxy : ChannelFactory<IDMSToSCADAContract>, IDMSToSCADAContract, IDisposable
    {
        IDMSToSCADAContract factory;

        public ScadaToDMSProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public ScadaToDMSProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public void ChangeOnSCADAAnalog(string mrID, float value)
        {
            try
            {
                factory.ChangeOnSCADAAnalog(mrID, value);
            }
            catch (Exception e)
            {
                Console.WriteLine("DMSServiceForScada not available yet.");
                //Console.WriteLine(e.StackTrace);
                //Console.WriteLine(e.Message);
            }
        }

        public void ChangeOnSCADADigital(string mrID, States state)
        {
            try
            {
                factory.ChangeOnSCADADigital(mrID, state);
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
