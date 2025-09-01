CREATE TABLE [dbo].[InfoscreensNews]
(
	[Id] INT IDENTITY(1,1) NOT NULL,

	[InfoscreenId] [int] NOT NULL,
	[NewsId] [int] NOT NULL,
	[CreationDate] [datetimeoffset](7) NOT NULL,

	CONSTRAINT [PK_InfoscreensNews] PRIMARY KEY CLUSTERED(
		[Id] ASC
	)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Foreign Keys ******/

ALTER TABLE [dbo].[InfoscreensNews]
	ADD CONSTRAINT [FK_InfoscreensNews_Infoscreen] FOREIGN KEY ([InfoscreenId]) REFERENCES [dbo].[Infoscreens] ([Id])
GO
ALTER TABLE [dbo].[InfoscreensNews] CHECK CONSTRAINT [FK_InfoscreensNews_Infoscreen]
GO

ALTER TABLE [dbo].[InfoscreensNews]
ADD CONSTRAINT [FK_InfoscreensNews_News] FOREIGN KEY ([NewsId]) REFERENCES [dbo].[News] ([Id])
GO
ALTER TABLE [dbo].[InfoscreensNews] CHECK CONSTRAINT [FK_InfoscreensNews_News]
GO

/****** Indexes ******/

CREATE UNIQUE NONCLUSTERED INDEX [IX_InfoscreenNews_Unique] ON [dbo].[InfoscreensNews]
(
	[InfoscreenId],
	[NewsId]
)
WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
GO
