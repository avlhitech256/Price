USE [PriceList]
GO
/****** Object:  StoredProcedure [dbo].[UpdateDirectories]    Script Date: 26.02.2018 2:20:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpdateBrands]
      @brands [dbo].[brandsTable] READONLY
AS
BEGIN
--    +---------------------------------------------------+
--    | Author:       OLEXANDR LIKHOSHVA                  |
--    | Create date:  2018.02.26 02:32                    |
--    | Copyright:    © 2017-2018  ALL RIGHT RESERVED     |
--    | https://www.linkedin.com/in/olexandrlikhoshva/    |
--    |---------------------------------------------------|
--    | Description:  Procedure for insert or update      |
--    |               BrandItemEntities                   |
--    +---------------------------------------------------+
--    SET NOCOUNT ON added to prevent extra result sets from
--    interfering with SELECT statements.
--    SET NOCOUNT ON;
 
      WITH [BrandsForUpdate] AS (SELECT * FROM [BrandItemEntities] AS [BrandsItems]
                                 WHERE [BrandsItems].[Id] IN (SELECT [Id] FROM @brands))
      UPDATE [ItemsForUpdate]
      SET [ItemsForUpdate].[Code] = [UpdateItems].[Code],
          [ItemsForUpdate].[Name] = [UpdateItems].[Name],
          [ItemsForUpdate].[DateOfCreation] = [UpdateItems].[DateOfCreation],
          [ItemsForUpdate].[LastUpdated] = [UpdateItems].[LastUpdated],
          [ItemsForUpdate].[ForceUpdated] = [UpdateItems].[ForceUpdated]
      FROM [BrandsForUpdate] AS [ItemsForUpdate]
      INNER JOIN @brands AS [UpdateItems] ON [ItemsForUpdate].[Id] = [UpdateItems].[Id]
 
      INSERT INTO  [BrandItemEntities]
      ([Id], [Code], [Name], [DateOfCreation], [LastUpdated], [ForceUpdated])
      SELECT [InsertItems].[Id], [InsertItems].[Code], [InsertItems].[Name],
             [InsertItems].[DateOfCreation], [InsertItems].[LastUpdated], [InsertItems].[ForceUpdated]
      FROM @brands AS [InsertItems]
      WHERE [InsertItems].[Id] NOT IN (SELECT [BrandItems].[Id] FROM [BrandItemEntities] AS [BrandItems])
 
END


-------------------------------------------------------------------------------------------------------------


            procedure.AppendLine("CREATE PROCEDURE [dbo].[UpdateBrands]");
            procedure.AppendLine("      @brands [dbo].[brandsTable] READONLY");
            procedure.AppendLine("AS");
            procedure.AppendLine("BEGIN");
            procedure.AppendLine("--    +---------------------------------------------------+");
            procedure.AppendLine("--    | Author:       OLEXANDR LIKHOSHVA                  |");
            procedure.AppendLine("--    | Create date:  2018.02.26 02:32                    |");
            procedure.AppendLine("--    | Copyright:    © 2017-2018  ALL RIGHT RESERVED     |");
            procedure.AppendLine("--    | https://www.linkedin.com/in/olexandrlikhoshva/    |");
            procedure.AppendLine("--    |---------------------------------------------------|");
            procedure.AppendLine("--    | Description:  Procedure for insert or update      |");
            procedure.AppendLine("--    |               BrandItemEntities                   |");
            procedure.AppendLine("--    +---------------------------------------------------+");
            procedure.AppendLine("--    SET NOCOUNT ON added to prevent extra result sets from");
            procedure.AppendLine("--    interfering with SELECT statements.");
            procedure.AppendLine("--    SET NOCOUNT ON;");
            procedure.AppendLine(" ");
            procedure.AppendLine("      WITH [BrandsForUpdate] AS (SELECT * FROM [BrandItemEntities] AS [BrandsItems]");
            procedure.AppendLine("                                 WHERE [BrandsItems].[Id] IN (SELECT [Id] FROM @brands))");
            procedure.AppendLine("      UPDATE [ItemsForUpdate]");
            procedure.AppendLine("      SET [ItemsForUpdate].[Code] = [UpdateItems].[Code],");
            procedure.AppendLine("          [ItemsForUpdate].[Name] = [UpdateItems].[Name],");
            procedure.AppendLine("          [ItemsForUpdate].[DateOfCreation] = [UpdateItems].[DateOfCreation],");
            procedure.AppendLine("          [ItemsForUpdate].[LastUpdated] = [UpdateItems].[LastUpdated],");
            procedure.AppendLine("          [ItemsForUpdate].[ForceUpdated] = [UpdateItems].[ForceUpdated]");
            procedure.AppendLine("      FROM [BrandsForUpdate] AS [ItemsForUpdate]");
            procedure.AppendLine("      INNER JOIN @brands AS [UpdateItems] ON [ItemsForUpdate].[Id] = [UpdateItems].[Id]");
            procedure.AppendLine(" ");
            procedure.AppendLine("      INSERT INTO  [BrandItemEntities]");
            procedure.AppendLine("      ([Id], [Code], [Name], [DateOfCreation], [LastUpdated], [ForceUpdated])");
            procedure.AppendLine("      SELECT [InsertItems].[Id], [InsertItems].[Code], [InsertItems].[Name],");
            procedure.AppendLine("             [InsertItems].[DateOfCreation], [InsertItems].[LastUpdated], [InsertItems].[ForceUpdated]");
            procedure.AppendLine("      FROM @brands AS [InsertItems]");
            procedure.AppendLine("      WHERE [InsertItems].[Id] NOT IN (SELECT [BrandItems].[Id] FROM [BrandItemEntities] AS [BrandItems])");
            procedure.AppendLine(" ");
            procedure.AppendLine("END");
