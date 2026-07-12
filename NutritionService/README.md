# Nutrition Service — Epic 6 (Backend)

Stack: **.NET 8, MediatR (CQRS), EF Core (SQL Server), RabbitMQ**, Vertical Slice Architecture.

## Project layout

```
src/
  NutritionService.Domain/        Entities + integration event contracts (no dependencies)
  NutritionService.Infrastructure/ EF Core DbContext, RabbitMQ consumer (depends on Domain)
  NutritionService.Application/   Vertical slices — one folder per user story (depends on Domain + Infrastructure)
    Features/
      GetMealRecommendations/            Story 6.1
        Queries/      GetMealRecommendationsQuery.cs
        Handlers/     GetMealRecommendationsHandler.cs
        Dtos/         MealRecommendationDtos.cs
        Validators/   GetMealRecommendationsValidator.cs
      GetMealRecommendationsByUserId/    Story 6.2 (reuses Story 6.1's Dtos — same response shape)
        Queries/  Handlers/  Validators/
      GetMealDetail/                     Story 6.3
        Queries/  Handlers/  Dtos/
      BrowseMealPlans/                   Story 6.4
        Queries/  Handlers/  Dtos/  Validators/
      GetMealPlansByCalories/            Story 6.5
        Queries/  Handlers/  Dtos/  Validators/
  NutritionService.Api/           Controllers, middleware, DI wiring (depends on Application)
```

Each `Features/<StoryName>/` folder is still a self-contained vertical slice — Story 6.3 owns
its own `Queries/`, `Handlers/`, `Dtos/` and nothing from Story 6.1 leaks into it. What changed
from the previous revision is purely internal: instead of one file per slice, each slice is now
split by CQRS component (Query / Handler / Dto / Validator), which is friendlier for code review
and for teams used to that convention. Cross-slice file moves are still zero — only within-slice
file boundaries changed.

## How FCE ↔ Nutrition communication works (key decision)

The original AC said "synchronously call the FCE Service" — but we're using RabbitMQ, which
is inherently async. Decision taken: **event-driven local read model**, not RPC-over-MQ.

- FCE Service publishes `CalorieTargetCalculatedEvent` to a `fce.calorie-target.calculated`
  fanout exchange whenever it (re)calculates a user's CalorieTarget.
- `CalorieTargetCalculatedConsumer` (a `BackgroundService` in Infrastructure) subscribes and
  upserts the local `UserCalorieTargets` table — this is Nutrition Service's own CQRS read
  model, never a live call to FCE.
- Stories 6.1 / 6.2 query handlers read **only** from `UserCalorieTargets`. If no row exists
  for the user → `400 FCE_METRICS_NOT_CALCULATED`, exactly per the AC.

Tradeoff: eventual consistency instead of strong consistency. Acceptable here because
CalorieTarget doesn't change every second, and it buys full decoupling — FCE being down
never breaks a Nutrition Service request.

## Known gap to confirm with product owner

**Story 6.5**, "calories parameter missing" branch had no defined `Then` in the original AC.
Implemented as `400 — VAL_CALORIES_REQUIRED`. If the intended behavior is instead "return the
full unfiltered list", that's a one-line change in `GetMealPlansByCaloriesHandler`.

## Error code → HTTP mapping

All domain exceptions inherit `AppException` (`Application/Common/Exceptions`) and are mapped
centrally in `ExceptionHandlingMiddleware` to `{ errorCode, message }` JSON:

| Exception                          | Status | errorCode                    |
|-------------------------------------|--------|-------------------------------|
| `FceMetricsNotCalculatedException`  | 400    | `FCE_METRICS_NOT_CALCULATED`  |
| `MealNotFoundException`             | 404    | `RES_MEAL_NOT_FOUND`          |
| `UserNotFoundException`             | 404    | `RES_USER_NOT_FOUND`          |
| `ValidationAppException`            | 400    | (per-case, e.g. `VAL_PAGE_INVALID`) |

## Not yet wired (left as explicit TODOs in code)

- `RES_USER_NOT_FOUND` for Story 6.2 needs a real user-existence check — either a sync call to
  a User/Identity Service, or (more CQRS-idiomatic) a local `Users` projection synced via events,
  the same pattern as `UserCalorieTargets`. Stubbed as a commented-out hook in the 6.2 handler.
- JWT bearer options (issuer/audience/signing key) in `Program.cs` — wire to your platform's
  identity provider config.
