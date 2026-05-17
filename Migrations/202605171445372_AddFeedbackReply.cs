namespace LibrarySystem99.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFeedbackReply : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WebsiteFeedbacks", "LibrarianReply", c => c.String(maxLength: 1000));
            AddColumn("dbo.WebsiteFeedbacks", "RepliedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.WebsiteFeedbacks", "RepliedDate");
            DropColumn("dbo.WebsiteFeedbacks", "LibrarianReply");
        }
    }
}
