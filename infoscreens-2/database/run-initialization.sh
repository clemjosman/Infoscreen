#!/bin/bash

# Wait to be sure that SQL Server is ready
sleep 30s
echo "Starting init check"

# Exit if DB already exists - This is only a setup script, not a migration script!
if /opt/mssql-tools18/bin/sqlcmd -S localhost -U local-admin -P abc123!+ -d vesact-infoscreens -N o -Q "SELECT * FROM [dbo].[Tenants]"; then
    echo "DB is already ready."
    exit 0
fi
echo "DB not yet set, init started..."

# Run the setup script to create the users, DB and schema
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P password123! -d master -i sql-server-init.sql -N o
/opt/mssql-tools18/bin/sqlcmd -S localhost -U local-admin -P abc123!+ -d vesact-infoscreens -i 000_0_Script_Dependencies.sql -N o
/opt/mssql-tools18/bin/sqlcmd -S localhost -U local-admin -P abc123!+ -d vesact-infoscreens -i 000_1_Common_Files.sql -N o
/opt/mssql-tools18/bin/sqlcmd -S localhost -U local-admin -P abc123!+ -d vesact-infoscreens -i 000_2_Common_Message.sql -N o
/opt/mssql-tools18/bin/sqlcmd -S localhost -U local-admin -P abc123!+ -d vesact-infoscreens -i 001_Translations.sql -N o
/opt/mssql-tools18/bin/sqlcmd -S localhost -U local-admin -P abc123!+ -d vesact-infoscreens -i 002_Languages.sql -N o
/opt/mssql-tools18/bin/sqlcmd -S localhost -U local-admin -P abc123!+ -d vesact-infoscreens -i 003_TranslatedTexts.sql -N o
/opt/mssql-tools18/bin/sqlcmd -S localhost -U local-admin -P abc123!+ -d vesact-infoscreens -i 004_Tenants.sql -N o
/opt/mssql-tools18/bin/sqlcmd -S localhost -U local-admin -P abc123!+ -d vesact-infoscreens -i 005_Users.sql -N o
/opt/mssql-tools18/bin/sqlcmd -S localhost -U local-admin -P abc123!+ -d vesact-infoscreens -i 006_UsersTenants.sql -N o
/opt/mssql-tools18/bin/sqlcmd -S localhost -U local-admin -P abc123!+ -d vesact-infoscreens -i 007_Categories.sql -N o
/opt/mssql-tools18/bin/sqlcmd -S localhost -U local-admin -P abc123!+ -d vesact-infoscreens -i 008_InfoscreenGroups.sql -N o
/opt/mssql-tools18/bin/sqlcmd -S localhost -U local-admin -P abc123!+ -d vesact-infoscreens -i 009_Infoscreens.sql -N o
/opt/mssql-tools18/bin/sqlcmd -S localhost -U local-admin -P abc123!+ -d vesact-infoscreens -i 010_News.sql -N o
/opt/mssql-tools18/bin/sqlcmd -S localhost -U local-admin -P abc123!+ -d vesact-infoscreens -i 011_InfoscreensNews.sql -N o
/opt/mssql-tools18/bin/sqlcmd -S localhost -U local-admin -P abc123!+ -d vesact-infoscreens -i 012_NewsCategories.sql -N o
/opt/mssql-tools18/bin/sqlcmd -S localhost -U local-admin -P abc123!+ -d vesact-infoscreens -i 013_Videos.sql -N o
/opt/mssql-tools18/bin/sqlcmd -S localhost -U local-admin -P abc123!+ -d vesact-infoscreens -i 014_InfoscreensVideo.sql -N o
/opt/mssql-tools18/bin/sqlcmd -S localhost -U local-admin -P abc123!+ -d vesact-infoscreens -i 015_VideosCategories.sql -N o
/opt/mssql-tools18/bin/sqlcmd -S localhost -U local-admin -P abc123!+ -d vesact-infoscreens -i 016_Subscriptions.sql -N o
/opt/mssql-tools18/bin/sqlcmd -S localhost -U local-admin -P abc123!+ -d vesact-infoscreens -i 017_Initial_Values.sql -N o
/opt/mssql-tools18/bin/sqlcmd -S localhost -U local-admin -P abc123!+ -d vesact-infoscreens -i 018_Permission_Management.sql -N o
echo "Init done!"
