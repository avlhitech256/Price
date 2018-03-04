SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpdateBrands] 
    @brands [dbo].[brandsTable] READONLY,
    @lastUpdate [datetimeoffset](7)
AS
BEGIN
--  +---------------------------------------------------+
--  | Author:       OLEXANDR LIKHOSHVA                  |
--  | Create date:  2018.03.03 21:47                    |
--  | Copyright:    © 2017-2018  ALL RIGHT RESERVED     |
--  | https://www.linkedin.com/in/olexandrlikhoshva/    |
--  |---------------------------------------------------|
--  | Description:  Procedure for insert or update      |
--  |               BrandItemEntities                   |
--  +---------------------------------------------------+
--  SET NOCOUNT ON added to prevent extra result sets from
--  interfering with SELECT statements.
--  SET NOCOUNT ON;

    WITH [BrandsForUpdate] AS 
          (SELECT [BrandsItems].[Code] AS [Code],
                    [BrandsItems].[Name] AS [Name],
                        [BrandsItems].[Name] AS [OldName],
                        [BrandsItems].[LastUpdated] AS [LastUpdated],
                        [BrandsItems].[ForceUpdated] AS [ForceUpdated]
             FROM [BrandItemEntities] AS [BrandsItems]
         WHERE [BrandsItems].[Code] IN (SELECT [Code] FROM @brands))
    UPDATE [ItemsForUpdate]
    SET [ItemsForUpdate].[LastUpdated] = CASE WHEN [ItemsForUpdate].[OldName] = [UpdateItems].[Name] 
                                                 THEN [ItemsForUpdate].[LastUpdated]
                                                                   ELSE @lastUpdate
                                                           END,
        [ItemsForUpdate].[ForceUpdated] = @lastUpdate,
            [ItemsForUpdate].[Code] = [UpdateItems].[Code],
        [ItemsForUpdate].[Name] = [UpdateItems].[Name]
    FROM [BrandsForUpdate] AS [ItemsForUpdate]
    INNER JOIN @brands AS [UpdateItems] ON [ItemsForUpdate].[Code] = [UpdateItems].[Code]

    INSERT INTO  [BrandItemEntities]
        ([Code], [Name], [DateOfCreation], [LastUpdated], [ForceUpdated])
    SELECT [InsertItems].[Code], [InsertItems].[Name],
           @lastUpdate, @lastUpdate, @lastUpdate
    FROM @brands AS [InsertItems]
    WHERE [InsertItems].[Code] NOT IN (SELECT [BrandItems].[Code] FROM [BrandItemEntities] AS [BrandItems])
 
END
GO


----------------------------------------------------------------------------------------------------------------

            body.AppendLine("CREATE PROCEDURE [dbo].[UpdateBrands]");
            body.AppendLine("    @brands [dbo].[brandsTable] READONLY,");
            body.AppendLine("    @lastUpdate [datetimeoffset](7)");
            body.AppendLine("AS");
            body.AppendLine("BEGIN");
            body.AppendLine("--  +---------------------------------------------------+");
            body.AppendLine("--  | Author:       OLEXANDR LIKHOSHVA                  |");
            body.AppendLine("--  | Create date:  2018.03.03 21:47                    |");
            body.AppendLine("--  | Copyright:    © 2017-2018  ALL RIGHT RESERVED     |");
            body.AppendLine("--  | https://www.linkedin.com/in/olexandrlikhoshva/    |");
            body.AppendLine("--  |---------------------------------------------------|");
            body.AppendLine("--  | Description:  Procedure for insert or update      |");
            body.AppendLine("--  |               BrandItemEntities                   |");
            body.AppendLine("--  +---------------------------------------------------+");
            body.AppendLine("--  SET NOCOUNT ON added to prevent extra result sets from");
            body.AppendLine("--  interfering with SELECT statements.");
            body.AppendLine("--  SET NOCOUNT ON;");
            body.AppendLine(" ");
            body.AppendLine("    WITH [BrandsForUpdate] AS");
            body.AppendLine("        (SELECT [BrandsItems].[Code] AS [Code],");
            body.AppendLine("                [BrandsItems].[Name] AS [Name],");
            body.AppendLine("                [BrandsItems].[Name] AS [OldName],");
            body.AppendLine("                [BrandsItems].[LastUpdated] AS [LastUpdated],");
            body.AppendLine("                [BrandsItems].[ForceUpdated] AS [ForceUpdated]");
            body.AppendLine("         FROM [BrandItemEntities] AS [BrandsItems]");
            body.AppendLine("         WHERE [BrandsItems].[Code] IN (SELECT [Code] FROM @brands))");
            body.AppendLine("    UPDATE [ItemsForUpdate]");
            body.AppendLine("    SET [ItemsForUpdate].[LastUpdated] = CASE WHEN [ItemsForUpdate].[OldName] = [UpdateItems].[Name]");
            body.AppendLine("                                             THEN [ItemsForUpdate].[LastUpdated]");
            body.AppendLine("                                             ELSE @lastUpdate");
            body.AppendLine("                                         END,");
            body.AppendLine("        [ItemsForUpdate].[ForceUpdated] = @lastUpdate,");
            body.AppendLine("        [ItemsForUpdate].[Code] = [UpdateItems].[Code],");
            body.AppendLine("        [ItemsForUpdate].[Name] = [UpdateItems].[Name]");
            body.AppendLine("    FROM [BrandsForUpdate] AS [ItemsForUpdate]");
            body.AppendLine("    INNER JOIN @brands AS [UpdateItems] ON [ItemsForUpdate].[Code] = [UpdateItems].[Code];");
            body.AppendLine(" ");
            body.AppendLine("    INSERT INTO  [BrandItemEntities]");
            body.AppendLine("        ([Code], [Name], [DateOfCreation], [LastUpdated], [ForceUpdated])");
            body.AppendLine("    SELECT [InsertItems].[Code], [InsertItems].[Name],");
            body.AppendLine("           @lastUpdate, @lastUpdate, @lastUpdate");
            body.AppendLine("    FROM @brands AS [InsertItems]");
            body.AppendLine("    WHERE [InsertItems].[Code] NOT IN (SELECT [BrandItems].[Code] FROM [BrandItemEntities] AS [BrandItems]);");
            body.AppendLine(" ");
            body.AppendLine("END");
