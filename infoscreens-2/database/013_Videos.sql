CREATE TABLE [dbo].[Videos](
	[Id] [int] IDENTITY(1,1) NOT NULL,

	[TenantId] [int] NOT NULL,
	[Description] [nvarchar](250) NULL,
	[TitleTranslationId] [int] NOT NULL,
	[Url] [nvarchar](max) NOT NULL,
	[Duration] [int] NULL,
	[Background] [nvarchar](250) NULL,
	[IsVisible] [bit] NOT NULL,
	[UsersNotified] [datetimeoffset](7) NULL,
	[PublicationDate] [datetimeoffset](7) NOT NULL,
	[ExpirationDate] [datetimeoffset](7) NULL,
	[CreationDate] [datetimeoffset](7) NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[LastEditDate] [datetimeoffset](7) NULL,
	[LastEditedBy] [int] NULL,
	[DeletionDate] [datetimeoffset](7) NULL,
	[DeletedBy] [int] NULL,
    [IsForInfoscreens] [bit] NOT NULL,
    [IsForApp] [bit] NOT NULL
	
	CONSTRAINT [PK_Videos] PRIMARY KEY CLUSTERED(
		[Id] ASC
	)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	
) ON [PRIMARY]
GO


/****** Foreign Keys ******/

ALTER TABLE [dbo].[Videos]
	ADD CONSTRAINT [FK_Videos_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [dbo].[Tenants] ([Id])
GO
ALTER TABLE [dbo].[Videos] CHECK CONSTRAINT [FK_Videos_Tenant]
GO

ALTER TABLE [dbo].[Videos]
	ADD CONSTRAINT [FK_Videos_TitleTranslation] FOREIGN KEY ([TitleTranslationId]) REFERENCES [dbo].[Translations] ([Id])
GO
ALTER TABLE [dbo].[Videos] CHECK CONSTRAINT [FK_Videos_TitleTranslation]
GO

ALTER TABLE [dbo].[Videos]
ADD CONSTRAINT [FK_Videos_Creator] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Videos] CHECK CONSTRAINT [FK_Videos_Creator]
GO

ALTER TABLE [dbo].[Videos]
ADD CONSTRAINT [FK_Videos_Editor] FOREIGN KEY ([LastEditedBy]) REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Videos] CHECK CONSTRAINT [FK_Videos_Editor]
GO

ALTER TABLE [dbo].[Videos]
ADD CONSTRAINT [FK_Videos_DeletedBy] FOREIGN KEY ([DeletedBy]) REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Videos] CHECK CONSTRAINT [FK_Videos_DeletedBy]
GO


/****** Indexes ******/

CREATE INDEX [IX_Videos_TenantId]        ON [dbo].[Videos] ([TenantId]);
CREATE INDEX [IX_Videos_IsVisible]       ON [dbo].[Videos] ([IsVisible]);
CREATE INDEX [IX_Videos_PublicationDate] ON [dbo].[Videos] ([PublicationDate]);
CREATE INDEX [IX_Videos_CreatedBy]       ON [dbo].[Videos] ([CreatedBy]);
CREATE INDEX [IX_Videos_LastEditedBy]    ON [dbo].[Videos] ([LastEditedBy]);
CREATE INDEX [IX_Videos_LastEditDate]    ON [dbo].[Videos] ([LastEditDate]);