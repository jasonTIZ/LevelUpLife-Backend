-- Hace opcional DSC_PERSON_PHONE_NUMBER (registro sin teléfono).
-- Aplicar con:
--   docker exec -i leveluplife-postgres psql -U levelup -d leveluplife < Scripts/make-person-phone-optional.sql

ALTER TABLE "LULM_PERSON"
    ALTER COLUMN "DSC_PERSON_PHONE_NUMBER" DROP NOT NULL;

-- Registrar migración EF si ya aplicaste el SQL a mano y luego harás dotnet ef database update:
-- INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
-- VALUES ('202606170001_MakePersonPhoneNumberOptional', '9.0.0')
-- ON CONFLICT DO NOTHING;
