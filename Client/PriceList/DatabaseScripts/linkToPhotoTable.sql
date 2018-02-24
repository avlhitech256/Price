USE [PriceList]
GO

/****** Object:  UserDefinedTableType [dbo].[linkToPhotoTable]    Script Date: 20.02.2018 22:48:16 ******/
CREATE TYPE [dbo].[linkToPhotoTable] AS TABLE(
	[Catalog_Id] [bigint] NOT NULL,
	[Photo_Id] [bigint] NOT NULL,
	PRIMARY KEY CLUSTERED 
(
	[Photo_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF)
)
GO


