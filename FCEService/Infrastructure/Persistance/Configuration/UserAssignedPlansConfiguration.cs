using FCEService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCEService.Infrastructure.Persistance.Configuration
{
    public class UserAssignedPlansConfiguration:IEntityTypeConfiguration<UserAssignedPlan>
    {
        public void Configure(EntityTypeBuilder<UserAssignedPlan> builder)
        {
            builder.ToTable("UserAssignedPlans");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.userId); // lookup only, no uniqueness

            builder.Property(x => x.userId)
                   .IsRequired();

            builder.Property(x => x.goal)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(x => x.calorieIntake)
                   .IsRequired();

            builder.Property(x => x.WorkoutPlan)
                   .HasMaxLength(100)
                   .IsRequired(false);

            builder.Property(x => x.NutritionPlan)
                   .HasMaxLength(100)
                   .IsRequired(false);

            builder.Property(x => x.IsActive)
                   .HasDefaultValue(true)
                   .IsRequired();
        }

    }
}
