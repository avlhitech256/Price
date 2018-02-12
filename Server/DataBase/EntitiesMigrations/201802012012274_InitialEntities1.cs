namespace DataBase.EntitiesMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class InitialEntities1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.CatalogItemEntities", "Price");
            DropColumn("dbo.CatalogItemEntities", "Currency");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CatalogItemEntities", "Currency", c => c.String(maxLength: 5));
            AddColumn("dbo.CatalogItemEntities", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
