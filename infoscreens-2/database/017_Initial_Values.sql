INSERT INTO [dbo].[Translations] ([TextCode]) VALUES 
	('language.german'),
	('language.french'),
	('language.italian'),
	('language.english')
;

INSERT INTO [dbo].[Languages] ([Iso2], [DisplayNameTranslationId]) VALUES 
	('de', 1),
	('fr', 2),
	('it', 3),
	('en', 4)
;

INSERT INTO [dbo].[TranslatedTexts] ([TranslationId], [LanguageId], [Text], [LastEditDate]) VALUES
	(1, 1, 'Deutsch', NULL),
	(1, 2, 'Allemand', NULL),
	(1, 3, 'Tedesco', NULL),
	(1, 4, 'German', NULL),
	(2, 1, 'Französisch', NULL),
	(2, 2, 'Français', NULL),
	(2, 3, 'Francese', NULL),
	(2, 4, 'French', NULL),
	(3, 1, 'Italienisch', NULL),
	(3, 2, 'Italien', NULL),
	(3, 3, 'Italiano', NULL),
	(3, 4, 'Italian', NULL),
	(4, 1, 'English', NULL),
	(4, 2, 'Anglais', NULL),
	(4, 3, 'Inglese', NULL),
	(4, 4, 'English', NULL)
;

INSERT INTO [dbo].[Tenants] ([Code], [DisplayName], [NotifyUsers], [CreationDate], [DeletionDate], [AppName], [ContentAdminEmail]) VALUES 
	('ch.actemium', 'Actemium Schweiz AG', 1, '2020-02-06 16:45:00.0000000', NULL, 'MyActemium', 'jonas.haberkorn@actemium.ch'),
	('compositesBusch', 'Composites Busch', 0, '2020-02-21 08:30:00.0000000', NULL, NULL, NULL),
    ('ch.axians', 'Axians Schweiz AG', 1, '2021-11-23 00:00:00.0000000 +00:00', NULL, 'MyAxians', NULL),
    ('ch.etavis', 'ETAVIS', 0, '2022-06-29 00:00:00.0000000 +02:00', NULL, 'MyETAVIS', NULL)
;

INSERT INTO [dbo].[Users] ([ObjectId], [DisplayName], [Upn], [Iso2], [SelectedTenantId], [CreationDate], [DeletionDate]) VALUES 
	('70a4dfec-110a-4ec9-89d0-233b29bd44a6', 'HABERKORN Jonas', 'jonas.haberkorn@actemium.ch', 'de', 1, '2020-02-06 16:45:00.0000000', NULL),
    ('60708694-d208-4076-806b-35e4d734d426', 'Robin FREYBURGER', 'robin.freyburger@actemium.ch', 'fr', 1, '2021-06-17 11:30:00.0000000 +02:00', NULL),
    ('f15079ce-2f5b-4b12-b237-22cb05b84489', 'LEMEUNIER Sebastien', 'sebastien.lemeunier@actemium.ch', 'fr', 1, '2022-07-07 00:00:00.0000000 +02:00', NULL),
    ('987be6dd-df03-443f-a05f-accf395f1fae', 'SEGET Yann', 'yann.seget@vinci-energies.net', 'fr', 1, '2023-02-01 09:15:42.3546986 +00:00', NULL),
    ('396b23d1-f613-4f70-9ecc-b42b0a78efae', 'HENZ Steven', 'steven.henz@vinci-energies.net', 'de', 1, '2023-07-25 00:00:00.0000000 +02:00', NULL),
    ('78136e2a-7311-4d07-bff6-5c99c97da5de', 'BERNAT Clement', 'clement.bernat@actemium.ch', 'fr', 1, '2025-07-14 00:00:00.0000000 +02:00', NULL),
    ('cfa43f6d-7ae5-49dc-8eff-f572d11aecba', 'HABERKORN Steeve', 'steeve.haberkorn@actemium.ch', 'fr', 1, '2025-07-14 00:00:00.0000000 +02:00', NULL)
;

INSERT INTO [dbo].[UsersTenants] ([UserId], [TenantId], [CreationDate]) VALUES 
	(1, 1, '2020-02-06 16:45:00.0000000'),
	(1, 2, '2020-02-21 08:30:00.0000000'),
	(2, 1, '2020-02-26 09:15:00.0000000'),
	(2, 2, '2020-02-26 09:15:00.0000000'),
	(3, 1, '2025-07-15 07:40:00.0000000'),
	(3, 2, '2025-07-15 07:40:00.0000000'),
	(4, 1, '2025-07-15 07:40:00.0000000'),
	(4, 2, '2025-07-15 07:40:00.0000000'),
	(5, 1, '2025-07-15 07:40:00.0000000'),
	(5, 2, '2025-07-15 07:40:00.0000000'),
	(6, 1, '2025-07-15 07:40:00.0000000'),
	(6, 2, '2025-07-15 07:40:00.0000000'),
	(7, 1, '2025-07-15 07:40:00.0000000'),
	(7, 2, '2025-07-15 07:40:00.0000000')
;

INSERT INTO [dbo].[InfoscreenGroups] ([Name], [TenantId]) VALUES 
	('Basel', 1),
	('LeitTec', 1),
	('Romandie', 1),
	('Composite Busch', 2)
;

