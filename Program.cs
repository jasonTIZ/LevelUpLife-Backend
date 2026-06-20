using System.Text;
using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.Models;
using Npgsql.NameTranslation;
using LevelUpLifeBackend.Infrastructure.Ai;
using LevelUpLifeBackend.Infrastructure.Errors;
using LevelUpLifeBackend.Infrastructure.Http;
using LevelUpLifeBackend.Infrastructure.Http.Context;
using LevelUpLifeBackend.Infrastructure.Http.Events;
using LevelUpLifeBackend.Infrastructure.Http.Handlers;
using LevelUpLifeBackend.Repositories;
using LevelUpLifeBackend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

static string GetRequiredSetting(IConfiguration configuration, string key)
{
    var value = configuration[key];
    if (string.IsNullOrWhiteSpace(value))
    {
        throw new InvalidOperationException(
            $"Falta la configuración obligatoria '{key}'. Configúrala en appsettings o variables de entorno."
        );
    }

    return value;
}

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter())
    );

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Falta la cadena de conexión 'ConnectionStrings:DefaultConnection'."
    );
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        connectionString,
        npgsqlOptions => npgsqlOptions
            .MapEnum<MeasurementUnit>("ENUM_MEASUREMENT_UNIT", nameTranslator: new NpgsqlNullNameTranslator())
            .MapEnum<TaskDifficulty>("ENUM_DIFFICULTY", nameTranslator: new NpgsqlNullNameTranslator())
            .MapEnum<TaskFrequency>("ENUM_FREQUENCY", nameTranslator: new NpgsqlNullNameTranslator())
            .MapEnum<TaskPeriodUnit>("ENUM_PERIOD_UNIT", nameTranslator: new NpgsqlNullNameTranslator())
            .MapEnum<TaskCompletionCriteria>("ENUM_COMPLETION_CRITERIA", nameTranslator: new NpgsqlNullNameTranslator())
            .MapEnum<TaskEvidence>("ENUM_EVIDENCE", nameTranslator: new NpgsqlNullNameTranslator())
    )
    .ConfigureWarnings(w => w
        .Ignore(CoreEventId.ManyServiceProvidersCreatedWarning)
        .Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));

// Repositorios y Servicios de Hábitos
builder.Services.AddScoped<IHabitRepository, HabitRepository>();
builder.Services.AddScoped<IHabitService, HabitService>();

builder.Services.AddScoped<IHabitTaskRepository, HabitTaskRepository>();
builder.Services.AddScoped<IHabitTaskService, HabitTaskService>();
builder.Services.AddScoped<IRepetitionCriteriaRepository, RepetitionCriteriaRepository>();
builder.Services.AddScoped<IRepetitionCriteriaService, RepetitionCriteriaService>();
builder.Services.AddScoped<ITimerCriteriaRepository, TimerCriteriaRepository>();
builder.Services.AddScoped<ITimerCriteriaService, TimerCriteriaService>();

builder.Services.AddScoped<IStreakLogRepository, StreakLogRepository>();
builder.Services.AddScoped<IPlayerEventRepository, PlayerEventRepository>();

builder.Services.AddScoped<IRewardItemRepository, RewardItemRepository>();
builder.Services.AddScoped<IRewardItemService, RewardItemService>();
builder.Services.AddScoped<IPlayerInventoryRepository, PlayerInventoryRepository>();
builder.Services.AddScoped<IPlayerInventoryService, PlayerInventoryService>();
builder.Services.AddScoped<IPlayerActiveEffectRepository, PlayerActiveEffectRepository>();
builder.Services.AddScoped<IPlayerEffectService, PlayerEffectService>();
builder.Services.AddScoped<IStreakService, StreakService>();

builder.Services.Configure<LevelUpLifeBackend.Infrastructure.Configuration.StreakProtectionOptions>(
    builder.Configuration.GetSection(
        LevelUpLifeBackend.Infrastructure.Configuration.StreakProtectionOptions.SectionName
    )
);

builder.Services.Configure<LevelUpLifeBackend.Infrastructure.Configuration.LevelingOptions>(
    builder.Configuration.GetSection(
        LevelUpLifeBackend.Infrastructure.Configuration.LevelingOptions.SectionName
    )
);

