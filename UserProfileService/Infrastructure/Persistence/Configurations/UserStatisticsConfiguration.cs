using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserProfileService.Domain.Entities;

namespace UserProfileService.Infrastructure.Persistence.Configurations
{
    public class UserStatisticsConfiguration : IEntityTypeConfiguration<UserStatistics>
    {
        public void Configure(EntityTypeBuilder<UserStatistics> builder)
        {
            builder.ToTable("UserStatistics");

            builder.HasKey(x => x.UserId);

            builder.Property(x => x.UserId)
                .ValueGeneratedNever();

            builder.Property(x => x.TotalWorkouts)
                .HasDefaultValue(0);

            builder.Property(x => x.CurrentStreak)
                .HasDefaultValue(0);
        }
    }
}
