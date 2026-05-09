using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LevelUpLife_Backend.Migrations
{
    /// <inheritdoc />
    public partial class SeedTestData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                DECLARE
                    person_id INT;
                    class_id INT;
                    category_id INT;
                    discipline_id INT;
                    habit_id INT;
                    task_id INT;
                BEGIN
                    -- 1. Persona
                    INSERT INTO ""LULM_PERSON"" (""DSC_PERSON_NAME"", ""DSC_PERSON_LAST_NAME"", ""DSC_PERSON_PHONE_NUMBER"", ""DSC_PERSON_EMAIL"", ""FEC_PERSON_BIRTHDATE"", ""STATUS_PERSON_IS_ACTIVE"")
                    VALUES ('Test', 'User', '123456789', 'test@leveluplife.com', '1990-01-01', true)
                    ON CONFLICT (""DSC_PERSON_EMAIL"") DO UPDATE SET ""DSC_PERSON_NAME"" = EXCLUDED.""DSC_PERSON_NAME""
                    RETURNING ""ID_PERSON"" INTO person_id;

                    -- 2. Clase
                    INSERT INTO ""LULM_USER_PLAYER_CLASS"" (""DSC_USER_PLAYER_CLASS_NAME"", ""DSC_USER_PLAYER_CLASS_DESCRIPTION"", ""NUM_CLASS_XP_MULT_STUDY"", ""NUM_CLASS_XP_MULT_SPORT"", ""NUM_CLASS_XP_MULT_FOOD"", ""STATUS_USER_PLAYER_CLASS_IS_ACTIVE"")
                    VALUES ('Warrior', 'Strong and brave', 1.0, 1.2, 1.0, true)
                    ON CONFLICT (""ID_USER_PLAYER_CLASS"") DO UPDATE SET ""DSC_USER_PLAYER_CLASS_DESCRIPTION"" = EXCLUDED.""DSC_USER_PLAYER_CLASS_DESCRIPTION""
                    RETURNING ""ID_USER_PLAYER_CLASS"" INTO class_id;

                    -- 3. Usuario
                    INSERT INTO ""LULM_PLAYER_USER"" (""ID_PERSON"", ""ID_USER_PLAYER_CLASS"", ""DSC_PLAYER_USER_USERNAME"", ""DSC_PLAYER_USER_PASSWORD"", ""NUM_PLAYER_USER_LEVEL"", ""NUM_PLAYER_USER_EXPERIENCE_POINTS"", ""NUM_PLAYER_USER_DAYS_STREAK"", ""FEC_PLAYER_USER_CREATION_DATE"", ""STATUS_PLAYER_USER_IS_ACTIVE"")
                    VALUES (person_id, class_id, 'testuser', 'hashedpassword', 1, 0, 0, NOW(), true)
                    ON CONFLICT (""DSC_PLAYER_USER_USERNAME"") DO UPDATE SET ""NUM_PLAYER_USER_LEVEL"" = EXCLUDED.""NUM_PLAYER_USER_LEVEL""
                    RETURNING ""ID_PLAYER_USER"" INTO habit_id; -- Reusing variable for convenience

                    -- 4. Categoría
                    INSERT INTO ""LULM_HABIT_CATEGORY"" (""DSC_HABIT_CATEGORY_NAME"", ""DSC_HABIT_CATEGORY_DESCRIPTION"", ""STATUS_HABIT_CATEGORY_IS_ACTIVE"")
                    VALUES ('Salud', 'Hábitos relacionados con el bienestar físico', true)
                    RETURNING ""ID_HABIT_CATEGORY"" INTO category_id;

                    -- 5. Disciplina
                    INSERT INTO ""LULM_HABIT_DISCIPLINE"" (""ID_HABIT_CATEGORY"", ""DSC_HABIT_DISCIPLINE_NAME"", ""DSC_HABIT_DISCIPLINE_DESCRIPTION"", ""STATUS_HABIT_DISCIPLINE_IS_ACTIVE"")
                    VALUES (category_id, 'Ejercicio', 'Actividad física diaria', true)
                    RETURNING ""ID_HABIT_DISCIPLINE"" INTO discipline_id;

                    -- 6. Hábito
                    INSERT INTO ""LULM_HABIT"" (""ID_HABIT_DISCIPLINE"", ""ID_PLAYER_USER"", ""DSC_HABIT_TITLE"", ""DSC_HABIT_DESCRIPTION"", ""STATUS_HABIT_IS_ACTIVE"")
                    VALUES (discipline_id, habit_id, 'Caminar Diariamente', 'Caminar al menos 30 minutos', true)
                    RETURNING ""ID_HABIT"" INTO habit_id;

                    -- 7. Tarea
                    INSERT INTO ""LULM_HABIT_TASK"" (""ID_HABIT"", ""DSC_HABIT_TASK_TITLE"", ""DSC_HABIT_TASK_DESCRIPTION"", ""DSC_HABIT_TASK_WEEK_DAYS"", ""TYPE_HABIT_TASK_DIFFICULTY"", ""TYPE_HABIT_TASK_FREQUENCY"", ""NUM_HABIT_TASK_PERIOD_LENGTH"", ""TYPE_HABIT_TASK_PERIOD_UNIT"", ""FEC_HABIT_TASK_START_DATE"", ""TYPE_COMPLETION_CRITERIA"", ""TYPE_HABIT_TASK_EVIDENCE"")
                    VALUES (habit_id, 'Caminata Mañanera', 'Caminata de prueba', 'Lunes,Martes,Miercoles', 'MEDIUM', 'DAILY', 1, 'DAYS', '2026-05-01', 'EVIDENCE', 'PHOTO')
                    RETURNING ""ID_HABIT_TASK"" INTO task_id;

                    -- 8. Evidencias
                    INSERT INTO ""LULT_HABIT_TASK_EVIDENCE_STORAGE"" (""ID_HABIT_TASK"", ""DSC_EVIDENCE_PATH_URL"", ""DSC_HEALTH_DATA_JSON"", ""FEC_UPLOADED"")
                    VALUES 
                    (task_id, 'https://storage.googleapis.com/leveluplife/evidences/photo1.jpg', NULL, NOW()),
                    (task_id, 'https://storage.googleapis.com/leveluplife/evidences/photo2.jpg', NULL, NOW() - INTERVAL '1 day'),
                    (task_id, NULL, '{""steps"": 10500, ""calories"": 450, ""distance_meters"": 8000}', NOW() - INTERVAL '2 days');
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM \"LULT_HABIT_TASK_EVIDENCE_STORAGE\" WHERE \"DSC_EVIDENCE_PATH_URL\" LIKE 'https://storage.googleapis.com/leveluplife/%';");
            migrationBuilder.Sql("DELETE FROM \"LULM_HABIT_TASK\" WHERE \"DSC_HABIT_TASK_TITLE\" = 'Caminata Mañanera';");
            migrationBuilder.Sql("DELETE FROM \"LULM_HABIT\" WHERE \"DSC_HABIT_TITLE\" = 'Caminar Diariamente';");
            migrationBuilder.Sql("DELETE FROM \"LULM_HABIT_DISCIPLINE\" WHERE \"DSC_HABIT_DISCIPLINE_NAME\" = 'Ejercicio';");
            migrationBuilder.Sql("DELETE FROM \"LULM_HABIT_CATEGORY\" WHERE \"DSC_HABIT_CATEGORY_NAME\" = 'Salud';");
            migrationBuilder.Sql("DELETE FROM \"LULM_PLAYER_USER\" WHERE \"DSC_PLAYER_USER_USERNAME\" = 'testuser';");
            migrationBuilder.Sql("DELETE FROM \"LULM_USER_PLAYER_CLASS\" WHERE \"DSC_USER_PLAYER_CLASS_NAME\" = 'Warrior';");
            migrationBuilder.Sql("DELETE FROM \"LULM_PERSON\" WHERE \"DSC_PERSON_EMAIL\" = 'test@leveluplife.com';");
        }
    }
}
