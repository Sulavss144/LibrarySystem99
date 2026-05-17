namespace LibrarySystem99.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBookReviews : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BookReviews",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BookId = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Rating = c.Int(nullable: false),
                        ReviewText = c.String(nullable: false, maxLength: 1000),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.BookId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.BookId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BookReviews", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.BookReviews", "BookId", "dbo.Books");
            DropIndex("dbo.BookReviews", new[] { "UserId" });
            DropIndex("dbo.BookReviews", new[] { "BookId" });
            DropTable("dbo.BookReviews");
        }
    }
}
