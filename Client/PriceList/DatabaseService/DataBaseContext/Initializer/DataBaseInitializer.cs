using System.Collections.Generic;
using System.Data.Entity;
using System.Text;
using Common.Data.Constant;
using DatabaseService.DataBaseContext.Entities;

namespace DatabaseService.DataBaseContext.Initializer
{
    public class DataBaseInitializer : DropCreateDatabaseIfModelChanges<DataBaseContext>
    {
        protected override void Seed(DataBaseContext dataBaseContext)
        {
            dataBaseContext.Database.ExecuteSqlCommand(CreateCatalogsTableType());
            dataBaseContext.Database.ExecuteSqlCommand(CreateLinkToPhotoTableType());
            dataBaseContext.Database.ExecuteSqlCommand(CreateDirectoriesTableType());
            dataBaseContext.Database.ExecuteSqlCommand(CreateProductDirectionsTableType());
            dataBaseContext.Database.ExecuteSqlCommand(CreateBrandsTableType());

            dataBaseContext.Database.ExecuteSqlCommand(CreateUpdateCatalogsProcedure());
            dataBaseContext.Database.ExecuteSqlCommand(CreateUpdateDirectoriesProcedure());
            dataBaseContext.Database.ExecuteSqlCommand(CreateUpdateProductDirectionsProcedure());
            dataBaseContext.Database.ExecuteSqlCommand(CreateUpdateBrandsProcedure());

            // Отключаем отслеживание и проверку изменений для оптимизации вставки множества полей
            dataBaseContext.Configuration.AutoDetectChangesEnabled = false;
            dataBaseContext.Configuration.ValidateOnSaveEnabled = false;

            PopulateOptionItemEntities(dataBaseContext);

            dataBaseContext.Configuration.AutoDetectChangesEnabled = true;
            dataBaseContext.Configuration.ValidateOnSaveEnabled = true;

            base.Seed(dataBaseContext);
        }

        private string CreateCatalogsTableType()
        {
            StringBuilder type = new StringBuilder();

            type.AppendLine("CREATE TYPE [dbo].[catalogsTable] AS TABLE(");
            type.AppendLine("	[Id] [bigint] NOT NULL,");
            type.AppendLine("	[UID] [uniqueidentifier] NOT NULL,");
            type.AppendLine("	[Code] [nvarchar](30) NULL,");
            type.AppendLine("	[Article] [nvarchar](30) NULL,");
            type.AppendLine("	[Name] [nvarchar](255) NULL,");
            type.AppendLine("	[BrandName] [nvarchar](255) NULL,");
            type.AppendLine("	[Unit] [nvarchar](10) NULL,");
            type.AppendLine("	[EnterpriceNormPack] [nvarchar](30) NULL,");
            type.AppendLine("	[BatchOfSales] [decimal](18, 2) NOT NULL,");
            type.AppendLine("	[Balance] [nvarchar](30) NULL,");
            type.AppendLine("	[Price] [decimal](18, 2) NOT NULL,");
            type.AppendLine("	[Currency] [nvarchar](5) NULL,");
            type.AppendLine("	[Multiplicity] [decimal](18, 2) NOT NULL,");
            type.AppendLine("	[HasPhotos] [bit] NOT NULL,");
            type.AppendLine("	[Status] [int] NOT NULL,");
            type.AppendLine("	[LastUpdatedStatus] [datetimeoffset](7) NOT NULL,");
            type.AppendLine("	[DateOfCreation] [datetimeoffset](7) NOT NULL,");
            type.AppendLine("	[LastUpdated] [datetimeoffset](7) NOT NULL,");
            type.AppendLine("	[ForceUpdated] [datetimeoffset](7) NOT NULL,");
            type.AppendLine("	[Brand_Id] [bigint] NULL,");
            type.AppendLine("	[Directory_Id] [bigint] NULL,");
            type.AppendLine("	PRIMARY KEY ([Id])");
            type.AppendLine(")");

            return type.ToString();
        }

        private string CreateLinkToPhotoTableType()
        {
            StringBuilder type = new StringBuilder();

            type.AppendLine("CREATE TYPE [dbo].[linkToPhotoTable] AS TABLE(");
            type.AppendLine("	[Catalog_Id] [bigint] NOT NULL,");
            type.AppendLine("	[Photo_Id] [bigint] NOT NULL,");
            type.AppendLine("	PRIMARY KEY ([Photo_Id])");
            type.AppendLine(")");

            return type.ToString();
        }

