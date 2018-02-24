USE [PriceList]
GO

/****** Object:  UserDefinedTableType [dbo].[catalogsTable]    Script Date: 20.02.2018 22:46:45 ******/
CREATE TYPE [dbo].[catalogsTable] AS TABLE(
	[Id] [bigint] NOT NULL,
	[UID] [uniqueidentifier] NOT NULL,
	[Code] [nvarchar](30) NULL,
	[Article] [nvarchar](30) NULL,
	[Name] [nvarchar](255) NULL,
	[BrandName] [nvarchar](255) NULL,
	[Unit] [nvarchar](10) NULL,
	[EnterpriceNormPack] [nvarchar](30) NULL,
	[BatchOfSales] [decimal](18, 2) NOT NULL,
	[Balance] [nvarchar](30) NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[Currency] [nvarchar](5) NULL,
	[Multiplicity] [decimal](18, 2) NOT NULL,
	[HasPhotos] [bit] NOT NULL,
	[Status] [int] NOT NULL,
	[LastUpdatedStatus] [datetimeoffset](7) NOT NULL,
	[DateOfCreation] [datetimeoffset](7) NOT NULL,
	[LastUpdated] [datetimeoffset](7) NOT NULL,
	[ForceUpdated] [datetimeoffset](7) NOT NULL,
	[Brand_Id] [bigint] NULL,
	[Directory_Id] [bigint] NULL,
	PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (IGNORE_DUP_KEY = OFF)
)
GO


