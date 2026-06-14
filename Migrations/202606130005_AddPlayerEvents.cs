using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LevelUpLife_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS "LULH_PLAYER_EVENT" (
                    "ID_PLAYER_EVENT" SERIAL PRIMARY KEY,
                    "ID_PLAYER_USER" INTEGER NOT NULL REFERENCES "LULM_PLAYER_USER"("ID_PLAYER_USER") ON DELETE CASCADE,
                    "TYPE_PLAYER_EVENT" VARCHAR(50) NOT NULL,
                    "DSC_PLAYER_EVENT_PAYLOAD" TEXT NULL,
                    "FEC_PLAYER_EVENT_CREATED" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP
                );

                CREATE INDEX IF NOT EXISTS "IX_LULH_PLAYER_EVENT_PLAYER_CREATED"
                    ON "LULH_PLAYER_EVENT"("ID_PLAYER_USER", "FEC_PLAYER_EVENT_CREATED");
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP INDEX IF EXISTS "IX_LULH_PLAYER_EVENT_PLAYER_CREATED";
                DROP TABLE IF EXISTS "LULH_PLAYER_EVENT";
                """);
        }
    }
}
