using DMSContract;
using OMSSCADACommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SCADASimulation
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Simuliraj SCADU");
            Console.ReadLine();
            ChannelFactory<IDMSToSCADAContract> factoryToDMS = new ChannelFactory<IDMSToSCADAContract>(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:8039/IDMSToSCADAContract"));
            IDMSToSCADAContract proxyToTransactionManager = factoryToDMS.CreateChannel();
            proxyToTransactionManager.ChangeOnSCADA("18", States.OPENED);
            Console.ReadLine();
        }
    }
}
