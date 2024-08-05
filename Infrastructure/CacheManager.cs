using Contracts.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Services;

namespace Infrastructure;

public class CacheManager(IMemoryCache memCache, ILoggerManager logger, IOptions<AppSettings> appSettings) : ICacheManager
{
    private readonly int cacheExpiryByMinutes = appSettings.Value.CacheExpiryByMinutes;

    public T GetCache<T>(string cacheKey)
    {
        if (!memCache.TryGetValue(cacheKey, out T cachedValue))
            return default;

        logger.LogInfo(string.Format("Retrieve Cached Value of {0}", cacheKey));

        return cachedValue;
    }

    public void SetCache<T>(string cacheKey, T value)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(cacheExpiryByMinutes));

        logger.LogInfo(string.Format("Set Cache Value of {0}", cacheKey));

        memCache.Set(cacheKey, value, cacheEntryOptions);
    }
}
