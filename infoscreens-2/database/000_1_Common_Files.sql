-- Source: https://dev.azure.com/vesact/_git/vesact.common.file.v2?path=/sql/v1.0.0_Create_Tables_And_Default_Data.sql

-- ***************************************
--             CREATE TABLES
-- ***************************************

/****** Object:  Table [dbo].[Files]    Script Date: 15.05.2020 ******/
CREATE TABLE [dbo].[Files](
	[Id] INT IDENTITY(1,1) NOT NULL,
	
	[Guid] UNIQUEIDENTIFIER NOT NULL,
	[FileName] NVARCHAR(100) NOT NULL,
	[BlobName] NVARCHAR(255) NOT NULL,
	[Size] INT NOT NULL,
	[Extension] NVARCHAR(15) NOT NULL,
	[MD5Checksum] NVARCHAR(32) NOT NULL,
	[PublicAccess] BIT NOT NULL,
	[Created] DATETIMEOFFSET(7) NOT NULL,
	[DeletionRequested] DATETIMEOFFSET(7) NULL,
	[Deleted] DATETIMEOFFSET(7) NULL,
	[UserId] NVARCHAR(100) NULL,
	[TenantId] NVARCHAR(100) NULL,
	[Custom1] NVARCHAR(255) NULL,
	[Custom2] NVARCHAR(4000) NULL,
	[Custom3] NVARCHAR(max) NULL,
	[Custom4] NVARCHAR(max) NULL,

	CONSTRAINT [PK_Files] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	
) ON [PRIMARY]
GO




-- ***************************************
--          CREATE FOREIGN KEYS
-- ***************************************




-- ***************************************
--          CREATE INDEXES
-- ***************************************

/****** Table [Files] ******/

/****** Object:  Index [IX_Files_Guid]    Script Date: 15.05.2020 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Files_Guid] ON [dbo].[Files]
(
	[Guid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

/****** Object:  Index [IX_Files_DeletionRequested]    Script Date: 15.05.2020 ******/
CREATE NONCLUSTERED INDEX [IX_Files_DeletionRequested] ON [dbo].[Files]
(
	[DeletionRequested] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

/****** Object:  Index [IX_Files_Deleted]    Script Date: 15.05.2020 ******/
CREATE NONCLUSTERED INDEX [IX_Files_Deleted] ON [dbo].[Files]
(
	[Deleted] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

/****** Object:  Index [IX_Files_BlobName]    Script Date: 16.04.2021 ******/
CREATE NONCLUSTERED INDEX [IX_Files_BlobName] ON [dbo].[Files]
(
	[BlobName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

/****** Object:  Index [IX_Files_UserId]    Script Date: 15.05.2020 ******/
CREATE NONCLUSTERED INDEX [IX_Files_UserId] ON [dbo].[Files]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

/****** Object:  Index [IX_Files_TenantId]    Script Date: 15.05.2020 ******/
CREATE NONCLUSTERED INDEX [IX_Files_TenantId] ON [dbo].[Files]
(
	[TenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

/****** Object:  Index [IX_Files_PublicAccess]    Script Date: 15.05.2020 ******/
CREATE NONCLUSTERED INDEX [IX_Files_PublicAccess] ON [dbo].[Files]
(
	[PublicAccess] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

/****** Object:  Index [IX_Files_Custom1]    Script Date: 15.05.2020 ******/
CREATE NONCLUSTERED INDEX [IX_Files_Custom1] ON [dbo].[Files]
(
	[Custom1] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO




-- ***************************************
--          DEFAULT VALUES
-- ***************************************
