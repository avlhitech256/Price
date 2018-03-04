USE [ServerPriceList]
GO

/****** Object:  StoredProcedure [dbo].[GetDirectoryItems]    Script Date: 01.03.2018 1:41:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[GetDirectoryItems] 
    @login [nvarchar](30),
    @countToUpdate [int]
AS
BEGIN
--  +-----------------------------------------------------+
--  | Author:       OLEXANDR LIKHOSHVA                    |
--  | Create date:  2018.02.24 17:21                      |
--  | Copyright:    © 2017-2018  ALL RIGHT RESERVED       |
--  | https://www.linkedin.com/in/olexandrlikhoshva/      |
--  |-----------------------------------------------------|
--  | Description:  Procedure for getting Directory items |
--  +-----------------------------------------------------+
--  SET NOCOUNT ON added to prevent extra result sets from
--  interfering with SELECT statements.
--  SET NOCOUNT ON;


    DECLARE @contragentId bigint = -1;
    
    
    SELECT TOP (1) @contragentId = [Contragent].[Id] 
    FROM [ContragentItemEntities] AS [Contragent]
    WHERE ([Contragent].[Login] = @login);
    
    WITH [DirectoryList] 
        ([DirectoryLevel], [Id], [Code], [Name], [Parent_Id], 
         [DateOfCreation], [LastUpdated], [ForceUpdated]) AS
        (SELECT 0 AS [DirectoryLevel],
                [Directory].[Id], [Directory].[Code], [Directory].[Name], 
                [Directory].[Parent_Id], [Directory].[DateOfCreation], 
                [Directory].[LastUpdated], [Directory].[ForceUpdated]
         FROM [DirectoryEntities] AS [Directory]
         WHERE [Directory].[Parent_Id] IS NULL
         UNION ALL
         SELECT [ParentDirectory].[DirectoryLevel] + 1,
                [DependencyDirectory].[Id], [DependencyDirectory].[Code], [DependencyDirectory].[Name], 
                [DependencyDirectory].[Parent_Id], [DependencyDirectory].[DateOfCreation], 
                [DependencyDirectory].[LastUpdated], [DependencyDirectory].[ForceUpdated]
         FROM [DirectoryEntities] AS [DependencyDirectory]
         INNER JOIN [DirectoryList] AS [ParentDirectory] 
             ON [DependencyDirectory].[Parent_Id] = [ParentDirectory].[Id])
    SELECT TOP (@countToUpdate) 
           [Dir].[Id] AS [Id], [Dir].[Code] AS [Code], [Dir].[Name] AS [Name], 
           [Dir].[Parent_Id] AS [ParentId], [Dir].[DateOfCreation] AS [DateOfCreation], 
           [Dir].[LastUpdated] AS [LastUpdated], [Dir].[ForceUpdated] AS [ForceUpdated]
    FROM [SendItemsEntities] AS [SendItems]
    INNER JOIN [DirectoryList] AS [Dir] ON [SendItems].[EntityId] = [Dir].[Id]
    WHERE [SendItems].[Contragent_Id] = @contragentId AND 
          [SendItems].[EntityName] = 3
    ORDER BY [Dir].[DirectoryLevel], [Dir].[Parent_Id], [Dir].[Id];

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


-------------------------------------------------------------------------------------------------------------------


            body.AppendLine("--  +-----------------------------------------------------+");
            body.AppendLine("--  | Author:       OLEXANDR LIKHOSHVA                    |");
            body.AppendLine("--  | Create date:  2018.02.24 17:21                      |");
            body.AppendLine("--  | Copyright:    © 2017-2018  ALL RIGHT RESERVED       |");
            body.AppendLine("--  | https://www.linkedin.com/in/olexandrlikhoshva/      |");
            body.AppendLine("--  |-----------------------------------------------------|");
            body.AppendLine("--  | Description:  Procedure for getting Directory items |");
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
            body.AppendLine("    WITH [DirectoryList]");
            body.AppendLine("        ([DirectoryLevel], [Id], [Code], [Name], [Parent_Id],");
            body.AppendLine("         [DateOfCreation], [LastUpdated], [ForceUpdated]) AS");
            body.AppendLine("        (SELECT 0 AS [DirectoryLevel],");
            body.AppendLine("                [Directory].[Id], [Directory].[Code], [Directory].[Name],");
            body.AppendLine("                [Directory].[Parent_Id], [Directory].[DateOfCreation],");
            body.AppendLine("                [Directory].[LastUpdated], [Directory].[ForceUpdated]");
            body.AppendLine("         FROM [DirectoryEntities] AS [Directory]");
            body.AppendLine("         WHERE [Directory].[Parent_Id] IS NULL");
            body.AppendLine("         UNION ALL");
            body.AppendLine("         SELECT [ParentDirectory].[DirectoryLevel] + 1,");
            body.AppendLine("                [DependencyDirectory].[Id], [DependencyDirectory].[Code], [DependencyDirectory].[Name],");
            body.AppendLine("                [DependencyDirectory].[Parent_Id], [DependencyDirectory].[DateOfCreation],");
            body.AppendLine("                [DependencyDirectory].[LastUpdated], [DependencyDirectory].[ForceUpdated]");
            body.AppendLine("         FROM [DirectoryEntities] AS [DependencyDirectory]");
            body.AppendLine("         INNER JOIN [DirectoryList] AS [ParentDirectory]");
            body.AppendLine("             ON [DependencyDirectory].[Parent_Id] = [ParentDirectory].[Id])");
            body.AppendLine("    SELECT TOP (@countToUpdate)");
            body.AppendLine("           [Dir].[Id] AS [Id], [Dir].[Code] AS [Code], [Dir].[Name] AS [Name],");
            body.AppendLine("           [Dir].[Parent_Id] AS [ParentId], [Dir].[DateOfCreation] AS [DateOfCreation],");
            body.AppendLine("           [Dir].[LastUpdated] AS [LastUpdated], [Dir].[ForceUpdated] AS [ForceUpdated]");
            body.AppendLine("    FROM [SendItemsEntities] AS [SendItems]");
            body.AppendLine("    INNER JOIN [DirectoryList] AS [Dir] ON [SendItems].[EntityId] = [Dir].[Id]");
            body.AppendLine("    WHERE [SendItems].[Contragent_Id] = @contragentId AND");
            body.AppendLine("          [SendItems].[EntityName] = 3");
            body.AppendLine("    ORDER BY [Dir].[DirectoryLevel], [Dir].[Parent_Id], [Dir].[Id];");
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
            body.AppendLine(" ");
