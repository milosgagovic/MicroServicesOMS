using IMSContract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IncidentManagementSystem.Model
{
    public class IncidentContext : DbContext
    {
        public IncidentContext()
        {

        }

        public DbSet<IncidentReport> IncidentReports { get; set; }
        public DbSet<ElementStateReport> ElementStateReports { get; set; }
        public DbSet<Crew> Crews { get; set; }
    }
    public class IncidentCloudContext : DbContext
    {
        public IncidentCloudContext() : base("Server=tcp:oms-sqlserver.database.windows.net,1433;Initial Catalog=OMS;Persist Security Info=False; User ID = milos.gagovic; Password=Omsbaza1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout = 30;")
        {

        }

        public DbSet<IncidentReport> IncidentReports { get; set; }
        public DbSet<ElementStateReport> ElementStateReports { get; set; }
        public DbSet<Crew> Crews { get; set; }
    }
}
