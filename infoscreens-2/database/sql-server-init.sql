CREATE DATABASE [vesact-infoscreens]
GO

USE [vesact-infoscreens];
GO

CREATE LOGIN [local-admin] WITH PASSWORD = N'abc123!+'
CREATE USER [local-admin] FOR LOGIN [local-admin]
ALTER ROLE db_owner ADD MEMBER [local-admin];
GO

CREATE LOGIN [local-svc] WITH PASSWORD = N'def456#-'
CREATE USER [local-svc] FOR LOGIN [local-svc]
ALTER ROLE db_datawriter ADD MEMBER [local-svc];
GO