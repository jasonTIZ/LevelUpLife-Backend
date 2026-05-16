using Microsoft.EntityFrameworkCore;
using LevelUpLifeBackend.Models;
using Npgsql.NameTranslation;

namespace LevelUpLifeBackend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // ==========================================================
    // DBSets (Tus tablas actuales)
    // ==========================================================
    public DbSet<Person> Persons { get; set; }
    public DbSet<UserPlayerClass> UserPlayerClasses { get; set; }
    public DbSet<PlayerUser> PlayerUsers { get; set; }
    
    public DbSet<HabitCategory> HabitCategories { get; set; }
    public DbSet<HabitDiscipline> Disciplines { get; set; }
    public DbSet<Habit> Habits { get; set; }
    public DbSet<HabitTask> HabitTasks { get; set; }
    public DbSet<RepetitionCriteria> RepetitionCriteriaRecords { get; set; }
    public DbSet<EvidenceStorage> EvidenceStorages { get; set; }

    public DbSet<RewardItemType> RewardItemTypes { get; set; }
    public DbSet<RewardItem> RewardItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Registrar el Enum de PostgreSQL para que Npgsql lo reconozca
        modelBuilder.HasPostgresEnum<MeasurementUnit>("ENUM_MEASUREMENT_UNIT", nameTranslator: new NpgsqlNullNameTranslator());
        modelBuilder.HasPostgresEnum<TaskDifficulty>("ENUM_DIFFICULTY", nameTranslator: new NpgsqlNullNameTranslator());
        modelBuilder.HasPostgresEnum<TaskFrequency>("ENUM_FREQUENCY", nameTranslator: new NpgsqlNullNameTranslator());
        modelBuilder.HasPostgresEnum<TaskPeriodUnit>("ENUM_PERIOD_UNIT", nameTranslator: new NpgsqlNullNameTranslator());
        modelBuilder.HasPostgresEnum<TaskCompletionCriteria>("ENUM_COMPLETION_CRITERIA", nameTranslator: new NpgsqlNullNameTranslator());
        modelBuilder.HasPostgresEnum<TaskEvidence>("ENUM_EVIDENCE", nameTranslator: new NpgsqlNullNameTranslator());

        // ==========================================================
        // 1. NÚCLEO DE IDENTIDAD Y USUARIO
        // ==========================================================
        modelBuilder.Entity<Person>(entity =>
        {
            entity.ToTable("LULM_PERSON");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ID_PERSON");
            
            entity.Property(e => e.Name).HasColumnName("DSC_PERSON_NAME").HasMaxLength(50).IsRequired();
            entity.Property(e => e.LastName).HasColumnName("DSC_PERSON_LAST_NAME").HasMaxLength(50).IsRequired();
            entity.Property(e => e.PhoneNumber).HasColumnName("DSC_PERSON_PHONE_NUMBER").HasMaxLength(20);
            entity.Property(e => e.Email).HasColumnName("DSC_PERSON_EMAIL").HasMaxLength(150).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.BirthDate).HasColumnName("FEC_PERSON_BIRTHDATE");
            entity.Property(e => e.IsActive).HasColumnName("STATUS_PERSON_IS_ACTIVE");
        });

        modelBuilder.Entity<UserPlayerClass>(entity =>
        {
            entity.ToTable("LULM_USER_PLAYER_CLASS");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ID_USER_PLAYER_CLASS");
            
            entity.Property(e => e.Name).HasColumnName("DSC_USER_PLAYER_CLASS_NAME").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Description).HasColumnName("DSC_USER_PLAYER_CLASS_DESCRIPTION");
            entity.Property(e => e.XpMultStudy).HasColumnName("NUM_CLASS_XP_MULT_STUDY");
            entity.Property(e => e.XpMultSport).HasColumnName("NUM_CLASS_XP_MULT_SPORT");
            entity.Property(e => e.XpMultFood).HasColumnName("NUM_CLASS_XP_MULT_FOOD");
            entity.Property(e => e.IsActive).HasColumnName("STATUS_USER_PLAYER_CLASS_IS_ACTIVE");
        });

        modelBuilder.Entity<PlayerUser>(entity =>
        {
            entity.ToTable("LULM_PLAYER_USER");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ID_PLAYER_USER");
            
            // --- Shadow Properties & Relaciones ---
            // 1. Relación con Person (1 a 1)
            entity.Property<int>("PersonId").HasColumnName("ID_PERSON");
            entity.HasOne(e => e.Person)
                  .WithOne()
                  .HasForeignKey<PlayerUser>("PersonId");
            entity.HasIndex("PersonId").IsUnique();

            // 2. Relación con UserPlayerClass (Muchos a 1)
            entity.Property<int>("ClassId").HasColumnName("ID_USER_PLAYER_CLASS");
            entity.HasOne(e => e.Class)
                  .WithMany()
                  .HasForeignKey("ClassId");
            
            entity.Property(e => e.UserName).HasColumnName("DSC_PLAYER_USER_USERNAME").HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.UserName).IsUnique();
            entity.Property(e => e.Password).HasColumnName("DSC_PLAYER_USER_PASSWORD").HasMaxLength(255).IsRequired();
            entity.Property(e => e.Level).HasColumnName("NUM_PLAYER_USER_LEVEL");
            entity.Property(e => e.ExperiencePoints).HasColumnName("NUM_PLAYER_USER_EXPERIENCE_POINTS");
            entity.Property(e => e.DaysStreak).HasColumnName("NUM_PLAYER_USER_DAYS_STREAK");
            entity.Property(e => e.LastLogin).HasColumnName("FEC_PLAYER_USER_LAST_LOGIN");
            entity.Property(e => e.CreationDate).HasColumnName("FEC_PLAYER_USER_CREATION_DATE");
            entity.Property(e => e.IsActive).HasColumnName("STATUS_PLAYER_USER_IS_ACTIVE");
        });

        // ==========================================================
        // 2. ESTRUCTURA DE HÁBITOS Y DISCIPLINAS
        // ==========================================================
        modelBuilder.Entity<HabitCategory>(entity =>
        {
            entity.ToTable("LULM_HABIT_CATEGORY");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ID_HABIT_CATEGORY");
            entity.Property(e => e.Name).HasColumnName("DSC_HABIT_CATEGORY_NAME").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Description).HasColumnName("DSC_HABIT_CATEGORY_DESCRIPTION");
            entity.Property(e => e.IsActive).HasColumnName("STATUS_HABIT_CATEGORY_IS_ACTIVE");
        });

        modelBuilder.Entity<HabitDiscipline>(entity =>
        {
            entity.ToTable("LULM_HABIT_DISCIPLINE");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ID_HABIT_DISCIPLINE");
            
            // --- Shadow Property: Relación con Categoría ---
            entity.Property<int>("CategoryId").HasColumnName("ID_HABIT_CATEGORY");
            entity.HasOne(e => e.Category)
                  .WithMany()
                  .HasForeignKey("CategoryId");

            entity.Property(e => e.Name).HasColumnName("DSC_HABIT_DISCIPLINE_NAME").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Description).HasColumnName("DSC_HABIT_DISCIPLINE_DESCRIPTION");
            entity.Property(e => e.IsActive).HasColumnName("STATUS_HABIT_DISCIPLINE_IS_ACTIVE");
        });

        modelBuilder.Entity<Habit>(entity =>
        {
            entity.ToTable("LULM_HABIT");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ID_HABIT");
            
            // --- Shadow Properties: Relaciones ---
            entity.Property<int>("DisciplineId").HasColumnName("ID_HABIT_DISCIPLINE");
            entity.HasOne(e => e.Discipline)
                  .WithMany()
                  .HasForeignKey("DisciplineId");

            entity.Property<int>("UserId").HasColumnName("ID_PLAYER_USER");
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey("UserId");

            entity.Property(e => e.Title).HasColumnName("DSC_HABIT_TITLE").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasColumnName("DSC_HABIT_DESCRIPTION");
            entity.Property(e => e.IsActive).HasColumnName("STATUS_HABIT_IS_ACTIVE");
        });

        modelBuilder.Entity<HabitTask>(entity =>
        {
            entity.ToTable("LULM_HABIT_TASK");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ID_HABIT_TASK");

            entity.Property(e => e.HabitId).HasColumnName("ID_HABIT");
            entity.HasOne(e => e.Habit)
                  .WithMany(h => h.Tasks)
                  .HasForeignKey(e => e.HabitId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.HabitDisciplineId).HasColumnName("ID_HABIT_DISCIPLINE");
            entity.HasOne(e => e.HabitDiscipline)
                .WithMany()
                .HasForeignKey(e => e.HabitDisciplineId);

            entity.Property(e => e.Title).HasColumnName("DSC_HABIT_TASK_TITLE").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasColumnName("DSC_HABIT_TASK_DESCRIPTION");
            entity.Property(e => e.WeekDays).HasColumnName("DSC_HABIT_TASK_WEEK_DAYS");
            entity.Property(e => e.Difficulty).HasColumnName("TYPE_HABIT_TASK_DIFFICULTY");
            entity.Property(e => e.XpValue).HasColumnName("NUM_HABIT_TASK_XP_VALUE");
            entity.Property(e => e.Frequency).HasColumnName("TYPE_HABIT_TASK_FREQUENCY");
            entity.Property(e => e.PeriodLength).HasColumnName("NUM_HABIT_TASK_PERIOD_LENGTH");
            entity.Property(e => e.PeriodUnit).HasColumnName("TYPE_HABIT_TASK_PERIOD_UNIT");
            entity.Property(e => e.StartDate).HasColumnName("FEC_HABIT_TASK_START_DATE");
            entity.Property(e => e.IsCompleted).HasColumnName("STATUS_HABIT_TASK_IS_COMPLETED");
            entity.Property(e => e.CompletionCriteria).HasColumnName("TYPE_COMPLETION_CRITERIA");
            entity.Property(e => e.Evidence).HasColumnName("TYPE_HABIT_TASK_EVIDENCE");
            entity.Property(e => e.IsActive).HasColumnName("STATUS_HABIT_TASK_IS_ACTIVE");
        });

        modelBuilder.Entity<RepetitionCriteria>(entity =>
        {
            entity.ToTable("LULT_HABIT_TASK_REPETITIONS_CRITERIA");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ID_HABIT_TASK_REPETITIONS_CRITERIA");

            entity.Property(e => e.HabitTaskId).HasColumnName("ID_HABIT_TASK");
            entity.HasOne(e => e.HabitTask)
                  .WithOne(ht => ht.RepetitionCriteria)
                  .HasForeignKey<RepetitionCriteria>(e => e.HabitTaskId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.NumRepetitionsObjective).HasColumnName("NUM_REPETITIONS_OBJECTIVE");
            entity.Property(e => e.TypeUnityMeasurementUnit)
                  .HasColumnName("TYPE_UNITY_MEASUREMENT_UNIT");
            entity.Property(e => e.StatusIsPartialAllowed).HasColumnName("STATUS_IS_PARTIAL_ALLOWED");
            entity.Property(e => e.StatusRepetitionsCriteriaIsActive)
                  .HasColumnName("STATUS_REPETITIONS_CRITERIA_IS_ACTIVE");
        });

        modelBuilder.Entity<EvidenceStorage>(entity =>
        {
            entity.ToTable("LULT_HABIT_TASK_EVIDENCE_STORAGE");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ID_EVIDENCE");

            entity.Property(e => e.HabitTaskId).HasColumnName("ID_HABIT_TASK");
            entity.HasOne(e => e.HabitTask)
                  .WithMany()
                  .HasForeignKey(e => e.HabitTaskId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.EvidencePathUrl).HasColumnName("DSC_EVIDENCE_PATH_URL");
            entity.Property(e => e.HealthDataJson).HasColumnName("DSC_HEALTH_DATA_JSON");
            entity.Property(e => e.UploadedAt).HasColumnName("FEC_UPLOADED");
        });

        modelBuilder.Entity<RewardItemType>(entity =>
        {
            entity.ToTable("LULM_REWARD_ITEM_TYPE");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ID_REWARD_ITEM_TYPE");
            entity.Property(e => e.Name).HasColumnName("DSC_REWARD_ITEM_TYPE_NAME").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasColumnName("DSC_REWARD_ITEM_TYPE_DESC");
            entity.Property(e => e.IsActive).HasColumnName("STATUS_REWARD_ITEM_TYPE_IS_ACTIVE");
        });

        modelBuilder.Entity<RewardItem>(entity =>
        {
            entity.ToTable("LULM_REWARD_ITEM");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ID_REWARD_ITEM");

            entity.Property(e => e.TypeId).HasColumnName("ID_REWARD_ITEM_TYPE");
            entity.HasOne(e => e.Type)
                  .WithMany()
                  .HasForeignKey(e => e.TypeId);

            entity.Property(e => e.Name).HasColumnName("DSC_REWARD_ITEM_NAME").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasColumnName("DSC_REWARD_ITEM_DESCRIPTION");
            entity.Property(e => e.CostGold).HasColumnName("NUM_REWARD_ITEM_COST_GOLD");
            entity.Property(e => e.EffectValue).HasColumnName("NUM_REWARD_ITEM_EFFECT_VALUE");
            entity.Property(e => e.IsActive).HasColumnName("STATUS_REWARD_ITEM_IS_ACTIVE");
        });
    }
}
