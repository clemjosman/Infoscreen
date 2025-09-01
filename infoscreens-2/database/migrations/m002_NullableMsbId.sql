-- Migration script for live running DB, changes in the tables are reflected in the setup scripts

ALTER TABLE [Infoscreens] ALTER COLUMN [MsbNodeId] [nvarchar](200) NULL
GO
