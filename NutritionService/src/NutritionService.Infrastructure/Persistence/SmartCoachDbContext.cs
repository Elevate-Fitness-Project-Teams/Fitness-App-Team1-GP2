using Microsoft.EntityFrameworkCore;
using SmartCoachService.Domain.Entities;
using SmartCoachService.Domain.Enums;

namespace SmartCoachService.Infrastructure.Persistence;

public class SmartCoachDbContext : DbContext
{
    public SmartCoachDbContext(DbContextOptions<SmartCoachDbContext> options) : base(options) { }

    public DbSet<ChatSession> ChatSessions => Set<ChatSession>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<RecommendationCache> RecommendationCaches => Set<RecommendationCache>();

    // Local CQRS read models — synced via RabbitMQ
    public DbSet<UserFceContext> UserFceContexts => Set<UserFceContext>();
    public DbSet<UserProgressContext> UserProgressContexts => Set<UserProgressContext>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<ChatSession>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.UserId);
            e.HasIndex(x => x.LastActivityAtUtc);
        });

        b.Entity<ChatMessage>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Sender).HasConversion<string>().HasMaxLength(8);
            e.HasOne(x => x.Session).WithMany(s => s.Messages).HasForeignKey(x => x.SessionId);
            e.HasIndex(x => new { x.SessionId, x.SentAtUtc });
        });

        b.Entity<RecommendationCache>(e =>
        {
            e.HasKey(x => x.UserId);
            e.HasIndex(x => x.ExpiresAt);
            e.Ignore(x => x.IsExpired); // computed property, not persisted
        });

        b.Entity<UserFceContext>(e => e.HasKey(x => x.UserId));
        b.Entity<UserProgressContext>(e => e.HasKey(x => x.UserId));
    }
}
