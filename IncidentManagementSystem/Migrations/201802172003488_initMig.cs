namespace IncidentManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initMig : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Crews",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CrewName = c.String(),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ElementStateReports",
                c => new
                    {
                        Time = c.DateTime(nullable: false),
                        MrID = c.String(),
                        State = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Time);
            
            CreateTable(
                "dbo.IncidentReports",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Time = c.DateTime(nullable: false),
                        MrID = c.String(),
                        LostPower = c.Single(nullable: false),
                        IncidentState = c.Int(nullable: false),
                        CrewSent = c.Boolean(nullable: false),
                        RepairTime = c.Time(nullable: false, precision: 7),
                        Reason = c.Int(nullable: false),
                        Crewtype = c.Int(nullable: false),
                        Crew_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Crews", t => t.Crew_Id)
                .Index(t => t.Crew_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IncidentReports", "Crew_Id", "dbo.Crews");
            DropIndex("dbo.IncidentReports", new[] { "Crew_Id" });
            DropTable("dbo.IncidentReports");
            DropTable("dbo.ElementStateReports");
            DropTable("dbo.Crews");
        }
    }
}
