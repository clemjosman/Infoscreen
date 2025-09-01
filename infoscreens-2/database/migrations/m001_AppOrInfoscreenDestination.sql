-- Migration script for live running DB, changes in the tables are reflected in the setup scripts

ALTER TABLE [News] ADD [IsForInfoscreens] [bit] NOT NULL CONSTRAINT [DF_News_IsForInfoscreens] DEFAULT 1
GO
ALTER TABLE [News] DROP CONSTRAINT [DF_News_IsForInfoscreens]
GO

ALTER TABLE [News] ADD [IsForApp] [bit] NOT NULL CONSTRAINT [DF_News_IsForApp] DEFAULT 1
GO
ALTER TABLE [News] DROP CONSTRAINT [DF_News_IsForApp]
GO



ALTER TABLE [Videos] ADD [IsForInfoscreens] [bit] NOT NULL CONSTRAINT [DF_Videos_IsForInfoscreens] DEFAULT 1
GO
ALTER TABLE [Videos] DROP CONSTRAINT [DF_Videos_IsForInfoscreens]
GO

ALTER TABLE [Videos] ADD [IsForApp] [bit] NOT NULL CONSTRAINT [DF_Videos_IsForApp] DEFAULT 1
GO
ALTER TABLE [Videos] DROP CONSTRAINT [DF_Videos_IsForApp]
GO

ALTER TABLE [Tenants] ADD [AppName] [nvarchar](100) NULL
GO
