using System;
using LevelUpLifeBackend.Models;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LevelUpLife_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddHabitTaskAndEvidenceTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:ENUM_COMPLETION_CRITERIA", "EVIDENCE,REPETITIONS,TIMER")
                .Annotation("Npgsql:Enum:ENUM_COMPLETION_CRITERIA.TaskCompletionCriteria", "REPETITIONS,TIMER,EVIDENCE")
                .Annotation("Npgsql:Enum:ENUM_DIFFICULTY", "EASY,EPIC,HARD,MEDIUM")
                .Annotation("Npgsql:Enum:ENUM_DIFFICULTY.TaskDifficulty", "EASY,MEDIUM,HARD,EPIC")
                .Annotation("Npgsql:Enum:ENUM_EVIDENCE", "HEALTH_CONNECT,PHOTO,VIDEO")
                .Annotation("Npgsql:Enum:ENUM_EVIDENCE.TaskEvidence", "PHOTO,VIDEO,HEALTH_CONNECT")
                .Annotation("Npgsql:Enum:ENUM_FREQUENCY", "DAILY,WEEKLY")
                .Annotation("Npgsql:Enum:ENUM_FREQUENCY.TaskFrequency", "DAILY,WEEKLY")
                .Annotation("Npgsql:Enum:ENUM_MEASUREMENT_UNIT", "CALS,KMS,REPS,SERIES")
                .Annotation("Npgsql:Enum:ENUM_MEASUREMENT_UNIT.MeasurementUnit", "REPS,SERIES,KMS,CALS")
                .Annotation("Npgsql:Enum:ENUM_PERIOD_UNIT", "DAYS,MONTHS,WEEKS")
                .Annotation("Npgsql:Enum:ENUM_PERIOD_UNIT.TaskPeriodUnit", "DAYS,WEEKS,MONTHS");

            migrationBuilder.CreateTable(
                name: "LULM_HABIT_TASK",
                columns: table => new
                {
                    ID_HABIT_TASK = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ID_HABIT = table.Column<int>(type: "integer", nullable: false),
                    DSC_HABIT_TASK_TITLE = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DSC_HABIT_TASK_DESCRIPTION = table.Column<string>(type: "text", nullable: true),
                    DSC_HABIT_TASK_WEEK_DAYS = table.Column<string>(type: "text", nullable: true),
                    TYPE_HABIT_TASK_DIFFICULTY = table.Column<TaskDifficulty>(type: "\"ENUM_DIFFICULTY\"", nullable: false),
                    TYPE_HABIT_TASK_FREQUENCY = table.Column<TaskFrequency>(type: "\"ENUM_FREQUENCY\"", nullable: false),
                    NUM_HABIT_TASK_PERIOD_LENGTH = table.Column<int>(type: "integer", nullable: false),
                    TYPE_HABIT_TASK_PERIOD_UNIT = table.Column<TaskPeriodUnit>(type: "\"ENUM_PERIOD_UNIT\"", nullable: false),
                    FEC_HABIT_TASK_START_DATE = table.Column<DateOnly>(type: "date", nullable: false),
                    TYPE_COMPLETION_CRITERIA = table.Column<TaskCompletionCriteria>(type: "\"ENUM_COMPLETION_CRITERIA\"", nullable: false),
                    TYPE_HABIT_TASK_EVIDENCE = table.Column<TaskEvidence>(type: "\"ENUM_EVIDENCE\"", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LULM_HABIT_TASK", x => x.ID_HABIT_TASK);
                    table.ForeignKey(
                        name: "FK_LULM_HABIT_TASK_LULM_HABIT_ID_HABIT",
                        column: x => x.ID_HABIT,
                        principalTable: "LULM_HABIT",
                        principalColumn: "ID_HABIT",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LULT_HABIT_TASK_EVIDENCE_STORAGE",
                columns: table => new
                {
                    ID_EVIDENCE = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ID_HABIT_TASK = table.Column<int>(type: "integer", nullable: false),
                    DSC_EVIDENCE_PATH_URL = table.Column<string>(type: "text", nullable: true),
                    DSC_HEALTH_DATA_JSON = table.Column<string>(type: "text", nullable: true),
                    FEC_UPLOADED = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LULT_HABIT_TASK_EVIDENCE_STORAGE", x => x.ID_EVIDENCE);
                    table.ForeignKey(
                        name: "FK_LULT_HABIT_TASK_EVIDENCE_STORAGE_LULM_HABIT_TASK_ID_HABIT_T~",
                        column: x => x.ID_HABIT_TASK,
                        principalTable: "LULM_HABIT_TASK",
                        principalColumn: "ID_HABIT_TASK",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LULT_HABIT_TASK_REPETITIONS_CRITERIA",
                columns: table => new
                {
                    ID_HABIT_TASK_REPETITIONS_CRITERIA = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ID_HABIT_TASK = table.Column<int>(type: "integer", nullable: false),
                    NUM_REPETITIONS_OBJECTIVE = table.Column<int>(type: "integer", nullable: false),
                    TYPE_UNITY_MEASUREMENT_UNIT = table.Column<MeasurementUnit>(type: "\"ENUM_MEASUREMENT_UNIT\"", nullable: false),
                    STATUS_IS_PARTIAL_ALLOWED = table.Column<bool>(type: "boolean", nullable: false),
                    STATUS_REPETITIONS_CRITERIA_IS_ACTIVE = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LULT_HABIT_TASK_REPETITIONS_CRITERIA", x => x.ID_HABIT_TASK_REPETITIONS_CRITERIA);
                    table.ForeignKey(
                        name: "FK_LULT_HABIT_TASK_REPETITIONS_CRITERIA_LULM_HABIT_TASK_ID_HAB~",
                        column: x => x.ID_HABIT_TASK,
                        principalTable: "LULM_HABIT_TASK",
                        principalColumn: "ID_HABIT_TASK",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LULM_HABIT_TASK_ID_HABIT",
                table: "LULM_HABIT_TASK",
                column: "ID_HABIT");

            migrationBuilder.CreateIndex(
                name: "IX_LULT_HABIT_TASK_EVIDENCE_STORAGE_ID_HABIT_TASK",
                table: "LULT_HABIT_TASK_EVIDENCE_STORAGE",
                column: "ID_HABIT_TASK");

            migrationBuilder.CreateIndex(
                name: "IX_LULT_HABIT_TASK_REPETITIONS_CRITERIA_ID_HABIT_TASK",
                table: "LULT_HABIT_TASK_REPETITIONS_CRITERIA",
                column: "ID_HABIT_TASK",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LULT_HABIT_TASK_EVIDENCE_STORAGE");

            migrationBuilder.DropTable(
                name: "LULT_HABIT_TASK_REPETITIONS_CRITERIA");

            migrationBuilder.DropTable(
                name: "LULM_HABIT_TASK");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:ENUM_COMPLETION_CRITERIA", "EVIDENCE,REPETITIONS,TIMER")
                .OldAnnotation("Npgsql:Enum:ENUM_COMPLETION_CRITERIA.TaskCompletionCriteria", "REPETITIONS,TIMER,EVIDENCE")
                .OldAnnotation("Npgsql:Enum:ENUM_DIFFICULTY", "EASY,EPIC,HARD,MEDIUM")
                .OldAnnotation("Npgsql:Enum:ENUM_DIFFICULTY.TaskDifficulty", "EASY,MEDIUM,HARD,EPIC")
                .OldAnnotation("Npgsql:Enum:ENUM_EVIDENCE", "HEALTH_CONNECT,PHOTO,VIDEO")
                .OldAnnotation("Npgsql:Enum:ENUM_EVIDENCE.TaskEvidence", "PHOTO,VIDEO,HEALTH_CONNECT")
                .OldAnnotation("Npgsql:Enum:ENUM_FREQUENCY", "DAILY,WEEKLY")
                .OldAnnotation("Npgsql:Enum:ENUM_FREQUENCY.TaskFrequency", "DAILY,WEEKLY")
                .OldAnnotation("Npgsql:Enum:ENUM_MEASUREMENT_UNIT", "CALS,KMS,REPS,SERIES")
                .OldAnnotation("Npgsql:Enum:ENUM_MEASUREMENT_UNIT.MeasurementUnit", "REPS,SERIES,KMS,CALS")
                .OldAnnotation("Npgsql:Enum:ENUM_PERIOD_UNIT", "DAYS,MONTHS,WEEKS")
                .OldAnnotation("Npgsql:Enum:ENUM_PERIOD_UNIT.TaskPeriodUnit", "DAYS,WEEKS,MONTHS");
        }
    }
}
