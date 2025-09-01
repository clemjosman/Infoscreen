CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,

	[ObjectId] [nvarchar](100) NOT NULL,
	[DisplayName] [nvarchar](100) NOT NULL,
	[Upn] [nvarchar](100) NOT NULL,
	[Iso2] [nvarchar](2) NOT NULL,
	[SelectedTenantId] [int] NULL,
	[CreationDate] [datetimeoffset](7) NOT NULL,
	[DeletionDate] [datetimeoffset](7) NULL,
	
	CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED(
		[Id] ASC
	)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY],

	CONSTRAINT [ObjectId_Unique] UNIQUE(
		[ObjectId]
	)
	
) ON [PRIMARY]
GO

/****** Foreign Keys ******/


ALTER TABLE [dbo].[Users]
	ADD CONSTRAINT [FK_Users_SelectedTenant] FOREIGN KEY ([SelectedTenantId]) REFERENCES [dbo].[Tenants] ([Id])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_SelectedTenant]
GO


/****** Indexes ******/

CREATE INDEX [IX_Users_ObjectId] ON [dbo].[Users] ([ObjectId]);
CREATE INDEX [IX_Users_Upn]      ON [dbo].[Users] ([Upn]);
