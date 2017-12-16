-- +---------------------------------------------------+                                           
-- | © 2017-2018 OLEXANDR LIKHOSHVA ALL RIGHT RESERVED |                                           
-- | https://www.linkedin.com/in/olexandrlikhoshva/    |                                           
-- +---------------------------------------------------+                                           
-- SET NOCOUNT ON added to prevent extra result sets from                                          
-- interfering with SELECT statements.                                                             
                                                                                                   
SET NOCOUNT ON;                                                                                    
                                                                                                   
-- BrandItemEntity = 1                                                                             
-- CatalogItemEntity = 2                                                                           
-- DirectoryEntity = 3                                                                             
-- PhotoItemEntity = 5                                                                             
-- ProductDirectionEntity = 6                                                                      
                                                                                                   
declare @entityName int = 1;                                                                       
declare @dateOfCreation datetimeoffset(7) = Sysdatetimeoffset();                                   
declare @countToUpdate bigint;                                                                     
                                                                                                   
INSERT INTO [dbo].[SendItemsEntities]                                                              
([Login], [EntityId], [Brands].[EntityName], [Brands].[RequestDate], [Brands].[DateOfCreation])    
SELECT @login, [Id], @entityName, @lastUpdate, @dateOfCreation                                     
FROM  [dbo].[BrandItemEntities] AS [Brands]                                                        
WHERE [Brands].[LastUpdated] > @lastUpdate AND                                                     
      NOT EXISTS (SELECT *                                                                         
                  FROM  [dbo].[SendItemsEntities] AS [SendItem]                                    
                  WHERE [SendItem].[EntityId] = [Brands].[Id] AND                                  
                        [SendItem].[Login] = @login AND                                            
                        [SendItem].[EntityName] = @entityName);                                    
                                                                                                   
SELECT @countToUpdate = (SELECT COUNT(*)                                                           
                         FROM  [dbo].[SendItemsEntities]                                           
                         WHERE [Login] = @login AND                                                
                                         [EntityName] = @entityName);                              
                                                                                                   
RETURN (@countToUpdate);                                                                           

