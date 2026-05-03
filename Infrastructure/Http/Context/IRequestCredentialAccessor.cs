namespace LevelUpLifeBackend.Infrastructure.Http.Context;

public interface IRequestCredentialAccessor
{
    string? GetBearerToken();
    string? GetSessionId();
}
