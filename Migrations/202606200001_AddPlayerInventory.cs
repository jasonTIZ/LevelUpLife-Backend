using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LevelUpLife_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS "LULT_PLAYER_INVENTORY" (
                    "ID_INVENTORY"                 SERIAL PRIMARY KEY,
                    "ID_PLAYER_USER"               INTEGER NOT NULL
                        REFERENCES "LULM_PLAYER_USER"("ID_PLAYER_USER") ON DELETE CASCADE,
                    "ID_REWARD_ITEM"               INTEGER NOT NULL
                        REFERENCES "LULM_REWARD_ITEM"("ID_REWARD_ITEM"),
                    "NUM_INVENTORY_QUANTITY"        INTEGER NOT NULL DEFAULT 1,
                    "STATUS_INVENTORY_IS_EQUIPPED"  BOOLEAN NOT NULL DEFAULT FALSE,
                    "FEC_ACQUIRED"                  TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP
                );

                CREATE INDEX IF NOT EXISTS "IX_LULT_PLAYER_INVENTORY_ID_PLAYER_USER"
                    ON "LULT_PLAYER_INVENTORY"("ID_PLAYER_USER");

                CREATE INDEX IF NOT EXISTS "IX_LULT_PLAYER_INVENTORY_ID_REWARD_ITEM"
                    ON "LULT_PLAYER_INVENTORY"("ID_REWARD_ITEM");
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP TABLE IF EXISTS "LULT_PLAYER_INVENTORY";
                """);
        }
    }
}
