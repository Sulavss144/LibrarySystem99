namespace LibrarySystem99.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialLibrarySystem : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 200),
                        Author = c.String(nullable: false, maxLength: 150),
                        ISBN = c.String(maxLength: 50),
                        Category = c.String(maxLength: 100),
                        TotalCopies = c.Int(nullable: false),
                        AvailableCopies = c.Int(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BorrowingTransactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BookId = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        BorrowDate = c.DateTime(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        ReturnDate = c.DateTime(),
                        IsReturned = c.Boolean(nullable: false),
                        RenewalCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.BookId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.BookId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Reservations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BookId = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ReservationDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsFulfilled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.BookId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.BookId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.BorrowingPolicies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MaxBooksPerUser = c.Int(nullable: false),
                        BorrowDays = c.Int(nullable: false),
                        MaxRenewals = c.Int(nullable: false),
                        FinePerDay = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Fines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BorrowingTransactionId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsPaid = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BorrowingTransactions", t => t.BorrowingTransactionId, cascadeDelete: true)
                .Index(t => t.BorrowingTransactionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Fines", "BorrowingTransactionId", "dbo.BorrowingTransactions");
            DropForeignKey("dbo.Reservations", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Reservations", "BookId", "dbo.Books");
            DropForeignKey("dbo.BorrowingTransactions", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.BorrowingTransactions", "BookId", "dbo.Books");
            DropIndex("dbo.Fines", new[] { "BorrowingTransactionId" });
            DropIndex("dbo.Reservations", new[] { "UserId" });
            DropIndex("dbo.Reservations", new[] { "BookId" });
            DropIndex("dbo.BorrowingTransactions", new[] { "UserId" });
            DropIndex("dbo.BorrowingTransactions", new[] { "BookId" });
            DropTable("dbo.Fines");
            DropTable("dbo.BorrowingPolicies");
            DropTable("dbo.Reservations");
            DropTable("dbo.BorrowingTransactions");
            DropTable("dbo.Books");
        }
    }
}
