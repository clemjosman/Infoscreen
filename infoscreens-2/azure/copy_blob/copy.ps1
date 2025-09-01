# Copy the infoscreen container from vesactinfoscreens to vesactinfoscreensstaging
.\azcopy.exe copy 'https://vesactinfoscreens.blob.core.windows.net/infoscreens?st=2020-01-03T11%3A43%3A09Z&se=2120-01-04T11%3A43%3A00Z&sp=rl&sv=2018-03-28&sr=c&sig=wxoWuzOkSYjgqsPPojdtxErx9cXc51yoKBQhf863KkU%3D' 'https://vesactinfoscreensstaging.blob.core.windows.net/infoscreens?st=2020-01-03T10%3A46%3A34Z&se=2120-01-04T10%3A46%3A00Z&sp=racwdl&sv=2018-03-28&sr=c&sig=866CPhQ2OVhEM2tbCWIyKs2WzJ3U7OeFTuRkWytdo%2Fw%3D' --recursive 

# Copy the $web container from vesactinfoscreens to vesactinfoscreensstaging
#.\azcopy.exe copy 'https://vesactinfoscreens.blob.core.windows.net/$web?st=2020-01-03T11%3A47%3A30Z&se=2120-01-04T11%3A47%3A00Z&sp=rl&sv=2018-03-28&sr=c&sig=kLQj4m9jFDzIp4zGeLeZoXsPiFvEZGQ2m4nuxjr9Low%3D' 'https://vesactinfoscreensstaging.blob.core.windows.net/$web?st=2020-01-03T11%3A52%3A24Z&se=2120-01-04T11%3A52%3A00Z&sp=racwdl&sv=2018-03-28&sr=c&sig=ycend%2FybOmcbQSM%2F2%2FqbFb0twDmDh5VzNfgWq1qK%2Bz8%3D' --recursive

# Display message waiting on user input to avoid automatic closing of the console
Read-Host -Prompt "Press Enter to close"

# To copy from staging to prod, use the Microsoft Azure Storage Explorer to generate the url with the SAS token. Read for the staging containers, and all the rights on prod containers.
#.\azcopy.exe copy '<url_to_staging_infoscreens_container>' '<url_to_prod_infoscreens_container_with_all_permissions>' --recursive
#.\azcopy.exe copy '<url_to_staging_$web_container>' '<url_to_prod_$web_container_with_all_permissions>' --recursive