#!/bin/bash

set -e

cd backend || { echo "Directory 'backend' not found"; exit 1; }

dotnet restore
dotnet build

TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
MIGRATION_NAME="AutoMigration_$TIMESTAMP"

dotnet ef migrations add "$MIGRATION_NAME"
dotnet ef database update

dotnet run
