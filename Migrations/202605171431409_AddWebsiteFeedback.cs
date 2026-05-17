namespace LibrarySystem99.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWebsiteFeedback : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WebsiteFeedbacks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Email = c.String(nullable: false, maxLength: 200),
                        Message = c.String(nullable: false, maxLength: 1000),
                        CreatedDate = c.DateTime(nullable: false),
                        IsApproved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WebsiteFeedbacks");
        }
    }
}
