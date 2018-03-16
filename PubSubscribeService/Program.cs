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
            NetTcpBinding tcpBindingPublish = new NetTcpBinding();
            tcpBindingPublish.SendTimeout = TimeSpan.MaxValue;
            tcpBindingPublish.ReceiveTimeout = TimeSpan.MaxValue;
            tcpBindingPublish.OpenTimeout = TimeSpan.MaxValue;
            tcpBindingPublish.CloseTimeout = TimeSpan.MaxValue;
            //binding.OpenTimeout = TimeSpan.FromMinutes(5);
            //binding.CloseTimeout = TimeSpan.FromMinutes(5);
            //MaxConnections = int.MaxValue,
            tcpBindingPublish.MaxReceivedMessageSize = 1024 * 1024;
            publishServiceHost.AddServiceEndpoint(typeof(IPublishing), tcpBindingPublish, "net.tcp://localhost:7001/Pub");
            publishServiceHost.Open();
        }

        private static void HostSubscribeService()
        {
            subscribeServiceHost = new ServiceHost(typeof(SubscriptionService));
            NetTcpBinding tcpBindingSubscribe = new NetTcpBinding();
            tcpBindingSubscribe.SendTimeout = TimeSpan.MaxValue;
            tcpBindingSubscribe.ReceiveTimeout = TimeSpan.MaxValue;
            tcpBindingSubscribe.OpenTimeout = TimeSpan.MaxValue;
            tcpBindingSubscribe.CloseTimeout = TimeSpan.MaxValue;
            //binding.OpenTimeout = TimeSpan.FromMinutes(5);
            //binding.CloseTimeout = TimeSpan.FromMinutes(5);
            //MaxConnections = int.MaxValue,
            tcpBindingSubscribe.MaxReceivedMessageSize = 1024 * 1024;

            subscribeServiceHost.AddServiceEndpoint(typeof(ISubscription), tcpBindingSubscribe, "net.tcp://localhost:7002/Sub");
            subscribeServiceHost.Open();
        }
    }
}
