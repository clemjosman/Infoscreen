CREATE TABLE [dbo].[News](
	[Id] [int] IDENTITY(1,1) NOT NULL,

	[TenantId] [int] NOT NULL,
	[Description] [nvarchar](250) NULL,
	[TitleTranslationId] [int] NOT NULL,
	[ContentMarkdownTranslationId] [int] NOT NULL,
	[ContentHTMLTranslationId] [int] NOT NULL,
	[FileId] [int] NULL,
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
    [Box1Size] [int] NOT NULL,
    [Box1Content] [nvarchar](250) NOT NULL,
    [Box2Size] [int] NOT NULL,
    [Box2Content] [nvarchar](250) NOT NULL,
    [Layout] [nvarchar](250) NOT NULL,
    [IsForInfoscreens] [bit] NOT NULL,
    [IsForApp] [bit] NOT NULL
	
	CONSTRAINT [PK_News] PRIMARY KEY CLUSTERED(
		[Id] ASC
	)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	
) ON [PRIMARY]
GO


/****** Foreign Keys ******/

ALTER TABLE [dbo].[News]
	ADD CONSTRAINT [FK_News_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [dbo].[Tenants] ([Id])
GO
ALTER TABLE [dbo].[News] CHECK CONSTRAINT [FK_News_Tenant]
GO

ALTER TABLE [dbo].[News]
	ADD CONSTRAINT [FK_News_TitleTranslation] FOREIGN KEY ([TitleTranslationId]) REFERENCES [dbo].[Translations] ([Id])
GO
ALTER TABLE [dbo].[News] CHECK CONSTRAINT [FK_News_TitleTranslation]
GO

ALTER TABLE [dbo].[News]
	ADD CONSTRAINT [FK_News_ContentMarkdownTranslation] FOREIGN KEY ([ContentMarkdownTranslationId]) REFERENCES [dbo].[Translations] ([Id])
GO
ALTER TABLE [dbo].[News] CHECK CONSTRAINT [FK_News_ContentMarkdownTranslation]
GO

ALTER TABLE [dbo].[News]
	ADD CONSTRAINT [FK_News_ContentHTMLTranslation] FOREIGN KEY ([ContentHTMLTranslationId]) REFERENCES [dbo].[Translations] ([Id])
GO
ALTER TABLE [dbo].[News] CHECK CONSTRAINT [FK_News_ContentHTMLTranslation]
GO

ALTER TABLE [dbo].[News]
ADD CONSTRAINT [FK_News_File] FOREIGN KEY ([FileId]) REFERENCES [dbo].[Files] ([Id])
GO
ALTER TABLE [dbo].[News] CHECK CONSTRAINT [FK_News_File]
GO

ALTER TABLE [dbo].[News]
ADD CONSTRAINT [FK_News_Creator] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[News] CHECK CONSTRAINT [FK_News_Creator]
GO

ALTER TABLE [dbo].[News]
ADD CONSTRAINT [FK_News_Editor] FOREIGN KEY ([LastEditedBy]) REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[News] CHECK CONSTRAINT [FK_News_Editor]
GO

ALTER TABLE [dbo].[News]
ADD CONSTRAINT [FK_News_DeletedBy] FOREIGN KEY ([DeletedBy]) REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[News] CHECK CONSTRAINT [FK_News_DeletedBy]
GO


/****** Indexes ******/

CREATE INDEX [IX_News_TenantId]        ON [dbo].[News] ([TenantId]);
CREATE INDEX [IX_News_IsVisible]       ON [dbo].[News] ([IsVisible]);
CREATE INDEX [IX_News_PublicationDate] ON [dbo].[News] ([PublicationDate]);
CREATE INDEX [IX_News_CreatedBy]       ON [dbo].[News] ([CreatedBy]);
CREATE INDEX [IX_News_LastEditedBy]    ON [dbo].[News] ([LastEditedBy]);
CREATE INDEX [IX_News_LastEditDate]    ON [dbo].[News] ([LastEditDate]);
