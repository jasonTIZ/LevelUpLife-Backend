using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.Models;
using LevelUpLifeBackend.Repositories;
using LevelUpLifeBackend.Services;
using Npgsql.NameTranslation;
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

builder.Services.AddControllers();
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
        npgsqlOptions => npgsqlOptions.MapEnum<MeasurementUnit>("ENUM_MEASUREMENT_UNIT", nameTranslator: new NpgsqlNullNameTranslator())
    ));

// Repositorios y Servicios de Hábitos
builder.Services.AddScoped<IHabitRepository, HabitRepository>();
builder.Services.AddScoped<IHabitService, HabitService>();

builder.Services.AddScoped<IHabitTaskRepository, HabitTaskRepository>();
builder.Services.AddScoped<IRepetitionCriteriaRepository, RepetitionCriteriaRepository>();

// Repositorios y Servicios de Autenticación
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

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
            ValidateLifetime = true,           // Rechaza tokens expirados
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

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
