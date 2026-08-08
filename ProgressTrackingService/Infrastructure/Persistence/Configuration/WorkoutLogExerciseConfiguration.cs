using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgressTrackingService.Domain.Entities;

namespace ProgressTrackingService.Infrastructure.Persistence.Configuration;

public class WorkoutLogExerciseConfiguration : IEntityTypeConfiguration<WorkoutLogExercise>
{
    public void Configure(EntityTypeBuilder<WorkoutLogExercise> builder)
    {
        builder.ToTable("WorkoutLogExercises", "ProgressTracking");
        builder.HasKey(w => w.Id);
        builder.Property(w => w.SetsCompleted).IsRequired();
        builder.Property(w => w.RepsCompleted).IsRequired();
        builder.Property(w => w.WeightUsed).IsRequired().HasPrecision(6, 2).HasDefaultValue(0);
        builder.Property(w => w.Completed).IsRequired();
        
        builder.HasIndex(x => x.WorkoutLogId);
        builder.HasIndex(x => x.ExerciseId);
    }
}