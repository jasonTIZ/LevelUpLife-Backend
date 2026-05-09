using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace LevelUpLifeBackend.Infrastructure.Http.Context;

public sealed class SecureCredentialStorage : ISecureCredentialStorage
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDataProtector _protector;
    private readonly BaseHttpClientOptions _options;
    private const string CredentialsCookieName = "LUL_SECURE_CREDS";

    public SecureCredentialStorage(
        IHttpContextAccessor httpContextAccessor,
        IDataProtectionProvider dataProtectionProvider,
        IOptions<BaseHttpClientOptions> options)
    {
        _httpContextAccessor = httpContextAccessor;
        _protector = dataProtectionProvider.CreateProtector("LevelUpLife.SecureCredentials");
        _options = options.Value;
    }

    public void SaveCredentials(string jwt, string sessionId)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return;

        var creds = new UserCredentials(jwt, sessionId);
        var json = JsonSerializer.Serialize(creds);
        var protectedData = _protector.Protect(json);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7) // Example expiration
        };

        context.Response.Cookies.Append(CredentialsCookieName, protectedData, cookieOptions);
    }

    public UserCredentials? GetCredentials()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return null;

        // 1. Try secure cookie first
        if (context.Request.Cookies.TryGetValue(CredentialsCookieName, out var protectedData))
        {
            try
            {
                var json = _protector.Unprotect(protectedData);
                var creds = JsonSerializer.Deserialize<UserCredentials>(json);
                if (creds != null) return creds;
            }
            catch
            {
                // Fallback if decryption fails
            }
        }

        // 2. Fallback to headers and legacy cookies (compatibility)
        string? jwt = null;
        string? sessionId = null;

        var authHeader = context.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            jwt = authHeader.Substring("Bearer ".Length).Trim();
        }

        sessionId = context.Request.Cookies[_options.SessionCookieName];
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            sessionId = context.Request.Headers[_options.SessionCookieName].ToString();
        }

        if (!string.IsNullOrWhiteSpace(jwt) || !string.IsNullOrWhiteSpace(sessionId))
        {
            return new UserCredentials(jwt ?? string.Empty, sessionId ?? string.Empty);
        }

        return null;
    }

    public void ClearCredentials()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return;

        context.Response.Cookies.Delete(CredentialsCookieName);
    }
}
