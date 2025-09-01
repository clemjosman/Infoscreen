CREATE TABLE [dbo].[UsersTenants]
(
	[Id] INT IDENTITY(1,1) NOT NULL,

	[UserId] [int] NOT NULL,
	[TenantId] [int] NOT NULL,
	[CreationDate] [datetimeoffset](7) NOT NULL,

	CONSTRAINT [PK_UsersTenants] PRIMARY KEY CLUSTERED(
		[Id] ASC
	)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Foreign Keys ******/

ALTER TABLE [dbo].[UsersTenants]
	ADD CONSTRAINT [FK_UsersTenants_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[UsersTenants] CHECK CONSTRAINT [FK_UsersTenants_User]
GO

ALTER TABLE [dbo].[UsersTenants]
ADD CONSTRAINT [FK_UsersTenants_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [dbo].[Tenants] ([Id])
GO
ALTER TABLE [dbo].[UsersTenants] CHECK CONSTRAINT [FK_UsersTenants_Tenant]
GO

/****** Indexes ******/

CREATE UNIQUE NONCLUSTERED INDEX [IX_UsersTenants_Unique] ON [dbo].[UsersTenants]
(
	[UserId],
	[TenantId]
)
WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
GO