        private string CreateDirectoriesTableType()
        {
            StringBuilder type = new StringBuilder();

            type.AppendLine("CREATE TYPE [dbo].[directoriesTable]  AS TABLE");
            type.AppendLine("( ");
            type.AppendLine("	[Id] [bigint] NOT NULL,");
            type.AppendLine("	[Code] [uniqueidentifier] NOT NULL,");
            type.AppendLine("	[Name] [nvarchar](255) NULL,");
            type.AppendLine("	[Parent_Id] [bigint] NULL,");
            type.AppendLine("	[DateOfCreation] [datetimeoffset](7) NOT NULL,");
            type.AppendLine("	[LastUpdated] [datetimeoffset](7) NOT NULL,");
            type.AppendLine("	[ForceUpdated] [datetimeoffset](7) NOT NULL,");
            type.AppendLine("    PRIMARY KEY ([Id])");
            type.AppendLine(")");

            return type.ToString();
        }

        private string CreateProductDirectionsTableType()
        {
            StringBuilder type = new StringBuilder();

            type.AppendLine("CREATE TYPE [dbo].[productDirectionsTable] AS TABLE(");
            type.AppendLine("      [Id] [bigint] NOT NULL,");
            type.AppendLine("      [Direction] [int] NOT NULL,");
            type.AppendLine("      [Directory_Id] [bigint] NULL,");
            type.AppendLine("      [DateOfCreation] [datetimeoffset](7) NOT NULL,");
            type.AppendLine("      [LastUpdated] [datetimeoffset](7) NOT NULL,");
            type.AppendLine("      [ForceUpdated] [datetimeoffset](7) NOT NULL,");
            type.AppendLine("      PRIMARY KEY ([Id])");
            type.AppendLine(")");

            return type.ToString();
        }

        private string CreateBrandsTableType()
        {
            StringBuilder type = new StringBuilder();

            type.AppendLine("CREATE TYPE [dbo].[brandsTable] AS TABLE");
            type.AppendLine("(");
            type.AppendLine("      [Id] [bigint] NOT NULL,");
            type.AppendLine("      [Code] [uniqueidentifier] NOT NULL,");
            type.AppendLine("      [Name] [nvarchar](255) NULL,");
            type.AppendLine("      [DateOfCreation] [datetimeoffset](7) NOT NULL,");
            type.AppendLine("      [LastUpdated] [datetimeoffset](7) NOT NULL,");
            type.AppendLine("      [ForceUpdated] [datetimeoffset](7) NOT NULL,");
            type.AppendLine("      PRIMARY KEY ([Id])");
            type.AppendLine(")");

            return type.ToString();
        }

