using ScadaDBClassLib.ModelData;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScadaDBClassLib
{
    public class ScadaCtxcs:DbContext
    {
        public ScadaCtxcs():base("ScadaContextDB")
        {

        }
        public DbSet<RTU> RTUs { get; set; }
        public DbSet<ProcessControlers> ProcessControlers { get; set; }
        public DbSet<Digital> Digitals { get; set; }
        public DbSet<Analog> Analogs { get; set; }
    }
}
