-- ================================
-- Create User-defined Table Type
-- ================================
USE [ServerPriceList]
GO

-- Create the data type
CREATE TYPE [dbo].[intTable] AS TABLE 
(
	[Id] int NOT NULL, 
    PRIMARY KEY ([Id])
)
GO



-- ================================
-- Create User-defined Table Type
-- ================================
USE [ServerPriceList]
GO

-- Create the data type
CREATE TYPE [dbo].[bigintTable] AS TABLE 
(
	[Id] [bigint] NOT NULL, 
    PRIMARY KEY ([Id])
)
GO
