-- OLD PROCEDURE


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

-------------------------------------------------------------------------------------------------------


USE [ServerPriceList]
GO

/****** Object:  StoredProcedure [dbo].[PrepareToUpdateBrands]    Script Date: 28.02.2018 23:03:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PrepareToUpdateBrands]
    @login [nvarchar](30),
    @lastUpdate [datetimeoffset](7),
    @countToUpdate [int] OUT
AS
BEGIN
--   +-----------------------------------------------------+
--   | Author:       OLEXANDR LIKHOSHVA                    |
--   | Create date:  2018.02.28 23:10                      |
--   | Copyright:    © 2017-2018  ALL RIGHT RESERVED       |
--   | https://www.linkedin.com/in/olexandrlikhoshva/      |
--   |-----------------------------------------------------|
--   | Description:  The procedure for inserting entries   |
--   |               into the SendItemsEntities for a new  |
--   |               or modified BrandItemEntities records |
--   |               which will be updated on the client   |
--   |               side.                                 |
--   +-----------------------------------------------------+

--  SET NOCOUNT ON added to prevent extra result sets from                                          
--  interfering with SELECT statements.                                                             
--  SET NOCOUNT ON;                                                                                  
    
--  +----------------------------+
--  | BrandItemEntity = 1        |
--  | CatalogItemEntity = 2      |
--  | DirectoryEntity = 3        |
--  | PhotoItemEntity = 5        |
--  | ProductDirectionEntity = 6 |
--  +----------------------------+
    
    DECLARE @entityName int = 1;                                                                       
    DECLARE @dateOfCreation datetimeoffset(7) = Sysdatetimeoffset();  
    DECLARE @contragentId bigint = -1;
    
    SELECT TOP (1) @contragentId = [Contragent].[Id] 
    FROM [ContragentItemEntities] AS [Contragent]
    WHERE ([Contragent].[Login] = @login);
    
    INSERT INTO [dbo].[SendItemsEntities]                                                              
        ([Contragent_Id], [EntityId], [Brands].[EntityName], 
         [Brands].[RequestDate], [Brands].[DateOfCreation])    
    SELECT @contragentId, [Id], @entityName, @lastUpdate, @dateOfCreation                                     
    FROM [dbo].[BrandItemEntities] AS [Brands]                                                         
    WHERE [Brands].[LastUpdated] > @lastUpdate AND                                                     
          NOT EXISTS (SELECT *                                                                         
                      FROM  [dbo].[SendItemsEntities] AS [SendItem]                                    
                      WHERE [SendItem].[EntityId] = [Brands].[Id] AND                                  
          [SendItem].[Contragent_Id] = @contragentId AND                                            
          [SendItem].[EntityName] = @entityName);                                    
    
    SELECT @countToUpdate = COUNT(*)                                                                   
    FROM  [dbo].[SendItemsEntities]                                                                    
    WHERE [Contragent_Id] = @contragentId AND                                                                         
    [EntityName] = @entityName;                                                                  
    
    RETURN (@countToUpdate);                                                                           
    
END
GO

-------------------------------------------------------------------------------------------------------

            body.AppendLine("--   +-----------------------------------------------------+");
            body.AppendLine("--   | Author:       OLEXANDR LIKHOSHVA                    |");
            body.AppendLine("--   | Create date:  2018.02.28 23:10                      |");
            body.AppendLine("--   | Copyright:    © 2017-2018  ALL RIGHT RESERVED       |");
            body.AppendLine("--   | https://www.linkedin.com/in/olexandrlikhoshva/      |");
            body.AppendLine("--   |-----------------------------------------------------|");
            body.AppendLine("--   | Description:  The procedure for inserting entries   |");
            body.AppendLine("--   |               into the SendItemsEntities for a new  |");
            body.AppendLine("--   |               or modified BrandItemEntities records |");
            body.AppendLine("--   |               which will be updated on the client   |");
            body.AppendLine("--   |               side.                                 |");
            body.AppendLine("--   +-----------------------------------------------------+");
            body.AppendLine(" ");
            body.AppendLine("--  SET NOCOUNT ON added to prevent extra result sets from");
            body.AppendLine("--  interfering with SELECT statements.");
            body.AppendLine("--  SET NOCOUNT ON;");
            body.AppendLine(" ");
            body.AppendLine("--  +----------------------------+");
            body.AppendLine("--  | BrandItemEntity = 1        |");
            body.AppendLine("--  | CatalogItemEntity = 2      |");
            body.AppendLine("--  | DirectoryEntity = 3        |");
            body.AppendLine("--  | PhotoItemEntity = 5        |");
            body.AppendLine("--  | ProductDirectionEntity = 6 |");
            body.AppendLine("--  +----------------------------+");
            body.AppendLine("  ");
            body.AppendLine("    DECLARE @entityName int = 1;");
            body.AppendLine("    DECLARE @dateOfCreation datetimeoffset(7) = Sysdatetimeoffset();");
            body.AppendLine("    DECLARE @contragentId bigint = -1;");
            body.AppendLine(" ");
            body.AppendLine("    SELECT TOP (1) @contragentId = [Contragent].[Id]");
            body.AppendLine("    FROM [ContragentItemEntities] AS [Contragent]");
            body.AppendLine("    WHERE ([Contragent].[Login] = @login);");
            body.AppendLine(" ");
            body.AppendLine("    INSERT INTO [dbo].[SendItemsEntities]");
            body.AppendLine("        ([Contragent_Id], [EntityId], [Brands].[EntityName],");
            body.AppendLine("         [Brands].[RequestDate], [Brands].[DateOfCreation])");
            body.AppendLine("    SELECT @contragentId, [Id], @entityName, @lastUpdate, @dateOfCreation");
            body.AppendLine("    FROM [dbo].[BrandItemEntities] AS [Brands]");
            body.AppendLine("    WHERE [Brands].[LastUpdated] > @lastUpdate AND");
            body.AppendLine("          NOT EXISTS (SELECT (1)");
            body.AppendLine("                      FROM  [dbo].[SendItemsEntities] AS [SendItem]");
            body.AppendLine("                      WHERE [SendItem].[EntityId] = [Brands].[Id] AND");
            body.AppendLine("                            [SendItem].[Contragent_Id] = @contragentId AND");
            body.AppendLine("                            [SendItem].[EntityName] = @entityName);");
            body.AppendLine(" ");
            body.AppendLine("    SELECT @countToUpdate = COUNT(*)");
            body.AppendLine("    FROM  [dbo].[SendItemsEntities]");                                                                    
            body.AppendLine("    WHERE [Contragent_Id] = @contragentId AND");
            body.AppendLine("          [EntityName] = @entityName;");
            body.AppendLine(" ");
            body.AppendLine("    RETURN (@countToUpdate);");
            body.AppendLine(" ");