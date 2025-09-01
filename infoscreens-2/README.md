# Infoscreens

## Dev

### Setup

1. Install Docker and login with your subscription
2. Connect to the vesact npm feed ba following [these instructions](https://dev.azure.com/vesact/infoscreens/_artifacts/feed/vesact/connect)
3. Copy your user .npmrc into `.npmrc.user` at the root of this repo
4. Create a [Personal Access Token](https://dev.azure.com/vesact/_usersSettings/tokens) with Packaging read permission
5. Copy the `docker-compose/.env.example` file into `docker-compose/.env`, complete the values and then copy it into `./backend/.env` as well
6. You can now run `docker compose -f docker-compose.all.yml up` or target the front or back files if only part of the system should be run

### Known issues

-   If there are issues to 'publish' the C# projects or 'unfinished npmtimer'/'ERR!code E401'/'reify failed'error for the JS projects, try to regenerate the credentials to access the package registries first (see step 2 and 4 above).

### Access local DB

Connect to it using [SSMS](https://learn.microsoft.com/en-us/ssms/sql-server-management-studio-ssms) with the following settings:

-   Server type: Databse Engine
-   Server name: localhost
-   Authentication: SQL Server Authentication
-   Login: local-admin
-   Password: abc123!+
-   Options >>
    -   Connection Properties
        -   Encrypt connection: disabled

### Access to blob storage

Open the [storage explorer](https://azure.microsoft.com/en-us/products/storage/storage-explorer), in the "Emulator & Attached" section open the Storage Accounts and open the Default Port account.

### Known issues

Image links for news are targeting the `infoscreens-blob` hostname insteat of `localhost` for it to be correctly displayed on the CMS and Slideshow interfaces.
