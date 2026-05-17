namespace LibrarySystem99.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserProfileFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FullName", c => c.String(maxLength: 150));
            AddColumn("dbo.AspNetUsers", "Address", c => c.String(maxLength: 250));
            AddColumn("dbo.AspNetUsers", "DateOfBirth", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "PhotoUrl", c => c.String(maxLength: 500));
            AddColumn("dbo.AspNetUsers", "Bio", c => c.String(maxLength: 1000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Bio");
            DropColumn("dbo.AspNetUsers", "PhotoUrl");
            DropColumn("dbo.AspNetUsers", "DateOfBirth");
            DropColumn("dbo.AspNetUsers", "Address");
            DropColumn("dbo.AspNetUsers", "FullName");
        }
    }
}
