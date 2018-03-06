using SCADA.ClientHandler;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SCADA.CommunicationAndControlling;

namespace SCADA
{
    public class Program
    {
       public static void Main(string[] args)
        {
            Console.Title = "SCADA";

            // to do: srediti kasnije, staviti fajlove u neki resource folder ili slicno   
            // ako je druga platforma npr. x86 mozda nije dobra putanja!            

            string pcConfig = "RtuConfiguration.xml";
            string scadaConfig = "ScadaModel.xml";
            //string basePath = Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;
            //string acqComConfigPath = Path.Combine(basePath, scadaConfig);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken;
            Task requestsConsumer, answersConsumer, acqRequestsProducer;

            PCCommunicationEngine PCCommEng = new PCCommunicationEngine();



            while (true)
            {
                if (!PCCommEng.ConfigureEngine("", pcConfig))
                {
                    Console.WriteLine("\nStart the simulator then press any key to continue the application.\n");
                    Thread.Sleep(5000);
                    continue;
                }
                break;
            }


            CommandingAcquisitionEngine AcqEngine = new CommandingAcquisitionEngine();
            if (AcqEngine.ConfigureEngine(""))
            {
                AcqEngine.InitializeSimulator();
                cancellationToken = cancellationTokenSource.Token;

                // to do: for IO bound operation you <await an operation which returns a task inside of an async method>
                // await yields control to the caller of the method thet performed await

                TimeSpan consumeReqTime = TimeSpan.FromMilliseconds(10000); // it should be at least twice than acquisition timeout
                requestsConsumer = Task.Factory.StartNew(() => PCCommEng.ProcessRequestsFromQueue(consumeReqTime, cancellationToken),
                   TaskCreationOptions.LongRunning);

                TimeSpan consumeAnswTime = TimeSpan.FromMilliseconds(10000);
                answersConsumer = Task.Factory.StartNew(() => AcqEngine.ProcessPCAnwers(consumeReqTime, cancellationToken),
                   TaskCreationOptions.LongRunning);

                // give simulator some time, and when everything is ready start acquisition
                Thread.Sleep(1000);

                acqRequestsProducer = Task.Factory.StartNew(() => AcqEngine.Acquisition());

                try
                {
                    Console.WriteLine("\n....");
                  //  SCADAService ss = new SCADAService();
                 //   ss.Start();
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

            //Console.WriteLine("Press <Enter> to stop the service.");
            ////Console.ReadKey();
            //if (cancellationToken.CanBeCanceled)
            //{
            //    // ako nisu bili ni pokrenuti, vrednost taskova je ovde null..
            //    cancellationTokenSource.Cancel();
            //}

            //// to do:
            //// wait tasks
            //AcqEngine.Stop();
            //PCCommEng.Stop();
        }

    }
}