        private string CreateUpdateCatalogsProcedure()
        {
            StringBuilder procedure = new StringBuilder();

            procedure.AppendLine("CREATE PROCEDURE [dbo].[UpdateCatalogs]");
            procedure.AppendLine("	@catalogs [dbo].[catalogsTable] READONLY,");
            procedure.AppendLine("	@linkToPhotos [dbo].[linkToPhotoTable] READONLY");
            procedure.AppendLine("AS");
            procedure.AppendLine("BEGIN");
            procedure.AppendLine(" ");
            procedure.AppendLine("--    +---------------------------------------------------+");
            procedure.AppendLine("--    | Author:       OLEXANDR LIKHOSHVA                  |");
            procedure.AppendLine("--    | Create date:  2018.02.11 23:45                    |");
            procedure.AppendLine("--    | Copyright:    © 2017-2018  ALL RIGHT RESERVED     |");
            procedure.AppendLine("--    | https://www.linkedin.com/in/olexandrlikhoshva/    |");
            procedure.AppendLine("--    |---------------------------------------------------|");
            procedure.AppendLine("--    | Description:  Procedure for insert or update      |");
            procedure.AppendLine("--    |               CatalogItemEntities                 |");
            procedure.AppendLine("--    +---------------------------------------------------+");
            procedure.AppendLine("--    SET NOCOUNT ON added to prevent extra result sets from");
            procedure.AppendLine("--    interfering with SELECT statements.");
            procedure.AppendLine("--    SET NOCOUNT ON;");
            procedure.AppendLine(" ");
            procedure.AppendLine("	WITH [CatalogForUpdate] AS (SELECT * FROM [CatalogItemEntities] AS [CatalogItems]");
            procedure.AppendLine("	                            WHERE [CatalogItems].[Id] IN (SELECT Id FROM @catalogs))");
            procedure.AppendLine("	UPDATE [ItemsForUpdate]");
            procedure.AppendLine("	SET [ItemsForUpdate].[UID] = [UpdateItems].[UID],");
            procedure.AppendLine("	    [ItemsForUpdate].[Code] = [UpdateItems].[Code],");
            procedure.AppendLine("          [ItemsForUpdate].[Article] = [UpdateItems].[Article],");
            procedure.AppendLine("          [ItemsForUpdate].[Name] = [UpdateItems].[Name],");
            procedure.AppendLine("          [ItemsForUpdate].[Brand_Id] = [BrandItems].[Id],");
            procedure.AppendLine("          [ItemsForUpdate].[BrandName] = [BrandItems].[Name],");
            procedure.AppendLine("          [ItemsForUpdate].[Unit] = [UpdateItems].[Unit],");
            procedure.AppendLine("          [ItemsForUpdate].[EnterpriceNormPack] = [UpdateItems].[EnterpriceNormPack],");
            procedure.AppendLine("          [ItemsForUpdate].[BatchOfSales] = [UpdateItems].[BatchOfSales],");
            procedure.AppendLine("          [ItemsForUpdate].[Balance] = [UpdateItems].[Balance],");
            procedure.AppendLine("          [ItemsForUpdate].[Price] = [UpdateItems].[Price],");
            procedure.AppendLine("          [ItemsForUpdate].[Currency] = [UpdateItems].[Currency],");
            procedure.AppendLine("          [ItemsForUpdate].[Multiplicity] = [UpdateItems].[Multiplicity],");
            procedure.AppendLine("          [ItemsForUpdate].[HasPhotos] = [UpdateItems].[HasPhotos],");
            procedure.AppendLine("          [ItemsForUpdate].[DateOfCreation] = [UpdateItems].[DateOfCreation],");
            procedure.AppendLine("          [ItemsForUpdate].[LastUpdated] = [UpdateItems].[LastUpdated],");
            procedure.AppendLine("          [ItemsForUpdate].[ForceUpdated] = [UpdateItems].[ForceUpdated],");
            procedure.AppendLine("          [ItemsForUpdate].[Status] = [UpdateItems].[Status],");
            procedure.AppendLine("          [ItemsForUpdate].[LastUpdatedStatus] = [UpdateItems].[LastUpdatedStatus],");
            procedure.AppendLine("          [ItemsForUpdate].[Directory_Id] = [Directories].[Id]");
            procedure.AppendLine("      FROM [CatalogForUpdate] AS [ItemsForUpdate]");
            procedure.AppendLine("      INNER JOIN @catalogs AS [UpdateItems] ON [ItemsForUpdate].[Id] = [UpdateItems].[Id]");
            procedure.AppendLine("      LEFT JOIN [BrandItemEntities] AS [BrandItems] ON [UpdateItems].[Brand_Id] = [BrandItems].[Id]");
            procedure.AppendLine("      LEFT JOIN [DirectoryEntities] AS [Directories] ON [UpdateItems].[Directory_Id] = [Directories].[Id];");
            procedure.AppendLine(" ");
            procedure.AppendLine("      INSERT INTO  [CatalogItemEntities]");
            procedure.AppendLine("      ([Id], [UID], [Code], [Article], [Name], [Brand_Id], [BrandName], [Unit], [EnterpriceNormPack],");
            procedure.AppendLine("       [BatchOfSales], [Balance], [Price], [Currency], [Multiplicity], [HasPhotos], [DateOfCreation],");
            procedure.AppendLine("       [LastUpdated], [ForceUpdated], [Status], [LastUpdatedStatus], [Directory_Id])");
            procedure.AppendLine("      SELECT [UpdateItems].[Id], [UpdateItems].[UID], [UpdateItems].[Code], [UpdateItems].[Article],");
            procedure.AppendLine("             [UpdateItems].[Name], [BrandItems].[Id], [BrandItems].[Name], [UpdateItems].[Unit],");
            procedure.AppendLine("             [UpdateItems].[EnterpriceNormPack], [UpdateItems].[BatchOfSales], [UpdateItems].[Balance],");
            procedure.AppendLine("             [UpdateItems].[Price], [UpdateItems].[Currency], [UpdateItems].[Multiplicity],");
            procedure.AppendLine("             [UpdateItems].[HasPhotos], [UpdateItems].[DateOfCreation], [UpdateItems].[LastUpdated],");
            procedure.AppendLine("             [UpdateItems].[ForceUpdated], [UpdateItems].[Status], [UpdateItems].[LastUpdatedStatus],");
            procedure.AppendLine("             [Directories].[Id]");
            procedure.AppendLine("      FROM @catalogs AS [UpdateItems]");
            procedure.AppendLine("      LEFT JOIN [BrandItemEntities] AS [BrandItems] ON [UpdateItems].[Brand_Id] = [BrandItems].[Id]");
            procedure.AppendLine("      LEFT JOIN [DirectoryEntities] AS [Directories] ON [UpdateItems].[Directory_Id] = [Directories].[Id]");
            procedure.AppendLine("      WHERE [UpdateItems].[Id] NOT IN (SELECT [CatalogItems].[Id] FROM [CatalogItemEntities] AS [CatalogItems]);");
            procedure.AppendLine(" ");
            procedure.AppendLine("      UPDATE [PhotoItems]");
            procedure.AppendLine("      SET [PhotoItems].[CatalogItem_Id] = [Link].[Catalog_Id]");
            procedure.AppendLine("      FROM [PhotoItemEntities] AS [PhotoItems]");
            procedure.AppendLine("      INNER JOIN @linkToPhotos AS [Link] ON [PhotoItems].[Id] = [Link].[Photo_Id];");
            procedure.AppendLine(" ");
            procedure.AppendLine("END");

            return procedure.ToString();
        }

