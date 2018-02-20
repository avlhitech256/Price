using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DataBase.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialEntities3 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(
                "dbo.GetCatalogItems",
                p => new
                {
                    login = p.String(maxLength: 30),
                    countToUpdate = p.Int(),
                },
                body: CreateGetCatalogItemsBody());

            Sql(CreateGetPhotosIdsProcedure());
        }

        public override void Down()
        {
            DropStoredProcedure("dbo.GetCatalogItems");
            DropStoredProcedure("dbo.GetPhotosIds");
        }

        private string CreateGetCatalogItemsBody()
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine("--     +---------------------------------------------------+");
            body.AppendLine("--     | Author:       OLEXANDR LIKHOSHVA                  |");
            body.AppendLine("--     | Create date:  2018.02.11 23:45                    |");
            body.AppendLine("--     | Copyright:    © 2017-2018  ALL RIGHT RESERVED     |");
            body.AppendLine("--     | https://www.linkedin.com/in/olexandrlikhoshva/    |");
            body.AppendLine("--     |---------------------------------------------------|");
            body.AppendLine("--     | Description:  Procedure for getting Catalog items |");
            body.AppendLine("--     +---------------------------------------------------+");
            body.AppendLine("--     SET NOCOUNT ON added to prevent extra result sets from");
            body.AppendLine("--     interfering with SELECT statements.");
            body.AppendLine("--     SET NOCOUNT ON;");
            body.AppendLine(" ");
            body.AppendLine(" ");
            body.AppendLine("    declare @contragentId bigint = -1;");
            body.AppendLine(" ");
            body.AppendLine(" ");
            body.AppendLine("    SELECT TOP (1) @contragentId = [Contragent].[Login] ");
            body.AppendLine(" ");
            body.AppendLine("    FROM [ContragentItemEntities] AS [Contragent];");
            body.AppendLine(" ");
            body.AppendLine("    SELECT TOP (@countToUpdate)");
            body.AppendLine("           [CatalogItems].[Id] AS [Id],");
            body.AppendLine("           [CatalogItems].[UID] AS [UID],");
            body.AppendLine("           [CatalogItems].[Code] AS [Code],");
            body.AppendLine("           [CatalogItems].[Article] AS [Article],");
            body.AppendLine("           [CatalogItems].[Name] AS [Name],");
            body.AppendLine("           [CatalogItems].[Brand_Id] AS [BrandId],");
            body.AppendLine("           [CatalogItems].[BrandName] AS [BrandName],");
            body.AppendLine("           [CatalogItems].[Unit] AS [Unit],");
            body.AppendLine("           [CatalogItems].[EnterpriceNormPack] AS [EnterpriceNormPack],");
            body.AppendLine("           [CatalogItems].[BatchOfSales] AS [BatchOfSales],");
            body.AppendLine("           [CatalogItems].[Balance] AS [Balance],");
            body.AppendLine("           [CatalogItems].[Balance] AS [Balance],");
            body.AppendLine("           CASE WHEN TOPN1.Price IS NOT NULL ");
            body.AppendLine("              THEN ");
            body.AppendLine("                 CASE WHEN Discounts.Rate IS NOT NULL ");
            body.AppendLine("                    THEN TOPN1.Price + Discounts.Rate * TOPN1.Price");
            body.AppendLine("                    ELSE TOPN1.Price");
            body.AppendLine("                 END");
            body.AppendLine("              ELSE ");
            body.AppendLine("                 CASE WHEN Discounts.Rate IS NOT NULL ");
            body.AppendLine("                    THEN ");
            body.AppendLine("                       CASE WHEN TOPN2.Price IS NOT NULL ");
            body.AppendLine("                          THEN TOPN2.Price + Discounts.Rate * TOPN2.Price");
            body.AppendLine("                          ELSE 0");
            body.AppendLine("                       END");
            body.AppendLine("                    ELSE");
            body.AppendLine("                       CASE WHEN TOPN2.Price IS NOT NULL ");
            body.AppendLine("                          THEN TOPN2.Price");
            body.AppendLine("                          ELSE 0");
            body.AppendLine("                       END");
            body.AppendLine(" ");
            body.AppendLine("             END");
            body.AppendLine("           END AS [Price],");
            body.AppendLine("           CASE WHEN TOPN1.Price IS NOT NULL ");
            body.AppendLine(" ");
            body.AppendLine("          THEN TOPN1.Currency");
            body.AppendLine("              ELSE TOPN2.Currency");
            body.AppendLine("           END AS [Currency],");
            body.AppendLine("           [CatalogItems].[Multiplicity] AS [Multiplicity],");
            body.AppendLine("           [CatalogItems].[HasPhotos] AS [HasPhotos],");
            body.AppendLine("           [CatalogItems].[DateOfCreation] AS [DateOfCreation],");
            body.AppendLine("           [CatalogItems].[LastUpdated] AS [LastUpdated],");
            body.AppendLine("           [CatalogItems].[ForceUpdated] AS [ForceUpdated],");
            body.AppendLine("           [CatalogItems].[Status] AS [Status],");
            body.AppendLine("           [CatalogItems].[Directory_Id] AS [DirectoryId]");
            body.AppendLine("    FROM [SendItemsEntities] AS [SendItems]");
            body.AppendLine("    INNER JOIN [CatalogItemEntities] AS [CatalogItems] ON [SendItems].[EntityId] = [CatalogItems].[Id] ");
            body.AppendLine("    LEFT JOIN [PriceTypePriceGroupContragentEntities] AS [PTPGC] ON [CatalogItems].[PriceGroup_Id] = [PTPGC].[PriceGroupItem_Id] AND ");
            body.AppendLine("                                                                    [SendItems].[Contragent_Id] = [PTPGC].[ContragentItem_Id]");
            body.AppendLine("    LEFT JOIN [TypeOfPricesNomenclatureItemEntities] AS [TOPN1] ON [CatalogItems].[Id] = [TOPN1].[CatalogItem_Id] AND ");
            body.AppendLine("                                                                   [PTPGC].[TypeOfPriceItem_Id] = [TOPN1].[TypeOfPriceItem_Id]");
            body.AppendLine("    LEFT JOIN [PriceTypeNomenclatureGroupContragentEntities] AS [PTNGC] ON [CatalogItems].[NomenclatureGroup_Id] = [PTNGC].[NomenclatureGroupItem_Id] AND ");
            body.AppendLine("                                                                           [SendItems].[Contragent_Id] = [PTNGC].[ContragentItem_Id]");
            body.AppendLine("    LEFT JOIN [TypeOfPricesNomenclatureItemEntities] AS [TOPN2] ON [CatalogItems].[Id] = [TOPN2].[CatalogItem_Id] AND ");
            body.AppendLine("                                                                   [PTNGC].[TypeOfPriceItem_Id] = [TOPN2].[TypeOfPriceItem_Id]");
            body.AppendLine("    LEFT JOIN [DiscountsContragentEntities] AS [Discounts] ON [CatalogItems].[Id] = [Discounts].[CatalogItem_Id] AND");
            body.AppendLine("                                                              [SendItems].[Contragent_Id] = [Discounts].[ContragentItem_Id]");
            body.AppendLine("    WHERE [SendItems].[Contragent_Id] = @contragentId AND ");
            body.AppendLine("          [SendItems].[EntityName] = 2;");
            body.AppendLine(" ");
            body.AppendLine("    -- Entity Names:");
            body.AppendLine("    -- BrandItemEntity = 1");
            body.AppendLine("    -- CatalogItemEntity = 2");
            body.AppendLine("    -- DirectoryEntity = 3");
            body.AppendLine("    -- PhotoItemEntity = 5");
            body.AppendLine("    -- ProductDirectionEntity = 6");
            body.AppendLine(" ");

            return body.ToString();
        }

        private string CreateGetPhotosIdsProcedure()
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine("USE [ServerPriceList]");
            body.AppendLine("GO");
            body.AppendLine("SET ANSI_NULLS ON");
            body.AppendLine("GO");
            body.AppendLine("SET QUOTED_IDENTIFIER ON");
            body.AppendLine("GO");
            body.AppendLine("CREATE PROCEDURE [dbo].[GetPhotosIds]");
            body.AppendLine("    @ids [dbo].[bigintTable] READONLY");
            body.AppendLine("AS");
            body.AppendLine("BEGIN");
            body.AppendLine("--     +---------------------------------------------------+");
            body.AppendLine("--     | Author:       OLEXANDR LIKHOSHVA                  |");
            body.AppendLine("--     | Create date:  2018.02.11 23:48                    |");
            body.AppendLine("--     | Copyright:    © 2017-2018  ALL RIGHT RESERVED     |");
            body.AppendLine("--     | https://www.linkedin.com/in/olexandrlikhoshva/    |");
            body.AppendLine("--     |---------------------------------------------------|");
            body.AppendLine("--     | Description:  Procedure for getting Photos ids    |");
            body.AppendLine("--     +---------------------------------------------------+");
            body.AppendLine("	-- SET NOCOUNT ON added to prevent extra result sets from");
            body.AppendLine("	-- interfering with SELECT statements.");
            body.AppendLine("	-- SET NOCOUNT ON;");
            body.AppendLine(" ");
            body.AppendLine("	SELECT [Photos].[CatalogItem_Id] AS [CatalogId], [Photos].[Id] AS [PhotoId]");
            body.AppendLine("    FROM [dbo].[PhotoItemEntities] AS [Photos]");
            body.AppendLine("    WHERE[Photos].[CatalogItem_Id] IN (SELECT Id FROM @ids)");
            body.AppendLine(" ");
            body.AppendLine("END");

            return body.ToString();
        }
    }
}
