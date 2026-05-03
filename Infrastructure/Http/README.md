# Base HTTP Client (Issue #45)

This backend now provides a single reusable outbound HTTP client via `IBaseApiClient`.

## Configuration

- Set `ExternalApi:BaseUrl` to a non-empty absolute URL (for example `https://api.example.com`) before calling `IBaseApiClient`. The typed `HttpClient` uses it as `BaseAddress`. If it is missing or whitespace, outbound calls throw `InvalidOperationException` with a configuration hint.
- `ExternalApi:ApiPrefix` defaults to `/v1` and is normalized together with each relative path (null-safe segments).

## Rules

- Do not create `new HttpClient()` inside services/repositories.
- Use `IBaseApiClient` for outbound integrations.
- Base path `/v1` is prepended automatically to every request path.
- On non-success responses, `BaseApiClient` throws typed `AppError` via `IApiErrorParser` (see [Global error handling (Issue #46)](../Errors/README.md)).

## Interceptors

- `AuthSessionHandler`
  - Injects `Authorization: Bearer <jwt>` if token exists in incoming request context.
  - Injects `SESSION_ID` when present (cookie/header fallback).
- `GlobalErrorHandler`
  - On `401`, clears `SESSION_ID` cookie from current response context.
  - On `5xx`, publishes `ServerErrorEvent` through `IGlobalErrorPublisher`.
