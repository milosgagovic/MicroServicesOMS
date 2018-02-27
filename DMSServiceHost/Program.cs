using DMSCommon.Model;
using DMSService;
using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMSServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title="Distribution Management System";

            try
            {
                string message = "Starting Distribution Management System Service...";
                CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);
                Console.WriteLine("\n{0}\n", message);

                DMSService.DMSService.Instance.Start();
                message = "Press <Enter> to stop the service.";
                CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);
                Console.WriteLine(message);
                Console.ReadLine();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("DMS failed.");
                Console.WriteLine(ex.StackTrace);
                CommonTrace.WriteTrace(CommonTrace.TraceError, ex.Message);
                CommonTrace.WriteTrace(CommonTrace.TraceError, "DMS failed.");
                CommonTrace.WriteTrace(CommonTrace.TraceError, ex.StackTrace);
                Console.ReadLine();
            }
        }
    }
}
