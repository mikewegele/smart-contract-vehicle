@echo off
setlocal

:: This script intelligently creates and applies EF Core migrations only when necessary.

:: --- Main Script ---

:: 1. Navigate to the project directory
cd backend
if %ERRORLEVEL% neq 0 (
    echo Error: Directory 'backend' not found.
    exit /b 1
)

:: 2. Ensure project is up-to-date
echo ➡️  Restoring .NET dependencies and building the project...
dotnet restore
dotnet build

:: 3. Check for model changes by attempting to add a temporary migration
echo 🔎 Checking for database model changes...
set TEMP_MIGRATION_NAME=PendingChanges
set TEMP_OUTPUT_FILE=migration_check.tmp

:: Attempt to add a migration and capture the output to a temporary file.
dotnet ef migrations add "%TEMP_MIGRATION_NAME%" > "%TEMP_OUTPUT_FILE%" 2>&1

:: 4. Analyze the output to decide if a real migration is needed
findstr /C:"No changes were found" "%TEMP_OUTPUT_FILE%" > nul
if %ERRORLEVEL% == 0 (
    :: Case 1: No changes detected.
    echo ✅ No model changes detected. Skipping migration creation.

    :: Safely remove the empty temporary migration if it was created.
    dotnet ef migrations list | findstr /C:"%TEMP_MIGRATION_NAME%" > nul
    if %ERRORLEVEL% == 0 (
        echo 🗑️   Removing empty temporary migration...
        dotnet ef migrations remove --force
    )
) else (
    :: Case 2: Changes were detected.
    echo ⚠️  Model changes detected. Creating a new timestamped migration...

    :: First, remove the temporary migration we just created.
    dotnet ef migrations remove --force

    :: Then, create the real migration with a timestamped name.
    :: We use PowerShell here because it's the most reliable way to get a formatted date in Windows.
    for /f "tokens=*" %%a in ('powershell -Command "Get-Date -Format 'yyyyMMdd_HHmmss'"') do set "TIMESTAMP=%%a"
    set "MIGRATION_NAME=AutoMigration_%TIMESTAMP%"
    
    dotnet ef migrations add "%MIGRATION_NAME%"
    echo ✨ Successfully created migration: %MIGRATION_NAME%
)

:: Clean up the temporary output file
if exist "%TEMP_OUTPUT_FILE%" del "%TEMP_OUTPUT_FILE%"

:: 5. Apply any pending migrations to the database
echo 🔄 Applying migrations to the database...
dotnet ef database update

:: 6. Run the application
echo 🚀 Launching the application...
dotnet run --launch-profile "https"

endlocal