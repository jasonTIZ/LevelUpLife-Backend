using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LevelUpLife_Backend.Migrations
{
    /// <inheritdoc />
    public partial class MakePersonPhoneNumberOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "LULM_PERSON"
                    ALTER COLUMN "DSC_PERSON_PHONE_NUMBER" DROP NOT NULL;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE "LULM_PERSON"
                SET "DSC_PERSON_PHONE_NUMBER" = ''
                WHERE "DSC_PERSON_PHONE_NUMBER" IS NULL;

                ALTER TABLE "LULM_PERSON"
                    ALTER COLUMN "DSC_PERSON_PHONE_NUMBER" SET NOT NULL;
                """);
        }
    }
}
