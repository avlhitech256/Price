SELECT TOP (50) CatalogItems.*,  TOPN1.Price, TOPN1.Currency, TypeOfPrice1.[Name], Contragent1.[Name]
FROM SendItemsEntities AS SendItems
INNER JOIN CatalogItemEntities AS CatalogItems ON CatalogItems.Id = SendItems.EntityId
LEFT JOIN TypeOfPricesNomenclatureItemEntities AS TOPN1 ON CatalogItems.Id = TOPN1.CatalogItem_Id
LEFT JOIN TypeOfPriceItemEntities AS TypeOfPrice1 ON TOPN1.TypeOfPriceItem_Id = TypeOfPrice1.Id
LEFT JOIN PriceTypePriceGroupContragentEntities AS PTPGC ON TypeOfPrice1.Id = PTPGC.TypeOfPriceItem_Id
LEFT JOIN ContragentItemEntities AS Contragent1 ON PTPGC.ContragentItem_Id = Contragent1.Id
WHERE SendItems.[Login] = 'k6731' AND 
      --Contragent1.[Login] = 'k6731' AND
      SendItems.EntityName = 2;


SELECT TOP (50) CatalogItems.*, TOPN1.Price, TOPN1.Currency, Contragent1.[Name], TOPN2.Price, TOPN2.Currency
FROM SendItemsEntities AS SendItems
INNER JOIN CatalogItemEntities AS CatalogItems ON CatalogItems.Id = SendItems.EntityId
LEFT JOIN TypeOfPricesNomenclatureItemEntities AS TOPN1 ON CatalogItems.Id = TOPN1.CatalogItem_Id
LEFT JOIN PriceTypePriceGroupContragentEntities AS PTPGC ON TOPN1.TypeOfPriceItem_Id = PTPGC.TypeOfPriceItem_Id
LEFT JOIN ContragentItemEntities AS Contragent1 ON PTPGC.ContragentItem_Id = Contragent1.Id
LEFT JOIN TypeOfPricesNomenclatureItemEntities AS TOPN2 ON CatalogItems.Id = TOPN2.CatalogItem_Id
LEFT JOIN PriceTypeNomenclatureGroupContragentEntities AS PTNGC ON TOPN2.TypeOfPriceItem_Id = PTNGC.TypeOfPriceItem_Id
LEFT JOIN ContragentItemEntities AS Contragent2 ON PTNGC.ContragentItem_Id = Contragent2.Id
WHERE SendItems.[Login] = 'k6731' AND 
      Contragent1.[Login] = SendItems.[Login] AND
      Contragent2.[Login] = SendItems.[Login] AND
      SendItems.EntityName = 2;


SELECT CatalogItems.*, TOPN1.Price, TOPN1.Currency, Contragent1.[Name]
FROM SendItemsEntities AS SendItems
INNER JOIN CatalogItemEntities AS CatalogItems ON SendItems.EntityId = CatalogItems.Id 
LEFT JOIN PriceTypePriceGroupContragentEntities AS PTPGC ON CatalogItems.PriceGroup_Id = PTPGC.PriceGroupItem_Id
LEFT JOIN ContragentItemEntities AS Contragent1 ON Contragent1.Id = PTPGC.ContragentItem_Id
LEFT JOIN TypeOfPricesNomenclatureItemEntities AS TOPN1 ON PTPGC.TypeOfPriceItem_Id = TOPN1.TypeOfPriceItem_Id
WHERE SendItems.[Login] = 'k5517' AND 
      Contragent1.[Login] = SendItems.[Login] AND
      SendItems.EntityName = 2 AND
	  CatalogItems.Id = TOPN1.CatalogItem_Id AND
	  TOPN1.Price IS NULL
ORDER BY SendItems.EntityId;




SELECT CatalogItems.*, TOPN1.Price, TOPN1.Currency, Contragent.[Name]
FROM SendItemsEntities AS SendItems
INNER JOIN CatalogItemEntities AS CatalogItems ON SendItems.EntityId = CatalogItems.Id 
LEFT JOIN ContragentItemEntities AS Contragent ON SendItems.[Login] = Contragent.[Login]
LEFT JOIN PriceTypePriceGroupContragentEntities AS PTPGC ON CatalogItems.PriceGroup_Id = PTPGC.PriceGroupItem_Id AND 
                                                            Contragent.Id = PTPGC.ContragentItem_Id
