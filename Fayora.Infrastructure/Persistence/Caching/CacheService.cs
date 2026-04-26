using Fayora.Application.Common.Abstractions.Caching;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;

namespace Fayora.Infrastructure.Persistence.Caching;

public class CacheService(IDistributedCache cache, IConnectionMultiplexer redisConnection) : ICacheService
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        string? cachedData = await cache.GetStringAsync(key, cancellationToken);
        if (cachedData is null)
        {
            return null;
        }

        return JsonSerializer.Deserialize<T>(cachedData);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await cache.RemoveAsync(key, cancellationToken);
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var db = redisConnection.GetDatabase();

        var endpoints = redisConnection.GetEndPoints();
        var server = redisConnection.GetServer(endpoints.First());

        var keys = server.Keys(pattern: $"{prefix}*").ToArray();

        if (keys.Length > 0)
        {
            await db.KeyDeleteAsync(keys);
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null, CancellationToken cancellationToken = default) where T : class
    {
        var options = new DistributedCacheEntryOptions();

        options.SetAbsoluteExpiration(absoluteExpirationRelativeToNow ?? TimeSpan.FromHours(1));

        string serializedData = JsonSerializer.Serialize(value);

        await cache.SetStringAsync(key, serializedData, options, cancellationToken);
    }
}
