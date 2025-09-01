CREATE TABLE [dbo].[Categories](
	[Id] [int] IDENTITY(1,1) NOT NULL,

	[TenantId] [int] NOT NULL,
	[Name] [nvarchar](25) NOT NULL,
	[CreationDate] [datetimeoffset](7) NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[LastEditDate] [datetimeoffset](7) NULL,
	[LastEditedBy] [int] NULL,
	[DeletionDate] [datetimeoffset](7) NULL,
	[DeletedBy] [int] NULL,
	
	CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED(
		[Id] ASC
	)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	
) ON [PRIMARY]
GO


/****** Foreign Keys ******/

ALTER TABLE [dbo].[Categories]
	ADD CONSTRAINT [FK_Categories_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [dbo].[Tenants] ([Id])
GO
ALTER TABLE [dbo].[Categories] CHECK CONSTRAINT [FK_Categories_Tenant]
GO

ALTER TABLE [dbo].[Categories]
	ADD CONSTRAINT [FK_Categories_Creator] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Categories] CHECK CONSTRAINT [FK_Categories_Creator]
GO

ALTER TABLE [dbo].[Categories]
	ADD CONSTRAINT [FK_Categories_Editor] FOREIGN KEY ([LastEditedBy]) REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Categories] CHECK CONSTRAINT [FK_Categories_Editor]
GO

ALTER TABLE [dbo].[Categories]
	ADD CONSTRAINT [FK_Categories_Deletor] FOREIGN KEY ([DeletedBy]) REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Categories] CHECK CONSTRAINT [FK_Categories_Deletor]
GO

/****** Indexes ******/

CREATE INDEX [IX_Categories_TenantId] ON [dbo].[Categories] ([TenantId]);
CREATE INDEX [IX_Categories_Name] ON [dbo].[Categories] ([Name]);