LEFT JOIN TypeOfPricesNomenclatureItemEntities AS TOPN1 ON CatalogItems.Id = TOPN1.CatalogItem_Id AND 
                                                           PTPGC.TypeOfPriceItem_Id = TOPN1.TypeOfPriceItem_Id
WHERE SendItems.[Login] = 'k5517' AND 
      SendItems.EntityName = 2;




SELECT CatalogItems.*, TOPN1.Price, TOPN1.Currency, TOPN2.Price, TOPN2.Currency, Contragent.[Name]
FROM SendItemsEntities AS SendItems
INNER JOIN CatalogItemEntities AS CatalogItems ON SendItems.EntityId = CatalogItems.Id 
LEFT JOIN ContragentItemEntities AS Contragent ON SendItems.[Login] = Contragent.[Login]
LEFT JOIN PriceTypePriceGroupContragentEntities AS PTPGC ON CatalogItems.PriceGroup_Id = PTPGC.PriceGroupItem_Id AND 
                                                            Contragent.Id = PTPGC.ContragentItem_Id
LEFT JOIN TypeOfPricesNomenclatureItemEntities AS TOPN1 ON (CatalogItems.Id = TOPN1.CatalogItem_Id) AND 
                                                           (PTPGC.TypeOfPriceItem_Id = TOPN1.TypeOfPriceItem_Id)
LEFT JOIN PriceTypeNomenclatureGroupContragentEntities AS PTNGC ON CatalogItems.NomenclatureGroup_Id = PTNGC.NomenclatureGroupItem_Id AND 
                                                                   Contragent.Id = PTNGC.ContragentItem_Id
LEFT JOIN TypeOfPricesNomenclatureItemEntities AS TOPN2 ON (CatalogItems.Id = TOPN2.CatalogItem_Id) AND 
                                                           (PTNGC.TypeOfPriceItem_Id = TOPN2.TypeOfPriceItem_Id)
WHERE SendItems.[Login] = 'k5517' AND 
      SendItems.EntityName = 2;



SELECT CatalogItems.*, TOPN1.Price, TOPN1.Currency, TOPN2.Price, TOPN2.Currency, Discounts.Rate, Contragent.[Name]
FROM SendItemsEntities AS SendItems
INNER JOIN CatalogItemEntities AS CatalogItems ON SendItems.EntityId = CatalogItems.Id 
LEFT JOIN ContragentItemEntities AS Contragent ON SendItems.[Login] = Contragent.[Login]
LEFT JOIN PriceTypePriceGroupContragentEntities AS PTPGC ON CatalogItems.PriceGroup_Id = PTPGC.PriceGroupItem_Id AND 
                                                            Contragent.Id = PTPGC.ContragentItem_Id
LEFT JOIN TypeOfPricesNomenclatureItemEntities AS TOPN1 ON CatalogItems.Id = TOPN1.CatalogItem_Id AND 
                                                           PTPGC.TypeOfPriceItem_Id = TOPN1.TypeOfPriceItem_Id
LEFT JOIN PriceTypeNomenclatureGroupContragentEntities AS PTNGC ON CatalogItems.NomenclatureGroup_Id = PTNGC.NomenclatureGroupItem_Id AND 
                                                                   Contragent.Id = PTNGC.ContragentItem_Id
LEFT JOIN TypeOfPricesNomenclatureItemEntities AS TOPN2 ON CatalogItems.Id = TOPN2.CatalogItem_Id AND 
                                                           PTNGC.TypeOfPriceItem_Id = TOPN2.TypeOfPriceItem_Id
LEFT JOIN DiscountsContragentEntities AS Discounts ON CatalogItems.Id = Discounts.CatalogItem_Id AND
                                                      Contragent.Id = Discounts.ContragentItem_Id
WHERE SendItems.[Login] = 'k6731' AND 
      SendItems.EntityName = 2;





