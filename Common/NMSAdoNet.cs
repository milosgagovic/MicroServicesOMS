namespace FTN.Common
{
    
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class NMSAdoNet : DbContext
    {
        // Your context has been configured to use a 'NMSAdoNet' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'FTN.Services.NetworkModelService.NMSAdoNet' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'NMSAdoNet' 
        // connection string in the application configuration file.
        public NMSAdoNet()
            : base("Server=tcp:nsmdb.database.windows.net,1433;Initial Catalog=NMS;Persist Security Info=False;User ID=milos.gagovic;Password=Nmsbaza1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;")
        {
        }

        public NMSAdoNet(string conn):base(conn)
        {

        }
        public DbSet<ResourceDescription> ResourceDescription { get; set; }
        public DbSet<Property> Property { get; set; }
        public DbSet<PropertyValue> PropertyValue { get; set; }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
    }
}