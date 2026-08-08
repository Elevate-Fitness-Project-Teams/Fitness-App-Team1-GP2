using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCoachService.Domain.Entities;

namespace SmartCoachService.Infrastructure.Persistence.Configurations;

public sealed class RecommendationCacheConfiguration : IEntityTypeConfiguration<RecommendationCache>
{
    public void Configure(EntityTypeBuilder<RecommendationCache> builder)
    {
        builder.ToTable("RecommendationCache");
        builder.HasKey(c => c.Id);
        builder.HasIndex(c => c.UserId).IsUnique();
        builder.Property(c => c.HomeFeedDataJson).HasColumnType("jsonb");
    }
}
