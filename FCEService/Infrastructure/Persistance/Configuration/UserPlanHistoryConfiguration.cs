using FCEService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCEService.Infrastructure.Persistance.Configuration
{
    public class UserPlanHistoryConfiguration:IEntityTypeConfiguration<UserPlanHistory>
    {
        public void Configure(EntityTypeBuilder<UserPlanHistory> builder)
        {
            builder.ToTable("UserPlanHistory");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.UserId);

            builder.Property(x => x.UserId)
                   .IsRequired();

            builder.Property(x => x.ResonForChange)
                   .HasMaxLength(255)
                   .IsRequired(false);

            builder.Property(x => x.EndedAt)
                   .IsRequired(false);
        }
    }
}
