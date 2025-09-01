-- Set Infoscreen Id to delete
DECLARE @InfoscreenId AS INT=0

-- Delete associated subsriptions and push tokens
DELETE FROM [dbo].[PushTokens] WHERE [Id] IN (SELECT [PushTokenId] FROM [dbo].[Subscriptions] WHERE [InfoscreenId] = @InfoscreenId)
DELETE FROM [dbo].[Subscriptions] WHERE [Id] = @InfoscreenId

-- Delete associated news and videos assigments
DELETE FROM [dbo].[InfoscreensNews] WHERE [InfoscreenId] = @InfoscreenId
DELETE FROM [dbo].[InfoscreensVideos] WHERE [InfoscreenId] = @InfoscreenId

-- Delete Infoscreen
DELETE FROM [dbo].[Infoscreens] WHERE [Id] = @InfoscreenId
