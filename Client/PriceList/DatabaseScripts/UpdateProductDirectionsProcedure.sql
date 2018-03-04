SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpdateProductDirections] 
    @productDirections [dbo].[productDirectionsTable] READONLY
AS
BEGIN
--  +---------------------------------------------------+
--  | Author:       OLEXANDR LIKHOSHVA                  |
--  | Create date:  2018.02.25 01:15                    |
--  | Copyright:    © 2017-2018  ALL RIGHT RESERVED     |
--  | https://www.linkedin.com/in/olexandrlikhoshva/    |
--  |---------------------------------------------------|
--  | Description:  Procedure for insert or update      |
--  |               ProductDirectionEntities            |
--  +---------------------------------------------------+
--  SET NOCOUNT ON added to prevent extra result sets from
--  interfering with SELECT statements.
--  SET NOCOUNT ON;

    WITH [ProductDirectionForUpdate] AS 
         (SELECT * FROM [ProductDirectionEntities] AS [ProductDirectionItems]
          WHERE [ProductDirectionItems].[Id] IN (SELECT [Id] FROM @productDirections))
    UPDATE [ItemsForUpdate]
    SET [ItemsForUpdate].[Direction] = [UpdateItems].[Direction],
        [ItemsForUpdate].[Directory_Id] = [UpdateItems].[Directory_Id],
        [ItemsForUpdate].[DateOfCreation] = [UpdateItems].[DateOfCreation],
        [ItemsForUpdate].[LastUpdated] = [UpdateItems].[LastUpdated],
        [ItemsForUpdate].[ForceUpdated] = [UpdateItems].[ForceUpdated]
    FROM [ProductDirectionForUpdate] AS [ItemsForUpdate]
    INNER JOIN @productDirections AS [UpdateItems] ON [ItemsForUpdate].[Id] = [UpdateItems].[Id]

    INSERT INTO  [ProductDirectionEntities]
        ([Id], [Direction], [Directory_Id], [DateOfCreation], [LastUpdated], [ForceUpdated])
    SELECT [InsertItems].[Id], [InsertItems].[Direction], [InsertItems].[Directory_Id],
           [InsertItems].[DateOfCreation], [InsertItems].[LastUpdated], [InsertItems].[ForceUpdated]
    FROM @productDirections AS [InsertItems]
    WHERE [InsertItems].[Id] NOT IN (SELECT [ProductDirectionItems].[Id] 
                                     FROM [ProductDirectionEntities] AS [ProductDirectionItems])
 
END
GO


----------------------------------------------------------------------------------------------------------------

            procedure.AppendLine("CREATE PROCEDURE [dbo].[UpdateProductDirections]");
            procedure.AppendLine("    @productDirections [dbo].[productDirectionsTable] READONLY");
            procedure.AppendLine("AS");
            procedure.AppendLine("BEGIN");
            procedure.AppendLine("--  +---------------------------------------------------+");
            procedure.AppendLine("--  | Author:       OLEXANDR LIKHOSHVA                  |");
            procedure.AppendLine("--  | Create date:  2018.02.25 01:15                    |");
            procedure.AppendLine("--  | Copyright:    © 2017-2018  ALL RIGHT RESERVED     |");
            procedure.AppendLine("--  | https://www.linkedin.com/in/olexandrlikhoshva/    |");
            procedure.AppendLine("--  |---------------------------------------------------|");
            procedure.AppendLine("--  | Description:  Procedure for insert or update      |");
            procedure.AppendLine("--  |               ProductDirectionEntities            |");
            procedure.AppendLine("--  +---------------------------------------------------+");
            procedure.AppendLine("--  SET NOCOUNT ON added to prevent extra result sets from");
            procedure.AppendLine("--  interfering with SELECT statements.");
            procedure.AppendLine("--  SET NOCOUNT ON;");
            procedure.AppendLine(" ");
            procedure.AppendLine("    WITH [ProductDirectionForUpdate] AS");
            procedure.AppendLine("         (SELECT * FROM [ProductDirectionEntities] AS [ProductDirectionItems]");
            procedure.AppendLine("          WHERE [ProductDirectionItems].[Id] IN (SELECT [Id] FROM @productDirections))");
            procedure.AppendLine("    UPDATE [ItemsForUpdate]");
            procedure.AppendLine("    SET [ItemsForUpdate].[Direction] = [UpdateItems].[Direction],");
            procedure.AppendLine("        [ItemsForUpdate].[Directory_Id] = [UpdateItems].[Directory_Id],");
            procedure.AppendLine("        [ItemsForUpdate].[DateOfCreation] = [UpdateItems].[DateOfCreation],");
            procedure.AppendLine("        [ItemsForUpdate].[LastUpdated] = [UpdateItems].[LastUpdated],");
            procedure.AppendLine("        [ItemsForUpdate].[ForceUpdated] = [UpdateItems].[ForceUpdated]");
            procedure.AppendLine("    FROM [ProductDirectionForUpdate] AS [ItemsForUpdate]");
            procedure.AppendLine("    INNER JOIN @productDirections AS [UpdateItems] ON [ItemsForUpdate].[Id] = [UpdateItems].[Id]");
            procedure.AppendLine(" ");
            procedure.AppendLine("    INSERT INTO  [ProductDirectionEntities]");
            procedure.AppendLine("        ([Id], [Direction], [Directory_Id], [DateOfCreation], [LastUpdated], [ForceUpdated])");
            procedure.AppendLine("    SELECT [InsertItems].[Id], [InsertItems].[Direction], [InsertItems].[Directory_Id],");
            procedure.AppendLine("           [InsertItems].[DateOfCreation], [InsertItems].[LastUpdated], [InsertItems].[ForceUpdated]");
            procedure.AppendLine("    FROM @productDirections AS [InsertItems]");
            procedure.AppendLine("    WHERE [InsertItems].[Id] NOT IN (SELECT [ProductDirectionItems].[Id]");
            procedure.AppendLine("                                     FROM [ProductDirectionEntities] AS [ProductDirectionItems])");
            procedure.AppendLine(" ");
            procedure.AppendLine("END");
