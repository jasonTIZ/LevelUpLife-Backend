-- ==============================================================================
-- 1. IDENTIDAD Y USUARIOS (Pocos registros de prueba)
-- ==============================================================================

INSERT INTO "LULM_PERSON" ("DSC_PERSON_NAME", "DSC_PERSON_LAST_NAME", "DSC_PERSON_PHONE_NUMBER", "DSC_PERSON_EMAIL", "FEC_PERSON_BIRTHDATE") VALUES
('Arthur', 'Pendragon', '555-0101', 'arthur.p@leveluplife.com', '1995-03-15'),
('Lara', 'Croft', '555-0202', 'lara.croft@leveluplife.com', '1992-02-14'),
('Geralt', 'Rivia', '555-0303', 'geralt.r@leveluplife.com', '1985-11-05');

INSERT INTO "LULM_USER_PLAYER_CLASS" ("DSC_USER_PLAYER_CLASS_NAME", "DSC_USER_PLAYER_CLASS_DESCRIPTION", "NUM_CLASS_XP_MULT_STUDY", "NUM_CLASS_XP_MULT_SPORT", "NUM_CLASS_XP_MULT_FOOD") VALUES
('Guerrero Físico', 'Especialista en fuerza y resistencia. Gana más XP en deportes.', 1.00, 1.50, 1.20),
('Mago Erudito', 'Enfocado en el aprendizaje continuo y salud mental.', 1.50, 1.00, 1.00),
('Pícaro Ágil', 'Balanceado, se adapta a todo. Experto en nutrición rápida.', 1.10, 1.10, 1.50);

INSERT INTO "LULM_PLAYER_USER" ("ID_PERSON", "ID_USER_PLAYER_CLASS", "DSC_PLAYER_USER_USERNAME", "DSC_PLAYER_USER_PASSWORD", "NUM_PLAYER_USER_LEVEL", "NUM_PLAYER_USER_EXPERIENCE_POINTS", "NUM_PLAYER_USER_DAYS_STREAK") VALUES
(1, 1, 'ArturoKing', 'hash_password_123', 10, 4500, 15),
(2, 3, 'TombRaider92', 'hash_password_456', 24, 12800, 42),
(3, 2, 'WhiteWolf', 'hash_password_789', 5, 1200, 3);

-- ==============================================================================
-- 2. CATÁLOGOS DE HÁBITOS (Bastantes registros)
-- ==============================================================================

INSERT INTO "LULM_HABIT_CATEGORY" ("DSC_HABIT_CATEGORY_NAME", "DSC_HABIT_CATEGORY_DESCRIPTION") VALUES
('Salud y Fitness', 'Hábitos de ejercicio físico y bienestar.'),
('Intelecto', 'Lectura, estudio y aprendizaje.'),
('Nutrición', 'Alimentación saludable y control de dietas.'),
('Productividad', 'Organización, trabajo profundo y finanzas.'),
('Salud Mental', 'Meditación, descanso y desconexión.'),
('Creatividad', 'Arte, música y expresión personal.'),
('Vida Social', 'Relaciones, familia y networking.'),
('Aventuras', 'Exploración, viajes y tiempo al aire libre.');

INSERT INTO "LULM_HABIT_DISCIPLINE" ("ID_HABIT_CATEGORY", "DSC_HABIT_DISCIPLINE_NAME", "DSC_HABIT_DISCIPLINE_DESCRIPTION") VALUES
(1, 'Levantamiento de Pesas', 'Hipertrofia y fuerza en gimnasio.'),
(1, 'Cardio', 'Correr, nadar o ciclismo.'),
(2, 'Programación', 'Aprender nuevos lenguajes de código.'),
(2, 'Idiomas', 'Práctica de gramática y vocabulario extranjero.'),
(3, 'Hidratación', 'Consumo adecuado de agua diaria.'),
(3, 'Ayuno', 'Práctica de ayuno intermitente.'),
(4, 'Finanzas Personales', 'Control de gastos y ahorro.'),
(4, 'Gestión del Tiempo', 'Uso de Pomodoro y agendas.'),
(5, 'Meditación', 'Prácticas de mindfulness y respiración.'),
(5, 'Higiene del Sueño', 'Rutinas para dormir mejor.'),
(6, 'Escritura', 'Creación de historias o journaling.'),
(6, 'Dibujo', 'Ilustración tradicional o digital.'),
(7, 'Tiempo en Familia', 'Actividades de calidad con seres queridos.'),
(8, 'Senderismo', 'Caminatas por la naturaleza.');

