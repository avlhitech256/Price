USE [ServerPriceList]
GO

/****** Object:  StoredProcedure [dbo].[GetCatalogItems]    Script Date: 01.03.2018 1:03:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetCatalogItems]
    @login [nvarchar](30),
    @countToUpdate [int]
AS
BEGIN
--  +---------------------------------------------------+
--  | Author:       OLEXANDR LIKHOSHVA                  |
--  | Create date:  2018.02.11 23:45                    |
--  | Copyright:    © 2017-2018  ALL RIGHT RESERVED     |
--  | https://www.linkedin.com/in/olexandrlikhoshva/    |
--  |---------------------------------------------------|
--  | Description:  Procedure for getting Catalog items |
--  +---------------------------------------------------+
--  SET NOCOUNT ON added to prevent extra result sets from
--  interfering with SELECT statements.
--  SET NOCOUNT ON;
    
    DECLARE @contragentId bigint = -1;
    
    SELECT TOP (1) @contragentId = [Contragent].[Id] 
    FROM [ContragentItemEntities] AS [Contragent]
    WHERE ([Contragent].[Login] = @login);
    
    SELECT TOP (@countToUpdate)
        [CatalogItems].[Id] AS [Id],
        [CatalogItems].[UID] AS [UID],
        [CatalogItems].[Code] AS [Code],
        [CatalogItems].[Article] AS [Article],
        [CatalogItems].[Name] AS [Name],
        [CatalogItems].[Brand_Id] AS [BrandId],
        [CatalogItems].[BrandName] AS [BrandName],
        [CatalogItems].[Unit] AS [Unit],
        [CatalogItems].[EnterpriceNormPack] AS [EnterpriceNormPack],
        [CatalogItems].[BatchOfSales] AS [BatchOfSales],
        [CatalogItems].[Balance] AS [Balance],
        [CatalogItems].[Balance] AS [Balance],
        CASE WHEN TOPN1.Price IS NOT NULL 
            THEN 
                CASE WHEN Discounts.Rate IS NOT NULL 
                    THEN TOPN1.Price + Discounts.Rate * TOPN1.Price
                    ELSE TOPN1.Price
                END
            ELSE 
                CASE WHEN Discounts.Rate IS NOT NULL 
                    THEN 
                        CASE WHEN TOPN2.Price IS NOT NULL 
                            THEN TOPN2.Price + Discounts.Rate * TOPN2.Price
                            ELSE 0
                        END
                    ELSE
                        CASE WHEN TOPN2.Price IS NOT NULL 
                            THEN TOPN2.Price
                            ELSE 0
                        END
                END
        END AS [Price],
        CASE WHEN TOPN1.Price IS NOT NULL 
            THEN TOPN1.Currency
            ELSE TOPN2.Currency
        END AS [Currency],
        [CatalogItems].[Multiplicity] AS [Multiplicity],
        [CatalogItems].[HasPhotos] AS [HasPhotos],
        [CatalogItems].[DateOfCreation] AS [DateOfCreation],
        [CatalogItems].[LastUpdated] AS [LastUpdated],
        [CatalogItems].[ForceUpdated] AS [ForceUpdated],
        [CatalogItems].[Status] AS [Status],
        [CatalogItems].[Directory_Id] AS [DirectoryId]
    FROM [SendItemsEntities] AS [SendItems]
    INNER JOIN [CatalogItemEntities] AS [CatalogItems] 
        ON [SendItems].[EntityId] = [CatalogItems].[Id] 
    LEFT JOIN [PriceTypePriceGroupContragentEntities] AS [PTPGC] 
        ON [CatalogItems].[PriceGroup_Id] = [PTPGC].[PriceGroupItem_Id] AND 
           [SendItems].[Contragent_Id] = [PTPGC].[ContragentItem_Id]
    LEFT JOIN [TypeOfPricesNomenclatureItemEntities] AS [TOPN1] 
        ON [CatalogItems].[Id] = [TOPN1].[CatalogItem_Id] AND 
           [PTPGC].[TypeOfPriceItem_Id] = [TOPN1].[TypeOfPriceItem_Id]
    LEFT JOIN [PriceTypeNomenclatureGroupContragentEntities] AS [PTNGC] 
        ON [CatalogItems].[NomenclatureGroup_Id] = [PTNGC].[NomenclatureGroupItem_Id] AND 
           [SendItems].[Contragent_Id] = [PTNGC].[ContragentItem_Id]
    LEFT JOIN [TypeOfPricesNomenclatureItemEntities] AS [TOPN2] 
        ON [CatalogItems].[Id] = [TOPN2].[CatalogItem_Id] AND 
           [PTNGC].[TypeOfPriceItem_Id] = [TOPN2].[TypeOfPriceItem_Id]
    LEFT JOIN [DiscountsContragentEntities] AS [Discounts] 
        ON [CatalogItems].[Id] = [Discounts].[CatalogItem_Id] AND
           [SendItems].[Contragent_Id] = [Discounts].[ContragentItem_Id]
    WHERE [SendItems].[Contragent_Id] = @contragentId AND 
          [SendItems].[EntityName] = 2;
    
--  +----------------------------+
--  | Entity Names:              |
--  +----------------------------+
--  | BrandItemEntity = 1        |
--  | CatalogItemEntity = 2      |
--  | DirectoryEntity = 3        |
--  | PhotoItemEntity = 5        |
--  | ProductDirectionEntity = 6 |
--  +----------------------------+
    
END
GO


---------------------------------------------------------------------------------------------------------------------------------

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
            body.AppendLine("                    THEN TOPN1.Price + Discounts.Rate * TOPN1.Price");
            body.AppendLine("                    ELSE TOPN1.Price");
            body.AppendLine("                END");
            body.AppendLine("            ELSE");
            body.AppendLine("                CASE WHEN Discounts.Rate IS NOT NULL");
            body.AppendLine("                    THEN");
            body.AppendLine("                        CASE WHEN TOPN2.Price IS NOT NULL");
            body.AppendLine("                            THEN TOPN2.Price + Discounts.Rate * TOPN2.Price");
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
