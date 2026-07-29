using SmartCoachService.Common.Abstractions;
using SmartCoachService.Domain.Entities;

namespace SmartCoachService.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly SmartCoachDbContext _context;

    private IGenericRepository<ChatSession>? _chatSessions;
    private IGenericRepository<ChatMessage>? _chatMessages;
    private IGenericRepository<RecommendationCache>? _recommendationCaches;

    public UnitOfWork(SmartCoachDbContext context) => _context = context;

    public IGenericRepository<ChatSession> ChatSessions => _chatSessions ??= new GenericRepository<ChatSession>(_context);
    public IGenericRepository<ChatMessage> ChatMessages => _chatMessages ??= new GenericRepository<ChatMessage>(_context);
    public IGenericRepository<RecommendationCache> RecommendationCaches => _recommendationCaches ??= new GenericRepository<RecommendationCache>(_context);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}