-- ==============================================================================
-- 3. HÁBITOS ACTIVOS (Asignados a los 3 jugadores)
-- ==============================================================================

INSERT INTO "LULM_HABIT" ("ID_HABIT_DISCIPLINE", "ID_PLAYER_USER", "DSC_HABIT_TITLE", "DSC_HABIT_DESCRIPTION") VALUES
(1, 1, 'Rutina de Fuerza Espartana', 'Entrenamiento de pesas de 4 días a la semana.'),
(2, 1, 'Trote Matutino', 'Correr por el parque antes del desayuno.'),
(5, 1, 'Beber 3L de Agua', 'Llevar siempre la botella de agua.'),
(3, 2, 'Dominar C# y .NET', 'Estudiar para mejorar en el backend.'),
(8, 2, 'Bloques de Trabajo Profundo', 'Usar técnica pomodoro en la oficina.'),
(11, 2, 'Diario de Gratitud', 'Escribir 3 cosas buenas al final del día.'),
(9, 3, 'Mindfulness Diario', 'Despejar la mente para reducir estrés.'),
(10, 3, 'Dormir 8 Horas', 'Apagar pantallas a las 10 PM.'),
(14, 3, 'Ruta de Fin de Semana', 'Explorar un sendero nuevo cada sábado.'),
(4, 1, 'Aprender Japonés', 'Lecciones de Duolingo diarias.'),
(7, 2, 'Registro de Gastos', 'Anotar cada compra en la app de finanzas.');

-- ==============================================================================
-- 4. TAREAS Y CRITERIOS (Variedad de enums y dificultades)
-- ==============================================================================

INSERT INTO "LULM_HABIT_TASK" ("ID_HABIT", "ID_HABIT_DISCIPLINE", "DSC_HABIT_TASK_TITLE", "DSC_HABIT_TASK_DESCRIPTION", "DSC_HABIT_TASK_WEEK_DAYS", "TYPE_HABIT_TASK_DIFFICULTY", "NUM_HABIT_TASK_XP_VALUE", "TYPE_HABIT_TASK_FREQUENCY", "NUM_HABIT_TASK_PERIOD_LENGTH", "TYPE_HABIT_TASK_PERIOD_UNIT", "FEC_HABIT_TASK_START_DATE", "TYPE_COMPLETION_CRITERIA", "TYPE_HABIT_TASK_EVIDENCE") VALUES
-- Tareas del Jugador 1 (Guerrero)
(1, 1, 'Hacer Pecho y Tríceps', 'Rutina A del gimnasio.', 'Lunes,Jueves', 'MEDIUM', 50, 'WEEKLY', 1, 'WEEKS', '2023-10-01', 'REPETITIONS', NULL),
(2, 2, 'Correr 5KM', 'Trote a ritmo constante.', 'Martes,Viernes', 'HARD', 100, 'WEEKLY', 1, 'WEEKS', '2023-10-01', 'REPETITIONS', 'HEALTH_CONNECT'),
(3, 5, 'Completar Botella', 'Terminar la botella de 1 litro.', 'Todos', 'EASY', 10, 'DAILY', 1, 'DAYS', '2023-10-01', 'REPETITIONS', NULL),
-- Tareas de la Jugadora 2 (Pícaro)
(4, 3, 'Curso de Entity Framework', 'Ver videos del curso online.', 'Lunes,Miercoles,Viernes', 'HARD', 120, 'WEEKLY', 1, 'WEEKS', '2023-10-01', 'TIMER', NULL),
(5, 8, 'Pomodoro de Programación', 'Codear sin distracciones.', 'Todos', 'MEDIUM', 40, 'DAILY', 1, 'DAYS', '2023-10-01', 'TIMER', NULL),
(6, 11, 'Foto del Diario', 'Subir evidencia de que escribí.', 'Domingo', 'EASY', 20, 'WEEKLY', 1, 'WEEKS', '2023-10-01', 'EVIDENCE', 'PHOTO'),
-- Tareas del Jugador 3 (Mago)
(7, 9, 'Meditación Zen', 'Sentarse en silencio absoluto.', 'Todos', 'EPIC', 200, 'DAILY', 1, 'DAYS', '2023-10-01', 'TIMER', NULL),
(9, 14, 'Caminata a la Montaña', 'Llegar a la cima.', 'Sabado', 'EPIC', 300, 'WEEKLY', 1, 'WEEKS', '2023-10-01', 'EVIDENCE', 'HEALTH_CONNECT');

