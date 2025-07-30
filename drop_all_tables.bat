@echo off
setlocal

:: --- Database Connection Configuration ---
set DB_HOST=localhost
set DB_PORT=5432
set DB_NAME=smart_contract_vehicle_db
set DB_USER=user
set DB_PASS=password

:: Set the password for the psql command-line utility
set "PGPASSWORD=%DB_PASS%"

echo Deleting all tables from database '%DB_NAME%'...
echo.

:: Construct the psql command to get all table names from the 'public' schema.
:: Then, loop through each table name found.
for /f %%t in ('psql -h %DB_HOST% -p %DB_PORT% -U %DB_USER% -d %DB_NAME% -Atc "SELECT tablename FROM pg_tables WHERE schemaname = 'public';"') do (
    echo  - Dropping table: %%t
    
    :: Execute the DROP TABLE command for each table.
    psql -h %DB_HOST% -p %DB_PORT% -U %DB_USER% -d %DB_NAME% -c "DROP TABLE IF EXISTS \"%%t\" CASCADE;" > nul
)

echo.
echo All tables deleted successfully.

endlocal