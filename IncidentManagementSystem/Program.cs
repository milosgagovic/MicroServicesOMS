using IncidentManagementSystem.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IncidentManagementSystem
{
	class Program
	{
		static void Main(string[] args)
		{
            Console.Title = "Incident Management System";
            IncidentManagementSystemService ims = new IncidentManagementSystemService();
		
			ims.Start();
            Console.WriteLine("Incident Management System started");

            Console.ReadLine();

			ims.Stop();
		}
	}
}
