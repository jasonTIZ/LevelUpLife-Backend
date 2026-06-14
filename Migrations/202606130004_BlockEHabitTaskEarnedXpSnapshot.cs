using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LevelUpLife_Backend.Migrations
{
    /// <inheritdoc />
    public partial class BlockEHabitTaskEarnedXpSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "LULM_HABIT_TASK"
                    ADD COLUMN IF NOT EXISTS "NUM_HABIT_TASK_EARNED_XP" INTEGER NULL;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "LULM_HABIT_TASK"
                    DROP COLUMN IF EXISTS "NUM_HABIT_TASK_EARNED_XP";
                """);
        }
    }
}
