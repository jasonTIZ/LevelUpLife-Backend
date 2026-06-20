using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LevelUpLife_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerGoldAndInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "LULM_PLAYER_USER"
                    ADD COLUMN IF NOT EXISTS "NUM_PLAYER_USER_GOLD" INTEGER NOT NULL DEFAULT 300;

                UPDATE "LULM_PLAYER_USER"
                SET "NUM_PLAYER_USER_GOLD" = 300
                WHERE "NUM_PLAYER_USER_GOLD" IS NULL OR "NUM_PLAYER_USER_GOLD" = 0;

                CREATE TABLE IF NOT EXISTS "LULT_PLAYER_INVENTORY" (
                    "ID_INVENTORY" SERIAL PRIMARY KEY,
                    "ID_PLAYER_USER" INTEGER NOT NULL
                        REFERENCES "LULM_PLAYER_USER"("ID_PLAYER_USER") ON DELETE CASCADE,
                    "ID_REWARD_ITEM" INTEGER NOT NULL
                        REFERENCES "LULM_REWARD_ITEM"("ID_REWARD_ITEM"),
                    "NUM_INVENTORY_QUANTITY" INTEGER NOT NULL DEFAULT 1,
                    "STATUS_INVENTORY_IS_EQUIPPED" BOOLEAN NOT NULL DEFAULT FALSE,
                    "FEC_ACQUIRED" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP
                );

                CREATE INDEX IF NOT EXISTS "IDX_INVENTORY_PLAYER"
                    ON "LULT_PLAYER_INVENTORY"("ID_PLAYER_USER");
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP TABLE IF EXISTS "LULT_PLAYER_INVENTORY";

                ALTER TABLE "LULM_PLAYER_USER"
                    DROP COLUMN IF EXISTS "NUM_PLAYER_USER_GOLD";
                """);
        }
    }
}
