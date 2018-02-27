using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TransactionManagerContract;

namespace DispatcherApp.Model
{
    public class MainModelClass
    {
        private static MainModelClass instance;

        private IOMSClient proxyToTransactionManager;

        private MainModelClass()
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.CloseTimeout = new TimeSpan(1, 0, 0, 0);
            binding.OpenTimeout = new TimeSpan(1, 0, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0, 0);

            ChannelFactory<IOMSClient> factoryToTMS = new ChannelFactory<IOMSClient>(binding,
                new EndpointAddress("net.tcp://localhost:6080/TransactionManagerService"));
            ProxyToTransactionManager = factoryToTMS.CreateChannel();
            TMSAnswerToClient answerFromTransactionManager = new TMSAnswerToClient();
            try
            {
                answerFromTransactionManager = ProxyToTransactionManager.GetNetwork();
            }
            catch (Exception e) { }
        }

        public static MainModelClass Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MainModelClass();
                }
                return instance;
            }
        }

        public IOMSClient ProxyToTransactionManager
        {
            get { return proxyToTransactionManager; }
            set { proxyToTransactionManager = value; }
        }
    }
}
