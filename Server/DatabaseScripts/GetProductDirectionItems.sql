SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetProductDirectionItems] 
    @login [nvarchar](30),
    @countToUpdate [int]
AS
BEGIN
--     +------------------------------------------------------------+
--     | Author:       OLEXANDR LIKHOSHVA                           |
--     | Create date:  2018.02.24 17:21                             |
--     | Copyright:    © 2017-2018  ALL RIGHT RESERVED              |
--     | https://www.linkedin.com/in/olexandrlikhoshva/             |
--     |------------------------------------------------------------|
--     | Description:  Procedure for getting ProductDirection items |
--     +------------------------------------------------------------+
--  SET NOCOUNT ON added to prevent extra result sets from
--  interfering with SELECT statements.
--  SET NOCOUNT ON;

    DECLARE @contragentId bigint = -1;
    
    SELECT TOP (1) @contragentId = [Contragent].[Id] 
    FROM   [ContragentItemEntities] AS [Contragent]
    WHERE ([Contragent].[Login] = @login);
    
    SELECT TOP (@countToUpdate) 
           [ProductDirection].[Id] AS [Id], 
           [ProductDirection].[Direction] AS [Direction], 
           [ProductDirection].[Directory_Id] AS [DirectoryId], 
           [ProductDirection].[DateOfCreation] AS [DateOfCreation], 
           [ProductDirection].[LastUpdated] AS [LastUpdated], 
           [ProductDirection].[ForceUpdated] AS [ForceUpdated] 
    FROM   [SendItemsEntities] AS [SendItems]
    INNER JOIN [ProductDirectionEntities] AS [ProductDirection] 
           ON [SendItems].[EntityId] = [ProductDirection].[Id]
    WHERE  [SendItems].[Contragent_Id] = @contragentId AND 
           [SendItems].[EntityName] = 6

    -- Entity Names:
    -- BrandItemEntity = 1
    -- CatalogItemEntity = 2
    -- DirectoryEntity = 3
    -- PhotoItemEntity = 5
    -- ProductDirectionEntity = 6

END
GO


-----------------------------------------------------------------------------------------

            body.AppendLine("CREATE PROCEDURE [dbo].[GetProductDirectionItems]");
            body.AppendLine("    @login [nvarchar](30),");
            body.AppendLine("    @countToUpdate [int]");
            body.AppendLine("AS");
            body.AppendLine("BEGIN");
            body.AppendLine("--     +------------------------------------------------------------+");
            body.AppendLine("--     | Author:       OLEXANDR LIKHOSHVA                           |");
            body.AppendLine("--     | Create date:  2018.02.24 17:21                             |");
            body.AppendLine("--     | Copyright:    © 2017-2018  ALL RIGHT RESERVED              |");
            body.AppendLine("--     | https://www.linkedin.com/in/olexandrlikhoshva/             |");
            body.AppendLine("--     |------------------------------------------------------------|");
            body.AppendLine("--     | Description:  Procedure for getting ProductDirection items |");
            body.AppendLine("--     +------------------------------------------------------------+");
            body.AppendLine("--  SET NOCOUNT ON added to prevent extra result sets from");
            body.AppendLine("--  interfering with SELECT statements.");
            body.AppendLine("--  SET NOCOUNT ON;");
            body.AppendLine(" ");
            body.AppendLine("    DECLARE @contragentId bigint = -1;");
            body.AppendLine(" ");
            body.AppendLine("    SELECT TOP (1) @contragentId = [Contragent].[Id]");
            body.AppendLine("    FROM   [ContragentItemEntities] AS [Contragent]");
            body.AppendLine("    WHERE ([Contragent].[Login] = @login);");
            body.AppendLine(" ");
            body.AppendLine("    SELECT TOP (@countToUpdate)");
            body.AppendLine("           [ProductDirection].[Id] AS [Id],");
            body.AppendLine("           [ProductDirection].[Direction] AS [Direction],");
            body.AppendLine("           [ProductDirection].[Directory_Id] AS [DirectoryId],");
            body.AppendLine("           [ProductDirection].[DateOfCreation] AS [DateOfCreation],");
            body.AppendLine("           [ProductDirection].[LastUpdated] AS [LastUpdated],");
            body.AppendLine("           [ProductDirection].[ForceUpdated] AS [ForceUpdated]");
            body.AppendLine("    FROM   [SendItemsEntities] AS [SendItems]");
            body.AppendLine("    INNER JOIN [ProductDirectionEntities] AS [ProductDirection]");
            body.AppendLine("           ON [SendItems].[EntityId] = [ProductDirection].[Id]");
            body.AppendLine("    WHERE  [SendItems].[Contragent_Id] = @contragentId AND");
            body.AppendLine("           [SendItems].[EntityName] = 6");
            body.AppendLine(" ");
            body.AppendLine("    -- Entity Names:");
            body.AppendLine("    -- BrandItemEntity = 1");
            body.AppendLine("    -- CatalogItemEntity = 2");
            body.AppendLine("    -- DirectoryEntity = 3");
            body.AppendLine("    -- PhotoItemEntity = 5");
            body.AppendLine("    -- ProductDirectionEntity = 6");
            body.AppendLine(" ");
            body.AppendLine("END");


