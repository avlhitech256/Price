CREATE TYPE [dbo].[productDirectionsTable] AS TABLE(
      [Id] [bigint] NOT NULL,
      [Direction] [int] NOT NULL,
      [Directory_Id] [bigint] NULL,
      [DateOfCreation] [datetimeoffset](7) NOT NULL,
      [LastUpdated] [datetimeoffset](7) NOT NULL,
      [ForceUpdated] [datetimeoffset](7) NOT NULL,
      PRIMARY KEY ([Id])
)

-----------------------------------------------------------

            type.AppendLine("CREATE TYPE [dbo].[productDirectionsTable] AS TABLE(");
            type.AppendLine("      [Id] [bigint] NOT NULL,");
            type.AppendLine("      [Direction] [int] NOT NULL,");
            type.AppendLine("      [Directory_Id] [bigint] NULL,");
            type.AppendLine("      [DateOfCreation] [datetimeoffset](7) NOT NULL,");
            type.AppendLine("      [LastUpdated] [datetimeoffset](7) NOT NULL,");
            type.AppendLine("      [ForceUpdated] [datetimeoffset](7) NOT NULL,");
            type.AppendLine("      PRIMARY KEY ([Id])");
            type.AppendLine(")");
