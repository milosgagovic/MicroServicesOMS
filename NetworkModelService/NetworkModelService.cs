using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.ServiceModel;
using FTN.Services.NetworkModelService;
using System.ServiceModel.Description;
using FTN.Common;
using TransactionManagerContract;

namespace FTN.Services.NetworkModelService
{
    public class NetworkModelService : IDisposable
    {
        private NetworkModel nm = null;
        private List<ServiceHost> hosts = null;

        public NetworkModel Nm { get => nm; set => nm = value; }

        public NetworkModelService()
        {
            Nm = new NetworkModel();
            GenericDataAccess.NetworkModel = Nm;
            ResourceIterator.NetworkModel = Nm;
          //  InitializeHosts();
        }

        public void Start()
        {
            StartHosts();
        }

        public void Dispose()
        {
            CloseHosts();
            GC.SuppressFinalize(this);
        }

        private void InitializeHosts()
        {
            hosts = new List<ServiceHost>();
            var binding = new NetTcpBinding();
            binding.CloseTimeout = TimeSpan.FromMinutes(10);
            binding.OpenTimeout = TimeSpan.FromMinutes(10);
            binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
            binding.SendTimeout = TimeSpan.FromMinutes(10);
            binding.TransactionFlow = true;

            ServiceHost svc = new ServiceHost(typeof(NetworkModelTransactionService));
            svc.Description.Name = "NetworkModelTransactionService";
            svc.AddServiceEndpoint(typeof(ITransaction),
                                    binding,
                                    new Uri("net.tcp://localhost:8018/NetworkModelTransactionService"));

            svc.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            svc.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });


            hosts.Add(svc);


            // Service Parameters for GenericDataAccess are defined in App.config
            hosts.Add(new ServiceHost(typeof(GenericDataAccess)));          
        }

        private void StartHosts()
        {
            if (hosts == null || hosts.Count == 0)
            {
                throw new Exception("Network Model Services can not be opend because it is not initialized.");
            }

            string message = string.Empty;
            foreach (ServiceHost host in hosts)
            {
                host.Open();

                message = string.Format("The WCF service {0} is ready.", host.Description.Name);
                Console.WriteLine(message);
                CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);

                message = "Endpoints:";
                Console.WriteLine(message);
                CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);

                foreach (Uri uri in host.BaseAddresses)
                {
                    Console.WriteLine(uri);
                    CommonTrace.WriteTrace(CommonTrace.TraceInfo, uri.ToString());
                }

                Console.WriteLine("\n");
            }

            message = string.Format("Connection string: {0}", Config.Instance.ConnectionString);
            Console.WriteLine(message);
            CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);

            message = string.Format("Trace level: {0}", CommonTrace.TraceLevel);
            Console.WriteLine(message);
            CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);


            message = "The Network Model Service is started.";
            Console.WriteLine("\n{0}", message);
            CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);
        }

        public void CloseHosts()
        {
            if (hosts == null || hosts.Count == 0)
            {
                throw new Exception("Network Model Services can not be closed because it is not initialized.");
            }

            foreach (ServiceHost host in hosts)
            {
                host.Close();
            }

            string message = "The Network Model Service is closed.";
            CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);
            Console.WriteLine("\n\n{0}", message);
        }
    }
}
