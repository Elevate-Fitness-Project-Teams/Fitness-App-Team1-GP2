namespace SmartCoachService.Common.Abstractions;

/// <summary>Small cache abstraction backing the free-tier rate limit check (5 messages / 24h).</summary>
public interface ICacheService
{
    Task<int> IncrementAsync(string key, TimeSpan expiry, CancellationToken cancellationToken = default);
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
}
