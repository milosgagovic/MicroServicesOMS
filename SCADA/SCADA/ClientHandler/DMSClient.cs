using DMSContract;
using OMSSCADACommon;
using System;
using System.ServiceModel;

namespace SCADA.ClientHandler
{
    public class DMSClient : ChannelFactory<IDMSToSCADAContract>, IDMSToSCADAContract, IDisposable
    {
        ScadaToDMSProxy proxy;

        public DMSClient()
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.CloseTimeout = TimeSpan.FromMinutes(10);
            binding.OpenTimeout = TimeSpan.FromMinutes(10);
            binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
            binding.SendTimeout = TimeSpan.FromMinutes(10);
            binding.MaxReceivedMessageSize = Int32.MaxValue;
            proxy = new ScadaToDMSProxy(binding, new EndpointAddress("net.tcp://localhost:8039/IDMSToSCADAContract"));
        }
     
        public void ChangeOnSCADADigital(string mrID, States state)
        {
            proxy.ChangeOnSCADADigital(mrID, state);
            Console.WriteLine("Scada - Digital changed time {0}", DateTime.Now.ToLongTimeString());
        }

        public void ChangeOnSCADAAnalog(string mrID, float value)
        {
            ((IDMSToSCADAContract)proxy).ChangeOnSCADAAnalog(mrID, value);
            Console.WriteLine("Scada - Analog changed time {0}", DateTime.Now.ToLongTimeString());
        }
    }
}
