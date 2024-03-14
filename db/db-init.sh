#!/bin/bash
set -e 

while ! /opt/mssql-tools/bin/sqlcmd -Q "SELECT 1" -b -o /dev/null
do
  printf "MSSQL Server not ready yet. Sleeping for 5s...\n"
  sleep 5
done
printf "MSSQL Server is ready to accept connections!\n"

# Execute tSQLt server preparation script
printf "Running tSQLt server preparation script...\n"
/opt/mssql-tools/bin/sqlcmd -i /usr/src/app/PrepareServer.sql
printf "tSQLt server preparation complete!\n"

# Execute database initialization script
printf "Running database initialization script...\n"
/opt/mssql-tools/bin/sqlcmd -i /usr/src/app/CreateDatabase.sql
printf "Database initialization complete!\n"
