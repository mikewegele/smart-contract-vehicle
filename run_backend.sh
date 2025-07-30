#!/bin/bash
# This script intelligently creates and applies EF Core migrations only when necessary.

# Exit immediately if any command fails
set -e

# --- Main Script ---

# 1. Navigate to the project directory
cd backend || { echo "Error: Directory 'backend' not found." >&2; exit 1; }

# 2. Ensure project is up-to-date
echo "➡️  Restoring .NET dependencies and building the project..."
dotnet restore
dotnet build

# 3. Check for model changes by attempting to add a temporary migration
echo "🔎 Checking for database model changes..."
TEMP_MIGRATION_NAME="PendingChanges"

# Attempt to add a migration and capture the output.
# The command exits with 0 if successful (even with no changes), but `set -e` will catch any real errors.
OUTPUT=$(dotnet ef migrations add "$TEMP_MIGRATION_NAME" 2>&1)

# 4. Analyze the output to decide if a real migration is needed
if echo "$OUTPUT" | grep -q "No changes were found"; then
    # Case 1: No changes detected.
    echo "✅ No model changes detected. Skipping migration creation."

    # If you use older EF Core tools, an empty migration file might still be created.
    # This block safely removes it without causing an error if it doesn't exist.
    if dotnet ef migrations list | grep -q "$TEMP_MIGRATION_NAME"; then
        echo "🗑️  Removing empty temporary migration..."
        dotnet ef migrations remove --force
    fi
else
    # Case 2: Changes were detected, and a temporary migration was created.
    echo "⚠️  Model changes detected. Creating a new timestamped migration..."

    # First, remove the temporary migration we just created.
    dotnet ef migrations remove --force

    # Then, create the real migration with a proper timestamped name.
    TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
    MIGRATION_NAME="AutoMigration_$TIMESTAMP"
    dotnet ef migrations add "$MIGRATION_NAME"
    echo "✨ Successfully created migration: $MIGRATION_NAME"
fi

# 5. Apply any pending migrations to the database
echo "🔄 Applying migrations to the database..."
dotnet ef database update

# 6. Run the application
echo "🚀 Launching the application..."
dotnet run --launch-profile "https"