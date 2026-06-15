-- Run ONLY if your database has obsolete migration IDs after the split/reward removal.
--
-- Usage:
--   docker exec -i leveluplife-postgres psql -U levelup -d leveluplife \
--     < Scripts/update-ef-history-after-migration-split.sql

BEGIN;

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" IN (
    '20260613224949_AddGamificationStreakRewards',
    '202606130005_BlockDRewardClaimAndPlayerEvents',
    '20260613230331_AddRewardItemRequiredExperiencePoints',
    '20260614002906_SeedGamificationCatalogAndQaData'
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT v."MigrationId", '9.0.0'
FROM (VALUES
    ('202606130001_Issue29StreakCompletionTracking'),
    ('202606130003_BlockBStreakProtection'),
    ('202606130004_BlockEHabitTaskEarnedXpSnapshot'),
    ('202606130005_AddPlayerEvents')
) AS v("MigrationId")
WHERE NOT EXISTS (
    SELECT 1 FROM "__EFMigrationsHistory" h WHERE h."MigrationId" = v."MigrationId"
);

COMMIT;