SELECT CatalogItems.*, TOPN1.Price, TOPN1.Currency, TOPN2.Price, TOPN2.Currency, Discounts.Rate, Contragent.[Name]
FROM SendItemsEntities AS SendItems
INNER JOIN CatalogItemEntities AS CatalogItems ON SendItems.EntityId = CatalogItems.Id 
LEFT JOIN ContragentItemEntities AS Contragent ON SendItems.[Login] = Contragent.[Login]
LEFT JOIN PriceTypePriceGroupContragentEntities AS PTPGC ON CatalogItems.PriceGroup_Id = PTPGC.PriceGroupItem_Id AND 
                                                            Contragent.Id = PTPGC.ContragentItem_Id
LEFT JOIN TypeOfPricesNomenclatureItemEntities AS TOPN1 ON CatalogItems.Id = TOPN1.CatalogItem_Id AND 
                                                           PTPGC.TypeOfPriceItem_Id = TOPN1.TypeOfPriceItem_Id
LEFT JOIN PriceTypeNomenclatureGroupContragentEntities AS PTNGC ON CatalogItems.NomenclatureGroup_Id = PTNGC.NomenclatureGroupItem_Id AND 
                                                                   Contragent.Id = PTNGC.ContragentItem_Id
LEFT JOIN TypeOfPricesNomenclatureItemEntities AS TOPN2 ON CatalogItems.Id = TOPN2.CatalogItem_Id AND 
                                                           PTNGC.TypeOfPriceItem_Id = TOPN2.TypeOfPriceItem_Id
LEFT JOIN DiscountsContragentEntities AS Discounts ON CatalogItems.Id = Discounts.CatalogItem_Id AND
                                                      Contragent.Id = Discounts.ContragentItem_Id
WHERE SendItems.[Login] = 'k6731' AND 
      SendItems.EntityName = 2 AND
	  Discounts.Rate IS NOT NULL;



using(PhoneContext db = new PhoneContext())
{
    System.Data.SqlClient.SqlParameter param = new System.Data.SqlClient.SqlParameter("@name", "Samsung");
    var phones = db.Database.SqlQuery<Phone>("GetPhonesByCompany @name",param);
    foreach (var p in phones)
        Console.WriteLine("{0} - {1}", p.Name, p.Price);
}




SELECT 1 AS [IsAuthorized], 
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
		       THEN TOPN2.Price + Discounts.Rate * TOPN2.Price
			   ELSE TOPN2.Price
		    END
       END AS [Price],
	   CASE WHEN TOPN1.Price IS NOT NULL 
	      THEN TOPN1.Currency
		  ELSE TOPN2.Currency
       END AS [Currency],
       [CatalogItems].[Multiplicity] AS [Multiplicity],
       [CatalogItems].[HasPhotos] AS [HasPhotos],
	   (SELECT [Photo].[Id] FROM [PhotoItemEntities] AS [Photo] 
	    WHERE [Photo].[CatalogItem_Id] = [CatalogItems].[Id]) AS [Photos],
       [CatalogItems].[DateOfCreation] AS [DateOfCreation],
       [CatalogItems].[LastUpdated] AS [LastUpdated],
       [CatalogItems].[ForceUpdated] AS [ForceUpdated],
       [CatalogItems].[Status] AS [Status],
	   [CatalogItems].[Directory_Id] AS [DirectoryId]
FROM SendItemsEntities AS SendItems
INNER JOIN CatalogItemEntities AS CatalogItems ON SendItems.EntityId = CatalogItems.Id 
LEFT JOIN ContragentItemEntities AS Contragent ON SendItems.[Login] = Contragent.[Login]
LEFT JOIN PriceTypePriceGroupContragentEntities AS PTPGC ON CatalogItems.PriceGroup_Id = PTPGC.PriceGroupItem_Id AND 
                                                            Contragent.Id = PTPGC.ContragentItem_Id
LEFT JOIN TypeOfPricesNomenclatureItemEntities AS TOPN1 ON CatalogItems.Id = TOPN1.CatalogItem_Id AND 
                                                           PTPGC.TypeOfPriceItem_Id = TOPN1.TypeOfPriceItem_Id
LEFT JOIN PriceTypeNomenclatureGroupContragentEntities AS PTNGC ON CatalogItems.NomenclatureGroup_Id = PTNGC.NomenclatureGroupItem_Id AND 
                                                                   Contragent.Id = PTNGC.ContragentItem_Id
