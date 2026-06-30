using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutService.Domain.Entities;

namespace WorkoutService.Presistance.Configurations
{
    public class WorkoutExerciseConfiguration : IEntityTypeConfiguration<WorkoutExercise>
    {
        public void Configure(EntityTypeBuilder<WorkoutExercise> builder)
        {
            builder.ToTable("WorkoutExercises");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.WorkoutId)
                .IsRequired();

            builder.Property(x => x.ExerciseId)
                .IsRequired();

            builder.Property(x => x.SetsDefault)
                .IsRequired();

            builder.Property(x => x.RepsDefault)
                .IsRequired();

            builder.Property(x => x.RestTimeInSeconds)
                .IsRequired();

            builder.Property(x => x.OrderIndex)
                .IsRequired();

            builder.HasOne(x => x.Workout)
                .WithMany(x => x.WorkoutExercises)
                .HasForeignKey(x => x.WorkoutId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Exercise)
                .WithMany(x => x.WorkoutExercises)
                .HasForeignKey(x => x.ExerciseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.WorkoutId, x.OrderIndex });

            builder.HasIndex(x => x.ExerciseId);
        }
    }
}
