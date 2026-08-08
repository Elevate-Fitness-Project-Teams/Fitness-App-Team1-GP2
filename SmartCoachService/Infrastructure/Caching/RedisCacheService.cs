using Microsoft.Extensions.Caching.Distributed;
using SmartCoachService.Common.Abstractions;
using System.Text.Json;

namespace SmartCoachService.Infrastructure.Caching;

/// <summary>
/// Backs both the free-tier rate limit counter (Epic 7.1: 5 messages / 24h) and the
/// short-lived reads used by the pipeline. Uses Redis distributed cache under the hood.
/// </summary>
public sealed class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache) => _cache = cache;

    public async Task<int> IncrementAsync(string key, TimeSpan expiry, CancellationToken cancellationToken = default)
    {
        var raw = await _cache.GetStringAsync(key, cancellationToken);
        var count = string.IsNullOrEmpty(raw) ? 0 : int.Parse(raw);
        count++;

        await _cache.SetStringAsync(key, count.ToString(), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry
        }, cancellationToken);

        return count;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var raw = await _cache.GetStringAsync(key, cancellationToken);
        return raw is null ? default : JsonSerializer.Deserialize<T>(raw);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions();
        if (expiry.HasValue) options.AbsoluteExpirationRelativeToNow = expiry;
        await _cache.SetStringAsync(key, JsonSerializer.Serialize(value), options, cancellationToken);
    }
}
