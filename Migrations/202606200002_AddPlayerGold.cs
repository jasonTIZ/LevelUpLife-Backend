using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LevelUpLife_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerGold : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "LULM_PLAYER_USER"
                    ADD COLUMN IF NOT EXISTS "NUM_PLAYER_USER_GOLD" INTEGER NOT NULL DEFAULT 0;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "LULM_PLAYER_USER"
                    DROP COLUMN IF EXISTS "NUM_PLAYER_USER_GOLD";
                """);
        }
    }
}
