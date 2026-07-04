using FCEService.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCEService.Infrastructure.Persistance.Configuration
{
    public class UserFitnessStatsConfiguration:IEntityTypeConfiguration<UserFitnessStats>
    {
        public void Configure(EntityTypeBuilder<UserFitnessStats> builder)
        {
            builder.ToTable("UserFitnessStats");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.userId).IsUnique();

            builder.Property(x => x.userId)
                   .IsRequired();

            builder.Property(x => x.goal)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(x => x.activity)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(x => x.WorkoutDays)
                   .IsRequired();

            builder.Property(x => x.IsActive)
                   .HasDefaultValue(true)
                   .IsRequired();

            builder.OwnsOne(x => x.physicalStats, p =>
            {
                p.Property(x => x.Weight).HasColumnName("Weight").IsRequired();
                p.Property(x => x.Height).HasColumnName("Height").IsRequired();
                p.Property(x => x.Age).HasColumnName("Age").IsRequired();
                p.Property(x => x.Gender)
                 .HasColumnName("Gender")
                 .HasConversion<string>()
                 .HasMaxLength(10)
                 .IsRequired();
            });
        }

    }
}