-- Criterios de Tiempo (Para las tareas con TIMER)
INSERT INTO "LULT_HABIT_TASK_TIMER_CRITERIA" ("ID_HABIT_TASK", "NUM_SECONDS_DEFINED", "TYPE_PAUSE_IS_ALLOWED") VALUES
(4, 3600, TRUE),   -- 1 hora de curso, pausa permitida
(5, 1500, FALSE),  -- 25 mins de pomodoro, sin pausas
(7, 1200, FALSE);  -- 20 mins de meditación, sin pausas

-- Criterios de Repeticiones (Para las tareas con REPETITIONS)
INSERT INTO "LULT_HABIT_TASK_REPETITIONS_CRITERIA" ("ID_HABIT_TASK", "NUM_REPETITIONS_OBJECTIVE", "TYPE_UNITY_MEASUREMENT_UNIT", "STATUS_IS_PARTIAL_ALLOWED") VALUES
(1, 4, 'SERIES', TRUE),
(2, 5, 'KMS', TRUE),
(3, 3, 'REPS', FALSE);

-- Criterios de Evidencia (Para las tareas con EVIDENCE)
INSERT INTO "LULT_HABIT_TASK_EVIDENCE_STORAGE" ("ID_HABIT_TASK", "DSC_EVIDENCE_PATH_URL") VALUES
(6, 'https://s3.aws.com/leveluplife/evidences/user2_journal_1.jpg'),
(8, 'https://s3.aws.com/leveluplife/evidences/user3_mountain_gps.json');

-- ==============================================================================
-- 5. RECOMPENSAS Y ECONOMÍA
-- ==============================================================================

INSERT INTO "LULM_REWARD_ITEM_TYPE" ("DSC_REWARD_ITEM_TYPE_NAME", "DSC_REWARD_ITEM_TYPE_DESC") VALUES
('Poción', 'Consumibles de un solo uso.'),
('Equipamiento', 'Ropa o equipo que da stats pasivos.'),
('Mascota', 'Acompañante virtual en el perfil.'),
('Título', 'Texto especial debajo del nombre.');

