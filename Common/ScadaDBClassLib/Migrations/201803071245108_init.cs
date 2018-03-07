namespace ScadaDBClassLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Analogs");

            CreateTable(
                "dbo.Analogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsInit = c.Boolean(nullable: false),
                        Name = c.String(),
                        NumOfRegisters = c.Int(nullable: false),
                        ProcContrName = c.String(),
                        RelativeAddress = c.Int(nullable: false),
                        AcqValue = c.Single(nullable: false),
                        CommValue = c.Single(nullable: false),
                        UnitSymbol = c.String(),
                        MinValue = c.Single(nullable: false),
                        MaxValue = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
        }
        
        public override void Down()
        {
            DropTable("dbo.RTUs");
            DropTable("dbo.ProcessControlers");
            DropTable("dbo.Digitals");
            DropTable("dbo.Analogs");
        }
    }
}
