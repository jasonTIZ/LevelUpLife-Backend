using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LevelUpLife_Backend.Migrations
{
    /// <inheritdoc />
    public partial class BlockBStreakProtection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "LULH_STREAK_LOG"
                    ADD COLUMN IF NOT EXISTS "DSC_STREAK_PROTECTION_TYPE" VARCHAR(20) NULL;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "LULH_STREAK_LOG"
                    DROP COLUMN IF EXISTS "DSC_STREAK_PROTECTION_TYPE";
                """);
        }
    }
}
