using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common
{
    public class BindingForTCP
    {
        public static NetTcpBinding CreateCustomNetTcp()
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.SendTimeout = new TimeSpan(10, 30, 0);
            binding.ReceiveTimeout = new TimeSpan(10, 30, 0);
            binding.CloseTimeout = new TimeSpan(10, 30, 0); ;
            binding.OpenTimeout = new TimeSpan(10, 30, 0);
            binding.MaxBufferSize = 2147483647;
            binding.MaxReceivedMessageSize = 2147483647;
            binding.ReaderQuotas.MaxStringContentLength = 2147483647;
            binding.ReaderQuotas.MaxArrayLength = 2147483647;
            binding.Security.Mode = SecurityMode.None;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.None;

            return binding;
        }
    }
}
