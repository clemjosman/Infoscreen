CREATE TABLE [dbo].[InfoscreenGroups]
(
	[Id] INT IDENTITY(1,1) NOT NULL,

	[Name] [nvarchar](200) NOT NULL,
	[TenantId] [int] NOT NULL,

	CONSTRAINT [PK_InfoscreenGroups] PRIMARY KEY CLUSTERED(
		[Id] ASC
	)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
)
GO


/****** Foreign Keys ******/

ALTER TABLE [dbo].[InfoscreenGroups]
	ADD CONSTRAINT [FK_InfoscreenGroup_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [dbo].[Tenants] ([Id])
GO
ALTER TABLE [dbo].[InfoscreenGroups] CHECK CONSTRAINT [FK_InfoscreenGroup_Tenant]
GO


/****** Indexes ******/

CREATE INDEX [IX_InfoscreenGroups_Name]     ON [dbo].[InfoscreenGroups] ([Name]);
CREATE INDEX [IX_InfoscreenGroups_TenantId] ON [dbo].[InfoscreenGroups] ([TenantId]);
