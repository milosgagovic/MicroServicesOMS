using IMSContract;
using IncidentManagementSystem.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace IncidentManagementSystem.Service
{
    public class IncidentManagementSystemService
    {
        private ServiceHost svc = null;
        public void Start()
        {
            Database.SetInitializer<IncidentContext>(new DropCreateDatabaseIfModelChanges<IncidentContext>());
            LoadCrews();

            svc = new ServiceHost(typeof(IMSService));
            var binding = new NetTcpBinding();
            svc.AddServiceEndpoint(typeof(IIMSContract),
                binding,
                new Uri("net.tcp://localhost:6090/IncidentManagementSystemService"));
          
            // redosled obratiti  paznju
            svc.Open();
            Console.WriteLine("IncidentManagementSystemService ready and waiting for requests.");
        }
        public void Stop()
        {
            svc.Close();
            Console.WriteLine("IncidentManagementSystemService server stopped.");
        }

        private void LoadCrews()
        {
            List<Crew> crews = new List<Crew>();
            Crew c1 = new Crew() { Id = "1", CrewName = "Crew_1", Type = CrewType.TYPE1 };
            Crew c2 = new Crew() { Id = "2", CrewName = "Crew_2", Type = CrewType.TYPE1 };
            Crew c3 = new Crew() { Id = "3", CrewName = "Crew_3", Type = CrewType.TYPE2 };
            Crew c4 = new Crew() { Id = "4", CrewName = "Crew_4", Type = CrewType.TYPE2 };
            Crew c5 = new Crew() { Id = "5", CrewName = "Crew_5", Type = CrewType.TYPE3 };
            crews.Add(c1);
            crews.Add(c2);
            crews.Add(c3);
            crews.Add(c4);
            crews.Add(c5);

            using (var ctx = new IncidentContext())
            {
                foreach (Crew c in crews)
                {
                    try
                    {
                        if (!ctx.Crews.Any(e => e.Id == c.Id))
                        {
                            ctx.Crews.Add(c);
                            ctx.SaveChanges();
                        }
                    }
                    catch (Exception e) { }
                }
            }
            using (var ctxCloud = new IncidentCloudContext())
            {
                foreach (Crew c in crews)
                {
                    try
                    {
                        if (!ctxCloud.Crews.Any(e => e.Id == c.Id))
                        {
                            ctxCloud.Crews.Add(c);
                            ctxCloud.SaveChanges();
                        }
                    }
                    catch (Exception e) { }
                }
            }
        }
    }
}
