using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScadaDBClassLib
{
    public class ScadaCtx:DbContext
    {
        public ScadaCtx():base("Server=tcp:scadadb.database.windows.net,1433;Initial Catalog=scadaDb;Persist Security Info=False;User ID=milos.gagovic;Password=Scadabaza1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;")
        {

        }

        
    }
}
