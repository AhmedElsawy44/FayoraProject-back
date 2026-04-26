using Fayora.Application.Common.Abstractions.Caching;
using Fayora.Application.Common.Interfaces.Services.AuthModule;

namespace Fayora.Infrastructure.Services.AuthModule;

public class RadisDailyUploadTracker(ICacheService cache) : IDailyUploadTracker
{

    private string GetCacheKey(Guid userId, UploadContext context)
    {
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        return $"UploadLimit_{userId}_{context}_{today}";
    }

    public async Task<int> GetTodayUploadCountAsync(Guid userId, UploadContext context, CancellationToken cancellationToken)
    {
        var key = GetCacheKey(userId, context);

        var cachedCount = await cache.GetAsync<int?>(key, cancellationToken);

        return cachedCount ?? 0;
    }

    public async Task IncrementUploadCountAsync(Guid userId, UploadContext context, int count, CancellationToken cancellationToken)
    {
        var key = GetCacheKey(userId, context);

        var cachedCount = await cache.GetAsync<int?>(key, cancellationToken);
        var currentCount = cachedCount ?? 0;
        var newCount = currentCount + count;

        var duration = TimeSpan.FromDays(1);

        await cache.SetAsync(key, newCount, duration);

    }
}
