/* This application is dependenant on the following sql scripts:
 	  - vesact.common.file.v2: from v1.0.0 and all until v1.0.0
	  - vesact.common.message.v2: from v1.3.0 and all until v1.3.0
*/

-- Done later in the setup : Adding foreign key between a news and its File ([dbo].[News].[FileId] -> [dbo].[Files].[Id])