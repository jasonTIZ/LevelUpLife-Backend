using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LevelUpLifeBackend.DTOs.Requests;
using LevelUpLifeBackend.DTOs.Responses;
using LevelUpLifeBackend.Repositories;
using Microsoft.IdentityModel.Tokens;

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