INSERT INTO "LULM_REWARD_ITEM" ("ID_REWARD_ITEM_TYPE", "DSC_REWARD_ITEM_NAME", "DSC_REWARD_ITEM_DESCRIPTION", "NUM_REWARD_ITEM_COST_GOLD", "NUM_REWARD_ITEM_EFFECT_VALUE") VALUES
(1, 'Poción Protectora de Racha', 'Evita que pierdas tu racha si fallas un día.', 500, NULL),
(1, 'Elixir de Experiencia', 'Doble XP por 24 horas.', 1000, 2.0),
(2, 'Espada del Madrugador', 'Otorga +10% XP en tareas completadas antes de las 8 AM.', 2500, 1.1),
(2, 'Túnica del Erudito', 'Otorga +15% XP en hábitos de Intelecto.', 3000, 1.15),
(3, 'Dragón Bebé', 'Mascota cosmética nivel 1.', 5000, NULL),
(3, 'Búho Sabio', 'Mascota cosmética nivel 1.', 5000, NULL),
(4, 'Título: El Imparable', 'Título cosmético legendario.', 10000, NULL);

-- Inventario de los jugadores
INSERT INTO "LULT_PLAYER_INVENTORY" ("ID_PLAYER_USER", "ID_REWARD_ITEM", "NUM_INVENTORY_QUANTITY", "STATUS_INVENTORY_IS_EQUIPPED") VALUES
(1, 1, 3, FALSE), -- Arturo tiene 3 pociones
(1, 3, 1, TRUE),  -- Arturo tiene la espada equipada
(2, 4, 1, TRUE),  -- Lara tiene la túnica equipada
(2, 6, 1, TRUE),  -- Lara tiene el búho equipado
(3, 1, 1, FALSE); -- Geralt tiene 1 poción

-- ==============================================================================
-- 6. HISTORIAL DE RACHAS (Para probar los gráficos en el Frontend)
-- ==============================================================================

INSERT INTO "LULH_STREAK_LOG" ("ID_PLAYER_USER", "NUM_STREAK_COUNT", "FEC_LOG_DATE", "TYPE_PROTECTION_USED") VALUES
(1, 13, '2023-10-15', FALSE),
(1, 14, '2023-10-16', FALSE),
(1, 15, '2023-10-17', FALSE),
(2, 40, '2023-10-15', FALSE),
(2, 41, '2023-10-16', FALSE),
(2, 42, '2023-10-17', TRUE), -- Lara usó una poción este día
(3, 1, '2023-10-15', FALSE),
(3, 2, '2023-10-16', FALSE),
(3, 3, '2023-10-17', FALSE);

-- ==============================================================================
-- 7. PATCH DE SEGUIMIENTO (08-05-2026) - ISSUE #26
-- ==============================================================================

-- Alinea ENUM_FREQUENCY con backend (TaskFrequency.MONTHLY)
ALTER TYPE "ENUM_FREQUENCY" ADD VALUE IF NOT EXISTS 'MONTHLY';

-- Semilla mínima para probar POST /api/habit-tasks (idempotente)
INSERT INTO "LULM_HABIT_CATEGORY"
    ("DSC_HABIT_CATEGORY_NAME", "DSC_HABIT_CATEGORY_DESCRIPTION", "STATUS_HABIT_CATEGORY_IS_ACTIVE")
SELECT
    'Fitness',
    'Seed category for habit-task endpoint tests',
    TRUE
WHERE NOT EXISTS (
    SELECT 1
    FROM "LULM_HABIT_CATEGORY"
    WHERE "DSC_HABIT_CATEGORY_NAME" = 'Fitness'
);

INSERT INTO "LULM_HABIT_DISCIPLINE"
    ("ID_HABIT_CATEGORY", "DSC_HABIT_DISCIPLINE_NAME", "DSC_HABIT_DISCIPLINE_DESCRIPTION", "STATUS_HABIT_DISCIPLINE_IS_ACTIVE")
SELECT
    c."ID_HABIT_CATEGORY",
    'Swimming',
    'Seed discipline for habit-task endpoint tests',
    TRUE
FROM "LULM_HABIT_CATEGORY" c
WHERE c."DSC_HABIT_CATEGORY_NAME" = 'Fitness'
  AND NOT EXISTS (
      SELECT 1
      FROM "LULM_HABIT_DISCIPLINE" d
      WHERE d."DSC_HABIT_DISCIPLINE_NAME" = 'Swimming'
  );

