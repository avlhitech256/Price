namespace DataBase.EntitiesMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BrandItemEntities",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    Code = c.Guid(nullable: false),
                    Name = c.String(maxLength: 255),
                    DateOfCreation = c.DateTimeOffset(nullable: false, precision: 7),
                    LastUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    ForceUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true)
                .Index(t => t.DateOfCreation)
                .Index(t => t.LastUpdated)
                .Index(t => t.ForceUpdated);

            CreateTable(
                "dbo.CatalogItemEntities",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    UID = c.Guid(nullable: false),
                    Code = c.String(maxLength: 30),
                    Article = c.String(maxLength: 30),
                    Name = c.String(maxLength: 255),
                    BrandName = c.String(maxLength: 255),
                    Unit = c.String(maxLength: 10),
                    EnterpriceNormPack = c.String(maxLength: 30),
                    BatchOfSales = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Balance = c.String(maxLength: 30),
                    Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Currency = c.String(maxLength: 5),
                    Multiplicity = c.Decimal(nullable: false, precision: 18, scale: 2),
                    HasPhotos = c.Boolean(nullable: false),
                    Status = c.Int(nullable: false),
                    LastUpdatedStatus = c.DateTimeOffset(nullable: false, precision: 7),
                    DateOfCreation = c.DateTimeOffset(nullable: false, precision: 7),
                    LastUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    ForceUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    Brand_Id = c.Long(),
                    Directory_Id = c.Long(),
                    NomenclatureGroup_Id = c.Long(),
                    PriceGroup_Id = c.Long(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BrandItemEntities", t => t.Brand_Id)
                .ForeignKey("dbo.DirectoryEntities", t => t.Directory_Id)
                .ForeignKey("dbo.NomenclatureGroupEntities", t => t.NomenclatureGroup_Id)
                .ForeignKey("dbo.PriceGroupItemEntities", t => t.PriceGroup_Id)
                .Index(t => t.UID, unique: true)
                .Index(t => t.DateOfCreation)
                .Index(t => t.LastUpdated)
                .Index(t => t.ForceUpdated)
                .Index(t => t.Brand_Id)
                .Index(t => t.Directory_Id)
                .Index(t => t.NomenclatureGroup_Id)
                .Index(t => t.PriceGroup_Id);

            CreateTable(
                "dbo.CommodityDirectionEntities",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    Code = c.Guid(nullable: false),
                    Name = c.String(maxLength: 255),
                    DateOfCreation = c.DateTimeOffset(nullable: false, precision: 7),
                    LastUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    ForceUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true)
                .Index(t => t.DateOfCreation)
                .Index(t => t.LastUpdated)
                .Index(t => t.ForceUpdated);

            CreateTable(
                "dbo.DirectoryEntities",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    Code = c.Guid(nullable: false),
                    Name = c.String(maxLength: 255),
                    DateOfCreation = c.DateTimeOffset(nullable: false, precision: 7),
                    LastUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    ForceUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    Parent_Id = c.Long(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DirectoryEntities", t => t.Parent_Id)
                .Index(t => t.Code, unique: true)
                .Index(t => t.DateOfCreation)
                .Index(t => t.LastUpdated)
                .Index(t => t.ForceUpdated)
                .Index(t => t.Parent_Id);

            CreateTable(
                "dbo.DiscountsContragentEntities",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                    DateOfCreation = c.DateTimeOffset(nullable: false, precision: 7),
                    LastUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    ForceUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    CatalogItem_Id = c.Long(),
                    ContragentItem_Id = c.Long(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CatalogItemEntities", t => t.CatalogItem_Id)
                .ForeignKey("dbo.ContragentItemEntities", t => t.ContragentItem_Id)
                .Index(t => t.DateOfCreation)
                .Index(t => t.LastUpdated)
                .Index(t => t.ForceUpdated)
                .Index(t => t.CatalogItem_Id)
                .Index(t => t.ContragentItem_Id);

            CreateTable(
                "dbo.ContragentItemEntities",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    UID = c.Guid(nullable: false),
                    Code = c.String(maxLength: 30),
                    Name = c.String(maxLength: 255),
                    Login = c.String(maxLength: 30),
                    Password = c.String(maxLength: 255),
                    MutualSettlements = c.Decimal(nullable: false, precision: 18, scale: 2),
                    MutualSettlementsCurrency = c.String(maxLength: 5),
                    PDZ = c.Decimal(nullable: false, precision: 18, scale: 2),
                    PDZCurrency = c.String(maxLength: 5),
                    DateOfCreation = c.DateTimeOffset(nullable: false, precision: 7),
                    LastUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    ForceUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UID, unique: true, name: "IX_Code")
                .Index(t => t.DateOfCreation)
                .Index(t => t.LastUpdated)
                .Index(t => t.ForceUpdated);

            CreateTable(
                "dbo.PriceTypeNomenclatureGroupContragentEntities",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    DateOfCreation = c.DateTimeOffset(nullable: false, precision: 7),
                    LastUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    ForceUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    ContragentItem_Id = c.Long(),
                    NomenclatureGroupItem_Id = c.Long(),
                    TypeOfPriceItem_Id = c.Long(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ContragentItemEntities", t => t.ContragentItem_Id)
                .ForeignKey("dbo.NomenclatureGroupEntities", t => t.NomenclatureGroupItem_Id)
                .ForeignKey("dbo.TypeOfPriceItemEntities", t => t.TypeOfPriceItem_Id)
                .Index(t => t.DateOfCreation)
                .Index(t => t.LastUpdated)
                .Index(t => t.ForceUpdated)
                .Index(t => t.ContragentItem_Id)
                .Index(t => t.NomenclatureGroupItem_Id)
                .Index(t => t.TypeOfPriceItem_Id);

            CreateTable(
                "dbo.NomenclatureGroupEntities",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    Code = c.Guid(nullable: false),
                    Name = c.String(maxLength: 255),
                    DateOfCreation = c.DateTimeOffset(nullable: false, precision: 7),
                    LastUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    ForceUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true)
                .Index(t => t.DateOfCreation)
                .Index(t => t.LastUpdated)
                .Index(t => t.ForceUpdated);

            CreateTable(
                "dbo.TypeOfPriceItemEntities",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    Code = c.Guid(nullable: false),
                    Name = c.String(maxLength: 255),
                    DateOfCreation = c.DateTimeOffset(nullable: false, precision: 7),
                    LastUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    ForceUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true)
                .Index(t => t.DateOfCreation)
                .Index(t => t.LastUpdated)
                .Index(t => t.ForceUpdated);

            CreateTable(
                "dbo.TypeOfPricesNomenclatureItemEntities",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Currency = c.String(maxLength: 5),
                    DateOfCreation = c.DateTimeOffset(nullable: false, precision: 7),
                    LastUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    ForceUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    CatalogItem_Id = c.Long(),
                    TypeOfPriceItem_Id = c.Long(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CatalogItemEntities", t => t.CatalogItem_Id)
                .ForeignKey("dbo.TypeOfPriceItemEntities", t => t.TypeOfPriceItem_Id)
                .Index(t => t.DateOfCreation)
                .Index(t => t.LastUpdated)
                .Index(t => t.ForceUpdated)
                .Index(t => t.CatalogItem_Id)
                .Index(t => t.TypeOfPriceItem_Id);

            CreateTable(
                "dbo.PriceTypePriceGroupContragentEntities",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    DateOfCreation = c.DateTimeOffset(nullable: false, precision: 7),
                    LastUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    ForceUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    ContragentItem_Id = c.Long(),
                    PriceGroupItem_Id = c.Long(),
                    TypeOfPriceItem_Id = c.Long(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ContragentItemEntities", t => t.ContragentItem_Id)
                .ForeignKey("dbo.PriceGroupItemEntities", t => t.PriceGroupItem_Id)
                .ForeignKey("dbo.TypeOfPriceItemEntities", t => t.TypeOfPriceItem_Id)
                .Index(t => t.DateOfCreation)
                .Index(t => t.LastUpdated)
                .Index(t => t.ForceUpdated)
                .Index(t => t.ContragentItem_Id)
                .Index(t => t.PriceGroupItem_Id)
                .Index(t => t.TypeOfPriceItem_Id);

            CreateTable(
                "dbo.PriceGroupItemEntities",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    Code = c.Guid(nullable: false),
                    Name = c.String(maxLength: 255),
                    DateOfCreation = c.DateTimeOffset(nullable: false, precision: 7),
                    LastUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    ForceUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true)
                .Index(t => t.DateOfCreation)
                .Index(t => t.LastUpdated)
                .Index(t => t.ForceUpdated);

            CreateTable(
                "dbo.PhotoItemEntities",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    Name = c.String(maxLength: 255),
                    IsLoad = c.Boolean(nullable: false),
                    Photo = c.Binary(storeType: "image"),
                    DateOfCreation = c.DateTimeOffset(nullable: false, precision: 7),
                    LastUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    ForceUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    CatalogItem_Id = c.Long(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CatalogItemEntities", t => t.CatalogItem_Id)
                .Index(t => t.Name, unique: true)
                .Index(t => t.DateOfCreation)
                .Index(t => t.LastUpdated)
                .Index(t => t.ForceUpdated)
                .Index(t => t.CatalogItem_Id);

            CreateTable(
                "dbo.OptionItemEntities",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    Code = c.String(maxLength: 50),
                    Name = c.String(maxLength: 255),
                    Value = c.String(maxLength: 255),
                    DateOfCreation = c.DateTimeOffset(nullable: false, precision: 7),
                    LastUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true)
                .Index(t => t.DateOfCreation)
                .Index(t => t.LastUpdated);

            CreateTable(
                "dbo.OrderEntities",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    OrderNumber = c.String(maxLength: 30),
                    Sum = c.Decimal(nullable: false, precision: 18, scale: 2),
                    OrderStatus = c.Int(nullable: false),
                    Comment = c.String(maxLength: 1024),
                    DateOfCreation = c.DateTimeOffset(nullable: false, precision: 7),
                    LastUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    ForceUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.OrderNumber, unique: true)
                .Index(t => t.DateOfCreation)
                .Index(t => t.LastUpdated)
                .Index(t => t.ForceUpdated);

            CreateTable(
                "dbo.ProductDirectionEntities",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    Direction = c.Int(nullable: false),
                    DateOfCreation = c.DateTimeOffset(nullable: false, precision: 7),
                    LastUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    ForceUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    Directory_Id = c.Long(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DirectoryEntities", t => t.Directory_Id)
                .Index(t => t.Direction, unique: true)
                .Index(t => t.DateOfCreation)
                .Index(t => t.LastUpdated)
                .Index(t => t.ForceUpdated)
                .Index(t => t.Directory_Id);

            CreateTable(
                "dbo.SendItemsEntities",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    Login = c.String(maxLength: 30),
                    EntityId = c.Long(nullable: false),
                    EntityName = c.Int(nullable: false),
                    RequestDate = c.DateTimeOffset(nullable: false, precision: 7),
                    DateOfCreation = c.DateTimeOffset(nullable: false, precision: 7),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Login)
                .Index(t => t.EntityName)
                .Index(t => t.RequestDate)
                .Index(t => t.DateOfCreation);

            CreateTable(
                "dbo.CommodityDirectionEntityCatalogItemEntities",
                c => new
                {
                    CommodityDirectionEntity_Id = c.Long(nullable: false),
                    CatalogItemEntity_Id = c.Long(nullable: false),
                })
                .PrimaryKey(t => new { t.CommodityDirectionEntity_Id, t.CatalogItemEntity_Id })
                .ForeignKey("dbo.CommodityDirectionEntities", t => t.CommodityDirectionEntity_Id, cascadeDelete: true)
                .ForeignKey("dbo.CatalogItemEntities", t => t.CatalogItemEntity_Id, cascadeDelete: true)
                .Index(t => t.CommodityDirectionEntity_Id)
                .Index(t => t.CatalogItemEntity_Id);

        }



        //public override void Down()
        //{
        //    DropForeignKey("dbo.ProductDirectionEntities", "Directory_Id", "dbo.DirectoryEntities");
        //    DropForeignKey("dbo.PhotoItemEntities", "CatalogItem_Id", "dbo.CatalogItemEntities");
        //    DropForeignKey("dbo.CatalogItemEntities", "NomenclatureGroup_Id", "dbo.NomenclatureGroupEntities");
        //    DropForeignKey("dbo.DirectoryEntities", "Parent_Id", "dbo.DirectoryEntities");
        //    DropForeignKey("dbo.CatalogItemEntities", "Directory_Id", "dbo.DirectoryEntities");
        //    DropForeignKey("dbo.CommodityDirectionEntityCatalogItemEntities", "CatalogItemEntity_Id", "dbo.CatalogItemEntities");
        //    DropForeignKey("dbo.CommodityDirectionEntityCatalogItemEntities", "CommodityDirectionEntity_Id", "dbo.CommodityDirectionEntities");
        //    DropForeignKey("dbo.CatalogItemEntities", "Brand_Id", "dbo.BrandItemEntities");
        //    DropIndex("dbo.CommodityDirectionEntityCatalogItemEntities", new[] { "CatalogItemEntity_Id" });
        //    DropIndex("dbo.CommodityDirectionEntityCatalogItemEntities", new[] { "CommodityDirectionEntity_Id" });
        //    DropIndex("dbo.SendItemsEntities", new[] { "DateOfCreation" });
        //    DropIndex("dbo.SendItemsEntities", new[] { "RequestDate" });
        //    DropIndex("dbo.SendItemsEntities", new[] { "EntityName" });
        //    DropIndex("dbo.SendItemsEntities", new[] { "Login" });
        //    DropIndex("dbo.ProductDirectionEntities", new[] { "Directory_Id" });
        //    DropIndex("dbo.ProductDirectionEntities", new[] { "ForceUpdated" });
        //    DropIndex("dbo.ProductDirectionEntities", new[] { "LastUpdated" });
        //    DropIndex("dbo.ProductDirectionEntities", new[] { "DateOfCreation" });
        //    DropIndex("dbo.ProductDirectionEntities", new[] { "Direction" });
        //    DropIndex("dbo.OrderEntities", new[] { "ForceUpdated" });
        //    DropIndex("dbo.OrderEntities", new[] { "LastUpdated" });
        //    DropIndex("dbo.OrderEntities", new[] { "DateOfCreation" });
        //    DropIndex("dbo.OrderEntities", new[] { "OrderNumber" });
        //    DropIndex("dbo.OptionItemEntities", new[] { "LastUpdated" });
        //    DropIndex("dbo.OptionItemEntities", new[] { "DateOfCreation" });
        //    DropIndex("dbo.OptionItemEntities", new[] { "Code" });
        //    DropIndex("dbo.PhotoItemEntities", new[] { "CatalogItem_Id" });
        //    DropIndex("dbo.PhotoItemEntities", new[] { "ForceUpdated" });
        //    DropIndex("dbo.PhotoItemEntities", new[] { "LastUpdated" });
        //    DropIndex("dbo.PhotoItemEntities", new[] { "DateOfCreation" });
        //    DropIndex("dbo.PhotoItemEntities", new[] { "Name" });
        //    DropIndex("dbo.NomenclatureGroupEntities", new[] { "ForceUpdated" });
        //    DropIndex("dbo.NomenclatureGroupEntities", new[] { "LastUpdated" });
        //    DropIndex("dbo.NomenclatureGroupEntities", new[] { "DateOfCreation" });
        //    DropIndex("dbo.NomenclatureGroupEntities", new[] { "Code" });
        //    DropIndex("dbo.DirectoryEntities", new[] { "Parent_Id" });
        //    DropIndex("dbo.DirectoryEntities", new[] { "ForceUpdated" });
        //    DropIndex("dbo.DirectoryEntities", new[] { "LastUpdated" });
        //    DropIndex("dbo.DirectoryEntities", new[] { "DateOfCreation" });
        //    DropIndex("dbo.DirectoryEntities", new[] { "Code" });
        //    DropIndex("dbo.CommodityDirectionEntities", new[] { "ForceUpdated" });
        //    DropIndex("dbo.CommodityDirectionEntities", new[] { "LastUpdated" });
        //    DropIndex("dbo.CommodityDirectionEntities", new[] { "DateOfCreation" });
        //    DropIndex("dbo.CommodityDirectionEntities", new[] { "Code" });
        //    DropIndex("dbo.CatalogItemEntities", new[] { "NomenclatureGroup_Id" });
        //    DropIndex("dbo.CatalogItemEntities", new[] { "Directory_Id" });
        //    DropIndex("dbo.CatalogItemEntities", new[] { "Brand_Id" });
        //    DropIndex("dbo.CatalogItemEntities", new[] { "ForceUpdated" });
        //    DropIndex("dbo.CatalogItemEntities", new[] { "LastUpdated" });
        //    DropIndex("dbo.CatalogItemEntities", new[] { "DateOfCreation" });
        //    DropIndex("dbo.CatalogItemEntities", new[] { "UID" });
        //    DropIndex("dbo.BrandItemEntities", new[] { "ForceUpdated" });
        //    DropIndex("dbo.BrandItemEntities", new[] { "LastUpdated" });
        //    DropIndex("dbo.BrandItemEntities", new[] { "DateOfCreation" });
        //    DropIndex("dbo.BrandItemEntities", new[] { "Code" });
        //    DropTable("dbo.CommodityDirectionEntityCatalogItemEntities");
        //    DropTable("dbo.SendItemsEntities");
        //    DropTable("dbo.ProductDirectionEntities");
        //    DropTable("dbo.OrderEntities");
        //    DropTable("dbo.OptionItemEntities");
        //    DropTable("dbo.PhotoItemEntities");
        //    DropTable("dbo.NomenclatureGroupEntities");
        //    DropTable("dbo.DirectoryEntities");
        //    DropTable("dbo.CommodityDirectionEntities");
        //    DropTable("dbo.CatalogItemEntities");
        //    DropTable("dbo.BrandItemEntities");
        //}

        public override void Down()
        {
            DropForeignKey("dbo.ProductDirectionEntities", "Directory_Id", "dbo.DirectoryEntities");
            DropForeignKey("dbo.PhotoItemEntities", "CatalogItem_Id", "dbo.CatalogItemEntities");
            DropForeignKey("dbo.PriceTypePriceGroupContragentEntities", "TypeOfPriceItem_Id", "dbo.TypeOfPriceItemEntities");
            DropForeignKey("dbo.PriceTypePriceGroupContragentEntities", "PriceGroupItem_Id", "dbo.PriceGroupItemEntities");
            DropForeignKey("dbo.CatalogItemEntities", "PriceGroup_Id", "dbo.PriceGroupItemEntities");
            DropForeignKey("dbo.PriceTypePriceGroupContragentEntities", "ContragentItem_Id", "dbo.ContragentItemEntities");
            DropForeignKey("dbo.PriceTypeNomenclatureGroupContragentEntities", "TypeOfPriceItem_Id", "dbo.TypeOfPriceItemEntities");
            DropForeignKey("dbo.TypeOfPricesNomenclatureItemEntities", "TypeOfPriceItem_Id", "dbo.TypeOfPriceItemEntities");
            DropForeignKey("dbo.TypeOfPricesNomenclatureItemEntities", "CatalogItem_Id", "dbo.CatalogItemEntities");
            DropForeignKey("dbo.PriceTypeNomenclatureGroupContragentEntities", "NomenclatureGroupItem_Id", "dbo.NomenclatureGroupEntities");
            DropForeignKey("dbo.CatalogItemEntities", "NomenclatureGroup_Id", "dbo.NomenclatureGroupEntities");
            DropForeignKey("dbo.PriceTypeNomenclatureGroupContragentEntities", "ContragentItem_Id", "dbo.ContragentItemEntities");
            DropForeignKey("dbo.DiscountsContragentEntities", "ContragentItem_Id", "dbo.ContragentItemEntities");
            DropForeignKey("dbo.DiscountsContragentEntities", "CatalogItem_Id", "dbo.CatalogItemEntities");
            DropForeignKey("dbo.DirectoryEntities", "Parent_Id", "dbo.DirectoryEntities");
            DropForeignKey("dbo.CatalogItemEntities", "Directory_Id", "dbo.DirectoryEntities");
            DropForeignKey("dbo.CommodityDirectionEntityCatalogItemEntities", "CatalogItemEntity_Id", "dbo.CatalogItemEntities");
            DropForeignKey("dbo.CommodityDirectionEntityCatalogItemEntities", "CommodityDirectionEntity_Id", "dbo.CommodityDirectionEntities");
            DropForeignKey("dbo.CatalogItemEntities", "Brand_Id", "dbo.BrandItemEntities");
            DropIndex("dbo.CommodityDirectionEntityCatalogItemEntities", new[] { "CatalogItemEntity_Id" });
            DropIndex("dbo.CommodityDirectionEntityCatalogItemEntities", new[] { "CommodityDirectionEntity_Id" });
            DropIndex("dbo.SendItemsEntities", new[] { "DateOfCreation" });
            DropIndex("dbo.SendItemsEntities", new[] { "RequestDate" });
            DropIndex("dbo.SendItemsEntities", new[] { "EntityName" });
            DropIndex("dbo.SendItemsEntities", new[] { "Login" });
            DropIndex("dbo.ProductDirectionEntities", new[] { "Directory_Id" });
            DropIndex("dbo.ProductDirectionEntities", new[] { "ForceUpdated" });
            DropIndex("dbo.ProductDirectionEntities", new[] { "LastUpdated" });
            DropIndex("dbo.ProductDirectionEntities", new[] { "DateOfCreation" });
            DropIndex("dbo.ProductDirectionEntities", new[] { "Direction" });
            DropIndex("dbo.OrderEntities", new[] { "ForceUpdated" });
            DropIndex("dbo.OrderEntities", new[] { "LastUpdated" });
            DropIndex("dbo.OrderEntities", new[] { "DateOfCreation" });
            DropIndex("dbo.OrderEntities", new[] { "OrderNumber" });
            DropIndex("dbo.OptionItemEntities", new[] { "LastUpdated" });
            DropIndex("dbo.OptionItemEntities", new[] { "DateOfCreation" });
            DropIndex("dbo.OptionItemEntities", new[] { "Code" });
            DropIndex("dbo.PhotoItemEntities", new[] { "CatalogItem_Id" });
            DropIndex("dbo.PhotoItemEntities", new[] { "ForceUpdated" });
            DropIndex("dbo.PhotoItemEntities", new[] { "LastUpdated" });
            DropIndex("dbo.PhotoItemEntities", new[] { "DateOfCreation" });
            DropIndex("dbo.PhotoItemEntities", new[] { "Name" });
            DropIndex("dbo.PriceGroupItemEntities", new[] { "ForceUpdated" });
            DropIndex("dbo.PriceGroupItemEntities", new[] { "LastUpdated" });
            DropIndex("dbo.PriceGroupItemEntities", new[] { "DateOfCreation" });
            DropIndex("dbo.PriceGroupItemEntities", new[] { "Code" });
            DropIndex("dbo.PriceTypePriceGroupContragentEntities", new[] { "TypeOfPriceItem_Id" });
            DropIndex("dbo.PriceTypePriceGroupContragentEntities", new[] { "PriceGroupItem_Id" });
            DropIndex("dbo.PriceTypePriceGroupContragentEntities", new[] { "ContragentItem_Id" });
            DropIndex("dbo.PriceTypePriceGroupContragentEntities", new[] { "ForceUpdated" });
            DropIndex("dbo.PriceTypePriceGroupContragentEntities", new[] { "LastUpdated" });
            DropIndex("dbo.PriceTypePriceGroupContragentEntities", new[] { "DateOfCreation" });
            DropIndex("dbo.TypeOfPricesNomenclatureItemEntities", new[] { "TypeOfPriceItem_Id" });
            DropIndex("dbo.TypeOfPricesNomenclatureItemEntities", new[] { "CatalogItem_Id" });
            DropIndex("dbo.TypeOfPricesNomenclatureItemEntities", new[] { "ForceUpdated" });
            DropIndex("dbo.TypeOfPricesNomenclatureItemEntities", new[] { "LastUpdated" });
            DropIndex("dbo.TypeOfPricesNomenclatureItemEntities", new[] { "DateOfCreation" });
            DropIndex("dbo.TypeOfPriceItemEntities", new[] { "ForceUpdated" });
            DropIndex("dbo.TypeOfPriceItemEntities", new[] { "LastUpdated" });
            DropIndex("dbo.TypeOfPriceItemEntities", new[] { "DateOfCreation" });
            DropIndex("dbo.TypeOfPriceItemEntities", new[] { "Code" });
            DropIndex("dbo.NomenclatureGroupEntities", new[] { "ForceUpdated" });
            DropIndex("dbo.NomenclatureGroupEntities", new[] { "LastUpdated" });
            DropIndex("dbo.NomenclatureGroupEntities", new[] { "DateOfCreation" });
            DropIndex("dbo.NomenclatureGroupEntities", new[] { "Code" });
            DropIndex("dbo.PriceTypeNomenclatureGroupContragentEntities", new[] { "TypeOfPriceItem_Id" });
            DropIndex("dbo.PriceTypeNomenclatureGroupContragentEntities", new[] { "NomenclatureGroupItem_Id" });
            DropIndex("dbo.PriceTypeNomenclatureGroupContragentEntities", new[] { "ContragentItem_Id" });
            DropIndex("dbo.PriceTypeNomenclatureGroupContragentEntities", new[] { "ForceUpdated" });
            DropIndex("dbo.PriceTypeNomenclatureGroupContragentEntities", new[] { "LastUpdated" });
            DropIndex("dbo.PriceTypeNomenclatureGroupContragentEntities", new[] { "DateOfCreation" });
            DropIndex("dbo.ContragentItemEntities", new[] { "ForceUpdated" });
            DropIndex("dbo.ContragentItemEntities", new[] { "LastUpdated" });
            DropIndex("dbo.ContragentItemEntities", new[] { "DateOfCreation" });
            DropIndex("dbo.ContragentItemEntities", "IX_Code");
            DropIndex("dbo.DiscountsContragentEntities", new[] { "ContragentItem_Id" });
            DropIndex("dbo.DiscountsContragentEntities", new[] { "CatalogItem_Id" });
            DropIndex("dbo.DiscountsContragentEntities", new[] { "ForceUpdated" });
            DropIndex("dbo.DiscountsContragentEntities", new[] { "LastUpdated" });
            DropIndex("dbo.DiscountsContragentEntities", new[] { "DateOfCreation" });
            DropIndex("dbo.DirectoryEntities", new[] { "Parent_Id" });
            DropIndex("dbo.DirectoryEntities", new[] { "ForceUpdated" });
            DropIndex("dbo.DirectoryEntities", new[] { "LastUpdated" });
            DropIndex("dbo.DirectoryEntities", new[] { "DateOfCreation" });
            DropIndex("dbo.DirectoryEntities", new[] { "Code" });
            DropIndex("dbo.CommodityDirectionEntities", new[] { "ForceUpdated" });
            DropIndex("dbo.CommodityDirectionEntities", new[] { "LastUpdated" });
            DropIndex("dbo.CommodityDirectionEntities", new[] { "DateOfCreation" });
            DropIndex("dbo.CommodityDirectionEntities", new[] { "Code" });
            DropIndex("dbo.CatalogItemEntities", new[] { "PriceGroup_Id" });
            DropIndex("dbo.CatalogItemEntities", new[] { "NomenclatureGroup_Id" });
            DropIndex("dbo.CatalogItemEntities", new[] { "Directory_Id" });
            DropIndex("dbo.CatalogItemEntities", new[] { "Brand_Id" });
            DropIndex("dbo.CatalogItemEntities", new[] { "ForceUpdated" });
            DropIndex("dbo.CatalogItemEntities", new[] { "LastUpdated" });
            DropIndex("dbo.CatalogItemEntities", new[] { "DateOfCreation" });
            DropIndex("dbo.CatalogItemEntities", new[] { "UID" });
            DropIndex("dbo.BrandItemEntities", new[] { "ForceUpdated" });
            DropIndex("dbo.BrandItemEntities", new[] { "LastUpdated" });
            DropIndex("dbo.BrandItemEntities", new[] { "DateOfCreation" });
            DropIndex("dbo.BrandItemEntities", new[] { "Code" });
            DropTable("dbo.CommodityDirectionEntityCatalogItemEntities");
            DropTable("dbo.SendItemsEntities");
            DropTable("dbo.ProductDirectionEntities");
            DropTable("dbo.OrderEntities");
            DropTable("dbo.OptionItemEntities");
            DropTable("dbo.PhotoItemEntities");
            DropTable("dbo.PriceGroupItemEntities");
            DropTable("dbo.PriceTypePriceGroupContragentEntities");
            DropTable("dbo.TypeOfPricesNomenclatureItemEntities");
            DropTable("dbo.TypeOfPriceItemEntities");
            DropTable("dbo.NomenclatureGroupEntities");
            DropTable("dbo.PriceTypeNomenclatureGroupContragentEntities");
            DropTable("dbo.ContragentItemEntities");
            DropTable("dbo.DiscountsContragentEntities");
            DropTable("dbo.DirectoryEntities");
            DropTable("dbo.CommodityDirectionEntities");
            DropTable("dbo.CatalogItemEntities");
            DropTable("dbo.BrandItemEntities");
        }
    }
}
