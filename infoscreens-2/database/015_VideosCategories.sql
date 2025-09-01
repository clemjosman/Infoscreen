CREATE TABLE [dbo].[VideosCategories]
(
	[Id] INT IDENTITY(1,1) NOT NULL,

	[VideoId] [int] NOT NULL,
	[CategoryId] [int] NOT NULL,
	[CreationDate] [datetimeoffset](7) NOT NULL,

	CONSTRAINT [PK_VideosCategories] PRIMARY KEY CLUSTERED(
		[Id] ASC
	)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Foreign Keys ******/

ALTER TABLE [dbo].[VideosCategories]
	ADD CONSTRAINT [FK_VideosCategories_Video] FOREIGN KEY ([VideoId]) REFERENCES [dbo].[Videos] ([Id])
GO
ALTER TABLE [dbo].[VideosCategories] CHECK CONSTRAINT [FK_VideosCategories_Video]
GO

ALTER TABLE [dbo].[VideosCategories]
ADD CONSTRAINT [FK_VideosCategories_Category] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([Id])
GO
ALTER TABLE [dbo].[VideosCategories] CHECK CONSTRAINT [FK_VideosCategories_Category]
GO

/****** Indexes ******/

CREATE UNIQUE NONCLUSTERED INDEX [IX_VideosCategories_Unique] ON [dbo].[VideosCategories]
(
	[VideoId],
	[CategoryId]
)
WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
GO
