using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Microsoft.Extensions.Caching.Memory;

namespace Fayora.Infrastructure.Services.AuthModule;

public class MemoryDailyUploadTracker : IDailyUploadTracker
{
    private readonly IMemoryCache _cache;

    public MemoryDailyUploadTracker(IMemoryCache cache)
    {
        _cache = cache;
    }

    private string GetCacheKey(Guid userId, UploadContext context)
    {
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        return $"UploadLimit_{userId}_{context}_{today}";
    }

    public Task<int> GetTodayUploadCountAsync(Guid userId, UploadContext context, CancellationToken cancellationToken)
    {
        var key = GetCacheKey(userId, context);

        _cache.TryGetValue(key, out int count);

        return Task.FromResult(count);
    }

    public Task IncrementUploadCountAsync(Guid userId, UploadContext context, int count, CancellationToken cancellationToken)
    {
        var key = GetCacheKey(userId, context);

        _cache.TryGetValue(key, out int currentCount);
        var newCount = currentCount + count;

        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTimeOffset.UtcNow.Date.AddDays(1)
        };

        _cache.Set(key, newCount, options);

        return Task.CompletedTask;
    }
}
