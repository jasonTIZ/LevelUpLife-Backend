namespace LevelUpLifeBackend.Infrastructure.Http.Context;

public record UserCredentials(string Jwt, string SessionId);

public interface ISecureCredentialStorage
{
    void SaveCredentials(string jwt, string sessionId);
    UserCredentials? GetCredentials();
    void ClearCredentials();
}