        private string CreateUpdateDirectoriesProcedure()
        {
            StringBuilder procedure = new StringBuilder();

            procedure.AppendLine("CREATE PROCEDURE [dbo].[UpdateDirectories]");
            procedure.AppendLine("	@directories [dbo].[directoriesTable] READONLY");
            procedure.AppendLine("AS");
            procedure.AppendLine("BEGIN");
            procedure.AppendLine("--    +---------------------------------------------------+");
            procedure.AppendLine("--    | Author:       OLEXANDR LIKHOSHVA                  |");
            procedure.AppendLine("--    | Create date:  2018.02.25 01:15                    |");
            procedure.AppendLine("--    | Copyright:    © 2017-2018  ALL RIGHT RESERVED     |");
            procedure.AppendLine("--    | https://www.linkedin.com/in/olexandrlikhoshva/    |");
            procedure.AppendLine("--    |---------------------------------------------------|");
            procedure.AppendLine("--    | Description:  Procedure for insert or update      |");
            procedure.AppendLine("--    |               DirectoryEntities                   |");
            procedure.AppendLine("--    +---------------------------------------------------+");
            procedure.AppendLine("--    SET NOCOUNT ON added to prevent extra result sets from");
            procedure.AppendLine("--    interfering with SELECT statements.");
            procedure.AppendLine("--    SET NOCOUNT ON;");
            procedure.AppendLine(" ");
            procedure.AppendLine("      WITH [DirectoriesForUpdate] AS (SELECT * FROM [DirectoryEntities] AS [DirectoryItems]");
            procedure.AppendLine("                                  WHERE [DirectoryItems].[Id] IN (SELECT [Id] FROM @directories))");
            procedure.AppendLine("      UPDATE [ItemsForUpdate]");
            procedure.AppendLine("      SET [ItemsForUpdate].[Code] = [UpdateItems].[Code],");
            procedure.AppendLine("          [ItemsForUpdate].[Name] = [UpdateItems].[Name],");
            procedure.AppendLine("          [ItemsForUpdate].[Parent_Id] = [UpdateItems].[Parent_Id],");
            procedure.AppendLine("          [ItemsForUpdate].[DateOfCreation] = [UpdateItems].[DateOfCreation],");
            procedure.AppendLine("          [ItemsForUpdate].[LastUpdated] = [UpdateItems].[LastUpdated],");
            procedure.AppendLine("          [ItemsForUpdate].[ForceUpdated] = [UpdateItems].[ForceUpdated]");
            procedure.AppendLine("      FROM [DirectoriesForUpdate] AS [ItemsForUpdate]");
            procedure.AppendLine("      INNER JOIN @directories AS [UpdateItems] ON [ItemsForUpdate].[Id] = [UpdateItems].[Id];");
            procedure.AppendLine(" ");
            procedure.AppendLine("      INSERT INTO  [DirectoryEntities]");
            procedure.AppendLine("      ([Id], [Code], [Name], [Parent_Id], [DateOfCreation], [LastUpdated], [ForceUpdated])");
            procedure.AppendLine("      SELECT [InsertItems].[Id], [InsertItems].[Code], [InsertItems].[Name], [InsertItems].[Parent_Id],");
            procedure.AppendLine("             [InsertItems].[DateOfCreation], [InsertItems].[LastUpdated], [InsertItems].[ForceUpdated]");
            procedure.AppendLine("      FROM @directories AS [InsertItems]");
            procedure.AppendLine("      WHERE [InsertItems].[Id] NOT IN (SELECT [DirectoryItems].[Id] FROM [DirectoryEntities] AS [DirectoryItems]);");
            procedure.AppendLine(" ");
            procedure.AppendLine("END");

            return procedure.ToString();
        }

