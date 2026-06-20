using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Models;
using LevelUpLifeBackend.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace LevelUpLifeBackend.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly ILevelProgressService _levelProgressService;
    private readonly IConfiguration _configuration;

    public AuthService(
        IAuthRepository authRepository,
        ILevelProgressService levelProgressService,
        IConfiguration configuration)
    {
        _authRepository = authRepository;
        _levelProgressService = levelProgressService;
        _configuration = configuration;
    }

    public async Task<(bool Success, string Message)> RegisterAsync(RegisterRequestDto request)
    {
        if (await _authRepository.EmailExistsAsync(request.PersonData.Email))
            return (false, "The email is already registered.");
        if (await _authRepository.UserNameExistsAsync(request.PlayerUserData.UserName))
            return (false, "The username is already in use.");
        var playerClass = await _authRepository.GetClassByIdAsync(request.PlayerUserData.ClassId);
        if (playerClass is null)
            return (false, "La clase seleccionada no existe o no está disponible.");
        var person = new Person
        {
            Name = request.PersonData.Name,
            LastName = request.PersonData.LastName,
            Email = request.PersonData.Email,
            BirthDate = request.PersonData.Birthdate,
            IsActive = true
        };
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.PlayerUserData.Password);
        var playerUser = new PlayerUser
        {
            UserName = request.PlayerUserData.UserName,
            Password = passwordHash,
            Class = playerClass,
            Level = 1,
            ExperiencePoints = 0,
            DaysStreak = 0,
            CreationDate = DateTime.UtcNow,
            IsActive = true
        };
        await _authRepository.RegisterAsync(person, playerUser);
        return (true, "Usuario registrado exitosamente.");
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var user = await _authRepository.GetByUserNameOrEmailAsync(request.UserNameOrEmail);
        if (user is null || !user.IsActive)
            return null;
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            return null;
        var token = GenerateJwtToken(user.Id, user.UserName);
        var levelProgress = _levelProgressService.GetLevelProgress(user.ExperiencePoints);

        return new LoginResponseDto
        {
            Token = token,
            UserName = user.UserName,
            Level = levelProgress.CurrentLevel,
            ClassName = user.Class?.Name ?? string.Empty,
            LevelProgress = levelProgress,
            LevelingConfig = _levelProgressService.GetLevelingConfig(),
        };
    }

    private string GenerateJwtToken(int userId, string userName)
    {
        var jwtKey = _configuration["Jwt:Key"]!;
        var issuer = _configuration["Jwt:Issuer"]!;
        var audience = _configuration["Jwt:Audience"]!;
        var expiresInSetting = _configuration["Jwt:ExpiresInMinutes"];
        var expiresInMinutes = 60;
        if (int.TryParse(expiresInSetting, out var parsedMinutes) && parsedMinutes > 0)
        {
            expiresInMinutes = parsedMinutes;
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, userName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
