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

---------------------------------------------------------------- NEW VERSION -----------------------------------------------------

USE [ServerPriceList]
GO

/****** Object:  StoredProcedure [dbo].[PrepareToUpdatePhotos]    Script Date: 01.03.2018 0:35:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PrepareToUpdatePhotos]                                                         
    @login [nvarchar](30),                                                                             
    @lastUpdate [datetimeoffset](7),                                                                   
    @ids [dbo].[bigintTable] READONLY,                                                                 
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
    declare @contragentId bigint = -1;
    
    SELECT TOP (1) @contragentId = [Contragent].[Id] 
    FROM [ContragentItemEntities] AS [Contragent]
      WHERE ([Contragent].[Login] = @login);
                                                                                                       
   INSERT INTO[dbo].[SendItemsEntities]                                                                
   ([Contragent_Id], [EntityId], [Photos].[EntityName], [Photos].[RequestDate], [Photos].[DateOfCreation])     
   SELECT @contragentId, [Id], @entityName, @lastUpdate, @dateOfCreation                                      
   FROM [dbo].[PhotoItemEntities] AS [Photos]                                                          
   WHERE[Photos].[Id] IN(SELECT Id FROM @ids) AND                                                      
   [Photos].[IsLoad] = 1 AND                                                                           
   NOT EXISTS (SELECT *                                                                                
   FROM  [dbo].[SendItemsEntities] AS [SendItem]                                                       
   WHERE [SendItem].[EntityId] = [Photos].[Id] AND                                                     
   [SendItem].[Contragent_Id] = @contragentId AND                                                                     
   [SendItem].[EntityName] = @entityName);                                                             
                                                                                                       
   INSERT INTO [dbo].[SendItemsEntities]                                                               
   ([Contragent_Id], [EntityId], [Photos].[EntityName], [Photos].[RequestDate], [Photos].[DateOfCreation])     
   SELECT @contragentId, [Id], @entityName, @lastUpdate, @dateOfCreation                                      
   FROM [dbo].[PhotoItemEntities] AS [Photos]                                                          
   WHERE [Photos].[LastUpdated] > @lastUpdate AND                                                      
         NOT EXISTS (SELECT *                                                                          
                     FROM  [dbo].[SendItemsEntities] AS [SendItem]                                     
                     WHERE [SendItem].[EntityId] = [Photos].[Id] AND                                   
                           [SendItem].[Contragent_Id] = @contragentId AND                                             
                           [SendItem].[EntityName] = @entityName);                                     
                                                                                                       
   SELECT @countToUpdate = COUNT(*)                                                                    
   FROM  [dbo].[SendItemsEntities]                                                                     
   WHERE [Contragent_Id] = @contragentId AND                                                                          
         [EntityName] = @entityName;                                                                   
                                                                                                       
   RETURN (@countToUpdate);                                                                            
END                                                                                                    
GO


-----------------------------------------------------------------------------------------------------------------------------------


            body.AppendLine("CREATE PROCEDURE [dbo].[PrepareToUpdatePhotos]");
            body.AppendLine("    @login [nvarchar](30),");
            body.AppendLine("    @lastUpdate [datetimeoffset](7),");
            body.AppendLine("    @ids [dbo].[bigintTable] READONLY,");
            body.AppendLine("    @countToUpdate [int] OUT");
            body.AppendLine("AS");
            body.AppendLine("BEGIN");
            body.AppendLine("--   +----------------------------------------------------+");
            body.AppendLine("--   | Author:       OLEXANDR LIKHOSHVA                   |");
            body.AppendLine("--   | Create date:  2018.03.01 00:47                     |");
            body.AppendLine("--   | Copyright:    © 2017-2018  ALL RIGHT RESERVED      |");
            body.AppendLine("--   | https://www.linkedin.com/in/olexandrlikhoshva/     |");
            body.AppendLine("--   |----------------------------------------------------|");
            body.AppendLine("--   | Description:  The procedure for inserting entries  |");
            body.AppendLine("--   |               into the SendItemsEntities for a new |");
            body.AppendLine("--   |               or modified PhotoItemEntities        |");
            body.AppendLine("--   |               records which will be updated on the |");
            body.AppendLine("--   |               client side.                         |");
            body.AppendLine("--   +----------------------------------------------------+");
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
            body.AppendLine("    DECLARE @entityName int = 2;");
            body.AppendLine("    DECLARE @dateOfCreation datetimeoffset(7) = Sysdatetimeoffset();");
            body.AppendLine("    DECLARE @contragentId bigint = -1;");
            body.AppendLine(" ");
            body.AppendLine("    SELECT TOP (1) @contragentId = [Contragent].[Id]");
            body.AppendLine("    FROM [ContragentItemEntities] AS [Contragent]");
            body.AppendLine("    WHERE ([Contragent].[Login] = @login);");
            body.AppendLine(" ");
            body.AppendLine("    INSERT INTO[dbo].[SendItemsEntities]");
            body.AppendLine("        ([Contragent_Id], [EntityId], [Photos].[EntityName],");
            body.AppendLine("         [Photos].[RequestDate], [Photos].[DateOfCreation])");
            body.AppendLine("    SELECT @contragentId, [Id], @entityName, @lastUpdate, @dateOfCreation");
            body.AppendLine("    FROM [dbo].[PhotoItemEntities] AS [Photos]");
            body.AppendLine("    WHERE [Photos].[Id] IN(SELECT Id FROM @ids) AND");
            body.AppendLine("          [Photos].[IsLoad] = 1 AND");
            body.AppendLine("          NOT EXISTS (SELECT (1)");
            body.AppendLine("                      FROM  [dbo].[SendItemsEntities] AS [SendItem]");
            body.AppendLine("                      WHERE [SendItem].[EntityId] = [Photos].[Id] AND");
            body.AppendLine("                            [SendItem].[Contragent_Id] = @contragentId AND");
            body.AppendLine("                            [SendItem].[EntityName] = @entityName);");
            body.AppendLine(" ");
            body.AppendLine("    INSERT INTO [dbo].[SendItemsEntities]");
            body.AppendLine("    ([Contragent_Id], [EntityId], [Photos].[EntityName], [Photos].[RequestDate], [Photos].[DateOfCreation])");
            body.AppendLine("    SELECT @contragentId, [Id], @entityName, @lastUpdate, @dateOfCreation");
            body.AppendLine("    FROM [dbo].[PhotoItemEntities] AS [Photos]");
            body.AppendLine("    WHERE [Photos].[LastUpdated] > @lastUpdate AND");
            body.AppendLine("          NOT EXISTS (SELECT (1)");
            body.AppendLine("                      FROM  [dbo].[SendItemsEntities] AS [SendItem]");
            body.AppendLine("                      WHERE [SendItem].[EntityId] = [Photos].[Id] AND");
            body.AppendLine("                            [SendItem].[Contragent_Id] = @contragentId AND");
            body.AppendLine("                            [SendItem].[EntityName] = @entityName);");
            body.AppendLine(" ");
            body.AppendLine("   SELECT @countToUpdate = COUNT(*)");
            body.AppendLine("   FROM  [dbo].[SendItemsEntities]");
            body.AppendLine("   WHERE [Contragent_Id] = @contragentId AND");
            body.AppendLine("         [EntityName] = @entityName;");
            body.AppendLine(" ");
            body.AppendLine("   RETURN (@countToUpdate);");
            body.AppendLine(" ");
            body.AppendLine("END");                                                                                                    
            body.AppendLine(" ");
