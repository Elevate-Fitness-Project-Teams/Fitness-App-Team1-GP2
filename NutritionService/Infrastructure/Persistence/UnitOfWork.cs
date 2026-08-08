using NutritionService.Common.Abstractions;
using NutritionService.Domain.Entities;

namespace NutritionService.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly NutritionDbContext _context;

    private IGenericRepository<Meal>? _meals;
    private IGenericRepository<MealPlan>? _mealPlans;
    private IGenericRepository<MealPlanItem>? _mealPlanItems;

    public UnitOfWork(NutritionDbContext context) => _context = context;

    public IGenericRepository<Meal> Meals => _meals ??= new GenericRepository<Meal>(_context);
    public IGenericRepository<MealPlan> MealPlans => _mealPlans ??= new GenericRepository<MealPlan>(_context);
    public IGenericRepository<MealPlanItem> MealPlanItems => _mealPlanItems ??= new GenericRepository<MealPlanItem>(_context);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}