LEFT JOIN TypeOfPricesNomenclatureItemEntities AS TOPN2 ON CatalogItems.Id = TOPN2.CatalogItem_Id AND 
                                                           PTNGC.TypeOfPriceItem_Id = TOPN2.TypeOfPriceItem_Id
LEFT JOIN DiscountsContragentEntities AS Discounts ON CatalogItems.Id = Discounts.CatalogItem_Id AND
                                                      Contragent.Id = Discounts.ContragentItem_Id
WHERE SendItems.[Login] = 'k5517' AND 
      SendItems.EntityName = 2;

	  
	  
	  
USE [ServerPriceList]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCatalogItems] 
	@login [nvarchar](30)
AS
BEGIN
    -- +---------------------------------------------------+                                           
    -- | © 2017-2018 OLEXANDR LIKHOSHVA ALL RIGHT RESERVED |                                           
    -- | https://www.linkedin.com/in/olexandrlikhoshva/    |                                           
    -- +---------------------------------------------------+                                           
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	-- SET NOCOUNT ON;

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
		           THEN TOPN2.Price + Discounts.Rate * TOPN2.Price
			       ELSE TOPN2.Price
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
    INNER JOIN [CatalogItemEntities] AS [CatalogItems] ON [SendItems].[EntityId] = [CatalogItems].[Id] 
    LEFT JOIN [ContragentItemEntities] AS [Contragent] ON [SendItems].[Login] = [Contragent].[Login]
    LEFT JOIN [PriceTypePriceGroupContragentEntities] AS [PTPGC] ON [CatalogItems].[PriceGroup_Id] = [PTPGC].[PriceGroupItem_Id] AND 
                                                                    [Contragent].[Id] = [PTPGC].[ContragentItem_Id]
    LEFT JOIN [TypeOfPricesNomenclatureItemEntities] AS [TOPN1] ON [CatalogItems].[Id] = [TOPN1].[CatalogItem_Id] AND 
                                                                   [PTPGC].[TypeOfPriceItem_Id] = [TOPN1].[TypeOfPriceItem_Id]
    LEFT JOIN [PriceTypeNomenclatureGroupContragentEntities] AS [PTNGC] ON [CatalogItems].[NomenclatureGroup_Id] = [PTNGC].[NomenclatureGroupItem_Id] AND 
                                                                           [Contragent].[Id] = [PTNGC].[ContragentItem_Id]
    LEFT JOIN [TypeOfPricesNomenclatureItemEntities] AS [TOPN2] ON [CatalogItems].[Id] = [TOPN2].[CatalogItem_Id] AND 
                                                                   [PTNGC].[TypeOfPriceItem_Id] = [TOPN2].[TypeOfPriceItem_Id]
    LEFT JOIN [DiscountsContragentEntities] AS [Discounts] ON [CatalogItems].[Id] = [Discounts].[CatalogItem_Id] AND
                                                              [Contragent].[Id] = [Discounts].[ContragentItem_Id]
    WHERE [SendItems].[Login] = @login AND 
          [SendItems].[EntityName] = 2;
    
	-- Entity Names:
	-- BrandItemEntity = 1                                                                             
    -- CatalogItemEntity = 2                                                                           
    -- DirectoryEntity = 3                                                                             
    -- PhotoItemEntity = 5                                                                             
    -- ProductDirectionEntity = 6                                                                      

END
GO


USE [ServerPriceList]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<OLEXANDR LIKHOSHVA>
-- Create date: <2018.02.11 23:45>
-- Description:	<Procedure for getting Catalog items>
-- =============================================
CREATE PROCEDURE [dbo].[GetPhotosIds]
	@ids [dbo].[bigintTable] READONLY
AS
BEGIN
    -- +---------------------------------------------------+                                           
    -- | © 2017-2018 OLEXANDR LIKHOSHVA ALL RIGHT RESERVED |                                           
    -- | https://www.linkedin.com/in/olexandrlikhoshva/    |                                           
    -- +---------------------------------------------------+                                           
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	-- SET NOCOUNT ON;
	
	SELECT [Photos].[CatalogItem_Id] AS [CatalogId], [Photos].[Id] AS [PhotoId]                                      
    FROM [dbo].[PhotoItemEntities] AS [Photos]                                                          
    WHERE[Photos].[CatalogItem_Id] IN(SELECT Id FROM @ids)
	
END
GO
	  