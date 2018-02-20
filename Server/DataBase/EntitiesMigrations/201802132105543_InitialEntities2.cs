using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DataBase.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialEntities2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PhotoItemEntities", new[] { "Name" });
            DropIndex("dbo.OptionItemEntities", new[] { "Code" });
            DropIndex("dbo.OrderEntities", new[] { "OrderNumber" });
            DropIndex("dbo.SendItemsEntities", new[] { "Login" });
            AddColumn("dbo.SendItemsEntities", "Contragent_Id", c => c.Long());
            AlterColumn("dbo.ContragentItemEntities", "Login", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.PhotoItemEntities", "Name", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.OptionItemEntities", "Code", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.OrderEntities", "OrderNumber", c => c.String(nullable: false, maxLength: 30));
            CreateIndex("dbo.ContragentItemEntities", "Login", unique: true);
            CreateIndex("dbo.PhotoItemEntities", "Name", unique: true);
            CreateIndex("dbo.OptionItemEntities", "Code", unique: true);
            CreateIndex("dbo.OrderEntities", "OrderNumber", unique: true);
            CreateIndex("dbo.SendItemsEntities", "Contragent_Id");
            AddForeignKey("dbo.SendItemsEntities", "Contragent_Id", "dbo.ContragentItemEntities", "Id");
            DropColumn("dbo.SendItemsEntities", "Login");
        }

        public override void Down()
        {
            AddColumn("dbo.SendItemsEntities", "Login", c => c.String(maxLength: 30));
            DropForeignKey("dbo.SendItemsEntities", "Contragent_Id", "dbo.ContragentItemEntities");
            DropIndex("dbo.SendItemsEntities", new[] { "Contragent_Id" });
            DropIndex("dbo.OrderEntities", new[] { "OrderNumber" });
            DropIndex("dbo.OptionItemEntities", new[] { "Code" });
            DropIndex("dbo.PhotoItemEntities", new[] { "Name" });
            DropIndex("dbo.ContragentItemEntities", new[] { "Login" });
            AlterColumn("dbo.OrderEntities", "OrderNumber", c => c.String(maxLength: 30));
            AlterColumn("dbo.OptionItemEntities", "Code", c => c.String(maxLength: 50));
            AlterColumn("dbo.PhotoItemEntities", "Name", c => c.String(maxLength: 255));
            AlterColumn("dbo.ContragentItemEntities", "Login", c => c.String(maxLength: 30));
            DropColumn("dbo.SendItemsEntities", "Contragent_Id");
            CreateIndex("dbo.SendItemsEntities", "Login");
            CreateIndex("dbo.OrderEntities", "OrderNumber", unique: true);
            CreateIndex("dbo.OptionItemEntities", "Code", unique: true);
            CreateIndex("dbo.PhotoItemEntities", "Name", unique: true);
        }
    }
}
