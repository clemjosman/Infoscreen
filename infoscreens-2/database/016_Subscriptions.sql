CREATE TABLE [dbo].[Subscriptions](
	[Id] [int] IDENTITY(1,1) NOT NULL,

	[UserId] INT NOT NULL,
	[PushTokenId] INT NULL,
	[InfoscreenId] INT NOT NULL,
	[LastUpdateDate] [datetimeoffset](7) NOT NULL,
	
	CONSTRAINT [PK_Subscriptions] PRIMARY KEY CLUSTERED(
		[Id] ASC
	)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY],
) ON [PRIMARY]
GO

/****** Foreign Keys ******/


ALTER TABLE [dbo].[Subscriptions]
	ADD CONSTRAINT [FK_Subscriptions_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Subscriptions] CHECK CONSTRAINT [FK_Subscriptions_User]
GO


ALTER TABLE [dbo].[Subscriptions]
	ADD CONSTRAINT [FK_Subscriptions_PushToken] FOREIGN KEY ([PushTokenId]) REFERENCES [dbo].[PushTokens] ([Id])
GO
ALTER TABLE [dbo].[Subscriptions] CHECK CONSTRAINT [FK_Subscriptions_PushToken]
GO


ALTER TABLE [dbo].[Subscriptions]
	ADD CONSTRAINT [FK_Subscriptions_Infoscreen] FOREIGN KEY ([InfoscreenId]) REFERENCES [dbo].[Infoscreens] ([Id])
GO
ALTER TABLE [dbo].[Subscriptions] CHECK CONSTRAINT [FK_Subscriptions_Infoscreen]
GO


/****** Indexes ******/

CREATE INDEX [IX_Subscriptions_InfoscreenId] ON [dbo].[Subscriptions] ([InfoscreenId]);
