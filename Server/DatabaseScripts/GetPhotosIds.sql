USE [ServerPriceList]
GO

/****** Object:  StoredProcedure [dbo].[GetPhotosIds]    Script Date: 01.03.2018 1:54:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetPhotosIds]
    @ids [dbo].[bigintTable] READONLY
AS
BEGIN
--  +---------------------------------------------------+
--  | Author:       OLEXANDR LIKHOSHVA                  |
--  | Create date:  2018.02.11 23:48                    |
--  | Copyright:    © 2017-2018  ALL RIGHT RESERVED     |
--  | https://www.linkedin.com/in/olexandrlikhoshva/    |
--  |---------------------------------------------------|
--  | Description:  Procedure for getting Photos ids    |
--  +---------------------------------------------------+

--  SET NOCOUNT ON added to prevent extra result sets from
--  interfering with SELECT statements.
--  SET NOCOUNT ON;

    SELECT [Photos].[CatalogItem_Id] AS [CatalogId], [Photos].[Id] AS [PhotoId]
    FROM [dbo].[PhotoItemEntities] AS [Photos]
    WHERE[Photos].[CatalogItem_Id] IN (SELECT Id FROM @ids)
 
END
GO


----------------------------------------------------------------------------------------------------------

            body.AppendLine("CREATE PROCEDURE [dbo].[GetPhotosIds]");
            body.AppendLine("    @ids [dbo].[bigintTable] READONLY");
            body.AppendLine("AS");
            body.AppendLine("BEGIN");
            body.AppendLine("--  +---------------------------------------------------+");
            body.AppendLine("--  | Author:       OLEXANDR LIKHOSHVA                  |");
            body.AppendLine("--  | Create date:  2018.02.11 23:48                    |");
            body.AppendLine("--  | Copyright:    © 2017-2018  ALL RIGHT RESERVED     |");
            body.AppendLine("--  | https://www.linkedin.com/in/olexandrlikhoshva/    |");
            body.AppendLine("--  |---------------------------------------------------|");
            body.AppendLine("--  | Description:  Procedure for getting Photos ids    |");
            body.AppendLine("--  +---------------------------------------------------+");
            body.AppendLine(" ");
            body.AppendLine("--  SET NOCOUNT ON added to prevent extra result sets from");
            body.AppendLine("--  interfering with SELECT statements.");
            body.AppendLine("--  SET NOCOUNT ON;");
            body.AppendLine(" ");
            body.AppendLine("    SELECT [Photos].[CatalogItem_Id] AS [CatalogId], [Photos].[Id] AS [PhotoId]");
            body.AppendLine("    FROM [dbo].[PhotoItemEntities] AS [Photos]");
            body.AppendLine("    WHERE[Photos].[CatalogItem_Id] IN (SELECT Id FROM @ids)");
            body.AppendLine(" ");
            body.AppendLine("END");
