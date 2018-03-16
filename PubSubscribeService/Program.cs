using PubSubContract;
using PubSubscribeService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PubSubscribeService
{
    class Program
    {
        private static ServiceHost publishServiceHost = null;
        private static ServiceHost subscribeServiceHost = null;

        static void Main(string[] args)
        {
            Console.Title = "Publisher-Subscribe";
            try
            {
                HostPublishService();
                HostSubscribeService();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine(" -----------------STARTED-----------------\n\n\n\n");

            Console.WriteLine(" Press any key to STOP services");
            Console.ReadLine();
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("-------------------------------------");
            Console.ReadLine();
        }

        private static void HostPublishService()
        {
            publishServiceHost = new ServiceHost(typeof(PublishingService));
            NetTcpBinding tcpBindingPublish = CreateListenBinding();
            publishServiceHost.AddServiceEndpoint(typeof(IPublishing), tcpBindingPublish, "net.tcp://localhost:7001/Pub");
            publishServiceHost.Open();
        }

        private static void HostSubscribeService()
        {
            subscribeServiceHost = new ServiceHost(typeof(SubscriptionService));
            NetTcpBinding tcpBindingSubscribe = CreateListenBinding();

            subscribeServiceHost.AddServiceEndpoint(typeof(ISubscription), tcpBindingSubscribe, "net.tcp://localhost:7002/Sub");
            subscribeServiceHost.Open();
        }

        private static NetTcpBinding CreateListenBinding()
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None)
            {
                SendTimeout = TimeSpan.MaxValue,
                ReceiveTimeout = TimeSpan.MaxValue,
                OpenTimeout = TimeSpan.FromSeconds(5),
                CloseTimeout = TimeSpan.FromSeconds(5),
                //OpenTimeout = TimeSpan.MaxValue,
                //CloseTimeout = TimeSpan.MaxValue,
                //binding.OpenTimeout = TimeSpan.FromMinutes(5);
                //binding.CloseTimeout = TimeSpan.FromMinutes(5);
                MaxConnections = int.MaxValue,
                MaxReceivedMessageSize = 1024 * 1024
            };
            binding.MaxBufferSize = (int)binding.MaxReceivedMessageSize;
            binding.MaxBufferPoolSize = Environment.ProcessorCount * binding.MaxReceivedMessageSize;
            return binding;
        }
    }
}
