using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LevelUpLife_Backend.Migrations
{
    /// <inheritdoc />
    public partial class Issue29StreakCompletionTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "LULH_STREAK_LOG"
                    ADD COLUMN IF NOT EXISTS "STATUS_STREAK_COMPLETION_RECORDED" BOOLEAN NOT NULL DEFAULT TRUE;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "LULH_STREAK_LOG"
                    DROP COLUMN IF EXISTS "STATUS_STREAK_COMPLETION_RECORDED";
                """);
        }
    }
}