        private string CreateUpdateProductDirectionsProcedure()
        {
            StringBuilder procedure = new StringBuilder();

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
            procedure.AppendLine("    INNER JOIN @productDirections AS [UpdateItems] ON [ItemsForUpdate].[Id] = [UpdateItems].[Id];");
            procedure.AppendLine(" ");
            procedure.AppendLine("    INSERT INTO  [ProductDirectionEntities]");
            procedure.AppendLine("        ([Id], [Direction], [Directory_Id], [DateOfCreation], [LastUpdated], [ForceUpdated])");
            procedure.AppendLine("    SELECT [InsertItems].[Id], [InsertItems].[Direction], [InsertItems].[Directory_Id],");
            procedure.AppendLine("           [InsertItems].[DateOfCreation], [InsertItems].[LastUpdated], [InsertItems].[ForceUpdated]");
            procedure.AppendLine("    FROM @productDirections AS [InsertItems]");
            procedure.AppendLine("    WHERE [InsertItems].[Id] NOT IN (SELECT [ProductDirectionItems].[Id]");
            procedure.AppendLine("                                     FROM [ProductDirectionEntities] AS [ProductDirectionItems]);");
            procedure.AppendLine(" ");
            procedure.AppendLine("END");

            return procedure.ToString();
        }

        private string CreateUpdateBrandsProcedure()
        {
            StringBuilder procedure = new StringBuilder();

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
            procedure.AppendLine("      INNER JOIN @brands AS [UpdateItems] ON [ItemsForUpdate].[Id] = [UpdateItems].[Id];");
            procedure.AppendLine(" ");
            procedure.AppendLine("      INSERT INTO  [BrandItemEntities]");
            procedure.AppendLine("      ([Id], [Code], [Name], [DateOfCreation], [LastUpdated], [ForceUpdated])");
            procedure.AppendLine("      SELECT [InsertItems].[Id], [InsertItems].[Code], [InsertItems].[Name],");
            procedure.AppendLine("             [InsertItems].[DateOfCreation], [InsertItems].[LastUpdated], [InsertItems].[ForceUpdated]");
            procedure.AppendLine("      FROM @brands AS [InsertItems]");
            procedure.AppendLine("      WHERE [InsertItems].[Id] NOT IN (SELECT [BrandItems].[Id] FROM [BrandItemEntities] AS [BrandItems]);");
            procedure.AppendLine(" ");
            procedure.AppendLine("END");

            return procedure.ToString();
        }