INSERT INTO "LULM_HABIT"
    ("ID_HABIT_DISCIPLINE", "ID_PLAYER_USER", "DSC_HABIT_TITLE", "DSC_HABIT_DESCRIPTION", "STATUS_HABIT_IS_ACTIVE")
SELECT
    d."ID_HABIT_DISCIPLINE",
    u."ID_PLAYER_USER",
    'Swim Conditioning',
    'Seed habit for habit-task endpoint tests',
    TRUE
FROM "LULM_HABIT_DISCIPLINE" d
JOIN "LULM_PLAYER_USER" u
  ON u."DSC_PLAYER_USER_USERNAME" = 'aaronqatest1'
WHERE d."DSC_HABIT_DISCIPLINE_NAME" = 'Swimming'
  AND NOT EXISTS (
      SELECT 1
      FROM "LULM_HABIT" h
      WHERE h."DSC_HABIT_TITLE" = 'Swim Conditioning'
        AND h."ID_PLAYER_USER" = u."ID_PLAYER_USER"
  );

INSERT INTO "LULM_HABIT_TASK"
    ("ID_HABIT", "ID_HABIT_DISCIPLINE", "DSC_HABIT_TASK_TITLE", "DSC_HABIT_TASK_DESCRIPTION",
     "DSC_HABIT_TASK_WEEK_DAYS", "TYPE_HABIT_TASK_DIFFICULTY", "NUM_HABIT_TASK_XP_VALUE",
     "TYPE_HABIT_TASK_FREQUENCY", "NUM_HABIT_TASK_PERIOD_LENGTH", "TYPE_HABIT_TASK_PERIOD_UNIT",
     "FEC_HABIT_TASK_START_DATE", "STATUS_HABIT_TASK_IS_COMPLETED", "TYPE_COMPLETION_CRITERIA",
     "TYPE_HABIT_TASK_EVIDENCE", "STATUS_HABIT_TASK_IS_ACTIVE")
SELECT
    h."ID_HABIT",
    h."ID_HABIT_DISCIPLINE",
    'Swim monthly challenge',
    'Nadar para condición física',
    'MON,WED,FRI',
    'EPIC',
    120,
    'MONTHLY',
    1,
    'MONTHS',
    CURRENT_DATE,
    FALSE,
    'REPETITIONS',
    NULL,
    TRUE
FROM "LULM_HABIT" h
JOIN "LULM_PLAYER_USER" u
  ON h."ID_PLAYER_USER" = u."ID_PLAYER_USER"
WHERE h."DSC_HABIT_TITLE" = 'Swim Conditioning'
  AND u."DSC_PLAYER_USER_USERNAME" = 'aaronqatest1'
  AND NOT EXISTS (
      SELECT 1
      FROM "LULM_HABIT_TASK" t
      WHERE t."ID_HABIT" = h."ID_HABIT"
        AND t."DSC_HABIT_TASK_TITLE" = 'Swim monthly challenge'
  );

INSERT INTO "LULT_HABIT_TASK_REPETITIONS_CRITERIA"
    ("ID_HABIT_TASK", "NUM_REPETITIONS_OBJECTIVE", "TYPE_UNITY_MEASUREMENT_UNIT",
     "STATUS_IS_PARTIAL_ALLOWED", "STATUS_REPETITIONS_CRITERIA_IS_ACTIVE")
SELECT
    t."ID_HABIT_TASK",
    10,
    'REPS',
    FALSE,
    TRUE
FROM "LULM_HABIT_TASK" t
WHERE t."DSC_HABIT_TASK_TITLE" = 'Swim monthly challenge'
  AND NOT EXISTS (
      SELECT 1
      FROM "LULT_HABIT_TASK_REPETITIONS_CRITERIA" r
      WHERE r."ID_HABIT_TASK" = t."ID_HABIT_TASK"
  );
