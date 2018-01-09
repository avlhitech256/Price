USE [master]
GO

/****** Object:  Login [IT-1\avl]    Script Date: 03.01.2018 22:58:40 ******/
DROP LOGIN [IT-1\avl]
GO

/****** Object:  Login [IT-1\avl]    Script Date: 03.01.2018 22:58:40 ******/
CREATE LOGIN [IT-1\avl] FROM WINDOWS WITH DEFAULT_DATABASE=[Создание базы данных], DEFAULT_LANGUAGE=[русский]
GO

ALTER SERVER ROLE [sysadmin] ADD MEMBER [IT-1\avl]
GO

ALTER SERVER ROLE [dbcreator] ADD MEMBER [IT-1\avl]
GO