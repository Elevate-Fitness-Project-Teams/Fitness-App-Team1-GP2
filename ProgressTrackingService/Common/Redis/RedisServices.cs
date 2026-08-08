using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using ProgressTrackingService.Common.Pagination;
using ProgressTrackingService.Common.Response;

namespace ProgressTrackingService.Common.Redis;

public class RedisServices<TEntity> (IDistributedCache redisCache)
{
    private readonly IDistributedCache _redisCache = redisCache;

    public async Task SetRedisCacheAsync(string key, TEntity value, int absoluteExpirationHours, int slidingExpirationMinutes, CancellationToken cancellationToken)
    {
        // Serialize
        var dataSerialized = JsonSerializer.Serialize(value);
        
        // Options
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(absoluteExpirationHours),
            SlidingExpiration = TimeSpan.FromMinutes(slidingExpirationMinutes)
        };
        
        await _redisCache.SetStringAsync(key, dataSerialized, options, cancellationToken);
    }
    
    // Overloading if you need to return List<> values
    public async Task SetRedisCacheAsync(string key, List<TEntity> value, int absoluteExpirationHours, int slidingExpirationMinutes, CancellationToken cancellationToken)
    {
        // Serialize
        var dataSerialized = JsonSerializer.Serialize(value);
        
        // Options
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(absoluteExpirationHours),
            SlidingExpiration = TimeSpan.FromMinutes(slidingExpirationMinutes)
        };
        
        await _redisCache.SetStringAsync(key, dataSerialized, options, cancellationToken);
    }

    public async Task<ResponseResult<TEntity>> GetRedisCacheAsync(string key, CancellationToken cancellationToken)
    {
        var result = await _redisCache.GetStringAsync(key, cancellationToken);
        
        if (string.IsNullOrEmpty(result))
            return ResponseResult<TEntity>.Failure(StatusCode.NotFound, "Cache Miss - Data not found in Redis.");
        
        var deserialize = JsonSerializer.Deserialize<TEntity>(result);

        if (deserialize == null)
            return ResponseResult<TEntity>.Failure(StatusCode.BadRequest, "Failed to deserialize cache data.");
        
        return ResponseResult<TEntity>.Success(deserialize, "Data retrieved successfully from cache.", StatusCode.Success);
    }
    
    
    public async Task RemoveRedisCacheAsync(string key, CancellationToken cancellationToken)
    {
        await _redisCache.RemoveAsync(key, cancellationToken);
    }
}