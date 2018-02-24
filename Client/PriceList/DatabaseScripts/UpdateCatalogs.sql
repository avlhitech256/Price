USE [PriceList]
GO
/****** Object:  StoredProcedure [dbo].[UpdateCatalogs]    Script Date: 20.02.2018 22:20:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[UpdateCatalogs]
	-- Add the parameters for the stored procedure here
	@catalogs [dbo].[catalogsTable] READONLY,
	@linkToPhotos [dbo].[linkToPhotoTable] READONLY 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	-- SET NOCOUNT ON;

	WITH [CatalogForUpdate] AS (SELECT * FROM [CatalogItemEntities] AS [CatalogItems]
	                            WHERE [CatalogItems].[Id] IN (SELECT Id FROM @catalogs))
	UPDATE [ItemsForUpdate] 
	SET [ItemsForUpdate].[UID] = [UpdateItems].[UID],
	    [ItemsForUpdate].[Code] = [UpdateItems].[Code],
		[ItemsForUpdate].[Article] = [UpdateItems].[Article],
		[ItemsForUpdate].[Name] = [UpdateItems].[Name],
		[ItemsForUpdate].[Brand_Id] = [BrandItems].[Id],
		[ItemsForUpdate].[BrandName] = [BrandItems].[Name],
		[ItemsForUpdate].[Unit] = [UpdateItems].[Unit],
		[ItemsForUpdate].[EnterpriceNormPack] = [UpdateItems].[EnterpriceNormPack],
		[ItemsForUpdate].[BatchOfSales] = [UpdateItems].[BatchOfSales],
		[ItemsForUpdate].[Balance] = [UpdateItems].[Balance],
		[ItemsForUpdate].[Price] = [UpdateItems].[Price],
		[ItemsForUpdate].[Currency] = [UpdateItems].[Currency],
		[ItemsForUpdate].[Multiplicity] = [UpdateItems].[Multiplicity],
		[ItemsForUpdate].[HasPhotos] = [UpdateItems].[HasPhotos],
		[ItemsForUpdate].[DateOfCreation] = [UpdateItems].[DateOfCreation],
		[ItemsForUpdate].[LastUpdated] = [UpdateItems].[LastUpdated],
		[ItemsForUpdate].[ForceUpdated] = [UpdateItems].[ForceUpdated],
		[ItemsForUpdate].[Status] = [UpdateItems].[Status],
		[ItemsForUpdate].[LastUpdatedStatus] = [UpdateItems].[LastUpdatedStatus],
		[ItemsForUpdate].[Directory_Id] = [Directories].[Id]
	FROM [CatalogForUpdate] AS [ItemsForUpdate]
	INNER JOIN @catalogs AS [UpdateItems] ON [ItemsForUpdate].[Id] = [UpdateItems].[Id]
	LEFT JOIN [BrandItemEntities] AS [BrandItems] ON [UpdateItems].[Brand_Id] = [BrandItems].[Id]
	LEFT JOIN [DirectoryEntities] AS [Directories] ON [UpdateItems].[Directory_Id] = [Directories].[Id]

	INSERT INTO  [CatalogItemEntities] 
	([Id], [UID], [Code], [Article], [Name], [Brand_Id], [BrandName], [Unit], [EnterpriceNormPack],
	 [BatchOfSales], [Balance], [Price], [Currency], [Multiplicity], [HasPhotos], [DateOfCreation], 
	 [LastUpdated], [ForceUpdated], [Status], [LastUpdatedStatus], [Directory_Id])
	SELECT [UpdateItems].[Id], [UpdateItems].[UID], [UpdateItems].[Code], [UpdateItems].[Article], 
	       [UpdateItems].[Name], [BrandItems].[Id], [BrandItems].[Name], [UpdateItems].[Unit], 
		   [UpdateItems].[EnterpriceNormPack], [UpdateItems].[BatchOfSales], [UpdateItems].[Balance], 
		   [UpdateItems].[Price], [UpdateItems].[Currency], [UpdateItems].[Multiplicity], 
		   [UpdateItems].[HasPhotos], [UpdateItems].[DateOfCreation], [UpdateItems].[LastUpdated], 
		   [UpdateItems].[ForceUpdated], [UpdateItems].[Status], [UpdateItems].[LastUpdatedStatus], 
		   [Directories].[Id]
	FROM @catalogs AS [UpdateItems]
	LEFT JOIN [BrandItemEntities] AS [BrandItems] ON [UpdateItems].[Brand_Id] = [BrandItems].[Id]
	LEFT JOIN [DirectoryEntities] AS [Directories] ON [UpdateItems].[Directory_Id] = [Directories].[Id]
	WHERE [UpdateItems].[Id] NOT IN (SELECT [CatalogItems].[Id] FROM [CatalogItemEntities] AS [CatalogItems])

	UPDATE [PhotoItems]
	SET [PhotoItems].[CatalogItem_Id] = [Link].[Catalog_Id]
	FROM [PhotoItemEntities] AS [PhotoItems]
	INNER JOIN @linkToPhotos AS [Link] ON [PhotoItems].[Id] = [Link].[Photo_Id]

	SELECT [Id] FROM @catalogs;

END
