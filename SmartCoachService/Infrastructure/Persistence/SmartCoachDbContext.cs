using Microsoft.EntityFrameworkCore;
using SmartCoachService.Domain.Entities;

namespace SmartCoachService.Infrastructure.Persistence;

public sealed class SmartCoachDbContext : DbContext
{
    public SmartCoachDbContext(DbContextOptions<SmartCoachDbContext> options) : base(options) { }

    public DbSet<ChatSession> ChatSessions => Set<ChatSession>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<RecommendationCache> RecommendationCaches => Set<RecommendationCache>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SmartCoachDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
