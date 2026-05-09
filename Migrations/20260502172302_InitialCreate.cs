using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LevelUpLife_Backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LULM_HABIT_CATEGORY",
                columns: table => new
                {
                    ID_HABIT_CATEGORY = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DSC_HABIT_CATEGORY_NAME = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DSC_HABIT_CATEGORY_DESCRIPTION = table.Column<string>(type: "text", nullable: false),
                    STATUS_HABIT_CATEGORY_IS_ACTIVE = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LULM_HABIT_CATEGORY", x => x.ID_HABIT_CATEGORY);
                });

            migrationBuilder.CreateTable(
                name: "LULM_PERSON",
                columns: table => new
                {
                    ID_PERSON = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DSC_PERSON_NAME = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DSC_PERSON_LAST_NAME = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DSC_PERSON_PHONE_NUMBER = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DSC_PERSON_EMAIL = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    FEC_PERSON_BIRTHDATE = table.Column<DateOnly>(type: "date", nullable: false),
                    STATUS_PERSON_IS_ACTIVE = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LULM_PERSON", x => x.ID_PERSON);
                });

            migrationBuilder.CreateTable(
                name: "LULM_USER_PLAYER_CLASS",
                columns: table => new
                {
                    ID_USER_PLAYER_CLASS = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DSC_USER_PLAYER_CLASS_NAME = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DSC_USER_PLAYER_CLASS_DESCRIPTION = table.Column<string>(type: "text", nullable: false),
                    NUM_CLASS_XP_MULT_STUDY = table.Column<double>(type: "double precision", nullable: false),
                    NUM_CLASS_XP_MULT_SPORT = table.Column<double>(type: "double precision", nullable: false),
                    NUM_CLASS_XP_MULT_FOOD = table.Column<double>(type: "double precision", nullable: false),
                    STATUS_USER_PLAYER_CLASS_IS_ACTIVE = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LULM_USER_PLAYER_CLASS", x => x.ID_USER_PLAYER_CLASS);
                });

            migrationBuilder.CreateTable(
                name: "LULM_HABIT_DISCIPLINE",
                columns: table => new
                {
                    ID_HABIT_DISCIPLINE = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ID_HABIT_CATEGORY = table.Column<int>(type: "integer", nullable: false),
                    DSC_HABIT_DISCIPLINE_NAME = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DSC_HABIT_DISCIPLINE_DESCRIPTION = table.Column<string>(type: "text", nullable: false),
                    STATUS_HABIT_DISCIPLINE_IS_ACTIVE = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LULM_HABIT_DISCIPLINE", x => x.ID_HABIT_DISCIPLINE);
                    table.ForeignKey(
                        name: "FK_LULM_HABIT_DISCIPLINE_LULM_HABIT_CATEGORY_ID_HABIT_CATEGORY",
                        column: x => x.ID_HABIT_CATEGORY,
                        principalTable: "LULM_HABIT_CATEGORY",
                        principalColumn: "ID_HABIT_CATEGORY",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LULM_PLAYER_USER",
                columns: table => new
                {
                    ID_PLAYER_USER = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ID_PERSON = table.Column<int>(type: "integer", nullable: false),
                    ID_USER_PLAYER_CLASS = table.Column<int>(type: "integer", nullable: false),
                    DSC_PLAYER_USER_USERNAME = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DSC_PLAYER_USER_PASSWORD = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NUM_PLAYER_USER_LEVEL = table.Column<int>(type: "integer", nullable: false),
                    NUM_PLAYER_USER_EXPERIENCE_POINTS = table.Column<int>(type: "integer", nullable: false),
                    NUM_PLAYER_USER_DAYS_STREAK = table.Column<int>(type: "integer", nullable: false),
                    FEC_PLAYER_USER_LAST_LOGIN = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FEC_PLAYER_USER_CREATION_DATE = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    STATUS_PLAYER_USER_IS_ACTIVE = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LULM_PLAYER_USER", x => x.ID_PLAYER_USER);
                    table.ForeignKey(
                        name: "FK_LULM_PLAYER_USER_LULM_PERSON_ID_PERSON",
                        column: x => x.ID_PERSON,
                        principalTable: "LULM_PERSON",
                        principalColumn: "ID_PERSON",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LULM_PLAYER_USER_LULM_USER_PLAYER_CLASS_ID_USER_PLAYER_CLASS",
                        column: x => x.ID_USER_PLAYER_CLASS,
                        principalTable: "LULM_USER_PLAYER_CLASS",
                        principalColumn: "ID_USER_PLAYER_CLASS",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LULM_HABIT",
                columns: table => new
                {
                    ID_HABIT = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ID_HABIT_DISCIPLINE = table.Column<int>(type: "integer", nullable: false),
                    ID_PLAYER_USER = table.Column<int>(type: "integer", nullable: false),
                    DSC_HABIT_TITLE = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DSC_HABIT_DESCRIPTION = table.Column<string>(type: "text", nullable: false),
                    STATUS_HABIT_IS_ACTIVE = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LULM_HABIT", x => x.ID_HABIT);
                    table.ForeignKey(
                        name: "FK_LULM_HABIT_LULM_HABIT_DISCIPLINE_ID_HABIT_DISCIPLINE",
                        column: x => x.ID_HABIT_DISCIPLINE,
                        principalTable: "LULM_HABIT_DISCIPLINE",
                        principalColumn: "ID_HABIT_DISCIPLINE",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LULM_HABIT_LULM_PLAYER_USER_ID_PLAYER_USER",
                        column: x => x.ID_PLAYER_USER,
                        principalTable: "LULM_PLAYER_USER",
                        principalColumn: "ID_PLAYER_USER",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LULM_HABIT_ID_HABIT_DISCIPLINE",
                table: "LULM_HABIT",
                column: "ID_HABIT_DISCIPLINE");

            migrationBuilder.CreateIndex(
                name: "IX_LULM_HABIT_ID_PLAYER_USER",
                table: "LULM_HABIT",
                column: "ID_PLAYER_USER");

            migrationBuilder.CreateIndex(
                name: "IX_LULM_HABIT_DISCIPLINE_ID_HABIT_CATEGORY",
                table: "LULM_HABIT_DISCIPLINE",
                column: "ID_HABIT_CATEGORY");

            migrationBuilder.CreateIndex(
                name: "IX_LULM_PERSON_DSC_PERSON_EMAIL",
                table: "LULM_PERSON",
                column: "DSC_PERSON_EMAIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LULM_PLAYER_USER_DSC_PLAYER_USER_USERNAME",
                table: "LULM_PLAYER_USER",
                column: "DSC_PLAYER_USER_USERNAME",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LULM_PLAYER_USER_ID_PERSON",
                table: "LULM_PLAYER_USER",
                column: "ID_PERSON",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LULM_PLAYER_USER_ID_USER_PLAYER_CLASS",
                table: "LULM_PLAYER_USER",
                column: "ID_USER_PLAYER_CLASS");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LULM_HABIT");

            migrationBuilder.DropTable(
                name: "LULM_HABIT_DISCIPLINE");

            migrationBuilder.DropTable(
                name: "LULM_PLAYER_USER");

            migrationBuilder.DropTable(
                name: "LULM_HABIT_CATEGORY");

            migrationBuilder.DropTable(
                name: "LULM_PERSON");

            migrationBuilder.DropTable(
                name: "LULM_USER_PLAYER_CLASS");
        }
    }
}
