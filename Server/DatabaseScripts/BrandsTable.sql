CREATE TYPE [dbo].[brandsTable] AS TABLE(
	[Code] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](255) NULL,
	PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (IGNORE_DUP_KEY = OFF)
)


------------------------------------------------------


            body.AppendLine("CREATE TYPE [dbo].[brandsTable] AS TABLE");
            body.AppendLine("(");
            body.AppendLine("	[Code] [uniqueidentifier] NOT NULL,");
            body.AppendLine("	[Name] [nvarchar](255) NULL,");
            body.AppendLine("	PRIMARY KEY ([Code])");
            body.AppendLine(")");
