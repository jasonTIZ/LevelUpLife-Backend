# Base HTTP Client (Issue #45)

This backend now provides a single reusable outbound HTTP client via `IBaseApiClient`.

## Rules

- Do not create `new HttpClient()` inside services/repositories.
- Use `IBaseApiClient` for outbound integrations.
- Base path `/v1` is prepended automatically to every request path.

## Interceptors

- `AuthSessionHandler`
  - Injects `Authorization: Bearer <jwt>` if token exists in incoming request context.
  - Injects `SESSION_ID` when present (cookie/header fallback).
- `GlobalErrorHandler`
  - On `401`, clears `SESSION_ID` cookie from current response context.
  - On `5xx`, publishes `ServerErrorEvent` through `IGlobalErrorPublisher`.
