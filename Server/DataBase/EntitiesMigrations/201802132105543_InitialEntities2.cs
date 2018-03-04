using System.Text;

namespace DataBase.EntitiesMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class InitialEntities2 : DbMigration
    {
        public override void Up()
        {
            DropStoredProcedure("dbo.PrepareToUpdateBrands");
            DropStoredProcedure("dbo.PrepareToUpdateCatalogs");
            DropStoredProcedure("dbo.PrepareToUpdateDirectories");
            DropStoredProcedure("dbo.PrepareToUpdatePhotos");
            DropStoredProcedure("dbo.PrepareToUpdateProductDirections");
            DropIndex("dbo.PhotoItemEntities", new[] { "Name" });
            DropIndex("dbo.OptionItemEntities", new[] { "Code" });
            DropIndex("dbo.OrderEntities", new[] { "OrderNumber" });
            DropIndex("dbo.SendItemsEntities", new[] { "Login" });
            AddColumn("dbo.SendItemsEntities", "Contragent_Id", c => c.Long());
            AlterColumn("dbo.ContragentItemEntities", "Login", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.PhotoItemEntities", "Name", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.OptionItemEntities", "Code", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.OrderEntities", "OrderNumber", c => c.String(nullable: false, maxLength: 30));
            CreateIndex("dbo.ContragentItemEntities", "Login", unique: true);
            CreateIndex("dbo.PhotoItemEntities", "Name", unique: true);
            CreateIndex("dbo.OptionItemEntities", "Code", unique: true);
            CreateIndex("dbo.OrderEntities", "OrderNumber", unique: true);
            CreateIndex("dbo.SendItemsEntities", "Contragent_Id");
            AddForeignKey("dbo.SendItemsEntities", "Contragent_Id", "dbo.ContragentItemEntities", "Id");
            DropColumn("dbo.SendItemsEntities", "Login");

            CreateStoredProcedure(
                "dbo.PrepareToUpdateBrands",
                p => new
                {
                    login = p.String(maxLength: 30),
                    lastUpdate = p.DateTimeOffset(precision: 7),
                    countToUpdate = p.Int(outParameter: true)
                },
                body: CreatePrepareToUpdateBrandsBody());

            CreateStoredProcedure(
                "dbo.PrepareToUpdateCatalogs",
                p => new
                {
                    login = p.String(maxLength: 30),
                    lastUpdate = p.DateTimeOffset(precision: 7),
                    countToUpdate = p.Int(outParameter: true)
                },
                body: CreatePrepareToUpdateCatalogsBody());

            CreateStoredProcedure(
                "dbo.PrepareToUpdateDirectories",
                p => new
                {
                    login = p.String(maxLength: 30),
                    lastUpdate = p.DateTimeOffset(precision: 7),
                    countToUpdate = p.Int(outParameter: true)
                },
                body: CreatePrepareToUpdateDirectoriesBody());

            Sql(CreatePrepareToUpdatePhotosProcedure());

            CreateStoredProcedure(
                "dbo.PrepareToUpdateProductDirections",
                p => new
                {
                    login = p.String(maxLength: 30),
                    lastUpdate = p.DateTimeOffset(precision: 7),
                    countToUpdate = p.Int(outParameter: true)
                },
                body: CreatePrepareToUpdateProductDirectionsBody());

        }

        public override void Down()
        {
            DropStoredProcedure("dbo.PrepareToUpdateBrands");
            DropStoredProcedure("dbo.PrepareToUpdateCatalogs");
            DropStoredProcedure("dbo.PrepareToUpdateDirectories");
            DropStoredProcedure("dbo.PrepareToUpdatePhotos");
            DropStoredProcedure("dbo.PrepareToUpdateProductDirections");
            AddColumn("dbo.SendItemsEntities", "Login", c => c.String(maxLength: 30));
            DropForeignKey("dbo.SendItemsEntities", "Contragent_Id", "dbo.ContragentItemEntities");
            DropIndex("dbo.SendItemsEntities", new[] { "Contragent_Id" });
            DropIndex("dbo.OrderEntities", new[] { "OrderNumber" });
            DropIndex("dbo.OptionItemEntities", new[] { "Code" });
            DropIndex("dbo.PhotoItemEntities", new[] { "Name" });
            DropIndex("dbo.ContragentItemEntities", new[] { "Login" });
            AlterColumn("dbo.OrderEntities", "OrderNumber", c => c.String(maxLength: 30));
            AlterColumn("dbo.OptionItemEntities", "Code", c => c.String(maxLength: 50));
            AlterColumn("dbo.PhotoItemEntities", "Name", c => c.String(maxLength: 255));
            AlterColumn("dbo.ContragentItemEntities", "Login", c => c.String(maxLength: 30));
            DropColumn("dbo.SendItemsEntities", "Contragent_Id");
            CreateIndex("dbo.SendItemsEntities", "Login");
            CreateIndex("dbo.OrderEntities", "OrderNumber", unique: true);
            CreateIndex("dbo.OptionItemEntities", "Code", unique: true);
            CreateIndex("dbo.PhotoItemEntities", "Name", unique: true);

            CreateStoredProcedure(
                "dbo.PrepareToUpdateBrands",
                p => new
                {
                    login = p.String(maxLength: 30),
                    lastUpdate = p.DateTimeOffset(precision: 7),
                    countToUpdate = p.Int(outParameter: true)
                },
                body: CreatePrepareToUpdateBrandsOldBody());

            CreateStoredProcedure(
                "dbo.PrepareToUpdateCatalogs",
                p => new
                {
                    login = p.String(maxLength: 30),
                    lastUpdate = p.DateTimeOffset(precision: 7),
                    countToUpdate = p.Int(outParameter: true)
                },
                body: CreatePrepareToUpdateCatalogsOldBody());

            CreateStoredProcedure(
                "dbo.PrepareToUpdateDirectories",
                p => new
                {
                    login = p.String(maxLength: 30),
                    lastUpdate = p.DateTimeOffset(precision: 7),
                    countToUpdate = p.Int(outParameter: true)
                },
                body: CreatePrepareToUpdateDirectoriesOldBody());

            Sql(CreatePrepareToUpdatePhotosOldProcedure());

            CreateStoredProcedure(
                "dbo.PrepareToUpdateProductDirections",
                p => new
                {
                    login = p.String(maxLength: 30),
                    lastUpdate = p.DateTimeOffset(precision: 7),
                    countToUpdate = p.Int(outParameter: true)
                },
                body: CreatePrepareToUpdateProductDirectionsOldBody());

        }

        private string CreatePrepareToUpdateBrandsBody()
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine("--  +----------------------------------------------------+");
            body.AppendLine("--  | Author:       OLEXANDR LIKHOSHVA                   |");
            body.AppendLine("--  | Create date:  2018.02.28 23:10                     |");
            body.AppendLine("--  | Copyright:    © 2017-2018  ALL RIGHT RESERVED      |");
            body.AppendLine("--  | https://www.linkedin.com/in/olexandrlikhoshva/     |");
            body.AppendLine("--  |----------------------------------------------------|");
            body.AppendLine("--  | Description:  The procedure for inserting entries  |");
            body.AppendLine("--  |               into the SendItemsEntities for a new |");
            body.AppendLine("--  |               or modified BrandItemEntities        |");
            body.AppendLine("--  |               records which will be updated on the |");
            body.AppendLine("--  |               client side.                         |");
            body.AppendLine("--  +----------------------------------------------------+");
            body.AppendLine(" ");
            body.AppendLine("--  SET NOCOUNT ON added to prevent extra result sets from");
            body.AppendLine("--  interfering with SELECT statements.");
            body.AppendLine("--  SET NOCOUNT ON;");
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

            return body.ToString();
        }

        private string CreatePrepareToUpdateCatalogsBody()
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine("--  +----------------------------------------------------+");
            body.AppendLine("--  | Author:       OLEXANDR LIKHOSHVA                   |");
            body.AppendLine("--  | Create date:  2018.03.01 00:17                     |");
            body.AppendLine("--  | Copyright:    © 2017-2018  ALL RIGHT RESERVED      |");
            body.AppendLine("--  | https://www.linkedin.com/in/olexandrlikhoshva/     |");
            body.AppendLine("--  |----------------------------------------------------|");
            body.AppendLine("--  | Description:  The procedure for inserting entries  |");
            body.AppendLine("--  |               into the SendItemsEntities for a new |");
            body.AppendLine("--  |               or modified CatalogItemEntities      |");
            body.AppendLine("--  |               records which will be updated on the |");
            body.AppendLine("--  |               client side.                         |");
            body.AppendLine("--  +----------------------------------------------------+");
            body.AppendLine(" ");
            body.AppendLine("--  SET NOCOUNT ON added to prevent extra result sets from");
            body.AppendLine("--  interfering with SELECT statements.");
            body.AppendLine("--  SET NOCOUNT ON;");
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
            body.AppendLine("    DECLARE @entityName int = 2;");
            body.AppendLine("    DECLARE @dateOfCreation datetimeoffset(7) = Sysdatetimeoffset();");
            body.AppendLine("    DECLARE @contragentId bigint = -1;");
            body.AppendLine(" ");
            body.AppendLine("    SELECT TOP (1) @contragentId = [Contragent].[Id]");
            body.AppendLine("    FROM [ContragentItemEntities] AS [Contragent]");
            body.AppendLine("    WHERE ([Contragent].[Login] = @login);");
            body.AppendLine(" ");
            body.AppendLine("    INSERT INTO [dbo].[SendItemsEntities]");
            body.AppendLine("        ([Contragent_Id], [EntityId], [Catalogs].[EntityName],");
            body.AppendLine("         [Catalogs].[RequestDate], [Catalogs].[DateOfCreation])");
            body.AppendLine("    SELECT @contragentId, [Id], @entityName, @lastUpdate, @dateOfCreation");
            body.AppendLine("    FROM [dbo].[CatalogItemEntities] AS [Catalogs]");
            body.AppendLine("    WHERE [Catalogs].[LastUpdated] > @lastUpdate AND");
            body.AppendLine("          NOT EXISTS (SELECT (1)");
            body.AppendLine("                      FROM  [dbo].[SendItemsEntities] AS [SendItem]");
            body.AppendLine("                      WHERE [SendItem].[EntityId] = [Catalogs].[Id] AND");
            body.AppendLine("                      [SendItem].[Contragent_Id] = @contragentId AND");
            body.AppendLine("                      [SendItem].[EntityName] = @entityName);");
            body.AppendLine(" ");
            body.AppendLine("    SELECT @countToUpdate = COUNT(*)");
            body.AppendLine("    FROM  [dbo].[SendItemsEntities]");
            body.AppendLine("    WHERE [Contragent_Id] = @contragentId AND");
            body.AppendLine("    [EntityName] = @entityName;");
            body.AppendLine(" ");
            body.AppendLine("    RETURN (@countToUpdate);");
            body.AppendLine(" ");

            return body.ToString();
        }

        private string CreatePrepareToUpdateDirectoriesBody()
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine("--  +----------------------------------------------------+");
            body.AppendLine("--  | Author:       OLEXANDR LIKHOSHVA                   |");
            body.AppendLine("--  | Create date:  2018.02.28 23:28                     |");
            body.AppendLine("--  | Copyright:    © 2017-2018  ALL RIGHT RESERVED      |");
            body.AppendLine("--  | https://www.linkedin.com/in/olexandrlikhoshva/     |");
            body.AppendLine("--  |----------------------------------------------------|");
            body.AppendLine("--  | Description:  The procedure for inserting entries  |");
            body.AppendLine("--  |               into the SendItemsEntities for a new |");
            body.AppendLine("--  |               or modified DirectoryEntities        |");
            body.AppendLine("--  |               records which will be updated on the |");
            body.AppendLine("--  |               client side.                         |");
            body.AppendLine("--  +----------------------------------------------------+");
            body.AppendLine(" ");
            body.AppendLine("--  SET NOCOUNT ON added to prevent extra result sets from");
            body.AppendLine("--  interfering with SELECT statements.");
            body.AppendLine("--  SET NOCOUNT ON;");
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
            body.AppendLine("    DECLARE @entityName int = 3;");
            body.AppendLine("    DECLARE @dateOfCreation datetimeoffset(7) = Sysdatetimeoffset();");
            body.AppendLine("    DECLARE @contragentId bigint = -1;");
            body.AppendLine(" ");
            body.AppendLine("    SELECT TOP (1) @contragentId = [Contragent].[Id]");
            body.AppendLine("    FROM [ContragentItemEntities] AS [Contragent]");
            body.AppendLine("    WHERE ([Contragent].[Login] = @login);");
            body.AppendLine(" ");
            body.AppendLine("    INSERT INTO [dbo].[SendItemsEntities]");
            body.AppendLine("        ([Contragent_Id], [EntityId], [Directories].[EntityName],");
            body.AppendLine("         [Directories].[RequestDate], [Directories].[DateOfCreation])");
            body.AppendLine("    SELECT @contragentId, [Id], @entityName, @lastUpdate, @dateOfCreation");
            body.AppendLine("    FROM [dbo].[DirectoryEntities] AS [Directories]");
            body.AppendLine("    WHERE [Directories].[LastUpdated] > @lastUpdate AND");
            body.AppendLine("          NOT EXISTS (SELECT (1)");
            body.AppendLine("                      FROM  [dbo].[SendItemsEntities] AS [SendItem]");
            body.AppendLine("                      WHERE [SendItem].[EntityId] = [Directories].[Id] AND");
            body.AppendLine("                            [SendItem].[Contragent_Id] = @contragentId AND");
            body.AppendLine("                            [SendItem].[EntityName] = @entityName);");
            body.AppendLine(" ");
            body.AppendLine("    SELECT @countToUpdate = COUNT(*)");
            body.AppendLine("    FROM  [dbo].[SendItemsEntities]");
            body.AppendLine("    WHERE [Contragent_Id] = @contragentId AND");
            body.AppendLine("    [EntityName] = @entityName;");
            body.AppendLine(" ");
            body.AppendLine("    RETURN (@countToUpdate);");
            body.AppendLine(" ");

            return body.ToString();
        }

        private string CreatePrepareToUpdatePhotosProcedure()
        {
            StringBuilder body = new StringBuilder();

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
            body.AppendLine("--  | Entity Name:               |");
            body.AppendLine("--  +----------------------------+");
            body.AppendLine("--  | BrandItemEntity = 1        |");
            body.AppendLine("--  | CatalogItemEntity = 2      |");
            body.AppendLine("--  | DirectoryEntity = 3        |");
            body.AppendLine("--  | PhotoItemEntity = 5        |");
            body.AppendLine("--  | ProductDirectionEntity = 6 |");
            body.AppendLine("--  +----------------------------+");
            body.AppendLine("  ");
            body.AppendLine("    DECLARE @entityName int = 5;");
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

            return body.ToString();
        }

        private string CreatePrepareToUpdateProductDirectionsBody()
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine("--   +----------------------------------------------------+");
            body.AppendLine("--   | Author:       OLEXANDR LIKHOSHVA                   |");
            body.AppendLine("--   | Create date:  2018.03.01 01:05                     |");
            body.AppendLine("--   | Copyright:    © 2017-2018  ALL RIGHT RESERVED      |");
            body.AppendLine("--   | https://www.linkedin.com/in/olexandrlikhoshva/     |");
            body.AppendLine("--   |----------------------------------------------------|");
            body.AppendLine("--   | Description:  The procedure for inserting entries  |");
            body.AppendLine("--   |               into the SendItemsEntities for a new |");
            body.AppendLine("--   |               or modified CatalogItemEntities      |");
            body.AppendLine("--   |               records which will be updated on the |");
            body.AppendLine("--   |               client side.                         |");
            body.AppendLine("--   +----------------------------------------------------+");
            body.AppendLine(" ");
            body.AppendLine("--  SET NOCOUNT ON added to prevent extra result sets from");
            body.AppendLine("--  interfering with SELECT statements.");
            body.AppendLine("--  SET NOCOUNT ON;");
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
            body.AppendLine("    DECLARE @entityName int = 6;");
            body.AppendLine("    DECLARE @dateOfCreation datetimeoffset(7) = Sysdatetimeoffset();");
            body.AppendLine("    DECLARE @contragentId bigint = -1;");
            body.AppendLine(" ");
            body.AppendLine("    SELECT TOP (1) @contragentId = [Contragent].[Id]");
            body.AppendLine("    FROM [ContragentItemEntities] AS [Contragent]");
            body.AppendLine("    WHERE ([Contragent].[Login] = @login);");
            body.AppendLine(" ");
            body.AppendLine("    INSERT INTO [dbo].[SendItemsEntities]");
            body.AppendLine("        ([Contragent_Id], [EntityId], [ProductDirectionEntity].[EntityName],");
            body.AppendLine("         [ProductDirectionEntity].[RequestDate], [ProductDirectionEntity].[DateOfCreation])");
            body.AppendLine("    SELECT @contragentId, [Id], @entityName, @lastUpdate, @dateOfCreation");
            body.AppendLine("    FROM [dbo].[ProductDirectionEntities] AS [ProductDirectionEntity]");
            body.AppendLine("    WHERE [ProductDirectionEntity].[LastUpdated] > @lastUpdate AND");
            body.AppendLine("          NOT EXISTS (SELECT (1)");
            body.AppendLine("                      FROM  [dbo].[SendItemsEntities] AS [SendItem]");
            body.AppendLine("                      WHERE [SendItem].[EntityId] = [ProductDirectionEntity].[Id] AND");
            body.AppendLine("                            [SendItem].[Contragent_Id] = @contragentId AND");
            body.AppendLine("                            [SendItem].[EntityName] = @entityName);");
            body.AppendLine(" ");
            body.AppendLine("    SELECT @countToUpdate = COUNT(*)");
            body.AppendLine("    FROM  [dbo].[SendItemsEntities]");
            body.AppendLine("    WHERE [Contragent_Id] = @contragentId AND");
            body.AppendLine("    [EntityName] = @entityName;");
            body.AppendLine(" ");
            body.AppendLine("    RETURN (@countToUpdate);");
            body.AppendLine(" ");

            return body.ToString();
        }

        //--------------------------------------------------------------------------------------------------------------

        private string CreatePrepareToUpdateBrandsOldBody()
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine("-- +---------------------------------------------------+                                           ");
            body.AppendLine("-- | © 2017-2018 OLEXANDR LIKHOSHVA ALL RIGHT RESERVED |                                           ");
            body.AppendLine("-- | https://www.linkedin.com/in/olexandrlikhoshva/    |                                           ");
            body.AppendLine("-- +---------------------------------------------------+                                           ");
            body.AppendLine("-- SET NOCOUNT ON added to prevent extra result sets from                                          ");
            body.AppendLine("-- interfering with SELECT statements.                                                             ");
            body.AppendLine("                                                                                                   ");
            body.AppendLine("--SET NOCOUNT ON;                                                                                  ");
            body.AppendLine("                                                                                                   ");
            body.AppendLine("-- BrandItemEntity = 1                                                                             ");
            body.AppendLine("-- CatalogItemEntity = 2                                                                           ");
            body.AppendLine("-- DirectoryEntity = 3                                                                             ");
            body.AppendLine("-- PhotoItemEntity = 5                                                                             ");
            body.AppendLine("-- ProductDirectionEntity = 6                                                                      ");
            body.AppendLine("                                                                                                   ");
            body.AppendLine("declare @entityName int = 1;                                                                       ");
            body.AppendLine("declare @dateOfCreation datetimeoffset(7) = Sysdatetimeoffset();                                   ");
            body.AppendLine("                                                                                                   ");
            body.AppendLine("INSERT INTO [dbo].[SendItemsEntities]                                                              ");
            body.AppendLine("([Login], [EntityId], [Brands].[EntityName], [Brands].[RequestDate], [Brands].[DateOfCreation])    ");
            body.AppendLine("SELECT @login, [Id], @entityName, @lastUpdate, @dateOfCreation                                     ");
            body.AppendLine("FROM [dbo].[BrandItemEntities] AS [Brands]                                                         ");
            body.AppendLine("WHERE [Brands].[LastUpdated] > @lastUpdate AND                                                     ");
            body.AppendLine("      NOT EXISTS (SELECT *                                                                         ");
            body.AppendLine("                  FROM  [dbo].[SendItemsEntities] AS [SendItem]                                    ");
            body.AppendLine("                  WHERE [SendItem].[EntityId] = [Brands].[Id] AND                                  ");
            body.AppendLine("                        [SendItem].[Login] = @login AND                                            ");
            body.AppendLine("                        [SendItem].[EntityName] = @entityName);                                    ");
            body.AppendLine("                                                                                                   ");
            body.AppendLine("SELECT @countToUpdate = COUNT(*)                                                                   ");
            body.AppendLine("FROM  [dbo].[SendItemsEntities]                                                                    ");
            body.AppendLine("WHERE [Login] = @login AND                                                                         ");
            body.AppendLine("      [EntityName] = @entityName;                                                                  ");
            body.AppendLine("                                                                                                   ");
            body.AppendLine("RETURN (@countToUpdate);                                                                           ");

            return body.ToString();
        }

        private string CreatePrepareToUpdateCatalogsOldBody()
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine("-- +---------------------------------------------------+                                              ");
            body.AppendLine("-- | © 2017-2018 OLEXANDR LIKHOSHVA ALL RIGHT RESERVED |                                              ");
            body.AppendLine("-- | https://www.linkedin.com/in/olexandrlikhoshva/    |                                              ");
            body.AppendLine("-- +---------------------------------------------------+                                              ");
            body.AppendLine("-- SET NOCOUNT ON added to prevent extra result sets from                                             ");
            body.AppendLine("-- interfering with SELECT statements.                                                                ");
            body.AppendLine("                                                                                                      ");
            body.AppendLine("--SET NOCOUNT ON;                                                                                     ");
            body.AppendLine("                                                                                                      ");
            body.AppendLine("-- BrandItemEntity = 1                                                                                ");
            body.AppendLine("-- CatalogItemEntity = 2                                                                              ");
            body.AppendLine("-- DirectoryEntity = 3                                                                                ");
            body.AppendLine("-- PhotoItemEntity = 5                                                                                ");
            body.AppendLine("-- ProductDirectionEntity = 6                                                                         ");
            body.AppendLine("                                                                                                      ");
            body.AppendLine("declare @entityName int = 2;                                                                          ");
            body.AppendLine("declare @dateOfCreation datetimeoffset(7) = Sysdatetimeoffset();                                      ");
            body.AppendLine("                                                                                                      ");
            body.AppendLine("INSERT INTO [dbo].[SendItemsEntities]                                                                 ");
            body.AppendLine("([Login], [EntityId], [Catalogs].[EntityName], [Catalogs].[RequestDate], [Catalogs].[DateOfCreation]) ");
            body.AppendLine("SELECT @login, [Id], @entityName, @lastUpdate, @dateOfCreation                                        ");
            body.AppendLine("FROM [dbo].[CatalogItemEntities] AS [Catalogs]                                                        ");
            body.AppendLine("WHERE [Catalogs].[LastUpdated] > @lastUpdate AND                                                      ");
            body.AppendLine("      NOT EXISTS (SELECT *                                                                            ");
            body.AppendLine("                  FROM  [dbo].[SendItemsEntities] AS [SendItem]                                       ");
            body.AppendLine("                  WHERE [SendItem].[EntityId] = [Catalogs].[Id] AND                                   ");
            body.AppendLine("                        [SendItem].[Login] = @login AND                                               ");
            body.AppendLine("                        [SendItem].[EntityName] = @entityName);                                       ");
            body.AppendLine("                                                                                                      ");
            body.AppendLine("SELECT @countToUpdate = COUNT(*)                                                                      ");
            body.AppendLine("FROM  [dbo].[SendItemsEntities]                                                                       ");
            body.AppendLine("WHERE [Login] = @login AND                                                                            ");
            body.AppendLine("      [EntityName] = @entityName;                                                                     ");
            body.AppendLine("                                                                                                      ");
            body.AppendLine("RETURN (@countToUpdate);                                                                              ");

            return body.ToString();
        }

        private string CreatePrepareToUpdateDirectoriesOldBody()
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine("-- +---------------------------------------------------+                                                       ");
            body.AppendLine("-- | © 2017-2018 OLEXANDR LIKHOSHVA ALL RIGHT RESERVED |                                                       ");
            body.AppendLine("-- | https://www.linkedin.com/in/olexandrlikhoshva/    |                                                       ");
            body.AppendLine("-- +---------------------------------------------------+                                                       ");
            body.AppendLine("-- SET NOCOUNT ON added to prevent extra result sets from                                                      ");
            body.AppendLine("-- interfering with SELECT statements.                                                                         ");
            body.AppendLine("                                                                                                               ");
            body.AppendLine("--SET NOCOUNT ON;                                                                                              ");
            body.AppendLine("                                                                                                               ");
            body.AppendLine("-- BrandItemEntity = 1                                                                                         ");
            body.AppendLine("-- CatalogItemEntity = 2                                                                                       ");
            body.AppendLine("-- DirectoryEntity = 3                                                                                         ");
            body.AppendLine("-- PhotoItemEntity = 5                                                                                         ");
            body.AppendLine("-- ProductDirectionEntity = 6                                                                                  ");
            body.AppendLine("                                                                                                               ");
            body.AppendLine("declare @entityName int = 3;                                                                                   ");
            body.AppendLine("declare @dateOfCreation datetimeoffset(7) = Sysdatetimeoffset();                                               ");
            body.AppendLine("                                                                                                               ");
            body.AppendLine("INSERT INTO [dbo].[SendItemsEntities]                                                                          ");
            body.AppendLine("([Login], [EntityId], [Directories].[EntityName], [Directories].[RequestDate], [Directories].[DateOfCreation]) ");
            body.AppendLine("SELECT @login, [Id], @entityName, @lastUpdate, @dateOfCreation                                                 ");
            body.AppendLine("FROM [dbo].[DirectoryEntities] AS [Directories]                                                                ");
            body.AppendLine("WHERE [Directories].[LastUpdated] > @lastUpdate AND                                                            ");
            body.AppendLine("      NOT EXISTS (SELECT *                                                                                     ");
            body.AppendLine("                  FROM  [dbo].[SendItemsEntities] AS [SendItem]                                                ");
            body.AppendLine("                  WHERE [SendItem].[EntityId] = [Directories].[Id] AND                                         ");
            body.AppendLine("                        [SendItem].[Login] = @login AND                                                        ");
            body.AppendLine("                        [SendItem].[EntityName] = @entityName);                                                ");
            body.AppendLine("                                                                                                               ");
            body.AppendLine("SELECT @countToUpdate = COUNT(*)                                                                               ");
            body.AppendLine("FROM  [dbo].[SendItemsEntities]                                                                                ");
            body.AppendLine("WHERE [Login] = @login AND                                                                                     ");
            body.AppendLine("      [EntityName] = @entityName;                                                                              ");
            body.AppendLine("                                                                                                               ");
            body.AppendLine("RETURN (@countToUpdate);                                                                                       ");

            return body.ToString();
        }

        private string CreatePrepareToUpdateProductDirectionsOldBody()
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine("-- +---------------------------------------------------+                                                                                        ");
            body.AppendLine("-- | © 2017-2018 OLEXANDR LIKHOSHVA ALL RIGHT RESERVED |                                                                                        ");
            body.AppendLine("-- | https://www.linkedin.com/in/olexandrlikhoshva/    |                                                                                        ");
            body.AppendLine("-- +---------------------------------------------------+                                                                                        ");
            body.AppendLine("-- SET NOCOUNT ON added to prevent extra result sets from                                                                                       ");
            body.AppendLine("-- interfering with SELECT statements.                                                                                                          ");
            body.AppendLine("                                                                                                                                                ");
            body.AppendLine("--SET NOCOUNT ON;                                                                                                                                 ");
            body.AppendLine("                                                                                                                                                ");
            body.AppendLine("-- BrandItemEntity = 1                                                                                                                          ");
            body.AppendLine("-- CatalogItemEntity = 2                                                                                                                        ");
            body.AppendLine("-- DirectoryEntity = 3                                                                                                                          ");
            body.AppendLine("-- PhotoItemEntity = 5                                                                                                                          ");
            body.AppendLine("-- ProductDirectionEntity = 6                                                                                                                   ");
            body.AppendLine("                                                                                                                                                ");
            body.AppendLine("declare @entityName int = 6;                                                                                                                    ");
            body.AppendLine("declare @dateOfCreation datetimeoffset(7) = Sysdatetimeoffset();                                                                                ");
            body.AppendLine("                                                                                                                                                ");
            body.AppendLine("INSERT INTO [dbo].[SendItemsEntities]                                                                                                           ");
            body.AppendLine("([Login], [EntityId], [ProductDirectionEntity].[EntityName], [ProductDirectionEntity].[RequestDate], [ProductDirectionEntity].[DateOfCreation]) ");
            body.AppendLine("SELECT @login, [Id], @entityName, @lastUpdate, @dateOfCreation                                                                                  ");
            body.AppendLine("FROM [dbo].[ProductDirectionEntities] AS [ProductDirectionEntity]                                                                                ");
            body.AppendLine("WHERE [ProductDirectionEntity].[LastUpdated] > @lastUpdate AND                                                                                  ");
            body.AppendLine("      NOT EXISTS (SELECT *                                                                                                                      ");
            body.AppendLine("                  FROM  [dbo].[SendItemsEntities] AS [SendItem]                                                                                 ");
            body.AppendLine("                  WHERE [SendItem].[EntityId] = [ProductDirectionEntity].[Id] AND                                                               ");
            body.AppendLine("                        [SendItem].[Login] = @login AND                                                                                         ");
            body.AppendLine("                        [SendItem].[EntityName] = @entityName);                                                                                 ");
            body.AppendLine("                                                                                                                                                ");
            body.AppendLine("SELECT @countToUpdate = COUNT(*)                                                                                                                ");
            body.AppendLine("FROM  [dbo].[SendItemsEntities]                                                                                                                 ");
            body.AppendLine("WHERE [Login] = @login AND                                                                                                                      ");
            body.AppendLine("      [EntityName] = @entityName;                                                                                                               ");
            body.AppendLine("                                                                                                                                                ");
            body.AppendLine("RETURN (@countToUpdate);                                                                                                                        ");

            return body.ToString();
        }

        private string CreatePrepareToUpdatePhotosOldProcedure()
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine("USE [ServerPriceList]                                                                                  ");
            body.AppendLine("GO                                                                                                     ");
            body.AppendLine("                                                                                                       ");
            body.AppendLine("SET ANSI_NULLS ON                                                                                      ");
            body.AppendLine("GO                                                                                                     ");
            body.AppendLine("SET QUOTED_IDENTIFIER ON                                                                               ");
            body.AppendLine("GO                                                                                                     ");
            body.AppendLine("CREATE PROCEDURE [dbo].[PrepareToUpdatePhotos]                                                         ");
            body.AppendLine("    @login [nvarchar](30),                                                                             ");
            body.AppendLine("    @lastUpdate [datetimeoffset](7),                                                                   ");
            body.AppendLine("    @ids [dbo].[bigintTable] READONLY,                                                                 ");
            body.AppendLine("    @countToUpdate [int] OUT                                                                           ");
            body.AppendLine("AS                                                                                                     ");
            body.AppendLine("BEGIN                                                                                                  ");
            body.AppendLine("    -- +---------------------------------------------------+                                           ");
            body.AppendLine("    -- | © 2017-2018 OLEXANDR LIKHOSHVA ALL RIGHT RESERVED |                                           ");
            body.AppendLine("    -- | https://www.linkedin.com/in/olexandrlikhoshva/    |                                           ");
            body.AppendLine("    -- +---------------------------------------------------+                                           ");
            body.AppendLine("    -- SET NOCOUNT ON added to prevent extra result sets from                                          ");
            body.AppendLine("    -- interfering with SELECT statements.                                                             ");
            body.AppendLine("                                                                                                       ");
            body.AppendLine("    --SET NOCOUNT ON;                                                                                  ");
            body.AppendLine("                                                                                                       ");
            body.AppendLine("    -- BrandItemEntity = 1                                                                             ");
            body.AppendLine("    -- CatalogItemEntity = 2                                                                           ");
            body.AppendLine("    -- DirectoryEntity = 3                                                                             ");
            body.AppendLine("    -- PhotoItemEntity = 5                                                                             ");
            body.AppendLine("    -- ProductDirectionEntity = 6                                                                      ");
            body.AppendLine("                                                                                                       ");
            body.AppendLine("   declare @entityName int = 5;                                                                        ");
            body.AppendLine("   declare @dateOfCreation datetimeoffset(7) = Sysdatetimeoffset();                                    ");
            body.AppendLine("                                                                                                       ");
            body.AppendLine("   INSERT INTO[dbo].[SendItemsEntities]                                                                ");
            body.AppendLine("   ([Login], [EntityId], [Photos].[EntityName], [Photos].[RequestDate], [Photos].[DateOfCreation])     ");
            body.AppendLine("   SELECT @login, [Id], @entityName, @lastUpdate, @dateOfCreation                                      ");
            body.AppendLine("   FROM [dbo].[PhotoItemEntities] AS [Photos]                                                          ");
            body.AppendLine("   WHERE[Photos].[Id] IN(SELECT Id FROM @ids) AND                                                      ");
            body.AppendLine("   [Photos].[IsLoad] = 1 AND                                                                           ");
            body.AppendLine("   NOT EXISTS (SELECT *                                                                                ");
            body.AppendLine("   FROM  [dbo].[SendItemsEntities] AS [SendItem]                                                       ");
            body.AppendLine("   WHERE [SendItem].[EntityId] = [Photos].[Id] AND                                                     ");
            body.AppendLine("   [SendItem].[Login] = @login AND                                                                     ");
            body.AppendLine("   [SendItem].[EntityName] = @entityName);                                                             ");
            body.AppendLine("                                                                                                       ");
            body.AppendLine("   INSERT INTO [dbo].[SendItemsEntities]                                                               ");
            body.AppendLine("   ([Login], [EntityId], [Photos].[EntityName], [Photos].[RequestDate], [Photos].[DateOfCreation])     ");
            body.AppendLine("   SELECT @login, [Id], @entityName, @lastUpdate, @dateOfCreation                                      ");
            body.AppendLine("   FROM [dbo].[PhotoItemEntities] AS [Photos]                                                          ");
            body.AppendLine("   WHERE [Photos].[LastUpdated] > @lastUpdate AND                                                      ");
            body.AppendLine("         NOT EXISTS (SELECT *                                                                          ");
            body.AppendLine("                     FROM  [dbo].[SendItemsEntities] AS [SendItem]                                     ");
            body.AppendLine("                     WHERE [SendItem].[EntityId] = [Photos].[Id] AND                                   ");
            body.AppendLine("                           [SendItem].[Login] = @login AND                                             ");
            body.AppendLine("                           [SendItem].[EntityName] = @entityName);                                     ");
            body.AppendLine("                                                                                                       ");
            body.AppendLine("   SELECT @countToUpdate = COUNT(*)                                                                    ");
            body.AppendLine("   FROM  [dbo].[SendItemsEntities]                                                                     ");
            body.AppendLine("   WHERE [Login] = @login AND                                                                          ");
            body.AppendLine("         [EntityName] = @entityName;                                                                   ");
            body.AppendLine("                                                                                                       ");
            body.AppendLine("   RETURN (@countToUpdate);                                                                            ");
            body.AppendLine("END                                                                                                    ");

            return body.ToString();
        }

    }
}
