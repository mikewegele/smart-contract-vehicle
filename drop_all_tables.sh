#!/bin/bash

DB_HOST="localhost"
DB_PORT="5432"
DB_NAME="smart_contract_vehicle_db"
DB_USER="user"
DB_PASS="password"

export PGPASSWORD=$DB_PASS

# Hole alle Tabellennamen außer den Systemtabellen (public schema)
TABLES=$(psql -h $DB_HOST -p $DB_PORT -U $DB_USER -d $DB_NAME -Atc "SELECT tablename FROM pg_tables WHERE schemaname = 'public';")

if [ -z "$TABLES" ]; then
  echo "Keine Tabellen gefunden."
  exit 0
fi

echo "Lösche alle Tabellen..."

for t in $TABLES; do
  echo "DROP TABLE IF EXISTS \"$t\" CASCADE;"
done | psql -h $DB_HOST -p $DB_PORT -U $DB_USER -d $DB_NAME

echo "Alle Tabellen gelöscht."
