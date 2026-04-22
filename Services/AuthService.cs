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
    private readonly IConfiguration _configuration;

    // IConfiguration allows us to read values from appsettings.json, such as the JWT secret key and token expiration time.
    public AuthService(IAuthRepository authRepository, IConfiguration configuration)
    {
        _authRepository = authRepository;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        // 1. Search for the user by UserName or Email.
        var user = await _authRepository.GetByUserNameOrEmailAsync(request.UserNameOrEmail);

        // 2. If the user does not exist or is inactive, return null (invalid credentials).
        if (user is null || !user.IsActive)
            return null;

        // 3. Verify the password with BCrypt.
        //    BCrypt.Verify compares the plain text with the hash stored in the DB.
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            return null;

        // 4. Generate the JWT token.
        var token = GenerateJwtToken(user.Id, user.UserName);

        return new LoginResponseDto
        {
            Token = token,
            UserName = user.UserName,
            Level = user.Level,
            ClassName = user.Class?.Name ?? string.Empty
        };
    }

    public async Task<(bool Success, string Message)> RegisterAsync(RegisterRequestDto request)
    {
        // 1. Verify that the email is not already in use.
        if (await _authRepository.EmailExistsAsync(request.PersonData.Email))
            return (false, "The email is already registered.");

        // 2. Verify that the username is not already in use.
        if (await _authRepository.UserNameExistsAsync(request.PlayerUserData.UserName))
            return (false, "The username is already in use.");

        // 3. Verify that the class exists and is active.
        var playerClass = await _authRepository.GetClassByIdAsync(request.PlayerUserData.ClassId);
        if (playerClass is null)
            return (false, "La clase seleccionada no existe o no está disponible.");

        // 4. Construir entidades.
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

        // 5. Persistir.
        await _authRepository.RegisterAsync(person, playerUser);

        return (true, "Usuario registrado exitosamente.");
    }

    private string GenerateJwtToken(int userId, string userName)
    {
        var jwtKey = _configuration["Jwt:Key"]!;
        var issuer = _configuration["Jwt:Issuer"]!;
        var audience = _configuration["Jwt:Audience"]!;
        var expiresInMinutes = int.Parse(_configuration["Jwt:ExpiresInMinutes"]!);

        // La clave secreta se convierte a bytes para firmar el token.
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Los "claims" son los datos que viajan dentro del token.
        // El frontend puede leerlos (no están encriptados), pero no puede modificarlos.
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

        // Serializa el token al formato estándar: xxxxx.yyyyy.zzzzz
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

namespace LevelUpLifeBackend.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IConfiguration _configuration;

    // IConfiguration permite leer los valores de appsettings.json.
    public AuthService(IAuthRepository authRepository, IConfiguration configuration)
    {
        _authRepository = authRepository;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        // 1. Buscar al usuario por UserName o Email.
        var user = await _authRepository.GetByUserNameOrEmailAsync(request.UserNameOrEmail);

        // 2. Si no existe o está inactivo, retornamos null (credenciales inválidas).
        if (user is null || !user.IsActive)
            return null;

        // 3. Verificar la contraseña con BCrypt.
        //    BCrypt.Verify compara el texto plano con el hash guardado en la BD.
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            return null;

        // 4. Generar el token JWT.
        var token = GenerateJwtToken(user.Id, user.UserName);

        return new LoginResponseDto
        {
            Token = token,
            UserName = user.UserName,
            Level = user.Level,
            ClassName = user.Class?.Name ?? string.Empty
        };
    }

    private string GenerateJwtToken(int userId, string userName)
    {
        var jwtKey = _configuration["Jwt:Key"]!;
        var issuer = _configuration["Jwt:Issuer"]!;
        var audience = _configuration["Jwt:Audience"]!;
        var expiresInMinutes = int.Parse(_configuration["Jwt:ExpiresInMinutes"]!);

        // La clave secreta se convierte a bytes para firmar el token.
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Los "claims" son los datos que viajan dentro del token.
        // El frontend puede leerlos (no están encriptados), pero no puede modificarlos.
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

        // Serializa el token al formato estándar: xxxxx.yyyyy.zzzzz
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
