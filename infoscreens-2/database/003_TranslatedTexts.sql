CREATE TABLE [dbo].[TranslatedTexts]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[TranslationId] [int] NOT NULL,

	[LanguageId] [int] NOT NULL,
	[Text] [nvarchar](max) NULL,
	[LastEditDate] [datetimeoffset] NULL,

	CONSTRAINT [PK_TranslatedTexts] PRIMARY KEY CLUSTERED(
		[Id] ASC
	)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Foreign Keys ******/

ALTER TABLE [dbo].[TranslatedTexts]
	ADD CONSTRAINT [FK_TranslatedTexts_Translation] FOREIGN KEY ([TranslationId]) REFERENCES [dbo].[Translations] ([Id])
GO
ALTER TABLE [dbo].[TranslatedTexts] CHECK CONSTRAINT [FK_TranslatedTexts_Translation]
GO

ALTER TABLE [dbo].[TranslatedTexts]
ADD CONSTRAINT [FK_TranslatedTexts_Language] FOREIGN KEY ([LanguageId]) REFERENCES [dbo].[Languages] ([Id])
GO
ALTER TABLE [dbo].[TranslatedTexts] CHECK CONSTRAINT [FK_TranslatedTexts_Language]
GO


/****** Indexes ******/

CREATE UNIQUE NONCLUSTERED INDEX [IX_TranslatedTexts_Unique] ON [dbo].[TranslatedTexts]
(
	[TranslationId],
	[LanguageId]
)
WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
GO

CREATE INDEX [IX_TranslatedTexts_TranslationId] ON [dbo].[TranslatedTexts] ([TranslationId]);
CREATE INDEX [IX_TranslatedTexts_LanguageId]    ON [dbo].[TranslatedTexts] ([LanguageId]);
