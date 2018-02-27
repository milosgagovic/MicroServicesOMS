using FTN.Common;
using IMSContract;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TransactionManagerContract;

namespace ConsoleApp1
{
	class Program
	{
        static void Main(string[] args)
        {
            //ChannelFactory<IIMSContract> factoryToIMS = new ChannelFactory<IIMSContract>(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:6090/IncidentManagementSystemService"));
            //IIMSContract IMSClient = factoryToIMS.CreateChannel();

            ChannelFactory<IOMSClient> factoryToTMS = new ChannelFactory<IOMSClient>(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:6080/TransactionManagerService"));
            IOMSClient proxyToTransactionManager = factoryToTMS.CreateChannel();
            while (true)
            {
                printMeni();
                string odg = Console.ReadLine();
                if (odg == "1")
                {
                    Console.WriteLine("MrID:");
                    string mrid = Console.ReadLine();
                    Console.WriteLine("State:");
                    string state = Console.ReadLine();
                    //proxyToTransactionManager.AddReport(mrid, DateTime.UtcNow, state);
                }
                else if (odg == "2")
                {
                    printMeni2();
                    string odg2 = Console.ReadLine();
                    List<ElementStateReport> reports = new List<ElementStateReport>(); // = IMSClient.GetAllReports();
                    if (odg2 == "1")
                    {
                        reports = proxyToTransactionManager.GetAllElementStateReports();
                    }
                    else if (odg2 == "2")
                    {
                        Console.WriteLine("Unesite MrID:");
                        string mrid2 = Console.ReadLine();
                      //  reports = proxyToTransactionManager.GetElementStateReportsForMrID(mrid2);
                    }
                    else if (odg2 == "3")
                    {
                        Console.WriteLine("Unesite vremenski interval u obliku godina-mjesec-dan sat:minut:sekund:");
                        Console.WriteLine("StartTime:");
                        string startTime = Console.ReadLine();
                        DateTime startDateTime = DateTime.Parse(startTime);
                        Console.WriteLine("EndTime:");
                        string endTime = Console.ReadLine();
                        DateTime endDateTime = DateTime.Parse(endTime);
                        reports = proxyToTransactionManager.GetElementStateReportsForSpecificTimeInterval(startDateTime, endDateTime);

                    }
                    else if (odg2 == "4")
                    {
                        Console.WriteLine("Unesite MrID:");
                        string mrid3 = Console.ReadLine();
                        Console.WriteLine("Unesite vremenski interval u obliku godina-mjesec-dan sat:minut:sekund:");
                        Console.WriteLine("StartTime:");
                        string startTime2 = Console.ReadLine();
                        DateTime startDateTime2 = DateTime.Parse(startTime2);
                        Console.WriteLine("EndTime:");
                        string endTime2 = Console.ReadLine();
                        DateTime endDateTime2 = DateTime.Parse(endTime2);
                        reports = proxyToTransactionManager.GetElementStateReportsForSpecificMrIDAndSpecificTimeInterval(mrid3, startDateTime2, endDateTime2);

                    }
                    else if (odg2 == "5")
                    {

                        List<Crew> crews = proxyToTransactionManager.GetCrews();
                        foreach (Crew cr in crews)
                        {
                            Console.WriteLine("Crew: " + cr.CrewName);
                        }
                    }
                    // reports = IMSClient.GetAllReports();
                    foreach (ElementStateReport ir in reports)
                    {
                        Console.WriteLine("MrID: " + ir.MrID + ", State:" + ir.State + ", DateTime: " + ir.Time.ToUniversalTime());
                    }
                }
                else if (odg == "3")
                {
                    Console.WriteLine("Unesite ime ekipe:");
                    string ekipa = Console.ReadLine();
                    Console.WriteLine("Unesite id ekipe:");
                    string id = Console.ReadLine();
                    Crew crew = new Crew();
                    crew.Id = id;
                    crew.CrewName = ekipa;

                    proxyToTransactionManager.AddCrew(crew);
                }
                else if (odg == "4")
                {
                    NetTcpBinding binding = new NetTcpBinding();
                    // Create a partition resolver
                    IServicePartitionResolver partitionResolver = ServicePartitionResolver.GetDefault();
                    // create a  WcfCommunicationClientFactory object.
                    var wcfClientFactory = new WcfCommunicationClientFactory<IOMSClient>
                        (clientBinding: binding, servicePartitionResolver: partitionResolver);

                    //
                    // Create a client for communicating with the ICalculator service that has been created with the
                    // Singleton partition scheme.
                    //
                    var ServiceCommunicationClient = new WCFIMSClient(
                                    wcfClientFactory,
                                    new Uri("fabric:/ServiceFabricOMS/TMStatelessService"),
                                    ServicePartitionKey.Singleton);

                    Delta d = new Delta();
                    ServiceCommunicationClient.InvokeWithRetry(client => client.Channel.UpdateSystem(d));
                    Console.ReadLine();
                }
                else if (odg == "5")
                {
                    NetTcpBinding binding = new NetTcpBinding();
                    // Create a partition resolver
                    IServicePartitionResolver partitionResolver = ServicePartitionResolver.GetDefault();
                    // create a  WcfCommunicationClientFactory object.
                    var wcfClientFactory = new WcfCommunicationClientFactory<IIMSContract>
                        (clientBinding: binding, servicePartitionResolver: partitionResolver);

                    //
                    // Create a client for communicating with the ICalculator service that has been created with the
                    // Singleton partition scheme.
                    //
                    var ServiceCommunicationClient = new IncidentClient(
                                     wcfClientFactory,
                                     new Uri("fabric:/ServiceFabricOMS/IMStatelessService"),
                                     ServicePartitionKey.Singleton);

                    List<Crew> crews = ServiceCommunicationClient.InvokeWithRetry(client => client.Channel.GetCrews());
                    Console.WriteLine("Procitao:\n");
                    foreach (Crew c in crews)
                    {
                        Console.WriteLine("Crew name: " + c.CrewName + ", Crew Type: " + c.Type);
                    }
                    Console.ReadLine();
                }
                else
                {
                    break;
                }
            }
        }

        static void printMeni()
        {
            Console.WriteLine("Izaberite opciju:\n");
            Console.WriteLine("1. Upisi\n");
            Console.WriteLine("2. Procitaj\n");
            Console.WriteLine("3. Dodaj novi tim\n");
            Console.WriteLine("4. Gadjaj TMS na cloud-u\n");
            Console.WriteLine("5. Gadjaj IMS na cloud-u\n");
            Console.WriteLine("Opcija:");
        }

        static void printMeni2()
        {
            Console.WriteLine("Izaberite opciju:\n");
            Console.WriteLine("\t\t1. Procitaj sve\n");
            Console.WriteLine("\t\t2. Procitaj za specifican mrID\n");
            Console.WriteLine("\t\t3. Procitaj za specifican vremenski interval\n");
            Console.WriteLine("\t\t4. Procitaj za specifican mrID i specifican vremenski interval\n");
            Console.WriteLine("\t\t5. Procitaj timove na terenu\n");
            Console.WriteLine("Opcija:");
        }
    }
}
