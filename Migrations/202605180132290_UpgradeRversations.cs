namespace LibrarySystem99.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpgradeRversations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "ReadyDate", c => c.DateTime());
            AddColumn("dbo.Reservations", "FulfilledDate", c => c.DateTime());
            AddColumn("dbo.Reservations", "CancelledDate", c => c.DateTime());
            AddColumn("dbo.Reservations", "Status", c => c.Int(nullable: false));
            DropColumn("dbo.Reservations", "IsActive");
            DropColumn("dbo.Reservations", "IsFulfilled");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reservations", "IsFulfilled", c => c.Boolean(nullable: false));
            AddColumn("dbo.Reservations", "IsActive", c => c.Boolean(nullable: false));
            DropColumn("dbo.Reservations", "Status");
            DropColumn("dbo.Reservations", "CancelledDate");
            DropColumn("dbo.Reservations", "FulfilledDate");
            DropColumn("dbo.Reservations", "ReadyDate");
        }
    }
}
