CREATE TABLE [dbo].[InfoscreensVideos]
(
	[Id] INT IDENTITY(1,1) NOT NULL,

	[InfoscreenId] [int] NOT NULL,
	[VideoId] [int] NOT NULL,
	[CreationDate] [datetimeoffset](7) NOT NULL,

	CONSTRAINT [PK_InfoscreensVideos] PRIMARY KEY CLUSTERED(
		[Id] ASC
	)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Foreign Keys ******/

ALTER TABLE [dbo].[InfoscreensVideos]
	ADD CONSTRAINT [FK_InfoscreensVideos_Infoscreen] FOREIGN KEY ([InfoscreenId]) REFERENCES [dbo].[Infoscreens] ([Id])
GO
ALTER TABLE [dbo].[InfoscreensVideos] CHECK CONSTRAINT [FK_InfoscreensVideos_Infoscreen]
GO

ALTER TABLE [dbo].[InfoscreensVideos]
ADD CONSTRAINT [FK_InfoscreensVideos_Video] FOREIGN KEY ([VideoId]) REFERENCES [dbo].[Videos] ([Id])
GO
ALTER TABLE [dbo].[InfoscreensVideos] CHECK CONSTRAINT [FK_InfoscreensVideos_Video]
GO

/****** Indexes ******/

CREATE UNIQUE NONCLUSTERED INDEX [IX_InfoscreenNews_Unique] ON [dbo].[InfoscreensVideos]
(
	[InfoscreenId],
	[VideoId]
)
WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
GO
