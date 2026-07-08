using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutService.Domain.Entities;

namespace WorkoutService.Presistance.Configurations
{
    public class WorkoutPlanConfiguration : IEntityTypeConfiguration<WorkoutPlan>
    {
        public void Configure(EntityTypeBuilder<WorkoutPlan> builder)
        {
            builder.ToTable("WorkoutPlans");

            builder.HasKey(x => x.PlanId);

            builder.Property(x => x.PlanId)
                .HasMaxLength(50)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(x => x.Description)
                .HasMaxLength(500)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(x => x.Goal)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(x => x.Difficulty)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired()
                .IsUnicode(false);
        } 
    }
}
