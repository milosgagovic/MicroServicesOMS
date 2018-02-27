using SCADA.ClientHandler;
using SCADA.RealtimeDatabase;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SCADA.CommunicationAndControlling;
using SCADA.CommunicationAndControlling.SecondaryDataProcessing;

namespace SCADA
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "SCADA";

            // ako je druga platforma npr. x86 nije dobra putanja!

            string acqComConfigPath = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "ScadaModel.xml");
            string pcConfig = "RtuConfiguration.xml";
            string fullPcConfig = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "RtuConfiguration.xml");
            string basePath = Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;

            // ovo dole ipak ne funkcionise ako stavis x64 ...videti sta sa ovom konfiguracijom

            //if (IntPtr.Size == 8)
            //{
            //    Console.WriteLine("size==8");
            //    // 64 bit machine
            //    acqComConfigPath = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "ScadaModel.xml");
            //    pcConfig = "RtuConfiguration.xml";
            //    fullPcConfig = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "RtuConfiguration.xml");
            //    basePath = Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
            //}
            //else if (IntPtr.Size == 4)
            //{
            //    Console.WriteLine("size==4");
            //    // 32 bit machine
            //    acqComConfigPath = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName, "ScadaModel.xml");
            //    pcConfig = "RtuConfiguration.xml";
            //    fullPcConfig = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName, "RtuConfiguration.xml");
            //    basePath = Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;
            //}
            //else
            //{
            //    Console.WriteLine("aaaa");
            //}

            // to do: use cancellation tokens and TPL

            PCCommunicationEngine PCCommEng;
            while (true)
            {
                PCCommEng = new PCCommunicationEngine();

                if (!PCCommEng.Configure(basePath, pcConfig))
                {
                    Console.WriteLine("\nStart the simulator then press any key to continue the application.\n");
                    Console.ReadKey();
                    continue;
                }
                break;
            }


            CommAcqEngine AcqEngine = new CommAcqEngine();
            if (AcqEngine.Configure(acqComConfigPath))
            {
                // stavlja zahteve za icijalno komandovanje u red 
                AcqEngine.InitializeSimulator();

                // uzimanje zahteva iz reda, i slanje zahteva MDBU-u. dobijanje MDB odgovora i stavljanje u red
                Thread processingRequestsFromQueue = new Thread(PCCommEng.ProcessRequestsFromQueue);

                // uzimanje odgovora iz reda
                Thread processingAnswersFromQueue = new Thread(AcqEngine.ProcessPCAnwers);

                // stavljanje zahteva za akviziju u red
                Thread producingAcquisitonRequests = new Thread(AcqEngine.StartAcquisition);

                processingRequestsFromQueue.Start();
                processingAnswersFromQueue.Start();

                // give simulator some time, and when everything is ready start acquisition
                Thread.Sleep(1000);
                producingAcquisitonRequests.Start();

                try
                {
                    Console.WriteLine("\n....");
                    SCADAService ss = new SCADAService();
                    ss.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("SCADA service failed.");
                    Console.WriteLine(ex.StackTrace);
                    Console.ReadLine();
                    return;
                }
            }
            else
            {
                Console.WriteLine("Configuration of scada failed.");
            }

            Console.WriteLine("Press <Enter> to stop the service.");

            Console.ReadKey();

            AcqEngine.Stop();
            PCCommEng.Stop();
        }
    }
}
