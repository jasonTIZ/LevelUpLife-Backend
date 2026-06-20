using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LevelUpLife_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddRewardEffectDurationAndActiveEffects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "LULM_REWARD_ITEM"
                    ADD COLUMN IF NOT EXISTS "NUM_EFFECT_DURATION_DAYS" INTEGER NULL;

                CREATE TABLE IF NOT EXISTS "LULT_PLAYER_ACTIVE_EFFECT" (
                    "ID_PLAYER_ACTIVE_EFFECT"   SERIAL PRIMARY KEY,
                    "ID_PLAYER_USER"            INTEGER NOT NULL
                        REFERENCES "LULM_PLAYER_USER"("ID_PLAYER_USER") ON DELETE CASCADE,
                    "ID_INVENTORY"              INTEGER NOT NULL
                        REFERENCES "LULT_PLAYER_INVENTORY"("ID_INVENTORY") ON DELETE CASCADE,
                    "ID_REWARD_ITEM"            INTEGER NOT NULL
                        REFERENCES "LULM_REWARD_ITEM"("ID_REWARD_ITEM"),
                    "ID_REWARD_ITEM_TYPE"       INTEGER NOT NULL
                        REFERENCES "LULM_REWARD_ITEM_TYPE"("ID_REWARD_ITEM_TYPE"),
                    "NUM_EFFECT_VALUE"            NUMERIC(10,2) NULL,
                    "NUM_REMAINING_CHARGES"     INTEGER NULL,
                    "FEC_ACTIVATED"             TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    "FEC_EXPIRES_AT"            TIMESTAMPTZ NULL,
                    "STATUS_IS_ACTIVE"          BOOLEAN NOT NULL DEFAULT TRUE
                );

                CREATE INDEX IF NOT EXISTS "IX_LULT_PLAYER_ACTIVE_EFFECT_ID_PLAYER_USER"
                    ON "LULT_PLAYER_ACTIVE_EFFECT"("ID_PLAYER_USER");

                CREATE INDEX IF NOT EXISTS "IX_LULT_PLAYER_ACTIVE_EFFECT_ID_INVENTORY"
                    ON "LULT_PLAYER_ACTIVE_EFFECT"("ID_INVENTORY");
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP TABLE IF EXISTS "LULT_PLAYER_ACTIVE_EFFECT";

                ALTER TABLE "LULM_REWARD_ITEM"
                    DROP COLUMN IF EXISTS "NUM_EFFECT_DURATION_DAYS";
                """);
        }
    }
}
