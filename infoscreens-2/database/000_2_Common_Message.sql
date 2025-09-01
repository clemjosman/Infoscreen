-- Source: https://dev.azure.com/vesact/_git/vesact.common.message.v2?path=/sql/v1.3.0_Create_Tables_And_Default_Data.sql

-- ***************************************
--             CREATE TABLES
-- ***************************************


/****** Object:  Table [dbo].[PushTokens]    Script Date: 16.04.2021 ******/

CREATE TABLE [dbo].[PushTokens](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TokenId] [nvarchar](255) NOT NULL,
	[Token] [nvarchar](255) NOT NULL,
	[UserId] [nvarchar](255) NOT NULL,
	[Created] [datetimeoffset](7) NOT NULL,
	[LastChecked] [datetimeoffset](7) NOT NULL,

	[Platform] [nvarchar](50) NULL,
	[Os] [nvarchar](50) NULL,
	[Model] [nvarchar](50) NULL,
	[AppVersion] [nvarchar](50) NULL,
	[SdkVersion] [nvarchar](50) NULL,
	[Ip] [nvarchar](50) NULL,
	[Iso2] [nvarchar](2) NULL,
	[Invalid] [bit] NULL,

CONSTRAINT [PK_PushTokens] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


/****** Foreign Keys ******/


/****** Indexes ******/

CREATE UNIQUE NONCLUSTERED INDEX [IX_PushTokens_TokenId] ON [dbo].[PushTokens]
(
	[TokenId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

CREATE NONCLUSTERED INDEX [IX_PushTokens_Token] ON [dbo].[PushTokens]
(
	[Token] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

CREATE NONCLUSTERED INDEX [IX_PushTokens_UserId] ON [dbo].[PushTokens]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
ALTER TABLE [dbo].[PushTokens]   
	ADD CONSTRAINT [AK_User] UNIQUE ([UserId]);   
GO  