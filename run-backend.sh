#!/bin/bash

set -e

cd backend || { echo "Directory 'backend' not found"; exit 1; }

echo "Restoring packages..."
dotnet restore

echo "Building project..."
dotnet build

if [ -d "Migrations" ]; then
  echo "Deleting existing Migrations folder..."
  rm -rf Migrations
fi

MIGRATIONS_COUNT=$(dotnet ef migrations list | wc -l)

if [ "$MIGRATIONS_COUNT" -eq 0 ]; then
  TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
  MIGRATION_NAME="InitialMigration_$TIMESTAMP"
  echo "No migrations found. Creating initial migration: $MIGRATION_NAME"
  dotnet ef migrations add "$MIGRATION_NAME"
else
  echo "Existing migrations found ($MIGRATIONS_COUNT). Skipping migration creation."
fi

echo "Applying migrations to database..."
dotnet ef database update

echo "Starting application..."
dotnet run
