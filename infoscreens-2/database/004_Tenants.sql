CREATE TABLE [dbo].[Tenants](
	[Id] [int] IDENTITY(1,1) NOT NULL,

	[Code] [nvarchar](100) NOT NULL,
	[DisplayName] [nvarchar](100) NOT NULL,
	[NotifyUsers] [bit] NOT NULL,
	[CreationDate] [datetimeoffset](7) NOT NULL,
	[DeletionDate] [datetimeoffset](7) NULL,
  [AppName] [nvarchar](100) NULL,
  [ContentAdminEmail] [nvarchar](255) NULL,
	
	CONSTRAINT [PK_Tenants] PRIMARY KEY CLUSTERED(
		[Id] ASC
	)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY],
	
	CONSTRAINT [Tenants_Code_Unique] UNIQUE (
		[Code]
	)
) ON [PRIMARY]
GO


/****** Indexes ******/

CREATE INDEX [IX_Tenants_Code] ON [dbo].[Tenants] ([Code]);
