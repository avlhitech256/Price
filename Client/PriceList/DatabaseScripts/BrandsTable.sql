CREATE TYPE [dbo].[brandsTable] AS TABLE 
(
      [Id] [bigint] NOT NULL,
      [Code] [uniqueidentifier] NOT NULL,
      [Name] [nvarchar](255) NULL,
      [DateOfCreation] [datetimeoffset](7) NOT NULL,
      [LastUpdated] [datetimeoffset](7) NOT NULL,
      [ForceUpdated] [datetimeoffset](7) NOT NULL,
      PRIMARY KEY ([Id])
)


----------------------------------------------------------

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
