# Issue #29 — Complete Habit Task (XP, Streak) + Bloques A, B, E

Documento de entrega para revisión de **Adam** (y QA). Describe la implementación sobre la Issue #29: rachas, protección, snapshot XP y eventos internos de progreso (p. ej. `LEVEL_UP`).

**Rama de trabajo:** `feature/29-habit-task-xp-and-streak` (basada en `dev` con #28 mergeado).

**Estado tests:** `52/52` pasando (`dotnet test` en `LevelUpLife-Backend.Tests`).

**Fuera de alcance en esta rama:** Bloque D (claim de recompensas), seed de catálogo, `GET /api/player/events` — lo implementará el equipo de recompensas.

---

## 1. Alcance de la issue

### 1.1 Qué ya estaba en #29 (contrato que NO se rompió)

| Elemento | Detalle |
|----------|---------|
| **Endpoint** | `PATCH /api/habit-tasks/{id}/complete` |
| **Body** | `{ "completedAt": "<ISO 8601 UTC>" }` |
| **200 OK** | `{ "xpEarned": int, "newLevel": int, "streakUpdated": bool }` |
| **400** | `COMPLETION_REQUIREMENTS_NOT_MET` (`TaskError`) |
| **404** | `TASK_NOT_FOUND` / hábito inactivo |
| **500** | error genérico |

Comportamientos #29 que siguen vigentes:

- Completar otorga `xpEarned` y recalcula `newLevel` (`level = max(1, 1 + XP/100)`).
- No hay doble XP/racha al repetir la misma tarea → **400**.
- `streakUpdated: true` solo en la **primera completion del día** (UTC); otras tareas del mismo día → `streakUpdated: false`.
- La racha inicia en **1** (no en 0) y se persiste en `LULH_STREAK_LOG`.
- `completedAt` anterior a `startDate` de la tarea → **400**.
- Validaciones EVIDENCE / REPETITIONS / TIMER activos antes de completar.

### 1.2 Qué se agregó en esta iteración (Bloques A, B, E)

| Bloque | Entregable |
|--------|------------|
| **A** | Motor automático de rachas: pérdida y recuperación al completar |
| **B** | `POST /api/streaks/protection` + integración con el motor de rachas |
| **C** | Bonus por reenganche — **no implementado** (documentado como futuro) |
| **D** | Claim de recompensas — **fuera de esta rama** (otro compañero) |
| **E** | Snapshot XP al completar; PUT no borra historial de completion |
| **Extra** | Eventos internos en `LULH_PLAYER_EVENT` (`LEVEL_UP`, racha) — sin endpoint GET público aún |

### 1.3 Impacto en funcionalidad existente

Esta sección responde explícitamente si el trabajo **modifica** algo que ya existía o solo **añade** capacidades nuevas. Útil para review de regresiones (Adam / QA).

#### Resumen ejecutivo

| Tipo de cambio | ¿Aplica? |
|----------------|----------|
| Rompe contratos HTTP de endpoints ya publicados | **No** |
| Cambia comportamiento interno de flujos ya usados | **Sí, en casos acotados** (racha en `PATCH complete`) |
| Añade endpoints / tablas / campos nuevos | **Sí** |
| Riesgo de regresión en el flujo “feliz” de #29 | **Bajo** |

---

#### Lo que NO se modificó

Funcionalidad existente que permanece igual en contrato y comportamiento habitual:

| Área | Detalle |
|------|---------|
| **Autenticación** | Login, JWT, flujo sin cambios |
| **Perfil de jugador** | `GET/PUT/PATCH` en `PlayerController` — **sin cambios** (no hay `GET events` en esta rama) |
| **Hábitos** | CRUD de hábitos sin cambios |
| **Tareas — crear / editar / desactivar** | `POST`, `PUT`, `DELETE` de habit-tasks; mismos códigos y payloads |
| **Criterios y evidencias** | Repetition, timer, evidences — sin cambios |
| **Listar recompensas** | `GET /api/RewardItem` — **sin cambios** respecto a `dev` (solo listado; sin claim) |
| **Contrato de complete** | Mismo path, body, forma de respuesta `{ xpEarned, newLevel, streakUpdated }`, mismos códigos 400/404/500 |

**Flujo #29 que debe seguir pasando igual:**

- Primera completion del día → `streakUpdated: true`, sube XP y nivel.
- Segunda tarea distinta el mismo día → `streakUpdated: false`, XP sí se otorga.
- Completar la misma tarea dos veces → **400** `COMPLETION_REQUIREMENTS_NOT_MET`.
- Tarea/hábito inactivo o ajeno al usuario → **404**.
- `completedAt` &lt; `startDate` → **400**.
- EVIDENCE / REPETITIONS / TIMER sin requisitos cumplidos → **400**.

---

#### Lo que SÍ cambia (mismo endpoint, lógica extendida)

Solo **`PATCH /api/habit-tasks/{id}/complete`** concentra cambios sobre código que ya existía en #29. El **contrato HTTP no cambia**; cambia la **lógica detrás** en escenarios que #29 no cubría:

| Escenario | Comportamiento #29 (antes) | Comportamiento actual (después) |
|-----------|----------------------------|----------------------------------|
| Día consecutivo normal | +1 racha, `streakUpdated: true` | **Igual** |
| Varios días sin completar (sin protección) | Reinicio a 1 (solo miraba log de “ayer”) | **Mismo resultado**, con motor explícito de hueco y anti-abuso (último log real, sin backfill) |
| Varios días ausente **con protección** en el hueco | No existía protección | **Nuevo:** racha continúa (+1), no reinicia |
| Protección activada hoy, luego primera completion | N/A | **Nuevo:** cuenta como primera completion (`streakUpdated: true`); un log solo-protección no bloquea |
| `completedAt` en fecha pasada con huecos sin logs | Comportamiento ambiguo / no documentado | **Explícito:** solo se evalúa vs último log anterior a esa fecha; no se rellenan días intermedios |
| XP al completar | Se leía `XpValue` / dificultad al momento del complete | **Igual en la respuesta**; además se persiste `EarnedXpSnapshot` en BD (columna nueva, no altera JSON de respuesta) |
| Editar tarea ya completada (`PUT`) | No reseteaba `IsCompleted` | **Igual**; confirmado + snapshot no se sobrescribe |

**Implicación para QA:** volver a probar escenarios de **ausencia multi-día**, **protección** y **fechas pasadas** en complete. El uso diario normal (completar hoy, varias tareas, no repetir) no debería regresionar.

---

#### Lo que es 100 % nuevo (no reemplaza nada previo)

Estos elementos **no existían** antes; no sustituyen endpoints ni flujos anteriores:

| Entregable | Notas |
|------------|-------|
| `POST /api/streaks/protection` | Endpoint y servicio nuevos |
| Tabla `LULH_PLAYER_EVENT` | Eventos **internos** (`LEVEL_UP`, racha) — escritura desde servicios; **sin GET público** |
| Columnas en streak / task | Aditivas; valores por defecto compatibles con filas existentes |

**Fuera de esta rama (otro compañero):** `POST /api/rewards/{id}/claim`, `LULT_REWARD_CLAIM`, inventario, seed de catálogo, `GET /api/player/events`.

---

#### Cambios aditivos en respuestas existentes

| Endpoint / modelo | Cambio en esta rama |
|-------------------|---------------------|
| `GET /api/RewardItem` | **Ninguno** — mismo contrato que `dev` |
| `LULM_REWARD_ITEM` | **Sin columnas nuevas** en esta rama |

---

#### Base de datos y despliegue

| Aspecto | Impacto |
|---------|---------|
| Migraciones EF | SQL idempotente (`IF NOT EXISTS`); no borra datos existentes |
| Filas legacy en `LULH_STREAK_LOG` | `STATUS_STREAK_COMPLETION_RECORDED` default `true` → se tratan como completions históricas |
| Backup SQL + EF | Puede requerir baseline en `__EFMigrationsHistory` (ver sección 4 del doc) |
| Soft delete | Sin borrado físico de historial de tareas ni logs de racha |

---

#### Matriz rápida para regresión (prioridad QA)

| Prioridad | Qué probar | Expectativa |
|-----------|------------|-------------|
| **P0** | Complete normal, doble complete misma tarea, 2 tareas mismo día | Igual que #29 |
| **P1** | Hueco &gt; 1 día sin protección → racha 1 | Comportamiento nuevo documentado |
| **P1** | Protección + complete tras ausencia | Racha continúa |
| **P2** | PUT tarea completada | `isCompleted` y snapshot intactos |
| **P2** | Verificar `LEVEL_UP` en BD | Fila en `LULH_PLAYER_EVENT` tras subir de nivel |

---

## 2. Endpoints

### 2.1 Extendido — Completar tarea (Issue #29 + Bloques A, B, E)

```
PATCH /api/habit-tasks/{id}/complete
Auth: JWT o X-User-Id (mismo patrón que el resto de habit-tasks)
```

**Lógica de racha (Bloque A)** al procesar `completedAt` (UTC → `DateOnly`):

1. Si ya existe log del día con `CompletionRecorded = true` → `streakUpdated: false`.
2. Se busca el último log con `CompletionRecorded = true` **anterior** a esa fecha (anti-abuso: sin backfill retroactivo).
3. Gap **1 día** → racha = último conteo + 1.
4. Gap **> 1 día** sin protección en el hueco → **reinicio a 1** (`streakUpdated: true`).
5. Gap **> 1 día** con al menos un día protegido en `(últimoLog, fechaCompletion)` → racha continúa (+1). Ver Bloque B.
6. Si hay log de protección del mismo día (`CompletionRecorded = false`) → se **actualiza** ese registro al completar (no bloquea la primera completion).

**Snapshot XP (Bloque E):**

- Al completar: `NUM_HABIT_TASK_EARNED_XP` = XP calculado en ese momento (`XpValue` o fallback EASY=10, MEDIUM=25, HARD=50).
- Editar la tarea después **no** cambia el XP ya otorgado.

**Eventos generados (persistidos en BD, sin GET público en esta rama):**

- `STREAK_RESET` — reinicio por hueco sin protección.
- `STREAK_CONTINUED` — racha salvada por protección en hueco > 1 día.
- `LEVEL_UP` — si sube de nivel en esa completion.

---

### 2.2 Nuevo — Protección de racha (Bloque B)

```
POST /api/streaks/protection
Auth: Bearer JWT únicamente (userId desde claims, no body/header arbitrario)
```

**Body:**

```json
{ "type": "TRABAJO" | "EVALUACION" | "EMERGENCIA" }
```

**200 OK:**

```json
{
  "success": true,
  "message": "Streak protection activated for today.",
  "remainingProtectionsThisMonth": 2
}
```

**Errores:**

| HTTP | Cuándo |
|------|--------|
| 401 | Token inválido o ausente |
| 400 | Protección ya activa hoy / ya completó tarea hoy |
| 403 | Límite mensual excedido (`StreakProtection:MaxPerMonth`, default **3**) |
| 404 | Jugador no encontrado/inactivo |

**Persistencia:** registro en `LULH_STREAK_LOG` para el día UTC actual con `TYPE_PROTECTION_USED = true`, `DSC_STREAK_PROTECTION_TYPE`, `CompletionRecorded = false`, `StreakCount` = racha actual del jugador.

**Evento:** `STREAK_PROTECTION_USED`.

---

### 2.3 Bloque D — Recompensas (fuera de alcance)

**No incluido en esta rama.** Lo implementará otro compañero del equipo.

Diseño de referencia futuro (no desplegado aquí):

- `POST /api/rewards/{id}/claim` por `RequiredExperiencePoints`
- Tablas `LULT_REWARD_CLAIM`, `LULT_PLAYER_INVENTORY`
- `GET /api/player/events` para consulta de eventos

El listado existente `GET /api/RewardItem` de `dev` **no cambia**.

---

### 2.4 Eventos internos del jugador (sin endpoint GET)

Los eventos se **escriben** en `LULH_PLAYER_EVENT` desde `HabitService` y `StreakService`. **No hay** `GET /api/player/events` en esta PR.

Tipos persistidos: `STREAK_RESET`, `STREAK_CONTINUED`, `STREAK_PROTECTION_USED`, `LEVEL_UP`.

Verificación QA (SQL):

```sql
SELECT "TYPE_PLAYER_EVENT", "DSC_PLAYER_EVENT_PAYLOAD", "FEC_PLAYER_EVENT_CREATED"
FROM "LULH_PLAYER_EVENT"
WHERE "ID_PLAYER_USER" = <userId>
ORDER BY "FEC_PLAYER_EVENT_CREATED" DESC
LIMIT 10;
```

---

## 3. Decisiones de diseño por bloque

### Bloque A — Motor automático de rachas

| Tema | Decisión |
|------|----------|
| Reinicio vs pausa | **Reinicio a 1** si hueco > 1 día sin protección. Nunca valores negativos en `DaysStreak`. |
| Día consecutivo | Último log con completion el día anterior → +1. |
| Anti-abuso | Solo último log **real** anterior a `completedAt`. No se crean logs intermedios aunque manden fechas pasadas consecutivas. |
| XP / nivel | Independientes del reinicio de racha. |
| Mismo día | Log con `CompletionRecorded = true` existente → `streakUpdated: false`. Log solo de protección no cuenta como “ya completó”. |

**Implementación:** `Services/StreakCalculator.cs`, `HabitService.ApplyStreakUpdateAsync`, `IStreakLogRepository` (consultas de último log, protección en hueco).

---

### Bloque B — Protección de racha

| Tema | Decisión |
|------|----------|
| Modelo ante huecos | **“Salvavidas”:** basta **un** día con `TYPE_PROTECTION_USED = true` en el intervalo `(últimoLog, fechaCompletion)` para evitar reinicio. |
| Ejemplo | Hueco de 4 días, 1 día protegido → racha +1 (igual que hueco de 2 días con 1 protegido). |
| Límite de abuso | Tope mensual configurable (`StreakProtection:MaxPerMonth = 3`), no N protecciones por N días. |
| Alternativa descartada | Exigir protección en **cada** día del hueco — cambio futuro si producto lo pide. |
| Tipos | `TRABAJO`, `EVALUACION`, `EMERGENCIA` → enum `StreakProtectionType`. |

**Tests específicos:** `StreakCalculatorTests` — hueco 4 días con/sin protección.

---

### Bloque C — Bonificación por reenganche

**No implementado.** Mejora futura: bonus de XP si ausencia ≥ 5 días sin logs ni protección al volver a completar.

---

### Bloque D — Recompensas e inventario

**Fuera de alcance en esta rama.** Referencia para el compañero que implemente claim:

| Campo | Uso previsto |
|-------|--------------|
| `NUM_REWARD_ITEM_COST_GOLD` | Precio en oro (tienda futura) |
| `NUM_REWARD_ITEM_REQUIRED_XP` | Umbral XP para desbloquear/reclamar |

No hay migración seed ni `RewardItemServiceTests` en esta PR.

---

### Bloque E — Adaptación de tareas

| Tema | Decisión |
|------|----------|
| `PUT /api/habit-tasks/{id}` | `HabitTaskMapper.ApplyStandaloneRequest` **no** toca `IsCompleted` ni `EarnedXpSnapshot`. |
| Al completar | Persiste `EarnedXpSnapshot` = XP del momento. |
| Test | `UpdateTaskAsync_WhenTaskCompleted_PreservesCompletionState` |

---

## 4. Modelo de datos y migraciones

### Tablas / columnas nuevas o extendidas

| Objeto | Cambio |
|--------|--------|
| `LULH_STREAK_LOG` | + `DSC_STREAK_PROTECTION_TYPE`, + `STATUS_STREAK_COMPLETION_RECORDED` (default `true` en filas legacy) |
| `LULM_HABIT_TASK` | + `NUM_HABIT_TASK_EARNED_XP` (snapshot al completar) |
| `LULH_PLAYER_EVENT` | Nueva — eventos internos (`LEVEL_UP`, racha) |

### Migraciones EF

| Migración | Contenido |
|-----------|-----------|
| `202606130001_Issue29StreakCompletionTracking` | `STATUS_STREAK_COMPLETION_RECORDED` en `LULH_STREAK_LOG` |
| `202606130003_BlockBStreakProtection` | `DSC_STREAK_PROTECTION_TYPE` |
| `202606130004_BlockEHabitTaskEarnedXpSnapshot` | `NUM_HABIT_TASK_EARNED_XP` |
| `202606130005_AddPlayerEvents` | Tabla `LULH_PLAYER_EVENT` (logs internos: `LEVEL_UP`, racha) |

### Setup QA (sin SQL manual)

```bash
docker start leveluplife-postgres

# BD nueva: cargar backup una vez
docker exec -i leveluplife-postgres psql -U levelup -d leveluplife \
  < Backups/LevelUpLife-Backup-08-05-2026.sql

# Solo la primera vez tras backup (evita fallo de InitialCreate):
docker exec -i leveluplife-postgres psql -U levelup -d leveluplife \
  < Scripts/baseline-ef-after-backup.sql

# Aplica migraciones de gamificación (idempotente)
dotnet ef database update
```

No hay seed automático de tareas QA ni catálogo de recompensas en esta rama. QA usa tareas/hábitos existentes del backup o crea tareas manualmente vía API.

### BD local (importante para Adam / QA)

Las migraciones de gamificación usan `IF NOT EXISTS` / `WHERE NOT EXISTS` para ser seguras sobre el backup y re-ejecutables.

---

## 5. Arquitectura y capas

Alineado con patrones #27–#28 (Adam):

| Responsabilidad | Ubicación |
|-----------------|-----------|
| Completar tarea, racha, snapshot XP | `HabitService.CompleteTaskAsync` |
| Cálculo puro de racha | `StreakCalculator` (estático) |
| Protección de racha | `StreakService` + `StreakController` |
| Persistencia transaccional complete | `HabitTaskRepository.CompleteTaskAsync` (upsert streak log) |
| Eventos internos | `PlayerEventRepository` + registro desde `HabitService` / `StreakService` |
| Mapeo XP/nivel/response | `PlayerProgressMapper` |

**Auth endpoints nuevos:** solo JWT (`[Authorize]` + `ResolveAuthenticatedUserId` desde claims). No `X-User-Id`.

**Configuración:** `appsettings.json` → sección `StreakProtection:MaxPerMonth` (default 3).

---

## 6. Suite de tests (52)

| Archivo | Qué cubre |
|---------|-----------|
| `HabitServiceUpdateTaskTests` | Complete #29, level-up, validaciones, PUT preserva completion |
| `StreakCalculatorTests` | Gap 1 día, reinicio, salvavidas, hueco 4 días |
| `PlayerProgressMapperTests` | XP por dificultad, nivel |

Ejecutar:

```bash
cd LevelUpLife-Backend.Tests && dotnet test
```

---

## 7. Guía rápida HOW TO TEST (para marcar en QA)

### Prerrequisitos

- `docker start leveluplife-postgres`
- `dotnet ef database update` (con baseline si aplica)
- `dotnet run` → Scalar en `http://localhost:5223/scalar/v1`
- Login → JWT (ej. `aarondev` / credenciales del equipo)

### Escenarios sugeridos

| # | Escenario | Endpoint | Resultado esperado |
|---|-----------|----------|-------------------|
| 1 | Primera completion del día | `PATCH .../complete` | `streakUpdated: true`, XP/nivel suben |
| 2 | Segunda tarea mismo día | `PATCH .../complete` | `streakUpdated: false`, XP sí |
| 3 | Repetir misma tarea | `PATCH .../complete` | 400 `COMPLETION_REQUIREMENTS_NOT_MET` |
| 4 | Hueco > 1 día sin protección | complete tras varios días sin log | racha = 1, evento `STREAK_RESET` |
| 5 | Activar protección hoy | `POST /api/streaks/protection` | 200, `remainingProtectionsThisMonth` |
| 6 | 4º intento protección en el mes | `POST .../protection` | 403 límite mensual |
| 7 | Hueco con 1 día protegido | protection + complete tras ausencia | racha +1, evento `STREAK_CONTINUED` |
| 8 | Editar tarea completada | `PUT .../habit-tasks/{id}` | `isCompleted` true, snapshot XP intacto |
| 9 | Subida de nivel | `PATCH .../complete` con XP suficiente | `newLevel` sube; fila `LEVEL_UP` en `LULH_PLAYER_EVENT` |

### Bloque C / D

- **Bloque C:** ⚠️ Pendiente / no implementado.
- **Bloque D (claim):** ⏸️ Fuera de esta rama — no probar claim ni inventario.

---

## 8. Archivos principales tocados

```
Controllers/
  HabitTaskController.cs      # PATCH complete
  StreakController.cs         # POST protection

Services/
  HabitService.cs             # Complete + motor racha + eventos internos
  StreakCalculator.cs
  StreakService.cs

Models/
  StreakLog.cs                # ProtectionType, CompletionRecorded
  HabitTask.cs                # EarnedXpSnapshot
  PlayerEvent.cs, PlayerEventType.cs
  StreakProtectionType.cs

Repositories/
  StreakLogRepository.cs
  PlayerEventRepository.cs
  HabitTaskRepository.cs      # upsert streak log en transacción

Mappers/
  PlayerProgressMapper.cs

Infrastructure/
  Configuration/StreakProtectionOptions.cs
  Errors/StreakError.cs, TaskError.cs

Migrations/
  202606130001_Issue29StreakCompletionTracking.cs
  202606130003_BlockBStreakProtection.cs
  202606130004_BlockEHabitTaskEarnedXpSnapshot.cs
  202606130005_AddPlayerEvents.cs

LevelUpLife-Backend.Tests/
  StreakCalculatorTests.cs
  HabitServiceUpdateTaskTests.cs
  PlayerProgressMapperTests.cs

docs/
  gamification-design-decisions.md
Scripts/
  baseline-ef-after-backup.sql
  update-ef-history-after-migration-split.sql
```

---

## 9. Checklist para revisión de Adam

- [ ] Contrato `PATCH /habit-tasks/{id}/complete` intacto (`xpEarned`, `newLevel`, `streakUpdated`, códigos de error).
- [ ] Mutaciones de tarea en `HabitService` (no `HabitTaskService`) — consistente con #27–#28.
- [ ] Transacción de complete en repositorio.
- [ ] Endpoints nuevos solo JWT (no `X-User-Id`).
- [ ] Modelo salvavidas de protección — ¿OK producto o cambiar a “protección por cada día del hueco”?
- [ ] Migraciones idempotentes compatibles con backup SQL del equipo.
- [ ] Bloque C y Bloque D (claim) explícitamente fuera de alcance.
- [ ] Sección **1.3 Impacto en funcionalidad existente** — regresión P0/P1 acordada con QA.

---

## 10. Fuera de alcance (otras issues)

- Notificaciones push / recordatorios (solo datos en `LULH_PLAYER_EVENT`).
- Tienda / gasto de oro (`CostGold`).
- Panel admin, ranking, pagos, i18n, despliegue.

---

*Última actualización: alcance reducido a #29 + bloques A, B, E; sin claim de recompensas ni GET events; migraciones partidas por bloque; 52 tests.*