        private void PopulateOptionItemEntities(DataBaseContext dataBaseContext)
        {
            List<OptionItemEntity> optionItems = new List<OptionItemEntity>
            {
                new OptionItemEntity
                {
                    Code = OptionName.Login,
                    Name = "User Login",
                    Value = "k0206"
                },
                new OptionItemEntity
                {
                    Code = OptionName.Password,
                    Name = "User Password",
                    Value = "autotrend"
                },
                new OptionItemEntity
                {
                    Code = OptionName.Debt,
                    Name = "Mutual Settlements",
                    Value = "0.00 грн."
                },
                new OptionItemEntity
                {
                    Code = OptionName.OverdueAccountsReceivable,
                    Name = "Overdue accounts receivable",
                    Value = "0.00 грн."
                },
                new OptionItemEntity
                {
                    Code = OptionName.LastOrderNumber,
                    Name = "Last Order Number",
                    Value = "0"
                },
                new OptionItemEntity
                {
                    Code = OptionName.CatalogMaximumRows,
                    Name = "Maximum Rows Displayed in Catalog Entry",
                    Value = "13"
                },
                new OptionItemEntity
                {
                    Code = OptionName.SplitterPosition,
                    Name = "Start Splitter position in Catalog Entry",
                    Value = "150"
                },
                new OptionItemEntity
                {
                    Code = CatalogColumnNames.NumberColumnName + PrefixOptions.Width,
                    Name = PrefixOptions.Width + " of " + CatalogColumnNames.NumberColumnName + "column",
                    Value = "55"
                },
                new OptionItemEntity
                {
                    Code = CatalogColumnNames.PhotoColumnName + PrefixOptions.Width,
                    Name = PrefixOptions.Width + " of " + CatalogColumnNames.PhotoColumnName + "column",
                    Value = "40"
                },
                new OptionItemEntity
                {
                    Code = CatalogColumnNames.CodeColumnName + PrefixOptions.Width,
                    Name = PrefixOptions.Width + " of " + CatalogColumnNames.CodeColumnName + "column",
                    Value = "75"
                },
                new OptionItemEntity
                {
                    Code = CatalogColumnNames.ArticleColumn + PrefixOptions.Width,
                    Name = PrefixOptions.Width + " of " + CatalogColumnNames.ArticleColumn + "column",
                    Value = "112"
                },
                new OptionItemEntity
                {
                    Code = CatalogColumnNames.NameColumn + PrefixOptions.Width,
                    Name = PrefixOptions.Width + " of " + CatalogColumnNames.NameColumn + "column",
                    Value = "330"
                },
                new OptionItemEntity
                {
                    Code = CatalogColumnNames.BrandColumn + PrefixOptions.Width,
                    Name = PrefixOptions.Width + " of " + CatalogColumnNames.BrandColumn + "column",
                    Value = "110"
                },
                new OptionItemEntity
                {
                    Code = CatalogColumnNames.UnitColumn + PrefixOptions.Width,
                    Name = PrefixOptions.Width + " of " + CatalogColumnNames.UnitColumn + "column",
                    Value = "50"
                },
                new OptionItemEntity
                {
                    Code = CatalogColumnNames.EnterpriceNormPackColumn + PrefixOptions.Width,
                    Name = PrefixOptions.Width + " of " + CatalogColumnNames.EnterpriceNormPackColumn + "column",
                    Value = "90"
                },
                new OptionItemEntity
                {
                    Code = CatalogColumnNames.BatchOfSalesColumn + PrefixOptions.Width,
                    Name = PrefixOptions.Width + " of " + CatalogColumnNames.BatchOfSalesColumn + "column",
                    Value = "90"
                },
                new OptionItemEntity
                {
                    Code = CatalogColumnNames.BalanceColumn + PrefixOptions.Width,
                    Name = PrefixOptions.Width + " of " + CatalogColumnNames.BalanceColumn + "column",
                    Value = "95"
                },
                new OptionItemEntity
                {
                    Code = CatalogColumnNames.PriceColumn + PrefixOptions.Width,
                    Name = PrefixOptions.Width + " of " + CatalogColumnNames.PriceColumn + "column",
                    Value = "90"
                },
                new OptionItemEntity
                {
                    Code = CatalogColumnNames.CountColumn + PrefixOptions.Width,
                    Name = PrefixOptions.Width + " of " + CatalogColumnNames.CountColumn + "column",
                    Value = "110"
                },
                new OptionItemEntity
                {
                    Code = PrefixOptions.Advance + CatalogColumnNames.NumberColumnName + PrefixOptions.Width,
                    Name = PrefixOptions.Width + " of " + CatalogColumnNames.NumberColumnName + "column during " + 
                           PrefixOptions.Advance + " search",
                    Value = "55"
                },
                new OptionItemEntity
                {
                    Code = PrefixOptions.Advance + CatalogColumnNames.PhotoColumnName + PrefixOptions.Width,
                    Name = PrefixOptions.Width + " of " + CatalogColumnNames.PhotoColumnName + "column during " + 
                           PrefixOptions.Advance + " search",
                    Value = "40"
                },
                new OptionItemEntity
                {
                    Code = PrefixOptions.Advance + CatalogColumnNames.CodeColumnName + PrefixOptions.Width,
                    Name = PrefixOptions.Width + " of " + CatalogColumnNames.CodeColumnName + "column during " + 
                           PrefixOptions.Advance + " search",
                    Value = "75"
                },
                new OptionItemEntity
                {
                    Code = PrefixOptions.Advance + CatalogColumnNames.ArticleColumn + PrefixOptions.Width,
                    Name = PrefixOptions.Width + " of " + CatalogColumnNames.ArticleColumn + "column during " +
                           PrefixOptions.Advance + " search",
                    Value = "112"
                },
                new OptionItemEntity
                {
                    Code = PrefixOptions.Advance + CatalogColumnNames.NameColumn + PrefixOptions.Width,
                    Name = PrefixOptions.Width + " of " + CatalogColumnNames.NameColumn + "column during " +
                           PrefixOptions.Advance + " search",
                    Value = "330"
                },
                new OptionItemEntity
                {
                    Code = PrefixOptions.Advance + CatalogColumnNames.BrandColumn + PrefixOptions.Width,
                    Name = PrefixOptions.Width + " of " + CatalogColumnNames.BrandColumn + "column during " +
                           PrefixOptions.Advance + " search",
                    Value = "110"
                },
                new OptionItemEntity
                {
                    Code = PrefixOptions.Advance + CatalogColumnNames.UnitColumn + PrefixOptions.Width,
                    Name = PrefixOptions.Width + " of " + CatalogColumnNames.UnitColumn + "column during " +
                           PrefixOptions.Advance + " search",
                    Value = "50"
                },
                new OptionItemEntity
                {
                    Code = PrefixOptions.Advance + CatalogColumnNames.EnterpriceNormPackColumn + PrefixOptions.Width,
                    Name = PrefixOptions.Width + " of " + CatalogColumnNames.EnterpriceNormPackColumn + "column during " +
                           PrefixOptions.Advance + " search",
                    Value = "90"
                },
                new OptionItemEntity
                {
                    Code = PrefixOptions.Advance + CatalogColumnNames.BatchOfSalesColumn + PrefixOptions.Width,
                    Name = PrefixOptions.Width + " of " + CatalogColumnNames.BatchOfSalesColumn + "column during " +
                           PrefixOptions.Advance + " search",
                    Value = "90"
                },
                new OptionItemEntity
                {
                    Code = PrefixOptions.Advance + CatalogColumnNames.BalanceColumn + PrefixOptions.Width,
                    Name = PrefixOptions.Width + " of " + CatalogColumnNames.BalanceColumn + "column during " +
                           PrefixOptions.Advance + " search",
                    Value = "95"
                },
                new OptionItemEntity
                {
                    Code = PrefixOptions.Advance + CatalogColumnNames.PriceColumn + PrefixOptions.Width,
                    Name = PrefixOptions.Width + " of " + CatalogColumnNames.PriceColumn + "column during " +
                           PrefixOptions.Advance + " search",
                    Value = "90"
                },
                new OptionItemEntity
                {
                    Code = PrefixOptions.Advance + CatalogColumnNames.CountColumn + PrefixOptions.Width,
                    Name = PrefixOptions.Width + " of " + CatalogColumnNames.CountColumn + "column during " +
                           PrefixOptions.Advance + " search",
                    Value = "110"
                }
            };

            dataBaseContext.OptionItemEntities.AddRange(optionItems);
            dataBaseContext.SaveChanges();
        }
    }
}
