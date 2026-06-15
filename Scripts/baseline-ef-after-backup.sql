-- Ejecutar UNA SOLA VEZ después de cargar Backups/LevelUpLife-Backup-08-05-2026.sql
-- si dotnet ef database update falla en InitialCreate (tablas ya existen).
-- Luego: dotnet ef database update

CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion") VALUES
    ('20260502172302_InitialCreate', '9.0.0'),
    ('20260502174226_12-update-habit', '9.0.0'),
    ('20260508034304_AddHabitTaskAndEvidenceTables', '9.0.0'),
    ('20260508034946_SeedTestData', '9.0.0')
ON CONFLICT ("MigrationId") DO NOTHING;
