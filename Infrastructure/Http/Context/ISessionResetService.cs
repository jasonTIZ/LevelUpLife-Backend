namespace LevelUpLifeBackend.Infrastructure.Http.Context;

public interface ISessionResetService
{
    Task ResetAsync(CancellationToken cancellationToken = default);
}
