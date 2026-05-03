# Global error handling (Issue #46)

## Modelo `ErrorResponse`

Contrato JSON esperado en cuerpos de error de APIs externas:

```json
{
  "code": 404,
  "message": "string",
  "details": "string"
}
```

Si el cuerpo no es JSON válido o viene incompleto, `ApiErrorParser` rellena `code` con el HTTP status, `message` con `ReasonPhrase` (o un texto por defecto) y `details` con el cuerpo bruto truncado.

## `parseApiError` → `IApiErrorParser.ParseApiErrorAsync`

Inyectá `IApiErrorParser` cuando manejes manualmente un `HttpResponseMessage` fallido:

```csharp
if (!response.IsSuccessStatusCode)
    throw await _parser.ParseApiErrorAsync(response, cancellationToken);
```

## Mapa HTTP → error tipado

| Status | Tipo |
|--------|------|
| 401 | `AuthError` (`Unauthorized`) |
| 403 | `AuthError` (`Forbidden`, mensaje con prefijo `FORBIDDEN:`) |
| 404 | `NotFoundError` |
| 409 | `ConflictError` |
| 412 | `ProfileError` (`ETagMismatch`) |
| 5xx | `ServerError` |
| Otros | `UnexpectedApiError` |

## Uso con `IBaseApiClient` (issue #45)

`BaseApiClient` llama al parser en respuestas no exitosas: no se devuelven errores HTTP “crudos” al llamador; se lanza un `AppError` derivado.

Los servicios de dominio que solo usan EF/repositorios no pasan por HTTP; la regla aplica a **cualquier** código que consuma APIs externas vía `IBaseApiClient` o que inspeccione `HttpResponseMessage` manualmente.

## Verificación

La evidencia automatizada está en el proyecto de pruebas: `dotnet test LevelUpLife-Backend.Tests/LevelUpLife-Backend.Tests.csproj`.
