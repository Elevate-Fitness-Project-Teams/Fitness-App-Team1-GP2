using SmartCoachService.Domain.Entities;

namespace SmartCoachService.Common.Abstractions;

/// <summary>Unit of Work exposing one generic repository per aggregate + a single SaveChanges commit.</summary>
public interface IUnitOfWork : IDisposable
{
    IGenericRepository<ChatSession> ChatSessions { get; }
    IGenericRepository<ChatMessage> ChatMessages { get; }
    IGenericRepository<RecommendationCache> RecommendationCaches { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
