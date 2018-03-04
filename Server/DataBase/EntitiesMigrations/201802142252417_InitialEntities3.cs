using System.Text;

namespace DataBase.EntitiesMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class InitialEntities3 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(
                "dbo.GetBrandItems",
                p => new
                {
                    login = p.String(maxLength: 30),
                    countToUpdate = p.Int(),
                },
                body: CreateGetGetBrandItemsBody());

            CreateStoredProcedure(
                "dbo.GetCatalogItems",
                p => new
                {
                    login = p.String(maxLength: 30),
                    countToUpdate = p.Int(),
                },
                body: CreateGetCatalogItemsBody());

            CreateStoredProcedure(
                "dbo.GetDirectoryItems",
                p => new 
                {
                    login = p.String(maxLength: 30),
                    countToUpdate = p.Int(),
                },
                body: CreateGetDirectoryItemsBody());

            Sql(CreateGetPhotosIdsProcedure());

            CreateStoredProcedure(
                "dbo.GetProductDirectionItems",
                p => new
                {
                    login = p.String(maxLength: 30),
                    countToUpdate = p.Int(),
                }, 
                body: CreateGetProductDirectionItemsBody());

            Sql(CreateBrandsTableType());
            Sql(CreateUpdateBrandsProcedure());

        }

        public override void Down()
        {
            DropStoredProcedure("dbo.GetBrandItems");
            DropStoredProcedure("dbo.GetCatalogItems");
            DropStoredProcedure("dbo.GetDirectoryItems");
            DropStoredProcedure("dbo.GetPhotosIds");
            DropStoredProcedure("dbo.GetProductDirectionItems");
            DropStoredProcedure("dbo.UpdateBrands");
            Sql("DROP TYPE [dbo].[brandsTable]");
        }

        private string CreateGetGetBrandItemsBody()
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine("--  +-----------------------------------------------------+");
            body.AppendLine("--  | Author:       OLEXANDR LIKHOSHVA                    |");
            body.AppendLine("--  | Create date:  2018.02.26 01:27                      |");
            body.AppendLine("--  | Copyright:    © 2017-2018  ALL RIGHT RESERVED       |");
            body.AppendLine("--  | https://www.linkedin.com/in/olexandrlikhoshva/      |");
            body.AppendLine("--  |-----------------------------------------------------|");
            body.AppendLine("--  | Description:  Procedure for getting Brand items     |");
            body.AppendLine("--  +-----------------------------------------------------+");
            body.AppendLine("--  SET NOCOUNT ON added to prevent extra result sets from");
            body.AppendLine("--  interfering with SELECT statements.");
            body.AppendLine("--  SET NOCOUNT ON;");
            body.AppendLine(" ");
            body.AppendLine("    DECLARE @contragentId bigint = -1;");
            body.AppendLine(" ");
            body.AppendLine("    SELECT TOP (1) @contragentId = [Contragent].[Id]");
            body.AppendLine("    FROM [ContragentItemEntities] AS [Contragent]");
            body.AppendLine("    WHERE ([Contragent].[Login] = @login);");
            body.AppendLine(" ");
            body.AppendLine("    SELECT TOP (@countToUpdate)");
            body.AppendLine("           [Brand].[Id] AS [Id],");
            body.AppendLine("           [Brand].[Code] AS [Code],");
            body.AppendLine("           [Brand].[Name] AS [Name],");
            body.AppendLine("           [Brand].[DateOfCreation] AS [DateOfCreation],");
            body.AppendLine("           [Brand].[LastUpdated] AS [LastUpdated],");
            body.AppendLine("           [Brand].[ForceUpdated] AS [ForceUpdated]");
            body.AppendLine("    FROM [SendItemsEntities] AS [SendItems]");
            body.AppendLine("    INNER JOIN [BrandItemEntities] AS [Brand] ON [SendItems].[EntityId] = [Brand].[Id]");
            body.AppendLine("    WHERE [SendItems].[Contragent_Id] = @contragentId AND");
            body.AppendLine("          [SendItems].[EntityName] = 1;");
            body.AppendLine(" ");
            body.AppendLine("--  +----------------------------+");
            body.AppendLine("--  | Entity Name:               |");
            body.AppendLine("--  +----------------------------+");
            body.AppendLine("--  | BrandItemEntity = 1        |");
            body.AppendLine("--  | CatalogItemEntity = 2      |");
            body.AppendLine("--  | DirectoryEntity = 3        |");
            body.AppendLine("--  | PhotoItemEntity = 5        |");
            body.AppendLine("--  | ProductDirectionEntity = 6 |");
            body.AppendLine("--  +----------------------------+");
            body.AppendLine("  ");

            return body.ToString();
        }

        private string CreateGetCatalogItemsBody()
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine("--  +---------------------------------------------------+");
            body.AppendLine("--  | Author:       OLEXANDR LIKHOSHVA                  |");
            body.AppendLine("--  | Create date:  2018.02.11 23:45                    |");
            body.AppendLine("--  | Copyright:    © 2017-2018  ALL RIGHT RESERVED     |");
            body.AppendLine("--  | https://www.linkedin.com/in/olexandrlikhoshva/    |");
            body.AppendLine("--  |---------------------------------------------------|");
            body.AppendLine("--  | Description:  Procedure for getting Catalog items |");
            body.AppendLine("--  +---------------------------------------------------+");
            body.AppendLine("--  SET NOCOUNT ON added to prevent extra result sets from");
            body.AppendLine("--  interfering with SELECT statements.");
            body.AppendLine("--  SET NOCOUNT ON;");
            body.AppendLine(" ");
            body.AppendLine("    DECLARE @contragentId bigint = -1;");
            body.AppendLine(" ");
            body.AppendLine("    SELECT TOP (1) @contragentId = [Contragent].[Id]");
            body.AppendLine("    FROM [ContragentItemEntities] AS [Contragent]");
            body.AppendLine("    WHERE ([Contragent].[Login] = @login);");
            body.AppendLine(" ");
            body.AppendLine("    SELECT TOP (@countToUpdate)");
            body.AppendLine("        [CatalogItems].[Id] AS [Id],");
            body.AppendLine("        [CatalogItems].[UID] AS [UID],");
            body.AppendLine("        [CatalogItems].[Code] AS [Code],");
            body.AppendLine("        [CatalogItems].[Article] AS [Article],");
            body.AppendLine("        [CatalogItems].[Name] AS [Name],");
            body.AppendLine("        [CatalogItems].[Brand_Id] AS [BrandId],");
            body.AppendLine("        [CatalogItems].[BrandName] AS [BrandName],");
            body.AppendLine("        [CatalogItems].[Unit] AS [Unit],");
            body.AppendLine("        [CatalogItems].[EnterpriceNormPack] AS [EnterpriceNormPack],");
            body.AppendLine("        [CatalogItems].[BatchOfSales] AS [BatchOfSales],");
            body.AppendLine("        [CatalogItems].[Balance] AS [Balance],");
            body.AppendLine("        [CatalogItems].[Balance] AS [Balance],");
            body.AppendLine("        CASE WHEN TOPN1.Price IS NOT NULL");
            body.AppendLine("            THEN");
            body.AppendLine("                CASE WHEN Discounts.Rate IS NOT NULL");
            body.AppendLine("                    THEN TOPN1.Price + Discounts.Rate * TOPN1.Price / 100");
            body.AppendLine("                    ELSE TOPN1.Price");
            body.AppendLine("                END");
            body.AppendLine("            ELSE");
            body.AppendLine("                CASE WHEN Discounts.Rate IS NOT NULL");
            body.AppendLine("                    THEN");
            body.AppendLine("                        CASE WHEN TOPN2.Price IS NOT NULL");
            body.AppendLine("                            THEN TOPN2.Price + Discounts.Rate * TOPN2.Price / 100");
            body.AppendLine("                            ELSE 0");
            body.AppendLine("                        END");
            body.AppendLine("                    ELSE");
            body.AppendLine("                        CASE WHEN TOPN2.Price IS NOT NULL");
            body.AppendLine("                            THEN TOPN2.Price");
            body.AppendLine("                            ELSE 0");
            body.AppendLine("                        END");
            body.AppendLine("                END");
            body.AppendLine("        END AS [Price],");
            body.AppendLine("        CASE WHEN TOPN1.Price IS NOT NULL");
            body.AppendLine("            THEN TOPN1.Currency");
            body.AppendLine("            ELSE TOPN2.Currency");
            body.AppendLine("        END AS [Currency],");
            body.AppendLine("        [CatalogItems].[Multiplicity] AS [Multiplicity],");
            body.AppendLine("        [CatalogItems].[HasPhotos] AS [HasPhotos],");
            body.AppendLine("        [CatalogItems].[DateOfCreation] AS [DateOfCreation],");
            body.AppendLine("        [CatalogItems].[LastUpdated] AS [LastUpdated],");
            body.AppendLine("        [CatalogItems].[ForceUpdated] AS [ForceUpdated],");
            body.AppendLine("        [CatalogItems].[Status] AS [Status],");
            body.AppendLine("        [CatalogItems].[Directory_Id] AS [DirectoryId]");
            body.AppendLine("    FROM [SendItemsEntities] AS [SendItems]");
            body.AppendLine("    INNER JOIN [CatalogItemEntities] AS [CatalogItems]");
            body.AppendLine("        ON [SendItems].[EntityId] = [CatalogItems].[Id]");
            body.AppendLine("    LEFT JOIN [PriceTypePriceGroupContragentEntities] AS [PTPGC]");
            body.AppendLine("        ON [CatalogItems].[PriceGroup_Id] = [PTPGC].[PriceGroupItem_Id] AND");
            body.AppendLine("           [SendItems].[Contragent_Id] = [PTPGC].[ContragentItem_Id]");
            body.AppendLine("    LEFT JOIN [TypeOfPricesNomenclatureItemEntities] AS [TOPN1]");
            body.AppendLine("        ON [CatalogItems].[Id] = [TOPN1].[CatalogItem_Id] AND");
            body.AppendLine("           [PTPGC].[TypeOfPriceItem_Id] = [TOPN1].[TypeOfPriceItem_Id]");
            body.AppendLine("    LEFT JOIN [PriceTypeNomenclatureGroupContragentEntities] AS [PTNGC]");
            body.AppendLine("        ON [CatalogItems].[NomenclatureGroup_Id] = [PTNGC].[NomenclatureGroupItem_Id] AND");
            body.AppendLine("           [SendItems].[Contragent_Id] = [PTNGC].[ContragentItem_Id]");
            body.AppendLine("    LEFT JOIN [TypeOfPricesNomenclatureItemEntities] AS [TOPN2]");
            body.AppendLine("        ON [CatalogItems].[Id] = [TOPN2].[CatalogItem_Id] AND");
            body.AppendLine("           [PTNGC].[TypeOfPriceItem_Id] = [TOPN2].[TypeOfPriceItem_Id]");
            body.AppendLine("    LEFT JOIN [DiscountsContragentEntities] AS [Discounts]");
            body.AppendLine("        ON [CatalogItems].[Id] = [Discounts].[CatalogItem_Id] AND");
            body.AppendLine("           [SendItems].[Contragent_Id] = [Discounts].[ContragentItem_Id]");
            body.AppendLine("    WHERE [SendItems].[Contragent_Id] = @contragentId AND");
            body.AppendLine("          [SendItems].[EntityName] = 2;");
            body.AppendLine(" ");
            body.AppendLine("--  +----------------------------+");
            body.AppendLine("--  | Entity Name:               |");
            body.AppendLine("--  +----------------------------+");
            body.AppendLine("--  | BrandItemEntity = 1        |");
            body.AppendLine("--  | CatalogItemEntity = 2      |");
            body.AppendLine("--  | DirectoryEntity = 3        |");
            body.AppendLine("--  | PhotoItemEntity = 5        |");
            body.AppendLine("--  | ProductDirectionEntity = 6 |");
            body.AppendLine("--  +----------------------------+");
            body.AppendLine("  ");

            return body.ToString();
        }

        private string CreateGetDirectoryItemsBody()
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine("--  +-----------------------------------------------------+");
            body.AppendLine("--  | Author:       OLEXANDR LIKHOSHVA                    |");
            body.AppendLine("--  | Create date:  2018.02.24 17:21                      |");
            body.AppendLine("--  | Copyright:    © 2017-2018  ALL RIGHT RESERVED       |");
            body.AppendLine("--  | https://www.linkedin.com/in/olexandrlikhoshva/      |");
            body.AppendLine("--  |-----------------------------------------------------|");
            body.AppendLine("--  | Description:  Procedure for getting Directory items |");
            body.AppendLine("--  +-----------------------------------------------------+");
            body.AppendLine("--  SET NOCOUNT ON added to prevent extra result sets from");
            body.AppendLine("--  interfering with SELECT statements.");
            body.AppendLine("--  SET NOCOUNT ON;");
            body.AppendLine(" ");
            body.AppendLine("    DECLARE @contragentId bigint = -1;");
            body.AppendLine(" ");
            body.AppendLine("    SELECT TOP (1) @contragentId = [Contragent].[Id]");
            body.AppendLine("    FROM [ContragentItemEntities] AS [Contragent]");
            body.AppendLine("    WHERE ([Contragent].[Login] = @login);");
            body.AppendLine(" ");
            body.AppendLine("    WITH [DirectoryList]");
            body.AppendLine("        ([DirectoryLevel], [Id], [Code], [Name], [Parent_Id],");
            body.AppendLine("         [DateOfCreation], [LastUpdated], [ForceUpdated]) AS");
            body.AppendLine("        (SELECT 0 AS [DirectoryLevel],");
            body.AppendLine("                [Directory].[Id], [Directory].[Code], [Directory].[Name],");
            body.AppendLine("                [Directory].[Parent_Id], [Directory].[DateOfCreation],");
            body.AppendLine("                [Directory].[LastUpdated], [Directory].[ForceUpdated]");
            body.AppendLine("         FROM [DirectoryEntities] AS [Directory]");
            body.AppendLine("         WHERE [Directory].[Parent_Id] IS NULL");
            body.AppendLine("         UNION ALL");
            body.AppendLine("         SELECT [ParentDirectory].[DirectoryLevel] + 1,");
            body.AppendLine("                [DependencyDirectory].[Id], [DependencyDirectory].[Code], [DependencyDirectory].[Name],");
            body.AppendLine("                [DependencyDirectory].[Parent_Id], [DependencyDirectory].[DateOfCreation],");
            body.AppendLine("                [DependencyDirectory].[LastUpdated], [DependencyDirectory].[ForceUpdated]");
            body.AppendLine("         FROM [DirectoryEntities] AS [DependencyDirectory]");
            body.AppendLine("         INNER JOIN [DirectoryList] AS [ParentDirectory]");
            body.AppendLine("             ON [DependencyDirectory].[Parent_Id] = [ParentDirectory].[Id])");
            body.AppendLine("    SELECT TOP (@countToUpdate)");
            body.AppendLine("           [Dir].[Id] AS [Id], [Dir].[Code] AS [Code], [Dir].[Name] AS [Name],");
            body.AppendLine("           [Dir].[Parent_Id] AS [ParentId], [Dir].[DateOfCreation] AS [DateOfCreation],");
            body.AppendLine("           [Dir].[LastUpdated] AS [LastUpdated], [Dir].[ForceUpdated] AS [ForceUpdated]");
            body.AppendLine("    FROM [SendItemsEntities] AS [SendItems]");
            body.AppendLine("    INNER JOIN [DirectoryList] AS [Dir] ON [SendItems].[EntityId] = [Dir].[Id]");
            body.AppendLine("    WHERE [SendItems].[Contragent_Id] = @contragentId AND");
            body.AppendLine("          [SendItems].[EntityName] = 3");
            body.AppendLine("    ORDER BY [Dir].[DirectoryLevel], [Dir].[Parent_Id], [Dir].[Id];");
            body.AppendLine(" ");
            body.AppendLine("--  +----------------------------+");
            body.AppendLine("--  | Entity Name:               |");
            body.AppendLine("--  +----------------------------+");
            body.AppendLine("--  | BrandItemEntity = 1        |");
            body.AppendLine("--  | CatalogItemEntity = 2      |");
            body.AppendLine("--  | DirectoryEntity = 3        |");
            body.AppendLine("--  | PhotoItemEntity = 5        |");
            body.AppendLine("--  | ProductDirectionEntity = 6 |");
            body.AppendLine("--  +----------------------------+");
            body.AppendLine(" ");

            return body.ToString();
        }

        private string CreateGetPhotosIdsProcedure()
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine("CREATE PROCEDURE [dbo].[GetPhotosIds]");
            body.AppendLine("    @ids [dbo].[bigintTable] READONLY");
            body.AppendLine("AS");
            body.AppendLine("BEGIN");
            body.AppendLine("--  +---------------------------------------------------+");
            body.AppendLine("--  | Author:       OLEXANDR LIKHOSHVA                  |");
            body.AppendLine("--  | Create date:  2018.02.11 23:48                    |");
            body.AppendLine("--  | Copyright:    © 2017-2018  ALL RIGHT RESERVED     |");
            body.AppendLine("--  | https://www.linkedin.com/in/olexandrlikhoshva/    |");
            body.AppendLine("--  |---------------------------------------------------|");
            body.AppendLine("--  | Description:  Procedure for getting Photos ids    |");
            body.AppendLine("--  +---------------------------------------------------+");
            body.AppendLine(" ");
            body.AppendLine("--  SET NOCOUNT ON added to prevent extra result sets from");
            body.AppendLine("--  interfering with SELECT statements.");
            body.AppendLine("--  SET NOCOUNT ON;");
            body.AppendLine(" ");
            body.AppendLine("    SELECT [Photos].[CatalogItem_Id] AS [CatalogId], [Photos].[Id] AS [PhotoId]");
            body.AppendLine("    FROM [dbo].[PhotoItemEntities] AS [Photos]");
            body.AppendLine("    WHERE[Photos].[CatalogItem_Id] IN (SELECT Id FROM @ids)");
            body.AppendLine(" ");
            body.AppendLine("END");

            return body.ToString();
        }

        private string CreateGetProductDirectionItemsBody()
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine("--  +------------------------------------------------------------+");
            body.AppendLine("--  | Author:       OLEXANDR LIKHOSHVA                           |");
            body.AppendLine("--  | Create date:  2018.02.24 17:21                             |");
            body.AppendLine("--  | Copyright:    © 2017-2018  ALL RIGHT RESERVED              |");
            body.AppendLine("--  | https://www.linkedin.com/in/olexandrlikhoshva/             |");
            body.AppendLine("--  |------------------------------------------------------------|");
            body.AppendLine("--  | Description:  Procedure for getting ProductDirection items |");
            body.AppendLine("--  +------------------------------------------------------------+");
            body.AppendLine(" ");
            body.AppendLine("--  SET NOCOUNT ON added to prevent extra result sets from");
            body.AppendLine("--  interfering with SELECT statements.");
            body.AppendLine("--  SET NOCOUNT ON;");
            body.AppendLine(" ");
            body.AppendLine("    DECLARE @contragentId bigint = -1;");
            body.AppendLine(" ");
            body.AppendLine("    SELECT TOP (1) @contragentId = [Contragent].[Id]");
            body.AppendLine("    FROM [ContragentItemEntities] AS [Contragent]");
            body.AppendLine("    WHERE ([Contragent].[Login] = @login);");
            body.AppendLine(" ");
            body.AppendLine("    SELECT TOP (@countToUpdate)");
            body.AppendLine("           [ProductDirection].[Id] AS [Id],");
            body.AppendLine("           [ProductDirection].[Direction] AS [Direction],");
            body.AppendLine("           [ProductDirection].[Directory_Id] AS [DirectoryId],");
            body.AppendLine("           [ProductDirection].[DateOfCreation] AS [DateOfCreation],");
            body.AppendLine("           [ProductDirection].[LastUpdated] AS [LastUpdated],");
            body.AppendLine("           [ProductDirection].[ForceUpdated] AS [ForceUpdated]");
            body.AppendLine("    FROM [SendItemsEntities] AS [SendItems]");
            body.AppendLine("    INNER JOIN [ProductDirectionEntities] AS [ProductDirection]");
            body.AppendLine("          ON [SendItems].[EntityId] = [ProductDirection].[Id]");
            body.AppendLine("    WHERE [SendItems].[Contragent_Id] = @contragentId AND");
            body.AppendLine("          [SendItems].[EntityName] = 6");
            body.AppendLine(" ");
            body.AppendLine("--  +----------------------------+");
            body.AppendLine("--  | Entity Name:               |");
            body.AppendLine("--  +----------------------------+");
            body.AppendLine("--  | BrandItemEntity = 1        |");
            body.AppendLine("--  | CatalogItemEntity = 2      |");
            body.AppendLine("--  | DirectoryEntity = 3        |");
            body.AppendLine("--  | PhotoItemEntity = 5        |");
            body.AppendLine("--  | ProductDirectionEntity = 6 |");
            body.AppendLine("--  +----------------------------+");
            body.AppendLine(" ");

            return body.ToString();
        }

        private string CreateBrandsTableType()
        {
            StringBuilder type = new StringBuilder();

            type.AppendLine("CREATE TYPE [dbo].[brandsTable] AS TABLE");
            type.AppendLine("(");
            type.AppendLine("    [Code] [uniqueidentifier] NOT NULL,");
            type.AppendLine("    [Name] [nvarchar](255) NULL,");
            type.AppendLine("    PRIMARY KEY ([Code])");
            type.AppendLine(")");

            return type.ToString();
        }

        private string CreateUpdateBrandsProcedure()
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine("CREATE PROCEDURE [dbo].[UpdateBrands]");
            body.AppendLine("    @brands [dbo].[brandsTable] READONLY,");
            body.AppendLine("    @lastUpdate [datetimeoffset](7)");
            body.AppendLine("AS");
            body.AppendLine("BEGIN");
            body.AppendLine("--  +---------------------------------------------------+");
            body.AppendLine("--  | Author:       OLEXANDR LIKHOSHVA                  |");
            body.AppendLine("--  | Create date:  2018.03.03 21:47                    |");
            body.AppendLine("--  | Copyright:    © 2017-2018  ALL RIGHT RESERVED     |");
            body.AppendLine("--  | https://www.linkedin.com/in/olexandrlikhoshva/    |");
            body.AppendLine("--  |---------------------------------------------------|");
            body.AppendLine("--  | Description:  Procedure for insert or update      |");
            body.AppendLine("--  |               BrandItemEntities                   |");
            body.AppendLine("--  +---------------------------------------------------+");
            body.AppendLine("--  SET NOCOUNT ON added to prevent extra result sets from");
            body.AppendLine("--  interfering with SELECT statements.");
            body.AppendLine("--  SET NOCOUNT ON;");
            body.AppendLine(" ");
            body.AppendLine("    WITH [BrandsForUpdate] AS");
            body.AppendLine("        (SELECT [BrandsItems].[Code] AS [Code],");
            body.AppendLine("                [BrandsItems].[Name] AS [Name],");
            body.AppendLine("                [BrandsItems].[LastUpdated] AS [LastUpdated],");
            body.AppendLine("                [BrandsItems].[ForceUpdated] AS [ForceUpdated]");
            body.AppendLine("         FROM [BrandItemEntities] AS [BrandsItems]");
            body.AppendLine("         WHERE [BrandsItems].[Code] IN (SELECT [Code] FROM @brands))");
            body.AppendLine("    UPDATE [ItemsForUpdate]");
            body.AppendLine("    SET [ItemsForUpdate].[LastUpdated] = CASE WHEN [ItemsForUpdate].[Name] = [UpdateItems].[Name]");
            body.AppendLine("                                             THEN [ItemsForUpdate].[LastUpdated]");
            body.AppendLine("                                             ELSE @lastUpdate");
            body.AppendLine("                                         END,");
            body.AppendLine("        [ItemsForUpdate].[ForceUpdated] = @lastUpdate,");
            body.AppendLine("        [ItemsForUpdate].[Code] = [UpdateItems].[Code],");
            body.AppendLine("        [ItemsForUpdate].[Name] = [UpdateItems].[Name]");
            body.AppendLine("    FROM [BrandsForUpdate] AS [ItemsForUpdate]");
            body.AppendLine("    INNER JOIN @brands AS [UpdateItems] ON [ItemsForUpdate].[Code] = [UpdateItems].[Code];");
            body.AppendLine(" ");
            body.AppendLine("    INSERT INTO  [BrandItemEntities]");
            body.AppendLine("        ([Code], [Name], [DateOfCreation], [LastUpdated], [ForceUpdated])");
            body.AppendLine("    SELECT [InsertItems].[Code], [InsertItems].[Name],");
            body.AppendLine("           @lastUpdate, @lastUpdate, @lastUpdate");
            body.AppendLine("    FROM @brands AS [InsertItems]");
            body.AppendLine("    WHERE [InsertItems].[Code] NOT IN (SELECT [BrandItems].[Code] FROM [BrandItemEntities] AS [BrandItems]);");
            body.AppendLine(" ");
            body.AppendLine("END");

            return body.ToString();
        }
    }
}
