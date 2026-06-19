using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LevelUpLife_Backend.Migrations
{
    /// <inheritdoc />
    public partial class FixMissingTablesAndColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Columns that exist in the SQL backup but were never added by any EF migration.
            migrationBuilder.Sql("""
                ALTER TABLE "LULM_HABIT_TASK"
                    ADD COLUMN IF NOT EXISTS "ID_HABIT_DISCIPLINE" INTEGER NULL
                        REFERENCES "LULM_HABIT_DISCIPLINE"("ID_HABIT_DISCIPLINE") ON DELETE SET NULL,
                    ADD COLUMN IF NOT EXISTS "NUM_HABIT_TASK_XP_VALUE" INTEGER NOT NULL DEFAULT 0,
                    ADD COLUMN IF NOT EXISTS "STATUS_HABIT_TASK_IS_COMPLETED" BOOLEAN NOT NULL DEFAULT FALSE,
                    ADD COLUMN IF NOT EXISTS "STATUS_HABIT_TASK_IS_ACTIVE" BOOLEAN NOT NULL DEFAULT TRUE;

                CREATE INDEX IF NOT EXISTS "IX_LULM_HABIT_TASK_ID_HABIT_DISCIPLINE"
                    ON "LULM_HABIT_TASK"("ID_HABIT_DISCIPLINE");
                """);

            // Table from backup: timer completion criteria for habit tasks.
            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS "LULT_HABIT_TASK_TIMER_CRITERIA" (
                    "ID_HABIT_TASK_TIMER_CRITERIA" SERIAL PRIMARY KEY,
                    "ID_HABIT_TASK" INTEGER UNIQUE NOT NULL
                        REFERENCES "LULM_HABIT_TASK"("ID_HABIT_TASK") ON DELETE CASCADE,
                    "NUM_SECONDS_DEFINED" INTEGER NOT NULL,
                    "NUM_SECONDS_LONG" INTEGER DEFAULT 0,
                    "TYPE_PAUSE_IS_ALLOWED" BOOLEAN NOT NULL DEFAULT FALSE,
                    "STATUS_TIMER_CRITERIA_IS_ACTIVE" BOOLEAN NOT NULL DEFAULT TRUE
                );
                """);

            // Table from backup: player events log.
            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS "LULH_PLAYER_EVENT" (
                    "ID_PLAYER_EVENT" SERIAL PRIMARY KEY,
                    "ID_PLAYER_USER" INTEGER NOT NULL
                        REFERENCES "LULM_PLAYER_USER"("ID_PLAYER_USER") ON DELETE CASCADE,
                    "TYPE_PLAYER_EVENT" VARCHAR(50) NOT NULL,
                    "DSC_PLAYER_EVENT_PAYLOAD" TEXT NULL,
                    "FEC_PLAYER_EVENT_CREATED" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP
                );

                CREATE INDEX IF NOT EXISTS "IX_LULH_PLAYER_EVENT_PLAYER_CREATED"
                    ON "LULH_PLAYER_EVENT"("ID_PLAYER_USER", "FEC_PLAYER_EVENT_CREATED");
                """);

            // Tables from backup: reward economy system.
            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS "LULM_REWARD_ITEM_TYPE" (
                    "ID_REWARD_ITEM_TYPE" SERIAL PRIMARY KEY,
                    "DSC_REWARD_ITEM_TYPE_NAME" VARCHAR(100) NOT NULL,
                    "DSC_REWARD_ITEM_TYPE_DESC" TEXT NULL,
                    "STATUS_REWARD_ITEM_TYPE_IS_ACTIVE" BOOLEAN NOT NULL DEFAULT TRUE
                );

                CREATE TABLE IF NOT EXISTS "LULM_REWARD_ITEM" (
                    "ID_REWARD_ITEM" SERIAL PRIMARY KEY,
                    "ID_REWARD_ITEM_TYPE" INTEGER NOT NULL
                        REFERENCES "LULM_REWARD_ITEM_TYPE"("ID_REWARD_ITEM_TYPE") ON DELETE CASCADE,
                    "DSC_REWARD_ITEM_NAME" VARCHAR(100) NOT NULL,
                    "DSC_REWARD_ITEM_DESCRIPTION" TEXT NULL,
                    "NUM_REWARD_ITEM_COST_GOLD" INTEGER NOT NULL DEFAULT 0,
                    "NUM_REWARD_ITEM_EFFECT_VALUE" NUMERIC(10,2) NULL,
                    "STATUS_REWARD_ITEM_IS_ACTIVE" BOOLEAN NOT NULL DEFAULT TRUE
                );

                CREATE INDEX IF NOT EXISTS "IX_LULM_REWARD_ITEM_ID_REWARD_ITEM_TYPE"
                    ON "LULM_REWARD_ITEM"("ID_REWARD_ITEM_TYPE");
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP TABLE IF EXISTS "LULM_REWARD_ITEM";
                DROP TABLE IF EXISTS "LULM_REWARD_ITEM_TYPE";
                DROP TABLE IF EXISTS "LULH_PLAYER_EVENT";
                DROP TABLE IF EXISTS "LULT_HABIT_TASK_TIMER_CRITERIA";

                DROP INDEX IF EXISTS "IX_LULM_HABIT_TASK_ID_HABIT_DISCIPLINE";
                ALTER TABLE "LULM_HABIT_TASK"
                    DROP COLUMN IF EXISTS "ID_HABIT_DISCIPLINE",
                    DROP COLUMN IF EXISTS "NUM_HABIT_TASK_XP_VALUE",
                    DROP COLUMN IF EXISTS "STATUS_HABIT_TASK_IS_COMPLETED",
                    DROP COLUMN IF EXISTS "STATUS_HABIT_TASK_IS_ACTIVE";
                """);
        }
    }
}
