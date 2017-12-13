-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_prepare_to_update_catalogs] (
	-- Add the parameters for the stored procedure here
	@login nvarchar(30), 
	@last_update datetimeoffset(7))
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @entity_name int = 2;
	declare @date_of_creation datetimeoffset(7) = Sysdatetimeoffset();
	declare @count_to_update bigint;

	INSERT INTO [dbo].[SendItemsEntities] 
	([Login], [EntityId], [Catalog].[EntityName], [Catalog].[RequestDate], [Catalog].[DateOfCreation]) 
	SELECT @login, [Id], 2, @last_update, @date_of_creation 
	FROM  [dbo].[CatalogItemEntities] AS [Catalog]
	WHERE [Catalog].[LastUpdated] > @last_update AND 
	      NOT EXISTS (SELECT * 
		              FROM  [dbo].[SendItemsEntities] AS [SendItem] 
		              WHERE [SendItem].[EntityId] = [Catalog].[Id] AND
					        [SendItem].[Login] = @login AND
							[SendItem].[EntityName] = @entity_name);

    SELECT @count_to_update = (SELECT COUNT(*) 
	                           FROM  [dbo].[SendItemsEntities]
	                           WHERE [Login] = @login AND
							         [EntityName] = @entity_name);

	RETURN (@count_to_update);
END
GO


---------------------------------------------------------------------------------------------------------


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
/****** Object:  StoredProcedure [dbo].[sp_prepare_to_update_catalogs]    Script Date: 12/13/2017 2:33:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_prepare_to_update_catalogs] (
	-- Add the parameters for the stored procedure here
	@login nvarchar(30), 
	@last_update datetimeoffset(7))
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @entity_name int = 2;
	declare @date_of_creation datetimeoffset(7) = Sysdatetimeoffset();
	declare @count_to_update bigint;

	INSERT INTO [dbo].[SendItemsEntities] 
	([Login], [EntityId], [Catalog].[EntityName], [Catalog].[RequestDate], [Catalog].[DateOfCreation]) 
	SELECT @login, [Id], 2, @last_update, @date_of_creation 
	FROM  [dbo].[CatalogItemEntities] AS [Catalog]
	WHERE [Catalog].[LastUpdated] > @last_update AND 
	      NOT EXISTS (SELECT * 
		              FROM  [dbo].[SendItemsEntities] AS [SendItem] 
		              WHERE [SendItem].[EntityId] = [Catalog].[Id] AND
					        [SendItem].[Login] = @login AND
							[SendItem].[EntityName] = @entity_name);

    SELECT @count_to_update = (SELECT COUNT(*) 
	                           FROM  [dbo].[SendItemsEntities]
	                           WHERE [Login] = @login AND
							         [EntityName] = @entity_name);

	RETURN (@count_to_update);
END
