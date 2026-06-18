using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LevelUpLife_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerProfileBioAndAvatar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "LULM_PLAYER_USER"
                    ADD COLUMN IF NOT EXISTS "DSC_PLAYER_USER_BIO" VARCHAR(500);

                ALTER TABLE "LULM_PLAYER_USER"
                    ADD COLUMN IF NOT EXISTS "DSC_PLAYER_USER_AVATAR_URL" VARCHAR(500);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "LULM_PLAYER_USER"
                    DROP COLUMN IF EXISTS "DSC_PLAYER_USER_AVATAR_URL";

                ALTER TABLE "LULM_PLAYER_USER"
                    DROP COLUMN IF EXISTS "DSC_PLAYER_USER_BIO";
                """);
        }
    }
}
