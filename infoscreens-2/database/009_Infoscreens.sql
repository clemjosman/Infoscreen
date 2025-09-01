CREATE TABLE [dbo].[Infoscreens]
(
	[Id] INT IDENTITY(1,1) NOT NULL,

	[NodeId] [nvarchar](200) NOT NULL,
	[MsbNodeId] [nvarchar](200) NULL,
	[InfoscreenGroupId] [int] NOT NULL,

	[DisplayName] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](500) NOT NULL,
	[ApiKey1] [nvarchar](256) NOT NULL,
	[ApiKey2] [nvarchar](256) NOT NULL,
	[DefaultContentLanguageId] [int] NOT NULL,
    [ContentAdminEmail] [nvarchar](255) NULL,
    [SendMailNoContent] [bit] NOT NULL,

	CONSTRAINT [PK_Infoscreens] PRIMARY KEY CLUSTERED(
		[Id] ASC
	)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
)
GO


/****** Foreign Keys ******/

ALTER TABLE [dbo].[Infoscreens]
ADD CONSTRAINT [FK_Infoscreen_InfoscreenGroup] FOREIGN KEY ([InfoscreenGroupId]) REFERENCES [dbo].[InfoscreenGroups] ([Id])
GO
ALTER TABLE [dbo].[Infoscreens] CHECK CONSTRAINT [FK_Infoscreen_InfoscreenGroup]
GO

ALTER TABLE [dbo].[Infoscreens]
ADD CONSTRAINT [FK_Infoscreen_Language] FOREIGN KEY ([DefaultContentLanguageId]) REFERENCES [dbo].[Languages] ([Id])
GO
ALTER TABLE [dbo].[Infoscreens] CHECK CONSTRAINT [FK_Infoscreen_Language]
GO


/****** Indexes ******/

CREATE INDEX [IX_Infoscreens_NodeId]                ON [dbo].[Infoscreens] ([NodeId]);
CREATE INDEX [IX_Infoscreens_MsbNodeId]             ON [dbo].[Infoscreens] ([MsbNodeId]);
CREATE INDEX [IX_Infoscreens_InfoscreenGroupId]     ON [dbo].[Infoscreens] ([InfoscreenGroupId]);
CREATE INDEX [IX_Infoscreens_ApiKey1]               ON [dbo].[Infoscreens] ([ApiKey1]);
CREATE INDEX [IX_Infoscreens_ApiKey2]               ON [dbo].[Infoscreens] ([ApiKey2]);
