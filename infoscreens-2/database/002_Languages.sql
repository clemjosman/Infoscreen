CREATE TABLE [dbo].[Languages]
(
	[Id] INT IDENTITY(1,1) NOT NULL,

	[Iso2] [nvarchar](2) NOT NULL,
	[DisplayNameTranslationId] [int] NOT NULL,

	CONSTRAINT [PK_Languages] PRIMARY KEY CLUSTERED(
		[Id] ASC
	)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY],
	
	CONSTRAINT [Iso2_Unique] UNIQUE(
		[Iso2]
	)
) ON [PRIMARY]
GO

/****** Foreign Keys ******/

ALTER TABLE [dbo].[Languages]
	ADD CONSTRAINT [FK_Languages_DisplayNameTranslation] FOREIGN KEY ([DisplayNameTranslationId]) REFERENCES [dbo].[Translations] ([Id])
GO
ALTER TABLE [dbo].[Languages] CHECK CONSTRAINT [FK_Languages_DisplayNameTranslation]
GO


/****** Indexes ******/

CREATE INDEX [IX_Languages_Iso2] ON [dbo].[Languages] ([Iso2]);