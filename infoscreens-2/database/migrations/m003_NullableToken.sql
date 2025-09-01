-- Migration script for live running DB, changes in the tables are reflected in the setup scripts

ALTER TABLE [Subscriptions] ALTER COLUMN [PushTokenId] [int] NULL
GO
