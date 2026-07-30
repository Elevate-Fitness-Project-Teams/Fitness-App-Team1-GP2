using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutritionService.Domain.Entities;

namespace NutritionService.Infrastructure.Persistence.Configurations;

public sealed class MealPlanItemConfiguration : IEntityTypeConfiguration<MealPlanItem>
{
    public void Configure(EntityTypeBuilder<MealPlanItem> builder)
    {
        builder.ToTable("MealPlanItems");
        builder.HasKey(i => i.Id);

        builder.HasOne(i => i.MealPlan)
            .WithMany(p => p.Items)
            .HasForeignKey(i => i.MealPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Meal)
            .WithMany(m => m.MealPlanItems)
            .HasForeignKey(i => i.MealId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => new { i.MealPlanId, i.DayNumber, i.MealType });
    }
}
