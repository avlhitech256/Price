/*    ==Параметры сценариев==

    Версия исходного сервера : SQL Server 2016 (13.0.4001)
    Выпуск исходного ядра СУБД : Выпуск Microsoft SQL Server Standard Edition
    Тип исходного ядра СУБД : Изолированный SQL Server

    Версия целевого сервера : SQL Server 2017
    Выпуск целевого ядра СУБД : Выпуск Microsoft SQL Server Standard Edition
    Тип целевого ядра СУБД : Изолированный SQL Server
*/

USE [ServerPriceList]
GO
/****** Object:  StoredProcedure [dbo].[PrepareToUpdatePhotos]    Script Date: 12/25/2017 10:11:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PrepareToUpdatePhotos]
    @login [nvarchar](30),
    @lastUpdate [datetimeoffset](7),
	@ids bigintTable READONLY,
    @countToUpdate [int] OUT
AS
BEGIN
    -- +---------------------------------------------------+                                              
    -- | © 2017-2018 OLEXANDR LIKHOSHVA ALL RIGHT RESERVED |                                              
    -- | https://www.linkedin.com/in/olexandrlikhoshva/    |                                              
    -- +---------------------------------------------------+                                              
    -- SET NOCOUNT ON added to prevent extra result sets from                                             
    -- interfering with SELECT statements.                                                                
    
    --SET NOCOUNT ON;                                                                                       
    
    -- BrandItemEntity = 1                                                                                
    -- CatalogItemEntity = 2                                                                              
    -- DirectoryEntity = 3                                                                                
    -- PhotoItemEntity = 5                                                                                
    -- ProductDirectionEntity = 6                                                                         
    
    declare @entityName int = 5;                                                                          
    declare @dateOfCreation datetimeoffset(7) = Sysdatetimeoffset();                                      

    INSERT INTO [dbo].[SendItemsEntities]                                                                 
    ([Login], [EntityId], [Photos].[EntityName], [Photos].[RequestDate], [Photos].[DateOfCreation])       
    SELECT @login, [Id], @entityName, @lastUpdate, @dateOfCreation                                        
    FROM [dbo].[PhotoItemEntities] AS [Photos]   
    WHERE [Photos].[Id] IN(SELECT Id FROM @ids) AND 
	[Photos].[IsLoad] = 1 AND                                                       
    NOT EXISTS (SELECT *                                                                            
    FROM  [dbo].[SendItemsEntities] AS [SendItem]                                       
    WHERE [SendItem].[EntityId] = [Photos].[Id] AND                                     
    [SendItem].[Login] = @login AND                                               
    [SendItem].[EntityName] = @entityName);                                       
    
    INSERT INTO [dbo].[SendItemsEntities]                                                                 
    ([Login], [EntityId], [Photos].[EntityName], [Photos].[RequestDate], [Photos].[DateOfCreation])       
    SELECT @login, [Id], @entityName, @lastUpdate, @dateOfCreation                                        
    FROM [dbo].[PhotoItemEntities] AS [Photos]                                                             
    WHERE [Photos].[LastUpdated] > @lastUpdate AND                                                        
    NOT EXISTS (SELECT *                                                                            
    FROM  [dbo].[SendItemsEntities] AS [SendItem]                                       
    WHERE [SendItem].[EntityId] = [Photos].[Id] AND                                     
    [SendItem].[Login] = @login AND                                               
    [SendItem].[EntityName] = @entityName);                                       
    
    SELECT @countToUpdate = COUNT(*)                                                                      
    FROM  [dbo].[SendItemsEntities]                                                                       
    WHERE [Login] = @login AND                                                                            
    [EntityName] = @entityName;                                                                     
    
    RETURN (@countToUpdate);                                                                              
    
END