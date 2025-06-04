#!/bin/bash
set -e

until /opt/mssql-tools/bin/sqlcmd -S sqlserver -U sa -P "YourStrong!Passw0rd" -C -Q "SELECT 1" > /dev/null 2>&1; do
  echo "Waiting for SQL Server to start..."
  sleep 5
done

echo "Running initialization script..."
/opt/mssql-tools/bin/sqlcmd -S sqlserver -U sa -P "YourStrong!Passw0rd" -C -i /ERD_DATN.sql -r1 -b -o ./init.log

if [ $? -eq 0 ]; then
    echo "Init script executed successfully."
    exit 0
else
    echo "Init script failed!"
    cat ./init.log
    exit 1
fi