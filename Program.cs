using System.Text;
using LevelUpLifeBackend.Data;
using LevelUpLifeBackend.Infrastructure.Errors;
using LevelUpLifeBackend.Infrastructure.Http;
using LevelUpLifeBackend.Infrastructure.Http.Context;
using LevelUpLifeBackend.Infrastructure.Http.Events;
using LevelUpLifeBackend.Infrastructure.Http.Handlers;
using LevelUpLifeBackend.Repositories;
using LevelUpLifeBackend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Repositorios y Servicios de Hábitos
builder.Services.AddScoped<IHabitRepository, HabitRepository>();
builder.Services.AddScoped<IHabitService, HabitService>();

// Services and Repositories of Habit Category
builder.Services.AddScoped<IHabitCategoryRepository, HabitCategoryRepository>();
builder.Services.AddScoped<IHabitCategoryService, HabitCategoryService>();

// Repositorios y Servicios de Autenticación
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Repositorios y Servicios de Jugador
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IPlayerService, PlayerService>();

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
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true, // Rechaza tokens expirados
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
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

app.MapControllers();

app.Run();
