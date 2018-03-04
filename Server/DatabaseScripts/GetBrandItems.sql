USE [ServerPriceList]
GO

/****** Object:  StoredProcedure [dbo].[GetBrandItems]    Script Date: 01.03.2018 1:30:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[GetBrandItems] 
    @login [nvarchar](30),
    @countToUpdate [int]
AS
BEGIN
--  +-----------------------------------------------------+
--  | Author:       OLEXANDR LIKHOSHVA                    |
--  | Create date:  2018.02.26 01:27                      |
--  | Copyright:    © 2017-2018  ALL RIGHT RESERVED       |
--  | https://www.linkedin.com/in/olexandrlikhoshva/      |
--  |-----------------------------------------------------|
--  | Description:  Procedure for getting Brand items     |
--  +-----------------------------------------------------+
--  SET NOCOUNT ON added to prevent extra result sets from
--  interfering with SELECT statements.
--  SET NOCOUNT ON;
    
    
    DECLARE @contragentId bigint = -1;
    
    
    SELECT TOP (1) @contragentId = [Contragent].[Id] 
    FROM [ContragentItemEntities] AS [Contragent]
    WHERE ([Contragent].[Login] = @login);
    
    SELECT TOP (@countToUpdate) 
           [Brand].[Id] AS [Id], 
           [Brand].[Code] AS [Code], 
           [Brand].[Name] AS [Name], 
           [Brand].[DateOfCreation] AS [DateOfCreation], 
           [Brand].[LastUpdated] AS [LastUpdated],
           [Brand].[ForceUpdated] AS [ForceUpdated]
    FROM [SendItemsEntities] AS [SendItems]
    INNER JOIN [BrandItemEntities] AS [Brand] ON [SendItems].[EntityId] = [Brand].[Id]
    WHERE [SendItems].[Contragent_Id] = @contragentId AND 
          [SendItems].[EntityName] = 1;

--  +----------------------------+
--  | Entity Name:               |
--  +----------------------------+
--  | BrandItemEntity = 1        |
--  | CatalogItemEntity = 2      |
--  | DirectoryEntity = 3        |
--  | PhotoItemEntity = 5        |
--  | ProductDirectionEntity = 6 |
--  +----------------------------+

END
GO

-------------------------------------------------------------------------------------------------------------


            body.AppendLine("--  +-----------------------------------------------------+");
            body.AppendLine("--  | Author:       OLEXANDR LIKHOSHVA                    |");
            body.AppendLine("--  | Create date:  2018.02.26 01:27                      |");
            body.AppendLine("--  | Copyright:    © 2017-2018  ALL RIGHT RESERVED       |");
            body.AppendLine("--  | https://www.linkedin.com/in/olexandrlikhoshva/      |");
            body.AppendLine("--  |-----------------------------------------------------|");
            body.AppendLine("--  | Description:  Procedure for getting Brand items     |");
            body.AppendLine("--  +-----------------------------------------------------+");
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
            body.AppendLine("           [Brand].[Id] AS [Id],");
            body.AppendLine("           [Brand].[Code] AS [Code],");
            body.AppendLine("           [Brand].[Name] AS [Name],");
            body.AppendLine("           [Brand].[DateOfCreation] AS [DateOfCreation],");
            body.AppendLine("           [Brand].[LastUpdated] AS [LastUpdated],");
            body.AppendLine("           [Brand].[ForceUpdated] AS [ForceUpdated]");
            body.AppendLine("    FROM [SendItemsEntities] AS [SendItems]");
            body.AppendLine("    INNER JOIN [BrandItemEntities] AS [Brand] ON [SendItems].[EntityId] = [Brand].[Id]");
            body.AppendLine("    WHERE [SendItems].[Contragent_Id] = @contragentId AND");
            body.AppendLine("          [SendItems].[EntityName] = 1;");
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
