using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutService.Domain.Entities;

namespace WorkoutService.Presistance.Configurations
{
    public class WorkoutSessionConfiguration : IEntityTypeConfiguration<WorkoutSession>
    {
        public void Configure(EntityTypeBuilder<WorkoutSession> builder)
        {
            builder.ToTable("WorkoutSessions");

            builder.HasKey(x => x.SessionId);

            builder.Property(x => x.SessionId)
                .HasMaxLength(100)
                .IsRequired()
                .IsUnicode(false)
                .ValueGeneratedNever(); 

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.WorkoutId)
                .IsRequired();

            builder.Property(x => x.StartedAt)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired()
                .IsUnicode(false);

            builder.HasIndex(x => x.UserId); // to check if user has any active workout session

            builder.HasOne(x => x.Workout)
                .WithMany(x => x.WorkoutSessions)
                .HasForeignKey(x => x.WorkoutId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
