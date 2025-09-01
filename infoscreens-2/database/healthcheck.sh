#!/bin/bash

# Check if DB is running and populated
result=$(/opt/mssql-tools18/bin/sqlcmd -S localhost -U local-admin -P abc123!+ -d vesact-infoscreens -N o -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM [dbo].[Tenants]" -W -h-1 -k | tr -d '\r\n')

# Exit on error
ret=$?
if [ $ret -ne 0 ]; then
    exit 1
fi

# Extract the first character using awk
first_char=$(echo "$result" | awk '{print substr($0, 1, 1)}')

echo $first_char

# Check if the first character is a number greater than zero
if [[ "$first_char" =~ ^[1-9]$ ]]; then
    exit 0
fi
exit 1
