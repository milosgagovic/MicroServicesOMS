namespace FTN.Services.NetworkModelService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitMigNMS : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Properties",
                c => new
                    {
                        IdDB = c.Int(nullable: false, identity: true),
                        ResourceDescription_Id = c.Int(nullable: false),
                        Id = c.Long(nullable: false),
                        PropertyValue_Id = c.Int(),
                    })
                .PrimaryKey(t => t.IdDB)
                .ForeignKey("dbo.PropertyValues", t => t.PropertyValue_Id)
                .ForeignKey("dbo.ResourceDescriptions", t => t.ResourceDescription_Id, cascadeDelete: true)
                .Index(t => t.ResourceDescription_Id)
                .Index(t => t.PropertyValue_Id);
            
            CreateTable(
                "dbo.PropertyValues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LongValue = c.Long(nullable: false),
                        FloatValue = c.Single(nullable: false),
                        StringValue = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ResourceDescriptions",
                c => new
                    {
                        IdDb = c.Int(nullable: false, identity: true),
                        Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.IdDb);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Properties", "ResourceDescription_Id", "dbo.ResourceDescriptions");
            DropForeignKey("dbo.Properties", "PropertyValue_Id", "dbo.PropertyValues");
            DropIndex("dbo.Properties", new[] { "PropertyValue_Id" });
            DropIndex("dbo.Properties", new[] { "ResourceDescription_Id" });
            DropTable("dbo.ResourceDescriptions");
            DropTable("dbo.PropertyValues");
            DropTable("dbo.Properties");
        }
    }
}
