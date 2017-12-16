namespace WpfApplication4.EntitiesMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class InitialEntities : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(
                "dbo.PrepareToUpdateCatalogs",
                p => new
                {
                    login = p.String(maxLength: 30),
                    lastUpdate = p.DateTimeOffset(precision: 7)
                },
                body:
@"-- SET NOCOUNT ON added to prevent extra result sets from
-- interfering with SELECT statements.
SET NOCOUNT ON;

declare @entityName int = 2;
declare @dateOfCreation datetimeoffset(7) = Sysdatetimeoffset();
declare @countToUpdate bigint;

INSERT INTO [dbo].[SendItemsEntities] 
([Login], [EntityId], [Catalog].[EntityName], [Catalog].[RequestDate], [Catalog].[DateOfCreation]) 
SELECT @login, [Id], 2, @lastUpdate, @dateOfCreation 
FROM  [dbo].[CatalogItemEntities] AS [Catalog]
WHERE [Catalog].[LastUpdated] > @lastUpdate AND 
      NOT EXISTS (SELECT * 
                  FROM  [dbo].[SendItemsEntities] AS [SendItem] 
		          WHERE [SendItem].[EntityId] = [Catalog].[Id] AND
                        [SendItem].[Login] = @login AND
                        [SendItem].[EntityName] = @entityName);

SELECT @countToUpdate = (SELECT COUNT(*) 
                         FROM  [dbo].[SendItemsEntities]
                         WHERE [Login] = @login AND
                                         [EntityName] = @entityName);

RETURN (@countToUpdate);"
                );
        }
        
        public override void Down()
        {
            DropStoredProcedure("dbo.PrepareToUpdateCatalogs");
        }
    }
}
