using SCADAContracts;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using TransactionManagerContract;

namespace SCADA.ClientHandler
{
    public class SCADAService
    {
        private List<ServiceHost> hosts = null;

        public SCADAService()
        {
            InitializeHosts();           
        }

        private void InitializeHosts()
        {
            var binding = new NetTcpBinding();
            binding.CloseTimeout = new TimeSpan(1, 0, 0, 0);
            binding.OpenTimeout = new TimeSpan(1, 0, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0, 0);

            hosts = new List<ServiceHost>();

            // service for handlling client requests
            ServiceHost invokerhost = new ServiceHost(typeof(Invoker));
            invokerhost.Description.Name = "SCADAInvokerservice";
            invokerhost.AddServiceEndpoint(typeof(ISCADAContract),
              binding,
               new Uri("net.tcp://localhost:4000/SCADAService"));
            hosts.Add(invokerhost);

            // 2PC transaction service
            ServiceHost transactionServiceHost = new ServiceHost(typeof(SCADATransactionService));
            transactionServiceHost.Description.Name = "SCADATransactionService";
            transactionServiceHost.AddServiceEndpoint(typeof(ITransactionSCADA),
                binding,
                new Uri("net.tcp://localhost:8078/SCADATransactionService"));

            transactionServiceHost.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            transactionServiceHost.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            hosts.Add(transactionServiceHost);

            // u nekom trenutku na debug mi je puklo, kontam da cu ovo dodati negde
            //binding.CloseTimeout = new TimeSpan(1, 0, 0, 0);
            //binding.OpenTimeout = new TimeSpan(1, 0, 0, 0);
            //binding.ReceiveTimeout = new TimeSpan(1, 0, 0, 0);
            //binding.SendTimeout = new TimeSpan(1, 0, 0, 0);

        }

        public void Start()
        {

            if (hosts == null || hosts.Count == 0)
            {
                throw new Exception("SCADA service can not start because it is not initialized.");
            }

            string message = string.Empty;

            foreach(ServiceHost host in hosts)
            {
                host.Open();

                message = string.Format("The WCF service {0} is ready.", host.Description.Name);
                Console.WriteLine(message);
                //CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);

                message = "Endpoints:";
                Console.WriteLine(message);

                foreach (Uri uri in host.BaseAddresses)
                {
                    Console.WriteLine(uri);
                }

                Console.WriteLine("\n");
            }

        }

        public void Dispose()
        {
            foreach(var host in hosts)
            {
                host.Close();
            }

            hosts.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
