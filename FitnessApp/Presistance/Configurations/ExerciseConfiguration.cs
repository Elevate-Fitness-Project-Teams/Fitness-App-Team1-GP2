using Microsoft.EntityFrameworkCore;
using WorkoutService.Domain.Entities;

namespace WorkoutService.Presistance.Configurations
{
    public class ExerciseConfiguration :IEntityTypeConfiguration<Exercise>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Exercise> builder)
        {
            builder.ToTable("Exercises");

            builder.HasKey(x => x.ExerciseId);

            builder.Property(x => x.ExerciseId)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(x => x.TargetMuscles)
                .HasMaxLength(255)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(x => x.Equipment)
                .HasMaxLength(150)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(x => x.Difficulty)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(x => x.Description)
                .HasMaxLength(1000)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(x => x.VideoUrl)
                .HasMaxLength(500)
                .IsUnicode(false);
        }
    }
}
