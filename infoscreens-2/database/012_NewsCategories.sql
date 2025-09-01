CREATE TABLE [dbo].[NewsCategories]
(
	[Id] INT IDENTITY(1,1) NOT NULL,

	[NewsId] [int] NOT NULL,
	[CategoryId] [int] NOT NULL,
	[CreationDate] [datetimeoffset](7) NOT NULL,

	CONSTRAINT [PK_NewsCategories] PRIMARY KEY CLUSTERED(
		[Id] ASC
	)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Foreign Keys ******/

ALTER TABLE [dbo].[NewsCategories]
	ADD CONSTRAINT [FK_NewsCategories_News] FOREIGN KEY ([NewsId]) REFERENCES [dbo].[News] ([Id])
GO
ALTER TABLE [dbo].[NewsCategories] CHECK CONSTRAINT [FK_NewsCategories_News]
GO

ALTER TABLE [dbo].[NewsCategories]
ADD CONSTRAINT [FK_NewsCategories_Category] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([Id])
GO
ALTER TABLE [dbo].[NewsCategories] CHECK CONSTRAINT [FK_NewsCategories_Category]
GO

/****** Indexes ******/

CREATE UNIQUE NONCLUSTERED INDEX [IX_NewsCategories_Unique] ON [dbo].[NewsCategories]
(
	[NewsId],
	[CategoryId]
)
WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
GO
