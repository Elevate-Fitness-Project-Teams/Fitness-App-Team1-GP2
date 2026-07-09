using Microsoft.EntityFrameworkCore;
using WorkoutService.Domain.Entities;

namespace WorkoutService.Presistance.Configurations
{
    public class WorkoutConfiguration :IEntityTypeConfiguration<Workout>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Workout> builder)
        {
            builder.ToTable("Workouts");

            builder.HasKey(x => x.WorkoutId);

            builder.Property(x => x.WorkoutId)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.PlanId)
                .HasMaxLength(50)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(x => x.Category)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(x => x.DurationInMinutes)
                .IsRequired();

            builder.Property(x => x.Difficulty)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired()
                .IsUnicode(false);

            builder.HasIndex(x => x.Category);

            builder.HasOne(x => x.WorkoutPlan)
                .WithMany(x => x.Workouts)
                .HasForeignKey(x => x.PlanId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
