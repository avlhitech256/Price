DECLARE @brandsTbl [dbo].[brandsTable]

insert into @brandsTbl ([Code], [Name])
VALUES ('265a2345-e510-11e4-ae6a-a6acd2529651', 'ООО "Автопласт"');

select * from @brandsTbl;

SELECT [BrandsItems].[Code] AS [Code],
       [BrandsItems].[Name] AS [Name],
       [BrandsItems].[LastUpdated] AS [LastUpdated],
       [BrandsItems].[ForceUpdated] AS [ForceUpdated]
FROM [BrandItemEntities] AS [BrandsItems]
WHERE [BrandsItems].[Code] IN (SELECT [Code] FROM @brandsTbl)

SELECT [BrandsItems].[Code] AS [Code],
       [BrandsItems].[Name] AS [Name]
FROM @brandsTbl AS [BrandsItems]
WHERE [BrandsItems].[Code] not IN (SELECT [Code] FROM [BrandItemEntities])


----------------------------------------------------------------------------


USE [ServerPriceList]
GO

DECLARE	@return_value int
DECLARE @brandsTbl [dbo].[brandsTable]
DECLARE @lastUpd [datetimeoffset](7) = Sysdatetimeoffset()

insert into @brandsTbl ([Code], [Name])
VALUES ('265a2345-e510-11e4-ae6a-a6acd2529651', 'ООО "Автопласт"')


EXEC	@return_value = [dbo].[UpdateBrands]
		@brands = @brandsTbl,
		@lastUpdate = @lastUpd

SELECT	'Return Value' = @return_value

GO