--------------------------------------------- [ NEW VERSION ] ---------------------------------------------------------


USE [ServerPriceList]
GO

/****** Object:  StoredProcedure [dbo].[GetProductDirectionItems]    Script Date: 01.03.2018 2:01:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetProductDirectionItems] 
    @login [nvarchar](30),
    @countToUpdate [int]
AS
BEGIN
--  +------------------------------------------------------------+
--  | Author:       OLEXANDR LIKHOSHVA                           |
--  | Create date:  2018.02.24 17:21                             |
--  | Copyright:    © 2017-2018  ALL RIGHT RESERVED              |
--  | https://www.linkedin.com/in/olexandrlikhoshva/             |
--  |------------------------------------------------------------|
--  | Description:  Procedure for getting ProductDirection items |
--  +------------------------------------------------------------+

--  SET NOCOUNT ON added to prevent extra result sets from
--  interfering with SELECT statements.
--  SET NOCOUNT ON;

    DECLARE @contragentId bigint = -1;
    
    SELECT TOP (1) @contragentId = [Contragent].[Id] 
    FROM [ContragentItemEntities] AS [Contragent]
    WHERE ([Contragent].[Login] = @login);
    
    SELECT TOP (@countToUpdate) 
           [ProductDirection].[Id] AS [Id], 
           [ProductDirection].[Direction] AS [Direction], 
           [ProductDirection].[Directory_Id] AS [DirectoryId], 
           [ProductDirection].[DateOfCreation] AS [DateOfCreation], 
           [ProductDirection].[LastUpdated] AS [LastUpdated], 
           [ProductDirection].[ForceUpdated] AS [ForceUpdated] 
    FROM [SendItemsEntities] AS [SendItems]
    INNER JOIN [ProductDirectionEntities] AS [ProductDirection] 
          ON [SendItems].[EntityId] = [ProductDirection].[Id]
    WHERE [SendItems].[Contragent_Id] = @contragentId AND 
          [SendItems].[EntityName] = 6

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

----------------------------------------------------------------------------------------------------------------------------------


            body.AppendLine("--  +------------------------------------------------------------+");
            body.AppendLine("--  | Author:       OLEXANDR LIKHOSHVA                           |");
            body.AppendLine("--  | Create date:  2018.02.24 17:21                             |");
            body.AppendLine("--  | Copyright:    © 2017-2018  ALL RIGHT RESERVED              |");
            body.AppendLine("--  | https://www.linkedin.com/in/olexandrlikhoshva/             |");
            body.AppendLine("--  |------------------------------------------------------------|");
            body.AppendLine("--  | Description:  Procedure for getting ProductDirection items |");
            body.AppendLine("--  +------------------------------------------------------------+");
            body.AppendLine(" ");
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
            body.AppendLine("           [ProductDirection].[Id] AS [Id],");
            body.AppendLine("           [ProductDirection].[Direction] AS [Direction],");
            body.AppendLine("           [ProductDirection].[Directory_Id] AS [DirectoryId],");
            body.AppendLine("           [ProductDirection].[DateOfCreation] AS [DateOfCreation],");
            body.AppendLine("           [ProductDirection].[LastUpdated] AS [LastUpdated],");
            body.AppendLine("           [ProductDirection].[ForceUpdated] AS [ForceUpdated]");
            body.AppendLine("    FROM [SendItemsEntities] AS [SendItems]");
            body.AppendLine("    INNER JOIN [ProductDirectionEntities] AS [ProductDirection]");
            body.AppendLine("          ON [SendItems].[EntityId] = [ProductDirection].[Id]");
            body.AppendLine("    WHERE [SendItems].[Contragent_Id] = @contragentId AND");
            body.AppendLine("          [SendItems].[EntityName] = 6");
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
