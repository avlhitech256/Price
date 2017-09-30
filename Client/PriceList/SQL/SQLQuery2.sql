SELECT 
    [Project6].[Status] AS [Status], 
    [Project6].[Id] AS [Id], 
    [Project6].[UID] AS [UID], 
    [Project6].[Code] AS [Code], 
    [Project6].[Article] AS [Article], 
    [Project6].[Name] AS [Name], 
    [Project6].[BrandName] AS [BrandName], 
    [Project6].[Unit] AS [Unit], 
    [Project6].[EnterpriceNormPack] AS [EnterpriceNormPack], 
    [Project6].[BatchOfSales] AS [BatchOfSales], 
    [Project6].[Balance] AS [Balance], 
    [Project6].[Price] AS [Price], 
    [Project6].[Currency] AS [Currency], 
    [Project6].[Multiplicity] AS [Multiplicity], 
    [Project6].[HasPhotos] AS [HasPhotos], 
    [Project6].[DateOfCreation] AS [DateOfCreation], 
    [Project6].[LastUpdated] AS [LastUpdated], 
    [Project6].[LastUpdatedStatus] AS [LastUpdatedStatus], 
    [Project6].[Brand_Id] AS [Brand_Id], 
    [Project6].[Directory_Id] AS [Directory_Id], 
    [Project6].[NomenclatureGroup_Id] AS [NomenclatureGroup_Id], 
    [Project6].[C1] AS [C1], 
    [Project6].[Id1] AS [Id1], 
    [Project6].[Count] AS [Count], 
    [Project6].[DateAction] AS [DateAction], 
    [Project6].[CatalogItem_Id] AS [CatalogItem_Id], 
    [Project6].[Order_Id] AS [Order_Id]
    FROM ( SELECT 
        [Limit1].[Id] AS [Id], 
        [Limit1].[UID] AS [UID], 
        [Limit1].[Code] AS [Code], 
        [Limit1].[Article] AS [Article], 
        [Limit1].[Name] AS [Name], 
        [Limit1].[BrandName] AS [BrandName], 
        [Limit1].[Unit] AS [Unit], 
        [Limit1].[EnterpriceNormPack] AS [EnterpriceNormPack], 
        [Limit1].[BatchOfSales] AS [BatchOfSales], 
        [Limit1].[Balance] AS [Balance], 
        [Limit1].[Price] AS [Price], 
        [Limit1].[Currency] AS [Currency], 
        [Limit1].[Multiplicity] AS [Multiplicity], 
        [Limit1].[HasPhotos] AS [HasPhotos], 
        [Limit1].[DateOfCreation] AS [DateOfCreation], 
        [Limit1].[LastUpdated] AS [LastUpdated], 
        [Limit1].[Status] AS [Status], 
        [Limit1].[LastUpdatedStatus] AS [LastUpdatedStatus], 
        [Limit1].[Brand_Id] AS [Brand_Id], 
        [Limit1].[Directory_Id] AS [Directory_Id], 
        [Limit1].[NomenclatureGroup_Id] AS [NomenclatureGroup_Id], 
        [Extent2].[Id] AS [Id1], 
        [Extent2].[Count] AS [Count], 
        [Extent2].[DateAction] AS [DateAction], 
        [Extent2].[CatalogItem_Id] AS [CatalogItem_Id], 
        [Extent2].[Order_Id] AS [Order_Id], 
        CASE WHEN ([Extent2].[Id] IS NULL) THEN CAST(NULL AS int) ELSE 1 END AS [C1]
        FROM   (SELECT [Project5].[Id] AS [Id], [Project5].[UID] AS [UID], [Project5].[Code] AS [Code], [Project5].[Article] AS [Article], [Project5].[Name] AS [Name], [Project5].[BrandName] AS [BrandName], [Project5].[Unit] AS [Unit], [Project5].[EnterpriceNormPack] AS [EnterpriceNormPack], [Project5].[BatchOfSales] AS [BatchOfSales], [Project5].[Balance] AS [Balance], [Project5].[Price] AS [Price], [Project5].[Currency] AS [Currency], [Project5].[Multiplicity] AS [Multiplicity], [Project5].[HasPhotos] AS [HasPhotos], [Project5].[DateOfCreation] AS [DateOfCreation], [Project5].[LastUpdated] AS [LastUpdated], [Project5].[Status] AS [Status], [Project5].[LastUpdatedStatus] AS [LastUpdatedStatus], [Project5].[Brand_Id] AS [Brand_Id], [Project5].[Directory_Id] AS [Directory_Id], [Project5].[NomenclatureGroup_Id] AS [NomenclatureGroup_Id]
            FROM ( SELECT 
                [Extent1].[Id] AS [Id], 
                [Extent1].[UID] AS [UID], 
                [Extent1].[Code] AS [Code], 
                [Extent1].[Article] AS [Article], 
                [Extent1].[Name] AS [Name], 
                [Extent1].[BrandName] AS [BrandName], 
                [Extent1].[Unit] AS [Unit], 
                [Extent1].[EnterpriceNormPack] AS [EnterpriceNormPack], 
                [Extent1].[BatchOfSales] AS [BatchOfSales], 
                [Extent1].[Balance] AS [Balance], 
                [Extent1].[Price] AS [Price], 
                [Extent1].[Currency] AS [Currency], 
                [Extent1].[Multiplicity] AS [Multiplicity], 
                [Extent1].[HasPhotos] AS [HasPhotos], 
                [Extent1].[DateOfCreation] AS [DateOfCreation], 
                [Extent1].[LastUpdated] AS [LastUpdated], 
                [Extent1].[Status] AS [Status], 
                [Extent1].[LastUpdatedStatus] AS [LastUpdatedStatus], 
                [Extent1].[Brand_Id] AS [Brand_Id], 
                [Extent1].[Directory_Id] AS [Directory_Id], 
                [Extent1].[NomenclatureGroup_Id] AS [NomenclatureGroup_Id]
                FROM [dbo].[CatalogItemEntities] AS [Extent1]
                WHERE ( NOT EXISTS (SELECT 
                    1 AS [C1]
                    FROM  ( SELECT 1 AS X ) AS [SingleRowTable1]
                    WHERE 1 = 0
                )) AND (( NOT EXISTS (SELECT 
                    1 AS [C1]
                    FROM  ( SELECT 1 AS X ) AS [SingleRowTable2]
                    WHERE 1 = 0
                )) OR ( NOT EXISTS (SELECT 
                    1 AS [C1]
                    FROM  ( SELECT 1 AS X ) AS [SingleRowTable3]
                    WHERE 1 = 0
                ))) AND ( NOT EXISTS (SELECT 
                    1 AS [C1]
                    FROM  ( SELECT 1 AS X ) AS [SingleRowTable4]
                    WHERE 1 = 0
                )) AND (((0 <> 1) AND (0 <> 1) AND (1 <> 1)) OR ((1 = [Extent1].[Status]) AND ([Extent1].[DateOfCreation] <= DATETIMEOFFSETFROMPARTS(2017,09,15,23,58,11,0,3,0,0))) OR ((3 = [Extent1].[Status]) AND ([Extent1].[LastUpdatedStatus] <= DATETIMEOFFSETFROMPARTS(2017,09,22,23,58,11,0,3,0,0))) OR ((2 = [Extent1].[Status]) AND ([Extent1].[LastUpdatedStatus] <= DATETIMEOFFSETFROMPARTS(2017,09,22,23,58,11,0,3,0,0)))) AND ((-1 <= -1) OR ([Extent1].[Brand_Id] = -1))
            )  AS [Project5]
            ORDER BY [Project5].[Name] ASC
            OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY  ) AS [Limit1]
        LEFT OUTER JOIN [dbo].[BasketItemEntities] AS [Extent2] ON [Limit1].[Id] = [Extent2].[CatalogItem_Id]
    )  AS [Project6]
    ORDER BY [Project6].[Name] ASC, [Project6].[Id] ASC, [Project6].[C1] ASC
