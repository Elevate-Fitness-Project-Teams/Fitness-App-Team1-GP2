using Microsoft.EntityFrameworkCore;
using NutritionService.Domain.Entities;

namespace NutritionService.Infrastructure.Persistence;

public class NutritionDbContext : DbContext
{
    public NutritionDbContext(DbContextOptions<NutritionDbContext> options) : base(options) { }

    public DbSet<Meal> Meals => Set<Meal>();
    public DbSet<MealPlan> MealPlans => Set<MealPlan>();
    public DbSet<MealPlanItem> MealPlanItems => Set<MealPlanItem>();

    public DbSet<UserCalorieTarget> UserCalorieTargets => Set<UserCalorieTarget>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Meal>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Type).HasMaxLength(32);
            e.HasIndex(x => x.Type);
            e.HasIndex(x => x.Calories);
        });

        b.Entity<MealPlan>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.TargetCalorieRangeMin, x.TargetCalorieRangeMax });
        });

        b.Entity<MealPlanItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.MealPlan).WithMany(p => p.Items).HasForeignKey(x => x.MealPlanId);
            e.HasOne(x => x.Meal).WithMany().HasForeignKey(x => x.MealId);
        });

        b.Entity<UserCalorieTarget>(e =>
        {
            e.HasKey(x => x.UserId);
        });
    }
}