INSERT INTO [dbo].[Infoscreens] ([NodeId], [MsbNodeId], [InfoscreenGroupId], [DisplayName], [Description], [ApiKey1], [ApiKey2], [DefaultContentLanguageId], [ContentAdminEmail], [SendMailNoContent]) VALUES 
	('INF-DEV-0001', '6502fb04-284b-4388-ba50-ba5bab2204b8', 2, 'Test Infoscreen 1', 'Monitoring test device', 'qpiQrcaUvzIAHrRksftuqZ+wB9dMb0LJPoVI60vJ5/E=', 'ob3IWdmIIP+0v0T6k9DzF3f6df22j5//lYLJrQ8sq54=', 1, NULL, 0),
	('INF-DEV-0002', 'aaf6baf3-723e-4688-88fa-0b15e5f920f6', 2, 'Test Infoscreen 2', 'Content test device', 'qpiQrcaUvzIAHrRksftuqZ+wB9dMb0LJPoVI60vJ5/E=', 'ob3IWdmIIP+0v0T6k9DzF3f6df22j5//lYLJrQ8sq54=', 2, NULL, 0)
;


INSERT INTO [dbo].[Translations] ([TextCode]) VALUES 
	('news1.title'),
    ('news1.content.markdown'),
    ('news1.content.html'),
	('video1.title')
;

INSERT INTO [dbo].[TranslatedTexts] ([TranslationId] ,[LanguageId] ,[Text] ,[LastEditDate]) VALUES
    (5, 1, 'News Title', '2025-07-14 00:00:00.0000000'),
    (6, 1, 'News **Inhalt**', '2025-07-14 00:00:00.0000000'),
    (7, 1, 'News <b>Inhalt</b>', '2025-07-14 00:00:00.0000000'),
    (8, 1, 'Video Title', '2025-07-14 00:00:00.0000000')
;

INSERT INTO [dbo].[Files] ([Guid], [FileName], [BlobName], [Size], [Extension], [MD5Checksum], [PublicAccess], [Created], [DeletionRequested], [Deleted], [UserId], [TenantId], [Custom1], [Custom2], [Custom3], [Custom4]) VALUES
    ('42E057FF-BB5E-4A0C-BFAA-0DB2730072D4', 'Safety Week 2023 DE.jpg','tenant-1/2023-04-26/a1238f4a-c0c5-4ae3-b743-285e0d8089c7.jpeg', 1339443, 'jpeg', 'd791ad2e96f496c097f699abbf4783a6', 0, '2027-07-14 09:00:00.0000000', NULL, NULL, 1, 1, NULL, NULL, NULL, NULL)
;

INSERT INTO [dbo].[News] ([TenantId], [Description], [TitleTranslationId], [ContentMarkdownTranslationId], [ContentHTMLTranslationId], [FileId], [IsVisible], [UsersNotified], [PublicationDate], [CreationDate], [CreatedBy], [LastEditDate], [LastEditedBy], [DeletionDate], [DeletedBy], [IsForInfoscreens], [IsForApp], [ExpirationDate], [Box1Size], [Box1Content], [Box2Size], [Box2Content], [Layout]) VALUES
    (1, 'Test news', 5, 6, 7, 1, 1, '2025-07-17 09:07:00.0000000', '2025-07-16 17:00:00.0000000', '2025-07-16 17:00:00.0000000', 1, '2025-07-16 17:00:00.0000000', 1, NULL, NULL, 1, 1, NULL, 25, 'text', 75,'file', 'horizontal')
;

INSERT INTO [dbo].[Videos] ([TenantId], [Description], [TitleTranslationId], [Url], [Duration], [IsVisible], [UsersNotified], [PublicationDate], [CreationDate], [CreatedBy], [LastEditDate], [LastEditedBy], [DeletionDate], [DeletedBy], [IsForInfoscreens], [IsForApp], [ExpirationDate], [Background]) VALUES
    (1, 'Test video', 8, 'https://youtu.be/y3B4YKxX3D4', 178, 1, NULL, '2025-07-17 09:07:00.0000000', '2025-07-16 17:00:00.0000000', 1, '2025-07-16 17:00:00.0000000', 1, NULL, NULL, 1, 1, NULL, 'blue-dot-mesh')
;

INSERT INTO [dbo].[InfoscreensNews] ([InfoscreenId], [NewsId], [CreationDate]) VALUES
    (1, 1, '2025-07-17 09:07:00.0000000'),
    (2, 1, '2025-07-17 09:07:00.0000000')
;

INSERT INTO [dbo].[InfoscreensVideos] ([InfoscreenId], [VideoId], [CreationDate]) VALUES
    (1, 1, '2025-07-17 09:07:00.0000000'),
    (2, 1, '2025-07-17 09:07:00.0000000')
;

INSERT INTO [dbo].[Categories] ([TenantId], [Name], [CreationDate], [CreatedBy], [LastEditDate], [LastEditedBy], [DeletionDate], [DeletedBy]) VALUES
    (1, 'Test', '2025-07-17 09:07:00.0000000', 1, '2025-07-17 09:07:00.0000000', 1, NULL, NULL)
;

INSERT INTO [dbo].[NewsCategories] ([NewsId], [CategoryId], [CreationDate]) VALUES
    (1, 1, '2025-07-17 09:07:00.0000000')
;

INSERT INTO [dbo].[VideosCategories] ([VideoId], [CategoryId], [CreationDate]) VALUES
    (1, 1, '2025-07-17 09:07:00.0000000')
;
