# LevelUpLife-Backend

API backend en **.NET 9** con **PostgreSQL**.

## Requisitos

- [.NET SDK 9](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/) (para PostgreSQL local)

## Configuración local

1. Copia la plantilla de configuración:

```bash
cp appsettings.Development.example.json appsettings.Development.json
```

2. Ajusta `appsettings.Development.json` si cambias usuario, contraseña o puerto de la base de datos.

3. Levanta PostgreSQL (puerto **5433** en el host):

```bash
docker run -d --name leveluplife-postgres \
  -e POSTGRES_USER=levelup \
  -e POSTGRES_PASSWORD=levelup_dev \
  -e POSTGRES_DB=leveluplife \
  -p 5433:5432 \
  postgres:16-alpine
```

4. Carga el esquema y datos de prueba:

```bash
docker exec -i leveluplife-postgres psql -U levelup -d leveluplife < Backups/LevelUpLife-Backup-08-05-2026.sql
```

> **Nota:** `dotnet ef database update` puede fallar si el modelo EF no coincide exactamente con las migraciones. Para desarrollo local, el backup SQL es el camino recomendado del equipo.

5. Ejecuta la API:

```bash
export DOTNET_ROOT="$HOME/.dotnet"   # si instalaste .NET en ~/.dotnet
export PATH="$DOTNET_ROOT:$PATH"
dotnet run --launch-profile http
```

- API: http://localhost:5223
- OpenAPI (dev): http://localhost:5223/scalar/v1

## Variables de entorno útiles

| Variable | Uso |
|----------|-----|
| `DOTNET_ROOT` | Ruta del SDK si no está en el PATH del sistema |
| `ASPNETCORE_ENVIRONMENT` | `Development` para Scalar y logs detallados |

## Rama de trabajo (issue #24)

```bash
git checkout -b feature/24-list-habit-tasks-with-filters
```
