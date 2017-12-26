using System.Text;

namespace DataBase.EntitiesMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class InitialEntities : DbMigration
    {
        public override void Up()
        {
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

            CreateStoredProcedure(
                "dbo.PrepareToUpdatePhotos",
                p => new
                {
                    login = p.String(maxLength: 30),
                    lastUpdate = p.DateTimeOffset(precision: 7),
                    countToUpdate = p.Int(outParameter: true)
                },
                body: CreatePrepareToUpdatePhotosBody());

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
        }

        private string CreatePrepareToUpdateBrandsBody()
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
            body.AppendLine("FROM [dbo].[BrandItemEntities] AS [Brands]                                                        ");
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

        private string CreatePrepareToUpdateCatalogsBody()
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine("-- +---------------------------------------------------+                                              ");
            body.AppendLine("-- | © 2017-2018 OLEXANDR LIKHOSHVA ALL RIGHT RESERVED |                                              ");
            body.AppendLine("-- | https://www.linkedin.com/in/olexandrlikhoshva/    |                                              ");
            body.AppendLine("-- +---------------------------------------------------+                                              ");
            body.AppendLine("-- SET NOCOUNT ON added to prevent extra result sets from                                             ");
            body.AppendLine("-- interfering with SELECT statements.                                                                ");
            body.AppendLine("                                                                                                      ");
            body.AppendLine("--SET NOCOUNT ON;                                                                                       ");
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
            body.AppendLine("FROM [dbo].[CatalogItemEntities] AS [Catalogs]                                                       ");
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

        private string CreatePrepareToUpdateDirectoriesBody()
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine("-- +---------------------------------------------------+                                                       ");
            body.AppendLine("-- | © 2017-2018 OLEXANDR LIKHOSHVA ALL RIGHT RESERVED |                                                       ");
            body.AppendLine("-- | https://www.linkedin.com/in/olexandrlikhoshva/    |                                                       ");
            body.AppendLine("-- +---------------------------------------------------+                                                       ");
            body.AppendLine("-- SET NOCOUNT ON added to prevent extra result sets from                                                      ");
            body.AppendLine("-- interfering with SELECT statements.                                                                         ");
            body.AppendLine("                                                                                                               ");
            body.AppendLine("--SET NOCOUNT ON;                                                                                                ");
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

        private string CreatePrepareToUpdatePhotosBody()
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
            body.AppendLine("declare @entityName int = 5;                                                                          ");
            body.AppendLine("declare @dateOfCreation datetimeoffset(7) = Sysdatetimeoffset();                                      ");
            body.AppendLine("                                                                                                      ");
            body.AppendLine("INSERT INTO [dbo].[SendItemsEntities]                                                                 ");
            body.AppendLine("([Login], [EntityId], [Photos].[EntityName], [Photos].[RequestDate], [Photos].[DateOfCreation])       ");
            body.AppendLine("SELECT @login, [Id], @entityName, @lastUpdate, @dateOfCreation                                        ");
            body.AppendLine("FROM [dbo].[PhotoItemEntities] AS [Photos]                                                            ");
            body.AppendLine("WHERE [Photos].[LastUpdated] > @lastUpdate AND                                                        ");
            body.AppendLine("      NOT EXISTS (SELECT *                                                                            ");
            body.AppendLine("                  FROM  [dbo].[SendItemsEntities] AS [SendItem]                                       ");
            body.AppendLine("                  WHERE [SendItem].[EntityId] = [Photos].[Id] AND                                     ");
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

        private string CreatePrepareToUpdateProductDirectionsBody()
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
    }
}
