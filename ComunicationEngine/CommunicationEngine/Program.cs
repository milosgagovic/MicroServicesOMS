using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using CommunicationEngine;
using OMSSCADACommon;
using OMSSCADACommon.Commands;
using OMSSCADACommon.Responses;
using SCADAContracts;

namespace CommunicationEngine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string message = "";
            try
            {
                CommunicEngineService ces = new CommunicEngineService();
                ces.Start();
                message = "Press <Enter> to stop the service.";
                Console.WriteLine(message);
                //Console.ReadLine();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("CommunicationEngine service failed.");
                Console.WriteLine(ex.StackTrace);
                Console.ReadLine();
            }

            //SCADAClient sc = new SCADAClient();
            //Response r = sc.ExecuteCommand(new ReadAll());

            Console.ReadKey();
        }
    }
}
