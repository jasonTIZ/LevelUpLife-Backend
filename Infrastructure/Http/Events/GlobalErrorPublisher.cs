namespace LevelUpLifeBackend.Infrastructure.Http.Events;

public sealed record ServerErrorEvent(int StatusCode, string Reason, string? Path);

public interface IGlobalErrorPublisher
{
    event Action<ServerErrorEvent>? ServerErrorOccurred;
    void Publish(ServerErrorEvent errorEvent);
}

public sealed class GlobalErrorPublisher : IGlobalErrorPublisher
{
    public event Action<ServerErrorEvent>? ServerErrorOccurred;

    public void Publish(ServerErrorEvent errorEvent)
    {
        ServerErrorOccurred?.Invoke(errorEvent);
    }
}