builder.Services.AddSingleton<ILevelProgressService, LevelProgressService>();

builder.Services.Configure<LevelUpLifeBackend.Infrastructure.Configuration.PlayerProfileOptions>(
    builder.Configuration.GetSection(
        LevelUpLifeBackend.Infrastructure.Configuration.PlayerProfileOptions.SectionName
    )
);

builder.Services.AddScoped<IAvatarStorageService, LocalAvatarStorageService>();

// Services and Repositories of Habit Category
builder.Services.AddScoped<IHabitCategoryRepository, HabitCategoryRepository>();
builder.Services.AddScoped<IHabitCategoryService, HabitCategoryService>();

builder.Services.AddScoped<IHabitDisciplineRepository, HabitDisciplineRepository>();
builder.Services.AddScoped<IHabitDisciplineService, HabitDisciplineService>();

// Repositorios y Servicios de Autenticación
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Repositorios y Servicios de Jugador
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IPlayerService, PlayerService>();

// AI chatbot — forwards requests to the external LLM gateway.
// Configure gateway URL and API key via Ai:BaseUrl and Ai:ApiKey in appsettings.
builder.Services.Configure<AiOptions>(builder.Configuration.GetSection(AiOptions.SectionName));
builder.Services.AddScoped<IAiChatService, AiChatService>();

var aiSection = builder.Configuration.GetSection(AiOptions.SectionName);
var aiApiKey  = GetRequiredSetting(builder.Configuration, $"{AiOptions.SectionName}:ApiKey");
var aiBaseUrl = GetRequiredSetting(builder.Configuration, $"{AiOptions.SectionName}:BaseUrl");

builder.Services.AddHttpClient<IAiProvider, GatewayAiProvider>(client =>
{
    client.BaseAddress = new Uri(aiBaseUrl.TrimEnd('/') + "/");
    // Gateway authenticates via x-api-key header.
    client.DefaultRequestHeaders.Add("x-api-key", aiApiKey);
});

// HTTP base client infrastructure (Issue #45)
builder.Services.Configure<BaseHttpClientOptions>(
    builder.Configuration.GetSection(BaseHttpClientOptions.SectionName)
);
builder.Services.AddSingleton<IApiErrorParser, ApiErrorParser>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IGlobalErrorPublisher, GlobalErrorPublisher>();
builder.Services.AddScoped<ISecureCredentialStorage, SecureCredentialStorage>();
builder.Services.AddScoped<ISessionResetService, HttpContextSessionResetService>();
builder.Services.AddTransient<AuthSessionHandler>();
builder.Services.AddTransient<GlobalErrorHandler>();
builder.Services
    .AddHttpClient<IBaseApiClient, BaseApiClient>((serviceProvider, client) =>
    {
        var options = serviceProvider
            .GetRequiredService<Microsoft.Extensions.Options.IOptions<BaseHttpClientOptions>>()
            .Value;

        if (!string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            client.BaseAddress = new Uri(options.BaseUrl);
        }
    })
    .AddHttpMessageHandler<AuthSessionHandler>()
    .AddHttpMessageHandler<GlobalErrorHandler>();

// Configuración de JWT
// Esto le dice al framework cómo validar los tokens que llegan en los requests.
var jwtKey = GetRequiredSetting(builder.Configuration, "Jwt:Key");
var jwtIssuer = GetRequiredSetting(builder.Configuration, "Jwt:Issuer");
var jwtAudience = GetRequiredSetting(builder.Configuration, "Jwt:Audience");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        };
    });

var app = builder.Build();

var avatarStoragePath = Path.Combine(
    app.Environment.ContentRootPath,
    builder.Configuration.GetSection("PlayerProfile:AvatarStoragePath").Value ?? "uploads/avatars");
Directory.CreateDirectory(avatarStoragePath);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(app.Environment.ContentRootPath, "uploads")),
    RequestPath = "/uploads",
});

// IMPORTANTE: Authentication va ANTES que Authorization.
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () =>
    Results.Ok(new
    {
        success = true,
        message = "LevelUpLife Backend activo.",
    })
);

app.MapControllers();

app.Run();
