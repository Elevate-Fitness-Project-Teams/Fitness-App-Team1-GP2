using NutritionService.Domain.Entities;

namespace NutritionService.Common.Abstractions;

/// <summary>
/// Unit of Work exposes one generic repository instance per aggregate and
/// commits every change made against them through a single SaveChanges call.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Meal> Meals { get; }
    IGenericRepository<MealPlan> MealPlans { get; }
    IGenericRepository<MealPlanItem> MealPlanItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
