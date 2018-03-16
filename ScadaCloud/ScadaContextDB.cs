namespace ScadaCloud
{
    using ScadaCloud.Model;
    using System.Data.Entity;

    public class ScadaContextDB : DbContext
    {
        // Your context has been configured to use a 'ScadaContextDB' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'ScadaCloud.ScadaContextDB' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'ScadaContextDB' 
        // connection string in the application configuration file.
        public ScadaContextDB()
            : base("ScadaContextDB")
        { 
        }

        public DbSet<Digital> Digirals { get; set; }
        public DbSet<Analog> Analogs { get; set; }
        public DbSet<ProcessControlers> ProcesControlers { get; set; }
        public DbSet<RTU> RTUs { get; set; }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
    }
}